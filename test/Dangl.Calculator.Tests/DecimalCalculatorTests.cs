using System;
using System.Globalization;
using Xunit;

namespace Dangl.Calculator.Tests
{
    public class DecimalCalculatorTests
    {
        [Theory]
        [InlineData(null, "0")]
        [InlineData("", "0")]
        [InlineData(" \r\n\t", "0")]
        [InlineData("0.1+0.2-0.3", "0")]
        [InlineData("9007199254740993-9007199254740992", "1")]
        [InlineData("0.1234567890123456789012345678", "0.1234567890123456789012345678")]
        [InlineData("0.12345678901234567890123456789", "0.1234567890123456789012345679")]
        [InlineData("79228162514264337593543950335", "79228162514264337593543950335")]
        [InlineData("-79228162514264337593543950335", "-79228162514264337593543950335")]
        [InlineData("0.0000000000000000000000000001", "0.0000000000000000000000000001")]
        [InlineData("0.0000000000000000000000000001/10", "0")]
        [InlineData("1/3*3", "0.9999999999999999999999999999")]
        [InlineData("2+3*4", "14")]
        [InlineData("2(3+4)", "14")]
        [InlineData("(2+3)4", "20")]
        [InlineData("[2+3]*{4-1}", "15")]
        [InlineData("+3-(-2)", "5")]
        [InlineData("2**3", "8")]
        [InlineData("2^95", "39614081257132168796771975168")]
        [InlineData("2^-3", "0.125")]
        [InlineData("3^-1", "0.3333333333333333333333333333")]
        [InlineData("2^-94", "0.0000000000000000000000000001")]
        [InlineData("2^-95", "0")]
        [InlineData("0^0", "1")]
        [InlineData("0^2", "0")]
        [InlineData("(-2)^3", "-8")]
        [InlineData("(-2)^-3", "-0.125")]
        [InlineData("(-1)^79228162514264337593543950335", "-1")]
        [InlineData("(-1)^79228162514264337593543950334", "1")]
        [InlineData("1^79228162514264337593543950335", "1")]
        [InlineData("0.5^79228162514264337593543950335", "0")]
        [InlineData("10e-29", "0.0000000000000000000000000001")]
        [InlineData("0.1e+29", "10000000000000000000000000000")]
        [InlineData("1e+28", "10000000000000000000000000000")]
        [InlineData("1e-28", "0.0000000000000000000000000001")]
        [InlineData("1e-100", "0")]
        [InlineData("5e-29", "0")]
        [InlineData("15e-29", "0.0000000000000000000000000002")]
        [InlineData("25e-29", "0.0000000000000000000000000002")]
        [InlineData("-15e-29", "-0.0000000000000000000000000002")]
        [InlineData("0e+79228162514264337593543950335", "0")]
        [InlineData("EEX(-28)", "0.0000000000000000000000000001")]
        [InlineData("EEX(-29)", "0")]
        [InlineData("7 MOD 3", "1")]
        [InlineData("-7%3", "-1")]
        [InlineData("7 DIV 3", "2")]
        [InlineData("-7 DIV 3", "-2")]
        [InlineData("SQR(-0.2)", "0.04")]
        [InlineData("SQRT(4)", "2")]
        [InlineData("SQRT(0)", "0")]
        [InlineData("SQRT(1e-28)", "0.00000000000001")]
        [InlineData("3//27", "3")]
        [InlineData("2~9", "3")]
        [InlineData("3//-8", "-2")]
        [InlineData("-3//8", "0.5")]
        [InlineData("FLOOR(-2.1)", "-3")]
        [InlineData("CEIL(-2.1)", "-2")]
        [InlineData("ABS(-2.1)", "2.1")]
        [InlineData("ROUND(2.5)", "2")]
        [InlineData("ROUND(3.5)", "4")]
        [InlineData("ROUNDK(1.015;2)", "1.02")]
        [InlineData("ROUNDK(1.025;2)", "1.02")]
        [InlineData("ROUNDK(0.1234567890123456789012345678;28)", "0.1234567890123456789012345678")]
        [InlineData("TRUNC(-2.9)", "-2")]
        [InlineData("MIN(0.2;0.1;0.3)", "0.1")]
        [InlineData("MAX(0.2;0.1;0.3)", "0.3")]
        [InlineData("MIN(3)", "3")]
        [InlineData("MAX(3)", "3")]
        [InlineData("PI()", "3.1415926535897932384626433833")]
        [InlineData("e", "2.7182818284590452353602874714")]
        [InlineData("EXP(0)", "1")]
        [InlineData("LN(1)", "0")]
        [InlineData("LOG(1e+28)", "28")]
        [InlineData("LOG(1e-28)", "-28")]
        [InlineData("SIN(0)", "0")]
        [InlineData("COS(0)", "1")]
        [InlineData("TAN(0)", "0")]
        [InlineData("SINH(0)", "0")]
        [InlineData("COSH(0)", "1")]
        [InlineData("TANH(79228162514264337593543950335)", "1")]
        [InlineData("TANH(-79228162514264337593543950335)", "-1")]
        [InlineData("ARCSIN(0)", "0")]
        [InlineData("ARCCOS(1)", "0")]
        [InlineData("ARCTAN(0)", "0")]
        [InlineData("ARCTAN2(0;0)", "0")]
        [InlineData("RAD(180)", "3.1415926535897932384626433833")]
        [InlineData("DEG(PI)", "180")]
        [InlineData("sIn(PI)", "0")]
        [InlineData("1,2+0,3=", "1.5")]
        [InlineData("1/* comment */+2", "3")]
        [InlineData("1'comment'+2", "3")]
        [InlineData("1\"comment\"+2", "3")]
        [InlineData("1+2; ignored #value", "3")]
        [InlineData("10 0+1", "101")]
        [InlineData("a/* comment */bs(-2)", "2")]
        [InlineData(".5+5.", "5.5")]
        public void CalculatesExactDecimalResults(string formula, string expected)
        {
            AssertResult(Calculator.CalculateDecimal(formula), Parse(expected));
        }

        [Theory]
        [InlineData("SQRT(2)", "1.4142135623730950488016887242")]
        [InlineData("3^0.5", "1.7320508075688772935274463415")]
        [InlineData("EXP(1)", "2.7182818284590452353602874714")]
        [InlineData("LN(2)", "0.6931471805599453094172321215")]
        [InlineData("LOG(2)", "0.3010299956639811952137388947")]
        [InlineData("SIN(1)", "0.8414709848078965066525023216")]
        [InlineData("COS(1)", "0.5403023058681397174009366074")]
        [InlineData("TAN(1)", "1.5574077246549022305069748075")]
        [InlineData("COT(1)", "0.6420926159343307030064199866")]
        [InlineData("SINH(1)", "1.1752011936438014568823818506")]
        [InlineData("COSH(1)", "1.5430806348152437784779056208")]
        [InlineData("TANH(1)", "0.7615941559557648881194582826")]
        [InlineData("ARCSIN(0.5)", "0.5235987755982988730771072305")]
        [InlineData("ARCCOS(0.5)", "1.0471975511965977461542144611")]
        [InlineData("ARCTAN(1)", "0.7853981633974483096156608458")]
        [InlineData("ARCCOT(1)", "0.7853981633974483096156608458")]
        [InlineData("ARCTAN2(1;-1)", "2.3561944901923449288469825375")]
        [InlineData("ARCTAN2(-1;-1)", "-2.3561944901923449288469825375")]
        [InlineData("ARCTAN2(-1;1)", "-0.7853981633974483096156608458")]
        [InlineData("ARCSIN(1)", "1.5707963267948966192313216916")]
        [InlineData("ARCSIN(-1)", "-1.5707963267948966192313216916")]
        [InlineData("ARCCOS(-1)", "3.1415926535897932384626433833")]
        [InlineData("ARCTAN2(1;0)", "1.5707963267948966192313216916")]
        [InlineData("ARCTAN2(-1;0)", "-1.5707963267948966192313216916")]
        [InlineData("ARCTAN2(0;-1)", "3.1415926535897932384626433833")]
        [InlineData("EEX(0.5)", "3.1622776601683793319988935444")]
        [InlineData("2e+0.5", "6.3245553203367586639977870889")]
        public void CalculatesScientificFunctionsWithoutDoublePrecisionLoss(string formula, string expected)
        {
            var actual = Calculator.CalculateDecimal(formula);
            Assert.True(actual.IsValid, actual.ErrorMessage);
            Assert.NotNull(actual.Result);
            Assert.InRange(Math.Abs(actual.Result.Value - Parse(expected)), 0m, 0.000000000000000000000000001m);
        }

        [Theory]
        [InlineData("SIN(79228162514264337593543950335)", "0.8753832556517298517600511989")]
        [InlineData("COS(79228162514264337593543950335)", "-0.4834295767995356907034052511")]
        [InlineData("TAN(79228162514264337593543950335)", "-1.8107772003671303142498240912")]
        [InlineData("SQRT(79228162514264337593543950335)", "281474976710656.00000000000000")]
        [InlineData("RAD(79228162514264337593543950335)", "1382792296179061584614010803.4")]
        [InlineData("COSH(67)", "62618158542110689025676098037")]
        [InlineData("SINH(67)", "62618158542110689025676098037")]
        [InlineData("ARCSIN(0.9999999999999999999999999999)", "1.5707963267948824770956979607")]
        [InlineData("ARCCOS(0.9999999999999999999999999999)", "0.0000000000000141421356237310")]
        [InlineData("1.0000000000000000000000000001^79228162514264337593543950335", "2759.5316476365851061797093784")]
        public void PreservesPrecisionAtScientificRangeBoundaries(string formula, string expected)
        {
            AssertResult(Calculator.CalculateDecimal(formula), Parse(expected));
        }

        [Theory]
        [InlineData("79228162514264337593543950336")]
        [InlineData("79228162514264337593543950335+1")]
        [InlineData("2^96")]
        [InlineData("2^79228162514264337593543950335")]
        [InlineData("1e+29")]
        [InlineData("1e+79228162514264337593543950335")]
        [InlineData("EXP(100)")]
        [InlineData("EXP(66.542129333754749704054897448)")]
        [InlineData("SINH(100)")]
        [InlineData("COSH(-100)")]
        [InlineData("1/0")]
        [InlineData("0/0")]
        [InlineData("1%0")]
        [InlineData("1 DIV 0")]
        [InlineData("COT(0)")]
        [InlineData("0^-1")]
        [InlineData("0//4")]
        [InlineData("-2//0")]
        [InlineData("2//-4")]
        [InlineData("(-2)^0.5")]
        [InlineData("SQRT(-1)")]
        [InlineData("LN(0)")]
        [InlineData("LN(-1)")]
        [InlineData("LOG(-1)")]
        [InlineData("ARCSIN(1.1)")]
        [InlineData("ARCCOS(-1.1)")]
        [InlineData("ROUNDK(1;29)")]
        [InlineData("ROUNDK(1;-1)")]
        [InlineData("ROUNDK(1;79228162514264337593543950335)")]
        [InlineData("1+#missing")]
        [InlineData("1+#start..#end")]
        [InlineData("1+")]
        [InlineData("d")]
        [InlineData("(1+2]")]
        public void ReturnsAnExplicitErrorInsteadOfNaNInfinityOrAnException(string formula)
        {
            var actual = Calculator.CalculateDecimal(formula);
            Assert.False(actual.IsValid);
            Assert.Null(actual.Result);
            Assert.True(actual.ErrorPosition >= 0);
            Assert.False(string.IsNullOrWhiteSpace(actual.ErrorMessage));
        }

        [Theory]
        [InlineData("1 + SQRT(-1)", 4)]
        [InlineData("1 + #missing", 4)]
        [InlineData("1 + #start..#end", 4)]
        [InlineData("1+", 2)]
        [InlineData("(1+2]", 4)]
        public void ReportsCharacterPositions(string formula, int position)
        {
            var actual = Calculator.CalculateDecimal(formula);
            Assert.False(actual.IsValid);
            Assert.Equal(position, actual.ErrorPosition);
        }

        [Fact]
        public void ResolvesDecimalSubstitutionsAndRangesWithoutPrecisionLoss()
        {
            var actual = Calculator.CalculateDecimal("#value + #A..#Z",
                name =>
                {
                    Assert.Equal("#value", name);
                    return 0.1234567890123456789012345678m;
                },
                range =>
                {
                    Assert.Equal("#A", range.Start);
                    Assert.Equal("#Z", range.End);
                    return 0.0000000000000000000000000001m;
                });
            AssertResult(actual, 0.1234567890123456789012345679m);
        }

        [Fact]
        public void SupportsTheSubstitutionOnlyOverload()
        {
            AssertResult(Calculator.CalculateDecimal("#value", _ => 0.1m), 0.1m);
        }

        [Fact]
        public void SupportsARangeWithoutASubstitutionResolver()
        {
            AssertResult(Calculator.CalculateDecimal("#A..#Z", null, _ => 0.1m), 0.1m);
        }

        [Theory]
        [InlineData(false, "9")]
        [InlineData(true, "-9")]
        public void PreservesTheUnaryMinusOption(bool detectUnaryMinus, string expected)
        {
            AssertResult(Calculator.CalculateDecimal("-3^2", new DecimalCalculatorOptions
            {
                DetectUnaryMinusInPowersAtStartOfFormula = detectUnaryMinus
            }), Parse(expected));
        }

        [Fact]
        public void AppliesTheUnaryMinusOptionToFractionalPowers()
        {
            AssertResult(Calculator.CalculateDecimal("-25^0.5", new DecimalCalculatorOptions
            {
                DetectUnaryMinusInPowersAtStartOfFormula = true
            }), -5m);
        }

        [Fact]
        public void ResolvesValuesThroughOptions()
        {
            AssertResult(Calculator.CalculateDecimal("#value+#A..#Z", new DecimalCalculatorOptions
            {
                SubstitutionResolver = _ => 0.1m,
                RangeResolver = _ => 0.2m
            }), 0.3m);
        }

        [Fact]
        public void DefaultOptionsReportUnresolvedValues()
        {
            AssertResult(Calculator.CalculateDecimal("0.1+0.2", new DecimalCalculatorOptions()), 0.3m);
            Assert.False(Calculator.CalculateDecimal("#missing", new DecimalCalculatorOptions()).IsValid);
            Assert.False(Calculator.CalculateDecimal("#A..#Z", new DecimalCalculatorOptions()).IsValid);
        }

        [Fact]
        public void RejectsNullOptions()
        {
            Assert.Throws<ArgumentNullException>(() => Calculator.CalculateDecimal("1", (DecimalCalculatorOptions)null));
        }

        [Fact]
        public void DoesNotRetryResolversForNumericErrors()
        {
            var calls = 0;
            var result = Calculator.CalculateDecimal("#value/0", _ =>
            {
                calls++;
                return 1m;
            });
            Assert.False(result.IsValid);
            Assert.Equal(1, calls);
        }

        [Fact]
        public void DoesNotHideExceptionsThrownByResolvers()
        {
            var error = new DivideByZeroException("Resolver failure");
            Assert.Same(error, Assert.Throws<DivideByZeroException>(() =>
                Calculator.CalculateDecimal("SQR(#value)+1", _ => throw error)));
            Assert.Same(error, Assert.Throws<DivideByZeroException>(() =>
                Calculator.CalculateDecimal("1+#A..#Z", null, _ => throw error)));
        }

        [Fact]
        public void UsesInvariantParsingRegardlessOfCurrentCulture()
        {
            var original = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");
                AssertResult(Calculator.CalculateDecimal("0.1+0,2"), 0.3m);
            }
            finally
            {
                CultureInfo.CurrentCulture = original;
            }
        }

        [Fact]
        public void LeavesTheDoubleApiUnchanged()
        {
            CalculationResult result = Calculator.Calculate("1/0");
            Assert.False(result.IsValid);
            Assert.Equal(double.PositiveInfinity, result.Result);
        }

        private static decimal Parse(string value)
        {
            return decimal.Parse(value, CultureInfo.InvariantCulture);
        }

        private static void AssertResult(DecimalCalculationResult actual, decimal expected)
        {
            Assert.True(actual.IsValid, actual.ErrorMessage);
            Assert.NotNull(actual.Result);
            Assert.Equal(expected, actual.Result.Value);
            Assert.Equal(-1, actual.ErrorPosition);
            Assert.Null(actual.ErrorMessage);
        }
    }
}
