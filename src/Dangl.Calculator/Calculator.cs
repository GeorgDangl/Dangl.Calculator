using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dangl.Calculator
{
    /// <summary>
    ///     This class provides functionality to calculate a mathematical expression from a string.
    /// </summary>
    public static partial class Calculator
    {
        /// <summary>
        ///     This takes a string as input and returns the calculated result as double.
        /// </summary>
        /// <param name="formula">The mathematical expression as string to be calculated.</param>
        public static CalculationResult Calculate(string formula)
        {
            return CalculateResult(formula, false, _ => null, _ => null, false, out _);
        }

        /// <summary>
        ///     This takes a string as input and returns the calculated result as double.
        /// </summary>
        /// <param name="formula">The mathematical expression as string to be calculated.</param>
        /// <param name="substitutionResolver">
        ///     This callback may be used to resolve substitutions. If a null value is returned
        ///     by this callback, the formula is considered invalid.
        /// </param>
        public static CalculationResult Calculate(string formula, Func<string, double?> substitutionResolver)
        {
            return CalculateResult(formula, false, substitutionResolver, _ => null, false, out _);
        }

        /// <summary>
        ///     This takes a string as input and returns the calculated result as double.
        /// </summary>
        /// <param name="formula">The mathematical expression as string to be calculated.</param>
        /// <param name="substitutionResolver">
        ///     This callback may be used to resolve substitutions. If a null value is returned
        ///     by this callback, the formula is considered invalid.
        /// </param>
        /// <param name="rangeResolver">This callback may be used to resolve range substitutions. If a null value is returned by this callback,
        /// the formula is considerd invalid.</param>
        public static CalculationResult Calculate(string formula,
            Func<string, double?> substitutionResolver,
            Func<RangeSubstitution, double?> rangeResolver)
        {
            return CalculateResult(formula, false, substitutionResolver, rangeResolver, false, out _);
        }

        /// <summary>
        ///    This takes a string as input and returns the calculated result as double.
        /// </summary>
        /// <param name="formula">The mathematical expression as string to be calculated.</param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static CalculationResult Calculate(string formula, CalculatorOptions options)
        {
            return CalculateResult(formula, false, options.SubstitutionResolver, options.RangeResolver, options.DetectUnaryMinusInPowersAtStartOfFormula, out _);
        }

        /// <summary>
        ///     This will attempt to recalculate if an error was encountered. Will try to skip whitespaces
        ///     and comments so to prevent number literals and function qualifiers not being identified.
        /// </summary>
        /// <param name="formula"></param>
        /// <param name="secondRun"></param>
        /// <param name="substitutionResolver">
        ///     This callback may be used to resolve substitutions. If a null value is returned
        ///     by this callback, the formula is considered invalid.
        /// </param>
        /// <param name="rangeResolver">This callback may be used to resolve range substitutions. If a null value is returned by this callback,
        /// the formula is considerd invalid.</param>
        /// <param name="detectUnaryMinusInPowersAtStartOfFormula"></param>
        /// <param name="syntaxError"></param>
        /// <returns></returns>
        private static CalculationResult CalculateResult(string formula,
            bool secondRun,
            Func<string, double?> substitutionResolver,
            Func<RangeSubstitution, double?> rangeResolver,
            bool detectUnaryMinusInPowersAtStartOfFormula,
            out bool syntaxError)
        {
            syntaxError = false;
            if (string.IsNullOrWhiteSpace(formula))
            {
                return new CalculationResult
                {
                    IsValid = true,
                    Result = 0
                };
            }
            var calculatorExpression = ParseExpression(formula, out var tokenStream, out var customErrorListener);
            syntaxError = !customErrorListener.IsValid;
            var visitor = new CalculatorVisitor(substitutionResolver,
                rangeResolver,
                customErrorListener,
                detectUnaryMinusInPowersAtStartOfFormula);

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
                var cleanedFormula = CleanFormula(tokenStream, out var originalPositions);
                var originalErrorLocation = errorLocation;
                var retriedResult = CalculateResult(cleanedFormula,
                    true,
                    substitutionResolver,
                    rangeResolver,
                    detectUnaryMinusInPowersAtStartOfFormula,
                    out var retrySyntaxError);
                if (retrySyntaxError)
                {
                    retriedResult.ErrorPosition = originalErrorLocation;
                    retriedResult.ErrorMessage = errorMessage;
                }
                else if (!retriedResult.IsValid)
                {
                    retriedResult.ErrorPosition = MapErrorPosition(retriedResult.ErrorPosition, originalPositions);
                }
                return retriedResult;
            }
            return new CalculationResult
            {
                IsValid = isValid,
                Result = isValid || double.IsInfinity(result)
                    ? result
                    : double.NaN,
                ErrorPosition = secondRun && !syntaxError && errorLocation >= 0
                    ? tokenStream.Get(errorLocation).StartIndex
                    : errorLocation,
                ErrorMessage = isValid ? null : errorMessage
            };
        }

        private static CalculatorParser.ExpressionContext ParseExpression(string formula,
            out CommonTokenStream tokenStream,
            out CalculatorErrorListener errorListener)
        {
            var lexer = new CalculatorLexer(new AntlrInputStream(formula));
            tokenStream = new CommonTokenStream(lexer);
            var parser = new CalculatorParser(tokenStream);
            lexer.RemoveErrorListeners();
            parser.RemoveErrorListeners();
            errorListener = new CalculatorErrorListener();
            parser.AddErrorListener(errorListener);

            parser.Interpreter.PredictionMode = Antlr4.Runtime.Atn.PredictionMode.SLL;
            try
            {
                return parser.calculator().expression();
            }
            catch
            {
                tokenStream.Reset();
                parser.Reset();
                parser.Interpreter.PredictionMode = Antlr4.Runtime.Atn.PredictionMode.LL;
                return parser.calculator().expression();
            }
        }

        private static string CleanFormula(CommonTokenStream tokenStream, out List<int> originalPositions)
        {
            var cleanedFormula = string.Empty;
            originalPositions = new List<int>();
            var tokenList = tokenStream.GetTokens().ToList();
            for (var i = 0; i < tokenList.Count - 1; i++)
            {
                cleanedFormula += tokenList[i].Text;
                for (var j = 0; j < tokenList[i].Text.Length; j++)
                {
                    originalPositions.Add(tokenList[i].StartIndex + j);
                }
            }
            return cleanedFormula;
        }

        private static int MapErrorPosition(int position, List<int> originalPositions)
        {
            return position >= 0 && position < originalPositions.Count
                ? originalPositions[position]
                : position;
        }
    }
}
