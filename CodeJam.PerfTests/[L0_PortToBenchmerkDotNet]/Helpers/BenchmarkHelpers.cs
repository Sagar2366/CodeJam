﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Extensions;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

using CodeJam;

using JetBrains.Annotations;

// ReSharper disable once CheckNamespace

namespace BenchmarkDotNet.Helpers
{
	// TODO: move to different classes
	/// <summary>
	/// Helper methods for benchmark infrastructure.
	/// </summary>
	[PublicAPI]
	public static class BenchmarkHelpers
	{
		#region BenchmarkCase-related
		/// <summary>Returns benchmark types from the assembly.</summary>
		/// <param name="assembly">The assembly to get benchmarks from.</param>
		/// <returns>BenchmarkCase types from the assembly</returns>
		public static Type[] GetBenchmarkTypes([NotNull] Assembly assembly) =>
			// Use reflection for a more maintainable way of creating the benchmark switcher,
			// Benchmarks are listed in namespace order first (e.g. BenchmarkDotNet.Samples.CPU,
			// BenchmarkDotNet.Samples.IL, etc) then by name, so the output is easy to understand.
			assembly
				.GetTypes()
				.Where(
					t => t.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public)
						.Any(m => m.GetCustomAttributes(true).OfType<BenchmarkAttribute>().Any()))
				.Where(t => !t.GetTypeInfo().IsGenericType && !t.IsAbstract)
				.OrderBy(t => t.Namespace)
				.ThenBy(t => t.Name)
				.ToArray();

		/// <summary>Creates read-only wrapper for the config.</summary>
		/// <param name="config">The config to wrap.</param>
		/// <returns>Read-only wrapper for the config.</returns>
		public static IConfig AsReadOnly(this IConfig config) => new ReadOnlyConfig(config);

		#region Selects
		/// <summary>Returns benchmarks for the summary sorted by execution order.</summary>
		/// <param name="summary">Summary for the run.</param>
		/// <returns>Benchmarks for the summary.</returns>
		public static IEnumerable<BenchmarkCase> GetExecutionOrderBenchmarksCases([NotNull] this Summary summary)
		{
			var orderProvider = summary.Config.GetOrderer() ?? DefaultOrderer.Instance;
			return orderProvider.GetExecutionOrder(summary.BenchmarksCases);
		}

		/// <summary>Returns benchmarks for the summary sorted by display order.</summary>
		/// <param name="summary">Summary for the run.</param>
		/// <returns>Benchmarks for the summary.</returns>
		public static IEnumerable<BenchmarkCase> GetSummaryOrderBenchmarksCases([NotNull] this Summary summary)
		{
			var orderProvider = summary.Config.GetOrderer() ?? DefaultOrderer.Instance;
			return orderProvider.GetSummaryOrder(summary.BenchmarksCases, summary);
		}

		/// <summary>Returns the baseline for the benchmark.</summary>
		/// <param name="summary">Summary for the run.</param>
		/// <param name="benchmark">The benchmark.</param>
		/// <returns>Baseline for the benchmark</returns>
		// TODO: migrate: review baseline logic
		public static BenchmarkCase TryGetBaseline(
			[NotNull] this Summary summary,
			[NotNull] BenchmarkCase benchmark) =>
				summary.GetBaseline(summary.GetLogicalGroupKey(benchmark));

		/// <summary>Gets the benchmark descriptors.</summary>
		/// <param name="summary">Summary for the run.</param>
		/// <returns></returns>
		public static Descriptor[] GetBenchmarkDescriptors(this Summary summary) =>
			summary.GetSummaryOrderBenchmarksCases()
				.Select(b => b.Descriptor)
				.Distinct()
				.ToArray();
		#endregion

		/// <summary>Returns a <see cref="TimeSpan" /> that represents a specified number of nanoseconds.</summary>
		/// <param name="nanoseconds">A number of nanoseconds. </param>
		/// <returns>An object that represents <paramref name="nanoseconds" />.</returns>
		public static TimeSpan TimeSpanFromNanoseconds(long nanoseconds) =>
			new TimeSpan((long)(nanoseconds / (1.0e9 / TimeSpan.TicksPerSecond)));

		/// <summary>Returns a <see cref="TimeSpan" /> that represents a specified number of microseconds.</summary>
		/// <param name="microseconds">A number of microseconds. </param>
		/// <returns>An object that represents <paramref name="microseconds" />.</returns>
		public static TimeSpan TimeSpanFromMicroseconds(double microseconds) =>
			TimeSpanFromNanoseconds((long)(microseconds * 1000.0));

		/// <summary>Gets the value of the current TimeSpan structure expressed in nanoseconds.</summary>
		/// <param name="timeSpan">The timespan.</param>
		/// <returns>The total number of nanoseconds represented by this instance.</returns>
		public static double TotalNanoseconds(this TimeSpan timeSpan) => timeSpan.Ticks * (1.0e9 / TimeSpan.TicksPerSecond);

		/// <summary>Gets the value of the current TimeSpan structure expressed in microseconds.</summary>
		/// <param name="timeSpan">The timespan.</param>
		/// <returns>The total number of microseconds represented by this instance.</returns>
		public static double TotalMicroseconds(this TimeSpan timeSpan) => timeSpan.TotalNanoseconds() / 1000;
		#endregion

		#region Priority & affinity
		/// <summary>Sets highest possible priority and Changes the cpu affinity of the process.</summary>
		/// <param name="processorAffinity">The processor affinity.</param>
		/// <param name="logger">The logger.</param>
		public static IDisposable SetupHighestPriorityScope(
			[CanBeNull] IntPtr? processorAffinity,
			[NotNull] ILogger logger)
		{
			var process = Process.GetCurrentProcess();
			var thread = Thread.CurrentThread;

			var oldThreadPriority = thread.Priority;
			var oldProcessPriority = process.PriorityClass;
			var oldAffinity = process.TryGetAffinity();

			process.TrySetPriority(ProcessPriorityClass.RealTime, logger);
			if (processorAffinity.HasValue && oldAffinity.HasValue)
			{
				process.TrySetAffinity(processorAffinity.Value, logger);
			}
			thread.TrySetPriority(ThreadPriority.Highest, logger);

			return Disposable.Create(
				() =>
				{
					thread.TrySetPriority(oldThreadPriority, logger);
					if (processorAffinity.HasValue && oldAffinity.HasValue)
					{
						process.TrySetAffinity(oldAffinity.Value, logger);
					}
					process.TrySetPriority(oldProcessPriority, logger);
				});
		}

#if CORE || !TARGETS_NET // WORKAROUND for missing TrySetPriority in BDN for .Net Core
		public static bool TrySetPriority(
			this Thread thread,
			ThreadPriority priority,
			ILogger logger)
		{
			if (thread == null)
				throw new ArgumentNullException(nameof(thread));
			if (logger == null)
				throw new ArgumentNullException(nameof(logger));

			try
			{
				thread.Priority = priority;
				return true;
			}
			catch (Exception ex)
			{
				logger.WriteLineError(
					$"// ! Failed to set up priority {priority} for thread {thread}. Make sure you have the right permissions. Message: {ex.Message}");
			}

			return false;

		}
#endif
		#endregion
	}
}