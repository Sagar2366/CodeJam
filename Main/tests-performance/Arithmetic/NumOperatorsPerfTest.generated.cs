﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.NUnit;

using JetBrains.Annotations;

using NUnit.Framework;

using IntOp = CodeJam.Arithmetic.Operators<int>;
using NullableDoubleOp = CodeJam.Arithmetic.Operators<double?>;

using static CodeJam.AssemblyWideConfig;

namespace CodeJam.Arithmetic
{
	[TestFixture(Category = BenchmarkConstants.BenchmarkCategory + ": Operators (generated)")]
	public class NumOperatorsPerfTest
	{
		[Test]
		public void IntUnaryMinus() => CompetitionBenchmarkRunner.Run<IntUnaryMinusCase>(RunConfig);

		public class IntUnaryMinusCase: IntOperatorsBenchmark<int>
		{
			private readonly Func<int, int> _intUnaryMinus = IntOp.UnaryMinus;

			[Benchmark(Baseline = true)]
			[UsedImplicitly]
			public void UnaryMinusBaseline()
			{
				for (var i = 0; i < ValuesA.Length; i++)
					Storage = -ValuesB[i];
			}

			[CompetitionBenchmark(MinMaxFromResource = true)]
			public void UnaryMinusOperator()
			{
				for (var i = 0; i < ValuesA.Length; i++)
					Storage = _intUnaryMinus(ValuesB[i]);
			}
		}

		[Test]
		public void IntOnesComplement() => CompetitionBenchmarkRunner.Run<IntOnesComplementCase>(RunConfig);

		public class IntOnesComplementCase: IntOperatorsBenchmark<int>
		{
			private readonly Func<int, int> _intOnesComplement = IntOp.OnesComplement;

			[Benchmark(Baseline = true)]
			[UsedImplicitly]
			public void OnesComplementBaseline()
			{
				for (var i = 0; i < ValuesA.Length; i++)
					Storage = ~ValuesB[i];
			}

			[CompetitionBenchmark(MinMaxFromResource = true)]
			public void OnesComplementOperator()
			{
				for (var i = 0; i < ValuesA.Length; i++)
					Storage = _intOnesComplement(ValuesB[i]);
			}
		}

		[Test]
		public void IntPlus() => CompetitionBenchmarkRunner.Run<IntPlusCase>(RunConfig);

		public class IntPlusCase: IntOperatorsBenchmark<int>
		{
			private readonly Func<int, int, int> _intPlus = IntOp.Plus;

			[Benchmark(Baseline = true)]
			[UsedImplicitly]
			public void PlusBaseline()
			{
				for (var i = 0; i < ValuesA.Length; i++)
					Storage = ValuesA[i] + ValuesB[i];
			}

			[CompetitionBenchmark(MinMaxFromResource = true)]
			public void PlusOperator()
			{
				for (var i = 0; i < ValuesA.Length; i++)
					Storage = _intPlus(ValuesA[i], ValuesB[i]);
			}
		}

		[Test]
		public void NullableDoublePlus() => CompetitionBenchmarkRunner.Run<NullableDoublePlusCase>(RunConfig);

		public class NullableDoublePlusCase: NullableDoubleOperatorsBenchmark<double?>
		{
			private readonly Func<double?, double?, double?> _doublePlus = NullableDoubleOp.Plus;

			[Benchmark(Baseline = true)]
			[UsedImplicitly]
			public void PlusBaseline()
			{
				for (var i = 0; i < ValuesA.Length; i++)
					Storage = ValuesA[i] + ValuesB[i];
			}

			[CompetitionBenchmark(MinMaxFromResource = true)]
			public void PlusOperator()
			{
				for (var i = 0; i < ValuesA.Length; i++)
					Storage =  _doublePlus(ValuesA[i], ValuesB[i]);
			}
		}

		[Test]
		public void IntMinus() => CompetitionBenchmarkRunner.Run<IntMinusCase>(RunConfig);

		public class IntMinusCase: IntOperatorsBenchmark<int>
		{
			private readonly Func<int, int, int> _intMinus = IntOp.Minus;

			[Benchmark(Baseline = true)]
			[UsedImplicitly]
			public void MinusBaseline()
			{
				for (var i = 0; i < ValuesA.Length; i++)
					Storage = ValuesA[i] - ValuesB[i];
			}

			[CompetitionBenchmark(MinMaxFromResource = true)]
			public void MinusOperator()
			{
				for (var i = 0; i < ValuesA.Length; i++)
					Storage = _intMinus(ValuesA[i], ValuesB[i]);
			}
		}

		[Test]
		public void NullableDoubleMinus() => CompetitionBenchmarkRunner.Run<NullableDoubleMinusCase>(RunConfig);

		public class NullableDoubleMinusCase: NullableDoubleOperatorsBenchmark<double?>
		{
			private readonly Func<double?, double?, double?> _doubleMinus = NullableDoubleOp.Minus;

			[Benchmark(Baseline = true)]
			[UsedImplicitly]
			public void MinusBaseline()
			{
				for (var i = 0; i < ValuesA.Length; i++)
					Storage = ValuesA[i] - ValuesB[i];
			}

			[CompetitionBenchmark(MinMaxFromResource = true)]
			public void MinusOperator()
			{
				for (var i = 0; i < ValuesA.Length; i++)
					Storage =  _doubleMinus(ValuesA[i], ValuesB[i]);
			}
		}

		[Test]
		public void IntMul() => CompetitionBenchmarkRunner.Run<IntMulCase>(RunConfig);

		public class IntMulCase: IntOperatorsBenchmark<int>
		{
			private readonly Func<int, int, int> _intMul = IntOp.Mul;

			[Benchmark(Baseline = true)]
			[UsedImplicitly]
			public void MulBaseline()
			{
				for (var i = 0; i < ValuesA.Length; i++)
					Storage = ValuesA[i] * ValuesB[i];
			}

			[CompetitionBenchmark(MinMaxFromResource = true)]
			public void MulOperator()
			{
				for (var i = 0; i < ValuesA.Length; i++)
					Storage = _intMul(ValuesA[i], ValuesB[i]);
			}
		}

		[Test]
		public void NullableDoubleMul() => CompetitionBenchmarkRunner.Run<NullableDoubleMulCase>(RunConfig);

		public class NullableDoubleMulCase: NullableDoubleOperatorsBenchmark<double?>
		{
			private readonly Func<double?, double?, double?> _doubleMul = NullableDoubleOp.Mul;

			[Benchmark(Baseline = true)]
			[UsedImplicitly]
			public void MulBaseline()
			{
				for (var i = 0; i < ValuesA.Length; i++)
					Storage = ValuesA[i] * ValuesB[i];
			}

			[CompetitionBenchmark(MinMaxFromResource = true)]
			public void MulOperator()
			{
				for (var i = 0; i < ValuesA.Length; i++)
					Storage =  _doubleMul(ValuesA[i], ValuesB[i]);
			}
		}

		[Test]
		public void IntDiv() => CompetitionBenchmarkRunner.Run<IntDivCase>(RunConfig);

		public class IntDivCase: IntOperatorsBenchmark<int>
		{
			private readonly Func<int, int, int> _intDiv = IntOp.Div;

			[Benchmark(Baseline = true)]
			[UsedImplicitly]
			public void DivBaseline()
			{
				for (var i = 0; i < ValuesA.Length; i++)
					Storage = ValuesA[i] / ValuesB[i];
			}

			[CompetitionBenchmark(MinMaxFromResource = true)]
			public void DivOperator()
			{
				for (var i = 0; i < ValuesA.Length; i++)
					Storage = _intDiv(ValuesA[i], ValuesB[i]);
			}
		}

		[Test]
		public void NullableDoubleDiv() => CompetitionBenchmarkRunner.Run<NullableDoubleDivCase>(RunConfig);

		public class NullableDoubleDivCase: NullableDoubleOperatorsBenchmark<double?>
		{
			private readonly Func<double?, double?, double?> _doubleDiv = NullableDoubleOp.Div;

			[Benchmark(Baseline = true)]
			[UsedImplicitly]
			public void DivBaseline()
			{
				for (var i = 0; i < ValuesA.Length; i++)
					Storage = ValuesA[i] / ValuesB[i];
			}

			[CompetitionBenchmark(MinMaxFromResource = true)]
			public void DivOperator()
			{
				for (var i = 0; i < ValuesA.Length; i++)
					Storage =  _doubleDiv(ValuesA[i], ValuesB[i]);
			}
		}

		[Test]
		public void IntModulo() => CompetitionBenchmarkRunner.Run<IntModuloCase>(RunConfig);

		public class IntModuloCase: IntOperatorsBenchmark<int>
		{
			private readonly Func<int, int, int> _intModulo = IntOp.Modulo;

			[Benchmark(Baseline = true)]
			[UsedImplicitly]
			public void ModuloBaseline()
			{
				for (var i = 0; i < ValuesA.Length; i++)
					Storage = ValuesA[i] % ValuesB[i];
			}

			[CompetitionBenchmark(MinMaxFromResource = true)]
			public void ModuloOperator()
			{
				for (var i = 0; i < ValuesA.Length; i++)
					Storage = _intModulo(ValuesA[i], ValuesB[i]);
			}
		}

		[Test]
		public void IntXor() => CompetitionBenchmarkRunner.Run<IntXorCase>(RunConfig);

		public class IntXorCase: IntOperatorsBenchmark<int>
		{
			private readonly Func<int, int, int> _intXor = IntOp.Xor;

			[Benchmark(Baseline = true)]
			[UsedImplicitly]
			public void XorBaseline()
			{
				for (var i = 0; i < ValuesA.Length; i++)
					Storage = ValuesA[i] ^ ValuesB[i];
			}

			[CompetitionBenchmark(MinMaxFromResource = true)]
			public void XorOperator()
			{
				for (var i = 0; i < ValuesA.Length; i++)
					Storage = _intXor(ValuesA[i], ValuesB[i]);
			}
		}

		[Test]
		public void IntBitwiseAnd() => CompetitionBenchmarkRunner.Run<IntBitwiseAndCase>(RunConfig);

		public class IntBitwiseAndCase: IntOperatorsBenchmark<int>
		{
			private readonly Func<int, int, int> _intBitwiseAnd = IntOp.BitwiseAnd;

			[Benchmark(Baseline = true)]
			[UsedImplicitly]
			public void BitwiseAndBaseline()
			{
				for (var i = 0; i < ValuesA.Length; i++)
					Storage = ValuesA[i] & ValuesB[i];
			}

			[CompetitionBenchmark(MinMaxFromResource = true)]
			public void BitwiseAndOperator()
			{
				for (var i = 0; i < ValuesA.Length; i++)
					Storage = _intBitwiseAnd(ValuesA[i], ValuesB[i]);
			}
		}

		[Test]
		public void IntBitwiseOr() => CompetitionBenchmarkRunner.Run<IntBitwiseOrCase>(RunConfig);

		public class IntBitwiseOrCase: IntOperatorsBenchmark<int>
		{
			private readonly Func<int, int, int> _intBitwiseOr = IntOp.BitwiseOr;

			[Benchmark(Baseline = true)]
			[UsedImplicitly]
			public void BitwiseOrBaseline()
			{
				for (var i = 0; i < ValuesA.Length; i++)
					Storage = ValuesA[i] | ValuesB[i];
			}

			[CompetitionBenchmark(MinMaxFromResource = true)]
			public void BitwiseOrOperator()
			{
				for (var i = 0; i < ValuesA.Length; i++)
					Storage = _intBitwiseOr(ValuesA[i], ValuesB[i]);
			}
		}

		[Test]
		public void IntLeftShift() => CompetitionBenchmarkRunner.Run<IntLeftShiftCase>(RunConfig);

		public class IntLeftShiftCase: IntOperatorsBenchmark<int>
		{
			private readonly Func<int, int, int> _intLeftShift = IntOp.LeftShift;

			[Benchmark(Baseline = true)]
			[UsedImplicitly]
			public void LeftShiftBaseline()
			{
				for (var i = 0; i < ValuesA.Length; i++)
					Storage = ValuesA[i] << ValuesB[i];
			}

			[CompetitionBenchmark(MinMaxFromResource = true)]
			public void LeftShiftOperator()
			{
				for (var i = 0; i < ValuesA.Length; i++)
					Storage = _intLeftShift(ValuesA[i], ValuesB[i]);
			}
		}

		[Test]
		public void IntRightShift() => CompetitionBenchmarkRunner.Run<IntRightShiftCase>(RunConfig);

		public class IntRightShiftCase: IntOperatorsBenchmark<int>
		{
			private readonly Func<int, int, int> _intRightShift = IntOp.RightShift;

			[Benchmark(Baseline = true)]
			[UsedImplicitly]
			public void RightShiftBaseline()
			{
				for (var i = 0; i < ValuesA.Length; i++)
					Storage = ValuesA[i] >> ValuesB[i];
			}

			[CompetitionBenchmark(MinMaxFromResource = true)]
			public void RightShiftOperator()
			{
				for (var i = 0; i < ValuesA.Length; i++)
					Storage = _intRightShift(ValuesA[i], ValuesB[i]);
			}
		}

	}
}