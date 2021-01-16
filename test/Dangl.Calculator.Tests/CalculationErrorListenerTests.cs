using Xunit;

namespace Dangl.Calculator.Tests
{
    public class CalculationErrorListenerTests
    {
        [Fact]
        public void IsValidIsTrueOnInitialization()
        {
            var errorListener = new CalculatorErrorListener();
            Assert.True(errorListener.IsValid);
        }

        [Fact]
        public void ErrorMessageNullOnInitialization()
        {
            var errorListener = new CalculatorErrorListener();
            Assert.Null(errorListener.ErrorMessage);
        }

        [Fact]
        public void ErrorPositionMinusOneOnInitialization()
        {
            var errorListener = new CalculatorErrorListener();
            Assert.Equal(-1, errorListener.ErrorLocation);
        }

        [Fact]
        public void SetToInvalidOnEncounteredError_ReportAmbiguity()
        {
            var errorListener = new CalculatorErrorListener();
            errorListener.ReportAmbiguity(null, null, 0, 0, false, null, null);
            Assert.False(errorListener.IsValid);
        }

        [Fact]
        public void SetToInvalidOnEncounteredError_ReportAttemptingFullContext()
        {
            var errorListener = new CalculatorErrorListener();
            errorListener.ReportAttemptingFullContext(null, null, 0, 0, null, null);
            Assert.False(errorListener.IsValid);
        }

        [Fact]
        public void SetToInvalidOnEncounteredError_ReportContextSensitivity()
        {
            var errorListener = new CalculatorErrorListener();
            errorListener.ReportContextSensitivity(null, null, 0, 0, 0, null);
            Assert.False(errorListener.IsValid);
        }

        [Fact]
        public void SetToInvalidOnEncounteredError_SyntaxError()
        {
            var errorListener = new CalculatorErrorListener();
            errorListener.SyntaxError(null, null, 0, 0, null, null);
            Assert.False(errorListener.IsValid);
        }

        [Fact]
        public void DisplayCorrectErrorMessageOnSyntaxError()
        {
            var errorListener = new CalculatorErrorListener();
            errorListener.SyntaxError(null, null, 0, 0, "Test Message", null);
            Assert.Equal("Test Message", errorListener.ErrorMessage);
        }

        [Fact]
        public void DisplayCorrectErrorLocationOnSyntaxError()
        {
            var errorListener = new CalculatorErrorListener();
            errorListener.SyntaxError(null, null, 0, 5, null, null);
            Assert.Equal(5, errorListener.ErrorLocation);
        }

        [Fact]
        public void CanReportUnresolvableSubstitutions()
        {
            var errorListener = new CalculatorErrorListener();
            errorListener.ReportSubstitutionNotFound(3, "#Test");
            Assert.False(errorListener.IsValid);
            Assert.Equal(3, errorListener.ErrorLocation);
            Assert.Contains("#Test", errorListener.ErrorMessage);
        }
    }
}
