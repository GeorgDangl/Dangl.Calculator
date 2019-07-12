using Xunit;

namespace Dangl.Calculator.Tests.Regression
{
    public class MultiplicationAsDefaultSeparator
    {
        [Theory]
        [InlineData("33", 33)]
        [InlineData("3(3)", 9)]
        [InlineData("(3)3", 9)]
        // This tests that multiplication has precendence over addition
        [InlineData("1+2(3)", 7)]
        [InlineData("2*2(3)", 12)]
        [InlineData("2^2(3)", 12)]
        [InlineData("(1+2)(3)", 9)]
        [InlineData("2(3+3)", 12)]
        [InlineData("2(3)+3", 9)]
        [InlineData("2+3(3)", 11)]
        [InlineData("2(3*3)", 18)]
        [InlineData("2(tan(3))", -0.2850930861485556)]
        [InlineData("2(pi)", 6.283185307179586)]
        [InlineData("(pi)pi", 9.869604401089358)]
        [InlineData("pi(pi)", 9.869604401089358)]
        [InlineData("94,70(0,30+0,75)*(3,16+0,10)", 324.1581)]
        public void UseMultiplicationOperatorWhenNoOperatorBetweenExpressions(string formula, double expectedResult)
        {
            var calculationResult = Calculator.Calculate(formula);
            Assert.True(calculationResult.IsValid);
            Assert.Equal(-1, calculationResult.ErrorPosition);
            Assert.True(string.IsNullOrWhiteSpace(calculationResult.ErrorMessage));
            Assert.Equal(expectedResult, calculationResult.Result, 7);
        }
    }
}
