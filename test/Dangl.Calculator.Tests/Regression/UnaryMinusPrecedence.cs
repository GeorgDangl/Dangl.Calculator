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
        public void UnaryPrecedence(string formula, int expectedResult)
        {
            var calculationResult = Calculator.Calculate(formula);
            Assert.True(calculationResult.IsValid);
            Assert.Equal(-1, calculationResult.ErrorPosition);
            Assert.True(string.IsNullOrWhiteSpace(calculationResult.ErrorMessage));
            Assert.Equal(expectedResult, calculationResult.Result, 7);
        }
    }
}
