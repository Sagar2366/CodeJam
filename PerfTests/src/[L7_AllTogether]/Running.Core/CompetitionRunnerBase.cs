﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

using BenchmarkDotNet.Characteristics;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Helpers;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Toolchains.InProcess;
using BenchmarkDotNet.Validators;

using CodeJam.PerfTests.Analysers;
using CodeJam.PerfTests.Columns;
using CodeJam.PerfTests.Configs;
using CodeJam.PerfTests.Loggers;
using CodeJam.PerfTests.Running.Messages;
using CodeJam.Strings;

using JetBrains.Annotations;

namespace CodeJam.PerfTests.Running.Core
{
	/// <summary>Base class for competition benchmark runners</summary>
	[PublicAPI]
	[SuppressMessage("ReSharper", "ArrangeBraces_using")]
	[SuppressMessage("ReSharper", "ArrangeRedundantParentheses")]
	[SuppressMessage("ReSharper", "SuggestVarOrType_BuiltInTypes")]
	[SuppressMessage("ReSharper", "VirtualMemberNeverOverridden.Global")]
	public abstract class CompetitionRunnerBase
	{
		/// <summary>Base class for competition runner's host logger.</summary>
		/// <seealso cref="Loggers.HostLogger"/>
		protected abstract class HostLogger : Loggers.HostLogger
		{
			/// <summary>Initializes a new instance of the <see cref="HostLogger"/> class.</summary>
			/// <param name="wrappedLogger">The logger to redirect the output. Cannot be null.</param>
			/// <param name="logMode">Host logging mode.</param>
			protected HostLogger([NotNull] ILogger wrappedLogger, HostLogMode logMode)
				: base(wrappedLogger, logMode) { }
		}

		#region Helpers
		private static void SetCurrentDirectoryIfNotNull(string currentDirectory)
		{
			if (currentDirectory.NotNullNorEmpty())
			{
				Environment.CurrentDirectory = currentDirectory;
			}
		}

		private static ICompetitionConfig GetFirstConfig(
			[NotNull] Type benchmarkType,
			[CanBeNull] ICompetitionConfig customConfig)
		{
			if (customConfig != null)
				return new ManualCompetitionConfig(customConfig);

			var configSource = benchmarkType.TryGetMetadataAttribute<ICompetitionConfigSource>();

			return configSource?.Config ?? CompetitionConfig.GetConfigForAssembly(benchmarkType.Assembly);
		}
		#endregion

		#region Public API (expose these via Competiton classes)
		/// <summary>Runs the benchmark.</summary>
		/// <typeparam name="T">Benchmark class to run.</typeparam>
		/// <param name="competitionConfig">
		/// The competition config.
		/// If<c>null</c> config from <see cref="CompetitionConfigAttribute"/> will be used.
		/// </param>
		/// <returns>The state of the competition.</returns>
		[NotNull]
		public CompetitionState Run<T>([CanBeNull] ICompetitionConfig competitionConfig = null)
			where T : class =>
				RunCore(typeof(T), competitionConfig);

		/// <summary>Runs the benchmark.</summary>
		/// <typeparam name="T">Benchmark class to run.</typeparam>
		/// <param name="thisReference">Reference used to infer type of the benchmark.</param>
		/// <param name="competitionConfig">
		/// The competition config.
		/// If<c>null</c> config from <see cref="CompetitionConfigAttribute"/> will be used.
		/// </param>
		/// <returns>The state of the competition.</returns>
		[NotNull]
		public CompetitionState Run<T>(
			[NotNull] T thisReference,
			[CanBeNull] ICompetitionConfig competitionConfig = null)
			where T : class =>
				RunCore(thisReference.GetType(), competitionConfig);

		/// <summary>Runs the benchmark.</summary>
		/// <param name="benchmarkType">Benchmark class to run.</param>
		/// <param name="competitionConfig">
		/// The competition config.
		/// If<c>null</c> config from <see cref="CompetitionConfigAttribute"/> will be used.
		/// </param>
		/// <returns>The state of the competition.</returns>
		[NotNull]
		public CompetitionState Run(
			[NotNull] Type benchmarkType,
			[CanBeNull] ICompetitionConfig competitionConfig = null) =>
				RunCore(benchmarkType, competitionConfig);
		#endregion

		#region Advanced public API (expose these if you wish)
		/// <summary>Runs all benchmarks defined in the assembly.</summary>
		/// <param name="assembly">Assembly with benchmarks to run.</param>
		/// <param name="competitionConfig">
		/// The competition config.
		/// If<c>null</c> config from <see cref="CompetitionConfigAttribute"/> will be used.
		/// </param>
		/// <returns>The state of the competition for each benchmark that was run.</returns>
		[NotNull]
		public IReadOnlyDictionary<Type, CompetitionState> Run(
			[NotNull] Assembly assembly,
			[CanBeNull] ICompetitionConfig competitionConfig = null) =>
				Run(BenchmarkHelpers.GetBenchmarkTypes(assembly), competitionConfig);

		/// <summary>Runs all benchmarks defined in the assembly.</summary>
		/// <param name="benchmarkTypes">Benchmark classes to run.</param>
		/// <param name="competitionConfig">
		/// The competition config.
		/// If<c>null</c> config from <see cref="CompetitionConfigAttribute"/> will be used.
		/// </param>
		/// <returns>The state of the competition for each benchmark that was run.</returns>
		[NotNull]
		public IReadOnlyDictionary<Type, CompetitionState> Run(
			[NotNull] Type[] benchmarkTypes,
			[CanBeNull] ICompetitionConfig competitionConfig = null)
		{
			var result = new Dictionary<Type, CompetitionState>();

			foreach (var benchmarkType in benchmarkTypes)
			{
				result[benchmarkType] = RunCore(benchmarkType, competitionConfig);
			}

			return result;
		}
		#endregion

		/// <summary>Runs the benchmark.</summary>
		/// <param name="benchmarkType">Benchmark class to run.</param>
		/// <param name="competitionConfig">
		/// The competition config.
		/// If<c>null</c> config from <see cref="CompetitionConfigAttribute"/> will be used.
		/// </param>
		/// <returns>State of the run.</returns>
		[NotNull]
		private CompetitionState RunCore(
			[NotNull] Type benchmarkType,
			[CanBeNull] ICompetitionConfig competitionConfig)
		{
			Code.NotNull(benchmarkType, nameof(benchmarkType));

			competitionConfig = CreateBenchmarkConfig(benchmarkType, competitionConfig);

			var hostLogger = competitionConfig.GetLoggers().OfType<HostLogger>().Single();

			var previousDirectory = Environment.CurrentDirectory;
			var currentDirectory = GetOutputDirectory(benchmarkType.Assembly);
			if (currentDirectory == previousDirectory)
			{
				currentDirectory = null;
				previousDirectory = null;
			}

			CompetitionState competitionState = null;
			try
			{
				SetCurrentDirectoryIfNotNull(currentDirectory);
				try
				{
					competitionState = CompetitionCore.Run(
						benchmarkType,
						new ReadOnlyConfig(competitionConfig),
						competitionConfig.Options);

					ProcessRunComplete(competitionConfig, competitionState);
				}
				finally
				{
					ReportHostLogger(hostLogger, competitionState?.LastRunSummary);
				}

				using (Loggers.HostLogger.BeginLogImportant(competitionConfig))
				{
					ReportMessagesToUser(competitionConfig, competitionState);
				}
			}
			finally
			{
				Loggers.HostLogger.FlushLoggers(competitionConfig);
				SetCurrentDirectoryIfNotNull(previousDirectory);
			}

			return competitionState;
		}

		#region Prepare & run completed logic
		private void ProcessRunComplete(
			[NotNull] ICompetitionConfig competitionConfig,
			[NotNull] CompetitionState competitionState)
		{
			var logger = competitionState.Logger;
			var summary = competitionState.LastRunSummary;

			if (logger == null)
				return;

			if (competitionConfig.Options.RunOptions.DetailedLogging)
			{
				var messages = competitionState.GetMessages();

				if (messages.Any())
				{
					logger.WriteSeparatorLine("All messages");
					foreach (var message in messages)
					{
						logger.LogMessage(message);
					}
				}
				else
				{
					logger.WriteSeparatorLine();
					logger.WriteLineInfo($"{Loggers.HostLogger.LogVerbosePrefix} No messages in run.");
				}
			}
			else if (summary != null)
			{
				using (Loggers.HostLogger.BeginLogImportant(summary.Config))
				{
					var summaryLogger = DumpSummaryToHostLogger
						? logger
						: new CompositeLogger(
							summary.Config
								.GetLoggers()
								.Where(l => !(l is HostLogger))
								.ToArray());

					// Dumping the benchmark summary
					summaryLogger.WriteSeparatorLine("Summary");
					MarkdownExporter.Console.ExportToLog(summary, summaryLogger);
				}
			}
		}
		#endregion

		#region Override test running behavior
		/// <summary>Returns output directory that should be used for running the test.</summary>
		/// <param name="targetAssembly">The target assembly tests will be run for.</param>
		/// <returns>
		/// Output directory that should be used for running the test or <c>null</c> if the current directory should be used.
		/// </returns>
		protected virtual string GetOutputDirectory(Assembly targetAssembly) => null;

		/// <summary>Gets a value indicating whether the last run summary should be dumped into host logger.</summary>
		/// <value>
		/// <c>true</c> if the last run summary should be dumped into host logger; otherwise, <c>false</c>.
		/// </value>
		protected virtual bool DumpSummaryToHostLogger => true;
		#endregion

		#region Host-related logic
		/// <summary>Creates a host logger.</summary>
		/// <param name="hostLogMode">The host log mode.</param>
		/// <returns>An instance of <see cref="HostLogger"/></returns>
		[NotNull]
		protected abstract HostLogger CreateHostLogger(HostLogMode hostLogMode);

		/// <summary>Reports content of the host logger to user.</summary>
		/// <param name="logger">The host logger.</param>
		/// <param name="summary">The summary to report.</param>
		protected abstract void ReportHostLogger([NotNull] HostLogger logger, [CanBeNull] Summary summary);

		/// <summary>Reports the execution errors to user.</summary>
		/// <param name="messages">The messages to report.</param>
		/// <param name="competitionState">State of the run.</param>
		protected abstract void ReportExecutionErrors([NotNull] string messages, [NotNull] CompetitionState competitionState);

		/// <summary>Reports failed assertions to user.</summary>
		/// <param name="messages">The messages to report.</param>
		/// <param name="competitionState">State of the run.</param>
		protected abstract void ReportAssertionsFailed([NotNull] string messages, [NotNull] CompetitionState competitionState);

		/// <summary>Reports warnings to user.</summary>
		/// <param name="messages">The messages to report.</param>
		/// <param name="competitionState">State of the run.</param>
		protected abstract void ReportWarnings([NotNull] string messages, [NotNull] CompetitionState competitionState);
		#endregion

		#region Create benchark config
		private ICompetitionConfig CreateBenchmarkConfig([NotNull] Type benchmarkType, ICompetitionConfig competitionConfig)
		{
			// ReSharper disable once UseObjectOrCollectionInitializer
			var result = new ManualCompetitionConfig(GetFirstConfig(benchmarkType, competitionConfig));
			PrepareCompetitionConfig(result);
			InitCompetitionConfigOverride(result);
			FixCompetitionConfig(result);

			return result.AsReadOnly();
		}

		private void PrepareCompetitionConfig(ManualCompetitionConfig competitionConfig)
		{
			if (HostEnvironmentInfo.GetCurrent().HasAttachedDebugger)
			{
				competitionConfig.ApplyCompetitionOptions(
					new CompetitionOptions
					{
						RunOptions =
						{
							AllowDebugBuilds = true
						}
					});
			}
			var jobs = competitionConfig.Jobs;
			if (jobs.Count == 0)
			{
				jobs.AddRange(CompetitionConfig.Default.GetJobs());
			}
		}

		private void FixCompetitionConfig(ManualCompetitionConfig competitionConfig)
		{
			// DONTTOUCH: call order is important.
			FixConfigJobs(competitionConfig);
			FixConfigLoggers(competitionConfig);
			FixConfigColumns(competitionConfig);
			FixConfigValidators(competitionConfig);
			FixConfigAnalysers(competitionConfig);
		}

		private static void FixConfigJobs(ManualCompetitionConfig competitionConfig)
		{
			var jobs = competitionConfig.Jobs;
			for (int i = 0; i < jobs.Count; i++)
			{
				var job = jobs[i];
				if (job.Infrastructure.Toolchain == null)
				{
					var id = job.ResolveValue(JobMode.IdCharacteristic, (string)null);
					jobs[i] = job
						.With(InProcessToolchain.Instance)
						.WithId(id);
				}
			}
		}

		private void FixConfigLoggers(ManualCompetitionConfig competitionConfig)
		{
			var runOptions = competitionConfig.Options.RunOptions;
			var hostLogMode = runOptions.DetailedLogging ? HostLogMode.AllMessages : HostLogMode.PrefixedOnly;
			competitionConfig.Loggers.Insert(0, CreateHostLogger(hostLogMode));
		}

		// TODO: better columns.
		// TODO: custom column provider?
		private void FixConfigColumns(ManualCompetitionConfig competitionConfig) =>
			competitionConfig.Add(
				StatisticColumn.Min,
				CompetitionLimitColumn.Min,
				CompetitionLimitColumn.Max,
				StatisticColumn.Max);

		private void FixConfigValidators(ManualCompetitionConfig competitionConfig)
		{
			var competitionOptions = competitionConfig.Options;
			bool debugMode = competitionOptions.RunOptions.AllowDebugBuilds;

			var validators = competitionConfig.Validators;
			bool customToolchain = competitionConfig.Jobs.Any(j => !(j.Infrastructure.Toolchain is InProcessToolchain));
			if (!customToolchain &&
				!validators.Any(v => v is InProcessValidator))
			{
				validators.Insert(0, debugMode ? InProcessValidator.DontFailOnError : InProcessValidator.FailOnError);
			}

			if (debugMode)
			{
				validators.RemoveAll(v => v is JitOptimizationsValidator && v.TreatsWarningsAsErrors);
			}
			else if (competitionOptions.SourceAnnotations.UpdateSources &&
				!validators.OfType<JitOptimizationsValidator>().Any())
			{
				validators.Insert(0, JitOptimizationsValidator.FailOnError);
			}

			Code.BugIf(
				validators.OfType<RunStateSlots>().Any(),
				"The config validators should not contain instances of RunStateSlots.");

			// DONTTOUCH: the RunStateSlots should be first in the chain.
			validators.Insert(0, new RunStateSlots());
		}

		private void FixConfigAnalysers(ManualCompetitionConfig competitionConfig)
		{
			Code.BugIf(
				competitionConfig.Analysers.OfType<CompetitionAnalyser>().Any(),
				"The config analysers should not contain instances of CompetitionAnalyser.");

			var annotationsMode = competitionConfig.Options.SourceAnnotations;

			// DONTTOUCH: the CompetitionAnnotateAnalyser should be last analyser in the chain.
			var analyzer = annotationsMode.UpdateSources
				? new CompetitionAnnotateAnalyser()
				: new CompetitionAnalyser();
			competitionConfig.Analysers.Add(analyzer);
		}

		/// <summary>Customize competition config.</summary>
		/// <param name="competitionConfig">The competition configuration.</param>
		protected virtual void InitCompetitionConfigOverride(ManualCompetitionConfig competitionConfig) { }
		#endregion

		#region Messages
		private void ReportMessagesToUser(
			[NotNull] ICompetitionConfig competitionConfig,
			[NotNull] CompetitionState competitionState)
		{
			var criticalErrorMessages = GetMessageLines(
				competitionState, m => m.MessageSeverity > MessageSeverity.TestError, true);
			bool hasCriticalMessages = criticalErrorMessages.Any();

			var testFailedMessages = GetMessageLines(
				competitionState, m => m.MessageSeverity == MessageSeverity.TestError, hasCriticalMessages);
			bool hasTestFailedMessages = testFailedMessages.Any();

			var warningMessages = GetMessageLines(
				competitionState, m => m.MessageSeverity == MessageSeverity.Warning, true);
			bool hasWarnings = warningMessages.Any();

			var infoMessages = GetMessageLines(competitionState, m => m.MessageSeverity < MessageSeverity.Warning, true);
			bool hasInfo = infoMessages.Any();

			if (!(hasCriticalMessages || hasTestFailedMessages || hasWarnings))
				return;

			var allMessages = new List<string>();

			// TODO: simplify
			if (hasCriticalMessages)
			{
				allMessages.Add("Test completed with errors, details below.");
			}
			else if (hasTestFailedMessages)
			{
				allMessages.Add("Test failed, details below.");
			}
			else
			{
				allMessages.Add("Test completed with warnings, details below.");
			}

			if (hasCriticalMessages)
			{
				allMessages.Add("Errors:");
				allMessages.AddRange(criticalErrorMessages);
			}
			if (hasTestFailedMessages)
			{
				allMessages.Add("Failed assertions:");
				allMessages.AddRange(testFailedMessages);
			}
			if (hasWarnings)
			{
				allMessages.Add("Warnings:");
				allMessages.AddRange(warningMessages);
			}
			if (hasInfo)
			{
				allMessages.Add("Diagnostic messages:");
				allMessages.AddRange(infoMessages);
			}

			var messageText = string.Join(Environment.NewLine, allMessages);
			var runOptions = competitionConfig.Options.RunOptions;
			if (hasCriticalMessages)
			{
				ReportExecutionErrors(messageText, competitionState);
			}
			else if (hasTestFailedMessages || runOptions.ReportWarningsAsErrors)
			{
				ReportAssertionsFailed(messageText, competitionState);
			}
			else
			{
				ReportWarnings(messageText, competitionState);
			}
		}

		private string[] GetMessageLines(
			CompetitionState competitionState,
			Func<IMessage, bool> severityFilter,
			bool fromAllRuns)
		{
			var result =
				from message in competitionState.GetMessages()
				where severityFilter(message) && ShouldReport(message, competitionState.RunNumber, fromAllRuns)
				orderby message.RunNumber, message.RunMessageNumber
				select $"    * Run #{message.RunNumber}: {message.MessageText}";

			return result.ToArray();
		}

		private bool ShouldReport(IMessage message, int runNumber, bool fromAllRuns)
		{
			if (fromAllRuns || message.RunNumber == runNumber)
				return true;

			// all of these on last run only.
			switch (message.MessageSource)
			{
				case MessageSource.Validator:
				case MessageSource.Analyser:
				case MessageSource.Diagnoser:
					return false;
				default:
					return true;
			}
		}
		#endregion
	}
}