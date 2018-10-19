using Xunit;

namespace Dangl.Calculator.Tests.Regression
{
    public class OperatorPrecedence
    {
        [Theory]
        [InlineData("-3+5", 2)]
        [InlineData("1+2*3", 7)]
        [InlineData("(1+2)*3", 9)]
        [InlineData("1+-2*3", -5)]
        [InlineData("-1+2*3", 5)]
        [InlineData("1+2*-3", -5)]
        [InlineData("log10+1", 2)]
        [InlineData("2^3+1", 9)]
        [InlineData("2**3+1", 9)]
        [InlineData("4^-2+1", 1.0625)]
        [InlineData("4**-2+1", 1.0625)]
        [InlineData("1e+2+1", 101)]
        [InlineData("1e-2+1", 1.01)]
        [InlineData("1e+2*2", 200)]
        [InlineData("1e+2*1", 100)]
        [InlineData("1e-2*2", 0.02)]
        [InlineData("1e+2^3", 1000000)]
        [InlineData("1e-2^2", 0.0001)]
        [InlineData("1e+(2+2)+1", 10001)]
        [InlineData("1e-(2+2)+1", 1.0001)]
        public void UsesCorrectPrecendence(string formula, double expectedResult)
        {
            var calculationResult = Calculator.Calculate(formula);
            Assert.True(calculationResult.IsValid);
            Assert.Equal(-1, calculationResult.ErrorPosition);
            Assert.True(string.IsNullOrWhiteSpace(calculationResult.ErrorMessage));
            Assert.Equal(expectedResult, calculationResult.Result, 7);
        }
    }
}
