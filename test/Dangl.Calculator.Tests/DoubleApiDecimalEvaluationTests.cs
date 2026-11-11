using System;
using System.Globalization;
using Xunit;

namespace Dangl.Calculator.Tests
{
    public class DoubleApiDecimalEvaluationTests
    {
        [Theory]
        [InlineData("roundk(5,475;2)", 5.48)]
        [InlineData("roundk(1.015;2)", 1.02)]
        [InlineData("roundk(1.025;2)", 1.02)]
        [InlineData("roundk(-5.475;2)", -5.48)]
        [InlineData("0.1+0.2-0.3", 0)]
        [InlineData("(10000000000000000+1)-10000000000000000", 1)]
        [InlineData("1e-29", 0)]
        [InlineData("eex2", 100)]
        [InlineData("3e-2", 0.03)]
        [InlineData("e", 2.718281828459045)]
        [InlineData("10000000000000.099609375", 10000000000000.099609375)]
        public void EvaluatesWithDecimalArithmetic(string formula, double expected)
        {
            AssertResult(Calculator.Calculate(formula), expected);
        }

        [Theory]
        [InlineData("1/0")]
        [InlineData("0/0")]
        [InlineData("sqrt(-1)")]
        [InlineData("log(0)")]
        [InlineData("1e29")]
        [InlineData("79228162514264337593543950335+1")]
        public void ReportsDecimalEvaluationErrors(string formula)
        {
            var expected = Calculator.CalculateDecimal(formula);
            var actual = Calculator.Calculate(formula);

            Assert.False(actual.IsValid);
            Assert.True(double.IsNaN(actual.Result));
            Assert.Equal(expected.ErrorPosition, actual.ErrorPosition);
            Assert.Equal(expected.ErrorMessage, actual.ErrorMessage);
        }

        [Fact]
        public void ConvertsOnlyTheFinalResultToDouble()
        {
            const string formula = "0.1234567890123456789012345678";
            AssertResult(Calculator.Calculate(formula), 0.12345678901234568);
        }

        [Fact]
        public void ConvertsSubstitutionToFifteenSignificantDigits()
        {
            AssertResult(Calculator.Calculate("roundk(#value;2)", _ => 5.475), 5.48);
            AssertResult(Calculator.Calculate("#value", _ => 0.12345678901234567),
                (double)0.123456789012346m);
            AssertResult(Calculator.Calculate("#value", _ => 1e-20), (double)0.00000000000000000001m);
        }

        [Fact]
        public void ConvertsRangeToFifteenSignificantDigits()
        {
            var result = Calculator.Calculate("roundk(#start..#end+#value;2)",
                name => name == "#value" ? (double?)0.1 : null,
                range => range.Start == "#start" && range.End == "#end" ? (double?)5.375 : null);
            AssertResult(result, 5.48);
        }

        [Fact]
        public void AdaptsOptionsResolversAndPrecedence()
        {
            var result = Calculator.Calculate("-2^2+roundk(#value;2)+#start..#end", new CalculatorOptions
            {
                SubstitutionResolver = _ => 5.475,
                RangeResolver = _ => 0.1,
                DetectUnaryMinusInPowersAtStartOfFormula = true
            });
            AssertResult(result, 1.58);
        }

        [Theory]
        [InlineData(double.NaN)]
        [InlineData(double.PositiveInfinity)]
        [InlineData(double.NegativeInfinity)]
        [InlineData(double.MaxValue)]
        [InlineData(double.MinValue)]
        public void RejectsUnrepresentableCallbackValues(double value)
        {
            var substitution = Calculator.Calculate("#value", _ => value);
            var range = Calculator.Calculate("#start..#end", _ => null, _ => value);

            foreach (var result in new[] { substitution, range })
            {
                Assert.False(result.IsValid);
                Assert.True(double.IsNaN(result.Result));
                Assert.Contains("decimal range", result.ErrorMessage);
            }
        }

        [Fact]
        public void DoesNotRetryCallbacksForNumericErrors()
        {
            var calls = 0;
            var result = Calculator.Calculate("#value/0", _ =>
            {
                calls++;
                return 1;
            });
            Assert.False(result.IsValid);
            Assert.True(double.IsNaN(result.Result));
            Assert.Equal(1, calls);
        }

        [Fact]
        public void DoesNotHideExceptionsThrownByCallbacks()
        {
            var error = new OverflowException("Callback failure");
            Assert.Same(error, Assert.Throws<OverflowException>(() =>
                Calculator.Calculate("#value", _ => throw error)));
            Assert.Same(error, Assert.Throws<OverflowException>(() =>
                Calculator.Calculate("#start..#end", _ => null, _ => throw error)));
        }

        [Fact]
        public void ConvertsCallbacksUsingInvariantCulture()
        {
            var original = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");
                AssertResult(Calculator.Calculate("roundk(#value;2)", _ => 5.475), 5.48);
            }
            finally
            {
                CultureInfo.CurrentCulture = original;
            }
        }

        [Fact]
        public void RejectsNullOptions()
        {
            Assert.Throws<ArgumentNullException>(() => Calculator.Calculate("1", (CalculatorOptions)null));
        }

        private static void AssertResult(CalculationResult result, double expected)
        {
            Assert.True(result.IsValid, result.ErrorMessage);
            Assert.Equal(-1, result.ErrorPosition);
            Assert.True(string.IsNullOrWhiteSpace(result.ErrorMessage));
            Assert.Equal(expected, result.Result);
        }
    }
}
