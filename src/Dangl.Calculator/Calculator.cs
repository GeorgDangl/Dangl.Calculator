using System.Linq;
using Antlr4.Runtime;
using Dangl.Calculator.Generated;

namespace Dangl.Calculator
{
    /// <summary>
    ///     This class provides functionality to calculate a mathematical expression from a string.
    /// </summary>
    public class Calculator
    {
        /// <summary>
        ///     This takes a string as input and returns the calculated result as decimal.
        /// </summary>
        /// <param name="formula">The mathematical expression as string to be calculated.</param>
        public static CalculationResult Calculate(string formula)
        {
            return CalculateResult(formula, false);
        }

        /// <summary>
        ///     This will attempt to recalculate if an error was encountered. Will try to skip whitespaces
        ///     and comments so to prevent number literals and function qualifiers not being identified.
        /// </summary>
        /// <param name="formula"></param>
        /// <param name="secondRun"></param>
        /// <returns></returns>
        private static CalculationResult CalculateResult(string formula, bool secondRun)
        {
            if (string.IsNullOrWhiteSpace(formula))
            {
                return new CalculationResult
                {
                    IsValid = true,
                    Result = 0
                };
            }
            var inputStream = new AntlrInputStream(formula);
            var lexer = new CalculatorLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new CalculatorParser(tokenStream);

            // Removing default error listeners due to noise in debug
            lexer.RemoveErrorListeners();
            parser.RemoveErrorListeners();
            // But adding the custom one
            var customErrorListener = new CalculatorErrorListener();
            parser.AddErrorListener(customErrorListener);
            var visitor = new CalculatorVisitor();
            var calculatorExpression = parser.calculator().expression();
            var result = visitor.Visit(calculatorExpression);
            var isValid = customErrorListener.IsValid;
            var errorLocation = customErrorListener.ErrorLocation;
            var errorMessage = customErrorListener.ErrorMessage;
            if (double.IsInfinity(result))
            {
                isValid = false;
            }

            if (!isValid && !secondRun)
            {
                var cleanedFormula = string.Empty;
                var tokenList = tokenStream.GetTokens().ToList();
                for (var i = 0; i < tokenList.Count - 1; i++)
                {
                    cleanedFormula += tokenList[i].Text;
                }
                var originalErrorLocation = errorLocation;
                var retriedResult = CalculateResult(cleanedFormula, true);
                if (!retriedResult.IsValid)
                {
                    retriedResult.ErrorPosition = originalErrorLocation;
                    retriedResult.ErrorMessage = errorMessage;
                }
                return retriedResult;
            }
            return new CalculationResult
            {
                IsValid = isValid,
                Result = isValid || double.IsInfinity(result)
                    ? result
                    : double.NaN,
                ErrorPosition = errorLocation,
                ErrorMessage = isValid ? null : errorMessage
            };
        }
    }
}
