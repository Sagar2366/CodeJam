﻿using System;

using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains;

using JetBrains.Annotations;

// ReSharper disable CheckNamespace

namespace BenchmarkDotNet.UnitTesting
{
	/// <summary>
	/// Use this to run fast but inaccurate measures
	/// </summary>
	[PublicAPI]
	public class FastRunConfig : ManualConfig
	{
		/// <summary>
		/// Instance of the config
		/// </summary>
		public static readonly IConfig Instance = new FastRunConfig();

		/// <summary>
		/// Constructor
		/// </summary>
		public FastRunConfig()
		{
			Add(
				new Job
				{
					IterationTime = 10,
					LaunchCount = 2,
					WarmupCount = 3,
					TargetCount = 20,
					//Platform = Platform.X64,
					Toolchain = InProcessToolchain.Default
				});
		}
	}

	/// <summary>
	/// Use this to run slower but proven-to-be-accurate perf tests
	/// </summary>
	[PublicAPI]
	public class TestProofConfig : ManualConfig
	{
		/// <summary>
		/// Instance of the config
		/// </summary>
		public static readonly IConfig Instance = new TestProofConfig();

		/// <summary>
		/// Constructor
		/// </summary>
		public TestProofConfig()
		{
			Add(
				new Job
				{
					TargetCount = 10
				});
		}
	}
}