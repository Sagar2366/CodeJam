﻿using System;
using System.Reflection;

using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;

using CodeJam.PerfTests.Configs;
using CodeJam.PerfTests.Configs.Factories;

namespace CodeJam.PerfTests
{
	internal class SelfTestConfigFactory : CompetitionConfigFactory
	{
		public SelfTestConfigFactory() : base("SelfTestConfig") { }

		protected override void CompleteFeatures(CompetitionFeatures competitionFeatures)
		{
			base.CompleteFeatures(competitionFeatures);
			if (competitionFeatures.TargetPlatform == null)
				competitionFeatures.TargetPlatform = Platform.X64;
		}

		protected override Job CreateJobUnfrozen(string jobId, ICustomAttributeProvider metadataSource, CompetitionFeatures competitionFeatures)
		{
			var result = base.CreateJobUnfrozen(jobId, metadataSource, competitionFeatures);

			result.Apply(
					new RunMode
					{
						LaunchCount = 1,
						WarmupCount = 2,
						TargetCount = 2
					},
					new EnvMode
					{
						Affinity = new IntPtr(-1)
					});

			return result;
		}

		protected override CompetitionOptions CreateCompetitionOptionsUnfrozen(
			ICustomAttributeProvider metadataSource, CompetitionFeatures competitionFeatures)
		{
			var result = base.CreateCompetitionOptionsUnfrozen(metadataSource, competitionFeatures);

			result.RunOptions.AllowDebugBuilds = true;

			return result;
		}
	}

	internal sealed class CompetitionHighAccuracyModifier : ICompetitionModifier
	{
		/// <summary>Updates competition config.</summary>
		/// <param name="competitionConfig">The competition config.</param>
		public void Modify(ManualCompetitionConfig competitionConfig) => competitionConfig.ApplyToJobs(new Job()
		{
			Run =
			{
				LaunchCount = 1,
				WarmupCount = 200,
				TargetCount = 500
			}
		});
	}
}