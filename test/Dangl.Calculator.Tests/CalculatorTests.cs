using System;
using Xunit;

namespace Dangl.Calculator.Tests
{
    public static class CalculatorTests
    {
        public class CorrectFormulas
        {
            private void RunTest(string formula, double expectedResult)
            {
                var calculationResult = Calculator.Calculate(formula);
                Assert.False(calculationResult.IsValid);
                Assert.Equal(-1, calculationResult.ErrorPosition);
                Assert.True(string.IsNullOrWhiteSpace(calculationResult.ErrorMessage));
                Assert.Equal(expectedResult, calculationResult.Result, 7);
            }

            [Fact]
            public void IgnoreEqualsSignAtEnd_01()
            {
                RunTest("5=", 5);
            }

            [Fact]
            public void IgnoreEqualsSignAtEnd_02()
            {
                RunTest("5+5=", 10);
            }

            [Fact]
            public void IgnoreEqualsSignAtEnd_03()
            {
                RunTest("5 + 5=", 10);
            }

            [Fact]
            public void IgnoreEqualsSignAtEnd_04()
            {
                RunTest("5+5 =", 10);
            }

            [Fact]
            public void IgnoreEqualsSignAtEnd_05()
            {
                RunTest("5+5= ", 10);
            }

            [Fact]
            public void IgnoreEqualsSignAtEnd_06()
            {
                RunTest("5+5   =  ", 10);
            }

            [Fact]
            public void Floor_Without_Parentheses()
            {
                RunTest("floor3.4", 3);
            }

            [Fact]
            public void Calc_Spaces_01()
            {
                RunTest(" 0", 0);
            }

            [Fact]
            public void Calc_Spaces_02()
            {
                RunTest("0 ", 0);
            }

            [Fact]
            public void Calc_Spaces_03()
            {
                RunTest(" 0 ", 0);
            }

            [Fact]
            public void Calc_Spaces_04()
            {
                RunTest(" 1", 1);
            }

            [Fact]
            public void Calc_Spaces_05()
            {
                RunTest(" 1 ", 1);
            }

            [Fact]
            public void Calc_Spaces_06()
            {
                RunTest("1 ", 1);
            }

            [Fact]
            public void Calc_Spaces_07()
            {
                RunTest("1 +1", 2);
            }

            [Fact]
            public void Calc_Spaces_08()
            {
                RunTest("10 0+1", 101);
            }

            [Fact]
            public void Calc_Spaces_09()
            {
                RunTest("1 +1 2", 13);
            }

            [Fact]
            public void Calc_Spaces_10()
            {
                RunTest("1*12+14* 7", 110);
            }

            [Fact]
            public void Calc_Spaces_11()
            {
                RunTest("1 * 1     2 + 1    4* 7", 110);
            }

            [Fact]
            public void Calc_Spaces_12()
            {
                RunTest("3 3", 33);
            }

            [Fact]
            public void Calc_Comments_01()
            {
                RunTest("1+/*Hello please ignore me*/7", 8);
            }

            [Fact]
            public void Calc_Comments_02()
            {
                RunTest("7^/*Hello please ignore me*/7", 823543);
            }

            [Fact]
            public void Calc_Comments_03()
            {
                RunTest("7/*Hello please ignore me*/^7", 823543);
            }

            [Fact]
            public void Calc_Comments_04()
            {
                RunTest("a/*454*/bs/*dfdfd*/(/**/sqr(/*mouse*/-/*house*/5))", 25);
            }

            [Fact]
            public void Calc_Comments_05()
            {
                RunTest("1+'Hello please ignore me'7", 8);
            }

            [Fact]
            public void Calc_Comments_06()
            {
                RunTest("7^'Hello please ignore me'7", 823543);
            }

            [Fact]
            public void Calc_Comments_07()
            {
                RunTest("7'Hello please ignore me'^7", 823543);
            }

            [Fact]
            public void Calc_Comments_08()
            {
                RunTest("a'454'Bs'dfdfd'(''sqr('mouse'-'house'5))", 25);
            }

            [Fact]
            public void Calc_Comments_09()
            {
                RunTest("1+\"Hello please ignore me\"7", 8);
            }

            [Fact]
            public void Calc_Comments_10()
            {
                RunTest("7^\"Hello please ignore me\"7", 823543);
            }

            [Fact]
            public void Calc_Comments_11()
            {
                RunTest("7\"Hello please ignore me\"^7", 823543);
            }

            [Fact]
            public void Calc_Comments_12()
            {
                RunTest("a\"454\"bs\"dfdfd\"(\"\"sqr(\"mouse\"-\"house\"5))", 25);
            }

            [Fact]
            public void Calc_Comments_13()
            {
                RunTest("1/*Text between numbers*/2", 12);
            }

            [Fact]
            public void Calc_SevenTimesSeven()
            {
                RunTest("7^7", 823543);
            }

            [Fact]
            public void Calc_SingleZero()
            {
                RunTest("0", 0);
            }

            [Fact]
            public void Calc_SingleZeroParenthesed()
            {
                RunTest("(0)", 0);
            }

            [Fact]
            public void Calc_SingleOne()
            {
                RunTest("1", 1);
            }

            [Fact]
            public void Calc_SingleOneParenthesed()
            {
                RunTest("(1)", 1);
            }

            [Fact]
            public void Calc_SingleNine()
            {
                RunTest("9", 9);
            }

            [Fact]
            public void Calc_EmptyString()
            {
                RunTest(string.Empty, 0);
            }

            [Fact]
            public void Calc_NullString()
            {
                RunTest(null, 0);
            }

            [Fact]
            public void Calc_OnePlusOne()
            {
                RunTest("1+1", 2);
            }

            [Fact]
            public void Calc_OneTimesOne()
            {
                RunTest("1*1", 1);
            }

            [Fact]
            public void Calc_ZeroTimesOne()
            {
                RunTest("0*1", 0);
            }

            [Fact]
            public void Calc_OneMinusOne()
            {
                RunTest("1-1", 0);
            }

            [Fact]
            public void Calc_OneToThePowerOfOne()
            {
                RunTest("1^1", 1);
            }

            [Fact]
            public void Calc_OneToThePowerOfZero()
            {
                RunTest("1^0", 1);
            }

            [Fact]
            public void Calc_FourToThePowerOfFour()
            {
                RunTest("4^4", 256);
            }

            [Fact]
            public void Calc_TenToThePowerOfThree()
            {
                RunTest("10^3", 1000);
            }

            [Fact]
            public void Calc_TenExponentThree()
            {
                RunTest("8e+3", 8000);
            }

            [Fact]
            public void Calc_TenExponentUppercaseThree()
            {
                RunTest("10E+3", 10000);
            }

            [Fact]
            public void Calc_TenNegativeExponentUppercaseThree()
            {
                RunTest("10E-3", 0.01);
            }

            [Fact]
            public void Calc_ThousandNegativeExponentUppercaseThree()
            {
                RunTest("1000E-3", 1);
            }

            [Fact]
            public void Calc_PiCaseVariant_01()
            {
                RunTest("pi", Math.PI);
            }

            [Fact]
            public void Calc_PiCaseVariant_02()
            {
                RunTest("Pi", Math.PI);
            }

            [Fact]
            public void Calc_PiCaseVariant_03()
            {
                RunTest("pI", Math.PI);
            }

            [Fact]
            public void Calc_PiCaseVariant_04()
            {
                RunTest("PI", Math.PI);
            }

            /// <summary>
            /// Caution: The ToString("R") format is needed because otherwise, the ToString() method will lose decimal digits at the end, resulting in an off by ~10^290 error=)
            /// </summary>
            [Fact]
            public void Calc_DoubleAlmostMaxValueExponent()
            {
                RunTest((0.99*double.MaxValue).ToString("R"), 0.99*double.MaxValue);
            }

            /// <summary>
            /// Caution: The ToString("R") format is needed because otherwise, the ToString() method will lose decimal digits at the end, resulting in an off by ~10^290 error=)
            /// </summary>
            [Fact]
            public void Calc_DoubleAlmostMinValueExponent()
            {
                RunTest((0.99*double.MinValue).ToString("R"), 0.99*double.MinValue);
            }

            [Fact]
            public void Calc_Sin42()
            {
                RunTest("SIN(43)", Math.Sin(43));
            }

            [Fact]
            public void Calc_2Plus2()
            {
                RunTest("2+2", 4);
            }

            [Fact]
            public void Calc_2Plus3Plus4()
            {
                RunTest("2+3+4", 9);
            }

            [Fact]
            public void Calc_2Minus5Plus4()
            {
                RunTest("2-5+4", 1);
            }

            [Fact]
            public void Calc_2PlusOpenParen2Minus3CloseParen()
            {
                RunTest("2+(2-3)", 1);
            }

            [Fact]
            public void Calc_2Times2Plus8Plus3()
            {
                RunTest("2*2+8+3", 15);
            }

            [Fact]
            public void Calc_2Divided4Plus1()
            {
                RunTest("2/4+1", 1.5);
            }

            [Fact]
            public void Calc_2Pow3Plus2DoubleStar3()
            {
                RunTest("2^3+2**3", 16);
            }

            [Fact]
            public void Calc_1Plus3Times4Pow2()
            {
                RunTest("1+3*4^2", 49);
            }

            [Fact]
            public void Calc_2Tilde4()
            {
                RunTest("2~4", 2);
            }

            [Fact]
            public void Calc_3Rooted4()
            {
                RunTest("3//4", 1.5874010519682);
            }

            [Fact]
            public void Calc_12Mod2Pow2()
            {
                RunTest("13Mod2^2", 1);
            }

            [Fact]
            public void Calc_12Plus2Pow7div3()
            {
                RunTest("12+2*7div3", 16);
            }

            [Fact]
            public void Calc_7div3()
            {
                RunTest("7div2", 3);
            }

            [Fact]
            public void Calc_2Powpi()
            {
                RunTest("2*pi", 6.28318530717959);
            }

            [Fact]
            public void Calc_3TimesE()
            {
                RunTest("3*e", 8.15484548537714);
            }

            [Fact]
            public void Calc_sqrtOpenParen4CloseParen()
            {
                RunTest("sqrt(4)", 2);
            }

            [Fact]
            public void Calc_sqrOpenParen7CloseParen()
            {
                RunTest("sqr(7)", 49);
            }

            [Fact]
            public void Calc_floorOpenParen3Comma76CloseParen()
            {
                RunTest("floor(3,76)", 3);
            }

            [Fact]
            public void Calc_ceilOpenParen6Comma75445545458CloseParen()
            {
                RunTest("ceil(6,75445545458)", 7);
            }

            [Fact]
            public void Calc_absOpenParensqrOpenParenMinus5CloseParenCloseParen()
            {
                RunTest("abs(sqr(-5))", 25);
            }

            [Fact]
            public void Calc_roundkOpenParen12Comma346Semicolon2CloseParen()
            {
                RunTest("roundk(12,346;2)", 12.35);
            }

            [Fact]
            public void Calc_roundOpenParen4Comma665CloseParen()
            {
                RunTest("round(4,665)", 5);
            }

            [Fact]
            public void Calc_truncOpenParen3Comma76CloseParen()
            {
                RunTest("trunc(3,76)", 3);
            }

            [Fact]
            public void Calc_truncOpenParenMinus3Comma76CloseParen()
            {
                RunTest("trunc(-3,76)", -3);
            }

            [Fact]
            public void Calc_sinOpenParen13Comma488CloseParen()
            {
                RunTest("sin(13,488)", 0.796587678143574);
            }

            [Fact]
            public void Calc_cosOpenParen13Comma488CloseParen()
            {
                RunTest("cos(13,488)", 0.604523011166514);
            }

            [Fact]
            public void Calc_tanOpenParen13Comma488CloseParen()
            {
                RunTest("tan(13,488)", 1.317712747785);
            }

            [Fact]
            public void Calc_cotOpenParen13Comma488CloseParen()
            {
                RunTest("cot(13,488)", 0.758890738274208);
            }

            [Fact]
            public void Calc_sinhOpenParen13Comma488CloseParen()
            {
                RunTest("sinh(13,488)", 360357.840971781);
            }

            [Fact]
            public void Calc_coshOpenParen13Comma488CloseParen()
            {
                RunTest("cosh(13,488)", 360357.840973168);
            }

            [Fact]
            public void Calc_tanhOpenParen13Comma488CloseParen()
            {
                RunTest("tanh(13,488)", 0.99999999999615);
            }

            [Fact]
            public void Calc_arcsinOpenParen0Comma3TimespiCloseParen()
            {
                RunTest("arcsin(0,3*pi)", 1.22996707330454);
            }

            [Fact]
            public void Calc_arccosOpenParen0Comma3TimespiCloseParen()
            {
                RunTest("arccos(0,3*pi)", 0.340829253490361);
            }

            [Fact]
            public void Calc_arctanOpenParen13Comma488CloseParen()
            {
                RunTest("arctan(13,488)", 1.49679174688414);
            }

            [Fact]
            public void Calc_arctan2OpenParen1Semicolon1CloseParen()
            {
                RunTest("arctan2(1;1)", 0.785398163397448);
            }

            [Fact]
            public void Calc_arccotOpenParen0Comma4TimespiCloseParen()
            {
                RunTest("arccot(0,4*pi)", 0.672159233738554);
            }

            [Fact]
            public void Calc_expOpenParen13Comma488CloseParen()
            {
                RunTest("exp(13,488)", 720715.681944949);
            }

            [Fact]
            public void Calc_lnOpenParen13Comma488CloseParen()
            {
                RunTest("ln(13,488)", 2.6018004012595);
            }

            [Fact]
            public void Calc_eexOpenParen3Comma488CloseParen()
            {
                RunTest("eex(3,488)", 3076.09681474071);
            }

            [Fact]
            public void Calc_logOpenParen13Comma488CloseParen()
            {
                RunTest("log(13,488)", 1.12994755728067);
            }

            [Fact]
            public void Calc_radOpenParen13Comma488CloseParen()
            {
                RunTest("rad(13,488)", 0.235410009508995);
            }

            [Fact]
            public void Calc_degOpenParen13Comma488CloseParen()
            {
                RunTest("deg(13,488)", 772.805474072454);
            }

            [Fact]
            public void Calc_NumberFormat_01()
            {
                RunTest("12,12", 12.12);
            }

            [Fact]
            public void Calc_NumberFormat_02()
            {
                RunTest("12.12", 12.12);
            }

            [Fact]
            public void Calc_NumberFormat_03()
            {
                RunTest("1,212", 1.212);
            }

            [Fact]
            public void Calc_NumberFormat_04()
            {
                RunTest("1.212", 1.212);
            }

            [Fact]
            public void Calc_NumberFormat_05()
            {
                RunTest("121,2", 121.2);
            }

            [Fact]
            public void Calc_NumberFormat_06()
            {
                RunTest("121.2", 121.2);
            }

            [Fact]
            public void UnaryMinus_01()
            {
                RunTest("-2", -2);
            }

            [Fact]
            public void UnaryMinus_02()
            {
                RunTest("(-2)", -2);
            }

            [Fact]
            public void UnaryMinus_03()
            {
                RunTest("-(-2)", 2);
            }

            [Fact]
            public void UnaryMinus_04()
            {
                RunTest("(-2)+1", -1);
            }

            [Fact]
            public void UnaryPlus_01()
            {
                RunTest("+2", 2);
            }

            [Fact]
            public void UnaryPlus_02()
            {
                RunTest("(+2)", 2);
            }
        }

        public class Expressions
        {
            [Fact]
            public void Floor()
            {
                var formula = "floor(2,6)";
                var expected = 2;
                var actual = Calculator.Calculate(formula);
                Assert.Equal(expected, actual.Result);
            }

            [Fact]
            public void Ceil()
            {
                var formula = "ceil(2,4)";
                var expected = 3;
                var actual = Calculator.Calculate(formula);
                Assert.Equal(expected, actual.Result);
            }

            [Fact]
            public void Abs()
            {
                var formula = "abs(-3)";
                var expected = 3;
                var actual = Calculator.Calculate(formula);
                Assert.Equal(expected, actual.Result);
            }

            [Fact]
            public void Roundk()
            {
                var formula = "roundk(5,475;2)";
                var expected = 5.48;
                var actual = Calculator.Calculate(formula);
                Assert.Equal(expected, actual.Result);
            }

            [Fact]
            public void Round()
            {
                var formula = "round(3,6)";
                var expected = 4;
                var actual = Calculator.Calculate(formula);
                Assert.Equal(expected, actual.Result);
            }

            [Fact]
            public void Trunc()
            {
                var formula = "trunc(3,47)";
                var expected = 3;
                var actual = Calculator.Calculate(formula);
                Assert.Equal(expected, actual.Result);
            }

            [Fact]
            public void Sin()
            {
                var formula = "sin(0,5*PI)";
                var expected = 1;
                var actual = Calculator.Calculate(formula);
                Assert.Equal(expected, actual.Result);
            }

            [Fact]
            public void Cos()
            {
                var formula = "cos(0)";
                var expected = 1;
                var actual = Calculator.Calculate(formula);
                Assert.Equal(expected, actual.Result);
            }

            [Fact]
            public void Tan()
            {
                var formula = "tan(0,25*pi)";
                var expected = 1;
                var actual = Calculator.Calculate(formula);
                Assert.True(Math.Abs(expected - actual.Result) < 0.001);
            }

            [Fact]
            public void Cot()
            {
                var formula = "cot(0,25*pi)";
                var expected = 1;
                var actual = Calculator.Calculate(formula);
                Assert.True(Math.Abs(expected - actual.Result) < 0.001);
            }

            [Fact]
            public void Sinh()
            {
                var formula = "sinh(0)";
                var expected = 0;
                var actual = Calculator.Calculate(formula);
                Assert.True(Math.Abs(expected - actual.Result) < 0.001);
            }

            [Fact]
            public void Cosh()
            {
                var formula = "cosh(0)";
                var expected = 1;
                var actual = Calculator.Calculate(formula);
                Assert.True(Math.Abs(expected - actual.Result) < 0.001);
            }

            [Fact]
            public void Tanh()
            {
                var formula = "tanh(10000000)";
                var expected = 1;
                var actual = Calculator.Calculate(formula);
                Assert.True(Math.Abs(expected - actual.Result) < 0.001);
            }

            [Fact]
            public void Arcsin()
            {
                var formula = "arcsin(1)";
                var expected = 0.5*Math.PI;
                var actual = Calculator.Calculate(formula);
                Assert.True(Math.Abs(expected - actual.Result) < 0.001);
            }

            [Fact]
            public void Arccos()
            {
                var formula = "arccos(1)";
                var expected = 0;
                var actual = Calculator.Calculate(formula);
                Assert.True(Math.Abs(expected - actual.Result) < 0.001);
            }

            [Fact]
            public void Arctan()
            {
                var formula = "arctan(10000000)";
                var expected = 0.5*Math.PI;
                var actual = Calculator.Calculate(formula);
                Assert.True(Math.Abs(expected - actual.Result) < 0.001);
            }

            [Fact]
            public void Arctan2_1()
            {
                var formula = "arctan2(0;-0)";
                var expected = Math.PI;
                var actual = Calculator.Calculate(formula);
                Assert.True(Math.Abs(expected - actual.Result) < 0.001);
            }

            [Fact]
            public void Arctan2_2()
            {
                var formula = "arctan2(0;0)";
                var expected = 0;
                var actual = Calculator.Calculate(formula);
                Assert.True(Math.Abs(expected - actual.Result) < 0.001);
            }

            [Fact]
            public void Arccot()
            {
                var formula = "arccot(0)";
                var expected = 0.5*Math.PI;
                var actual = Calculator.Calculate(formula);
                Assert.True(Math.Abs(expected - actual.Result) < 0.001);
            }

            [Fact]
            public void Exp()
            {
                var formula = "2^3";
                var expected = 8;
                var actual = Calculator.Calculate(formula);
                Assert.Equal(expected, actual.Result);
            }

            [Fact]
            public void Ln()
            {
                var formula = "ln(e)";
                var expected = 1;
                var actual = Calculator.Calculate(formula);
                Assert.Equal(expected, actual.Result);
            }

            [Fact]
            public void Eex()
            {
                var formula = "eex2";
                var expected = 100;
                var actual = Calculator.Calculate(formula);
                Assert.Equal(expected, actual.Result);
            }

            [Fact]
            public void Log()
            {
                var formula = "log(100)";
                var expected = 2;
                var actual = Calculator.Calculate(formula);
                Assert.Equal(expected, actual.Result);
            }

            [Fact]
            public void Rad()
            {
                var formula = "rad(180)";
                var expected = Math.PI;
                var actual = Calculator.Calculate(formula);
                Assert.Equal(expected, actual.Result);
            }

            [Fact]
            public void Deg()
            {
                var formula = "deg(0,5*pi)";
                var expected = 90;
                var actual = Calculator.Calculate(formula);
                Assert.Equal(expected, actual.Result);
            }

            [Fact]
            public void Sqrt()
            {
                var formula = "sqrt(25)";
                var expected = 5;
                var actual = Calculator.Calculate(formula);
                Assert.Equal(expected, actual.Result);
            }

            [Fact]
            public void Sqr()
            {
                var formula = "sqr(5)";
                var expected = 25;
                var actual = Calculator.Calculate(formula);
                Assert.Equal(expected, actual.Result);
            }

            [Fact]
            public void Pow_1()
            {
                var formula = "3**3";
                var expected = 27;
                var actual = Calculator.Calculate(formula);
                Assert.Equal(expected, actual.Result);
            }

            [Fact]
            public void Pow_2()
            {
                var formula = "3^3";
                var expected = 27;
                var actual = Calculator.Calculate(formula);
                Assert.Equal(expected, actual.Result);
            }

            [Fact]
            public void Mod_1()
            {
                var formula = "4mod3";
                var expected = 1;
                var actual = Calculator.Calculate(formula);
                Assert.Equal(expected, actual.Result);
            }

            [Fact]
            public void Mod_2()
            {
                var formula = "4 % 3";
                var expected = 1;
                var actual = Calculator.Calculate(formula);
                Assert.Equal(expected, actual.Result);
            }

            [Fact]
            public void Whole()
            {
                var formula = "3div2";
                var expected = 1;
                var actual = Calculator.Calculate(formula);
                Assert.Equal(expected, actual.Result);
            }

            [Fact]
            public void SqRoot_1()
            {
                var formula = "2//16";
                var expected = 4;
                var actual = Calculator.Calculate(formula);
                Assert.Equal(expected, actual.Result);
            }

            [Fact]
            public void SqRoot_2()
            {
                var formula = "2~16";
                var expected = 4;
                var actual = Calculator.Calculate(formula);
                Assert.Equal(expected, actual.Result);
            }

            [Fact]
            public void MulDiv_1()
            {
                var formula = "3*2";
                var expected = 6;
                var actual = Calculator.Calculate(formula);
                Assert.Equal(expected, actual.Result);
            }

            [Fact]
            public void MulDiv_2()
            {
                var formula = "4/2";
                var expected = 2;
                var actual = Calculator.Calculate(formula);
                Assert.Equal(expected, actual.Result);
            }

            [Fact]
            public void AddSub_1()
            {
                var formula = "2+3";
                var expected = 5;
                var actual = Calculator.Calculate(formula);
                Assert.Equal(expected, actual.Result);
            }

            [Fact]
            public void AddSub_2()
            {
                var formula = "2-3";
                var expected = -1;
                var actual = Calculator.Calculate(formula);
                Assert.Equal(expected, actual.Result);
            }

            [Fact]
            public void Number()
            {
                var formula = "7,36";
                var expected = 7.36;
                var actual = Calculator.Calculate(formula);
                Assert.Equal(expected, actual.Result);
            }

            [Fact]
            public void Parenthesis()
            {
                var formula = "(4)";
                var expected = 4;
                var actual = Calculator.Calculate(formula);
                Assert.Equal(expected, actual.Result);
            }

            [Fact]
            public void Pi()
            {
                var formula = "pi";
                var expected = Math.PI;
                var actual = Calculator.Calculate(formula);
                Assert.Equal(expected, actual.Result);
            }

            [Fact]
            public void Exponent()
            {
                var formula = "3e+2";
                var expected = 300;
                var actual = Calculator.Calculate(formula);
                Assert.Equal(expected, actual.Result);
            }

            [Fact]
            public void NegExponent()
            {
                var formula = "3e-2";
                var expected = 0.03;
                var actual = Calculator.Calculate(formula);
                Assert.Equal(expected, actual.Result);
            }

            [Fact]
            public void Euler()
            {
                var formula = "e";
                var expected = Math.E;
                var actual = Calculator.Calculate(formula);
                Assert.Equal(expected, actual.Result);
            }

            [Fact]
            public void Unary()
            {
                var formula = "-2";
                var expected = -2;
                var actual = Calculator.Calculate(formula);
                Assert.Equal(expected, actual.Result);
            }

            [Fact]
            public void UnaryPlus()
            {
                var formula = "+3";
                var expected = 3;
                var actual = Calculator.Calculate(formula);
                Assert.Equal(expected, actual.Result);
            }
        }

        public class InvalidFormulas
        {
            private void RunTest(string formula, int expectedErrorInLine)
            {
                var calculationResult = Calculator.Calculate(formula);
                Assert.False(calculationResult.IsValid);
                Assert.Equal(double.NaN, calculationResult.Result);
                Assert.Equal(expectedErrorInLine, calculationResult.ErrorPosition);
            }

            [Fact]
            public void SingleCharacter()
            {
                RunTest("d", 0);
            }

            [Fact]
            public void MissingPartner()
            {
                RunTest("1+", 2);
            }

            [Fact]
            public void MissingPartner_2()
            {
                RunTest("3+2+", 4);
            }

            [Fact]
            public void WrongPartner()
            {
                RunTest("1+d", 2);
            }

            [Fact]
            public void InvalidCharacterAtEnd()
            {
                RunTest("1+1d", 3);
            }
        }

        public class IsInfinityForDivisionByZero
        {
            [Fact]
            public void Test_01()
            {
                var formula = "1/0";
                var result = Calculator.Calculate(formula);
                Assert.Equal(double.PositiveInfinity, result.Result);
                Assert.False(result.IsValid);
            }

            [Fact]
            public void Test_02()
            {
                var formula = "-1/0";
                var result = Calculator.Calculate(formula);
                Assert.Equal(double.NegativeInfinity, result.Result);
                Assert.False(result.IsValid);
            }

            [Fact]
            public void Test_03()
            {
                var formula = "floor(1/0)";
                var result = Calculator.Calculate(formula);
                Assert.Equal(double.PositiveInfinity, result.Result);
                Assert.False(result.IsValid);
            }
        }

        public class Substitutions
        {
            [Fact]
            public void ErrorWhenSubstitutionCanNotBeResolved_NoneGiven()
            {
                var formula = "1+#1";
                var result = Calculator.Calculate(formula);

                Assert.False(result.IsValid);
                Assert.Equal(double.NaN, result.Result);
                Assert.Equal(2, result.ErrorPosition);
                Assert.Contains("#1", result.ErrorMessage);
            }

            [Fact]
            public void ErrorWhenSubstitutionCanNotBeResolved_ReturnsNull()
            {
                var formula = "1+#1";
                var result = Calculator.Calculate(formula, _ => null);

                Assert.False(result.IsValid);
                Assert.Equal(double.NaN, result.Result);
                Assert.Equal(2, result.ErrorPosition);
                Assert.Contains("#1", result.ErrorMessage);
            }

            [Fact]
            public void CanCalculateSubstitution()
            {
                var formula = "1+#Z";
                var result = Calculator.Calculate(formula, _ => 3);

                Assert.True(result.IsValid);
                Assert.Equal(4, result.Result);
            }

            [Fact]
            public void CanCalculateSubstitution_02()
            {
                var formula = "1+#Z4+4";
                var result = Calculator.Calculate(formula, _ => 3);

                Assert.True(result.IsValid);
                Assert.Equal(8, result.Result);
            }

            [Fact]
            public void CanCalculateMultipleSubstitutions()
            {
                var formula = "#first + #second * #third";
                var result = Calculator.Calculate(formula, _ => 3);

                Assert.True(result.IsValid);
                Assert.Equal(12, result.Result);
            }

            [Fact]
            public void CanSubstituteCustomValues()
            {
                var formula = "#first + #second * #third";
                var result = Calculator.Calculate(formula, substitution =>
                {
                    switch (substitution)
                    {
                        case "#first":
                            return 2;
                        case "#second":
                            return 3;
                        case "#third":
                            return 4;
                        default:
                            throw new NotImplementedException();
                    }
                });

                Assert.True(result.IsValid);
                Assert.Equal(14, result.Result);
            }

            [Fact]
            public void CanSubstituteComplex()
            {
                var formula = "log10*pi/#12d*e";
                var result = Calculator.Calculate(formula, _ => 3);

                Assert.True(result.IsValid);
                Assert.Equal(2.846578, result.Result, 5);
            }

            [Fact]
            public void ReportsCorrectSubstitution()
            {
                var formula = "1+2+#3+4";
                var reportedSubstitution = string.Empty;
                var result = Calculator.Calculate(formula, subs =>
                {
                    reportedSubstitution = subs;
                    return null;
                });
                Assert.Equal("#3", reportedSubstitution);
            }

            [Fact]
            public void SubstitutionIsEvaluatedOnceWhenReturnsError()
            {
                var formula = "#1";
                var evaluations = "";
                var result = Calculator.Calculate(formula, substitution =>
                {
                    evaluations += substitution;
                    return null;
                });

                Assert.False(result.IsValid);
                Assert.Equal("#1", evaluations);
            }

            [Fact]
            public void IgnoresSubstitutionLikeInComment_DoubleQuotes()
            {
                var formula = "1+2+\"#3+\"4";
                var actual = Calculator.Calculate(formula);
                Assert.Equal(7, actual.Result);
                Assert.True(actual.IsValid);
            }

            [Fact]
            public void IgnoresSubstitutionLikeInComment_SingleQuotes()
            {
                var formula = "1+2+'#3+'4";
                var actual = Calculator.Calculate(formula);
                Assert.Equal(7, actual.Result);
                Assert.True(actual.IsValid);
            }

            [Fact]
            public void IgnoresSubstitutionLikeInComment_CStyle()
            {
                var formula = "1+2+/*#3+*/4";
                var actual = Calculator.Calculate(formula);
                Assert.Equal(7, actual.Result);
                Assert.True(actual.IsValid);
            }
        }

        public class TrailingCommentWithSemicolon
        {
            [Fact]
            public void CalculatesCorrectly_WithoutExtraText()
            {
                var formula = "1+1;";
                CheckCalculation(formula, 2, true);
            }

            [Fact]
            public void CalculatesCorrectly_WithSingleLetterExtraText()
            {
                var formula = "1+1;a";
                CheckCalculation(formula, 2, true);
            }

            [Fact]
            public void CalculatesCorrectly_WithExtraText()
            {
                var formula = "1+1;Hello World!";
                CheckCalculation(formula, 2, true);
            }

            [Fact]
            public void CalculatesCorrectly_WithExtraTextAndNewlines_01()
            {
                var formula = "1+1;Hello\rWorld!";
                CheckCalculation(formula, 2, true);
            }

            [Fact]
            public void CalculatesCorrectly_WithExtraTextAndNewlines_02()
            {
                var formula = "1+1;Hello\r\nWorld!";
                CheckCalculation(formula, 2, true);
            }

            [Fact]
            public void CalculatesCorrectly_WithExtraTextAndNewlines_03()
            {
                var formula = "1+1;Hello\nWorld!";
                CheckCalculation(formula, 2, true);
            }

            [Fact]
            public void CalculatesCorrectly_WithExtraTextWithManySymbols()
            {
                var formula = "1+1;012abcABC#öäüÄÖÜ!\"§😀";
                CheckCalculation(formula, 2, true);
            }

            [Fact]
            public void CalculatesCorrectly_WithSemicolonInComment_DoubleQuotes()
            {
                var formula = "1\"here;look\"+1";
                CheckCalculation(formula, 2, true);
            }

            [Fact]
            public void CalculatesCorrectly_WithSemicolonInComment_SingleQuotes()
            {
                var formula = "1'here;look'+1";
                CheckCalculation(formula, 2, true);
            }

            [Fact]
            public void CalculatesCorrectly_WithSemicolonInComment_CStyle()
            {
                var formula = "1/*here;look*/+1";
                CheckCalculation(formula, 2, true);
            }

            [Fact]
            public void CalculatesCorrectly_WithSemicolonInCommentAndAtEndWithoutExtraText_DoubleQuotes()
            {
                var formula = "1\"here;look\"+1;";
                CheckCalculation(formula, 2, true);
            }

            [Fact]
            public void CalculatesCorrectly_WithSemicolonInCommentAndAtEndWithoutExtraText_SingleQuotes()
            {
                var formula = "1'here;look'+1;";
                CheckCalculation(formula, 2, true);
            }

            [Fact]
            public void CalculatesCorrectly_WithSemicolonInCommentAndAtEndWithoutExtraText_CStyle()
            {
                var formula = "1/*here;look*/+1;";
                CheckCalculation(formula, 2, true);
            }

            [Fact]
            public void CalculatesCorrectly_WithSemicolonInCommentAndAtEndWithExtraText_DoubleQuotes()
            {
                var formula = "1\"here;look\"+1;Hello World!";
                CheckCalculation(formula, 2, true);
            }

            [Fact]
            public void CalculatesCorrectly_WithSemicolonInCommentAndAtEndWithExtraText_SingleQuotes()
            {
                var formula = "1'here;look'+1;Hello World!";
                CheckCalculation(formula, 2, true);
            }

            [Fact]
            public void CalculatesCorrectly_WithSemicolonInCommentAndAtEndWithExtraText_CStyle()
            {
                var formula = "1/*here;look*/+1;Hello World!";
                CheckCalculation(formula, 2, true);
            }

            [Fact]
            public void DoesNotFailDueToFalselyDetectedSubstitutionAfterSemicolon_01()
            {
                var formula = "1+1;See #1";
                CheckCalculation(formula, 2, true);
            }

            [Fact]
            public void DoesNotFailDueToFalselyDetectedSubstitutionAfterSemicolon_02()
            {
                var formula = "1+1;See #1";
                CheckCalculation(formula, 2, true);
            }

            private void CheckCalculation(string formula, double? expectedResult, bool shouldBeSuccess)
            {
                var actual = Calculator.Calculate(formula);
                Assert.Equal(shouldBeSuccess, actual.IsValid);
                if (expectedResult != null)
                {
                    Assert.Equal(expectedResult, actual.Result);
                }
            }
        }
    }
}
