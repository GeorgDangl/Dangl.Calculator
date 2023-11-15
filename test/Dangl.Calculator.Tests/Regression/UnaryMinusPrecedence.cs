using Xunit;

namespace Dangl.Calculator.Tests.Regression
{
    public class UnaryMinusPrecedence
    {
        // See GitHub Issue #1
        // https://github.com/GeorgDangl/Dangl.Calculator/issues/1

        [Fact]
        public void EvaluatesUnaryMinusCorrect()
        {
            var formula = "-3+5";
            var expectedResult = 2;
            var calculationResult = Calculator.Calculate(formula);
            Assert.True(calculationResult.IsValid);
            Assert.Equal(-1, calculationResult.ErrorPosition);
            Assert.True(string.IsNullOrWhiteSpace(calculationResult.ErrorMessage));
            Assert.Equal(expectedResult, calculationResult.Result, 7);
        }

        [Theory]
        [InlineData("-3+5", 2)]
        [InlineData("(-3)+5", 2)]
        [InlineData("-(3+5)", -8)]
        [InlineData("-3^2", 9)]
        [InlineData("15-3^2", 6)]
        [InlineData("6-3^2", -3)]
        public void UnaryPrecedence_WithDefaults(string formula, int expectedResult)
        {
            var calculationResult = Calculator.Calculate(formula);
            Assert.True(calculationResult.IsValid);
            Assert.Equal(-1, calculationResult.ErrorPosition);
            Assert.True(string.IsNullOrWhiteSpace(calculationResult.ErrorMessage));
            Assert.Equal(expectedResult, calculationResult.Result, 7);
        }

        [Theory]
        [InlineData("-3+5", 2)]
        [InlineData("(-3)+5", 2)]
        [InlineData("-(3+5)", -8)]
        [InlineData("-3^2", -9)]
        [InlineData("15-3^2", 6)]
        [InlineData("6-3^2", -3)]
        public void UnaryPrecedence_WithSpecialHandlingOfFirstPArt(string formula, int expectedResult)
        {
            var calculationResult = Calculator.Calculate(formula, new CalculatorOptions { DetectUnaryMinusInPowersAtStartOfFormula = true });
            Assert.True(calculationResult.IsValid);
            Assert.Equal(-1, calculationResult.ErrorPosition);
            Assert.True(string.IsNullOrWhiteSpace(calculationResult.ErrorMessage));
            Assert.Equal(expectedResult, calculationResult.Result, 7);
        }

        [Theory]
        [InlineData("-25,00^0,5", double.NaN, false)]
        [InlineData("-25,00^0,5", -5, true)]
        [InlineData("25,00^0,5", 5, false)]
        [InlineData("25,00^0,5", 5, true)]
        public void OtherTests(string formula, double expectedResult, bool detectUnaryMinusAtStartOfFormula)
        {
            var calculationResult = Calculator.Calculate(formula, new CalculatorOptions { DetectUnaryMinusInPowersAtStartOfFormula = detectUnaryMinusAtStartOfFormula });
            Assert.True(calculationResult.IsValid);
            Assert.Equal(-1, calculationResult.ErrorPosition);
            Assert.True(string.IsNullOrWhiteSpace(calculationResult.ErrorMessage));
            Assert.Equal(expectedResult, calculationResult.Result, 7);
        }
    }
}
