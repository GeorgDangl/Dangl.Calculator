using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Dangl.Calculator
{
    /// <summary>
    ///     This class provides functionality to calculate a mathematical expression from a string.
    /// </summary>
    /// <remarks>
    /// All formulas are evaluated using decimal arithmetic. Calculate converts only the final result to double.
    /// Double substitution and range values are converted using 15 significant decimal digits.
    /// </remarks>
    public static partial class Calculator
    {
        /// <summary>
        ///     This takes a string as input and returns the calculated result as double.
        /// </summary>
        /// <param name="formula">The mathematical expression as string to be calculated.</param>
        public static CalculationResult Calculate(string formula)
        {
            return CalculateResult(formula, null, null, false);
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
            return CalculateResult(formula, substitutionResolver, null, false);
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
            return CalculateResult(formula, substitutionResolver, rangeResolver, false);
        }

        /// <summary>
        ///    This takes a string as input and returns the calculated result as double.
        /// </summary>
        /// <param name="formula">The mathematical expression as string to be calculated.</param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static CalculationResult Calculate(string formula, CalculatorOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            return CalculateResult(formula, options.SubstitutionResolver, options.RangeResolver,
                options.DetectUnaryMinusInPowersAtStartOfFormula);
        }

        private static CalculationResult CalculateResult(string formula,
            Func<string, double?> substitutionResolver,
            Func<RangeSubstitution, double?> rangeResolver,
            bool detectUnaryMinusInPowersAtStartOfFormula)
        {
            var result = CalculateDecimal(formula, new DecimalCalculatorOptions
            {
                SubstitutionResolver = substitutionResolver == null
                    ? (Func<string, decimal?>)null
                    : name => ConvertToDecimal(substitutionResolver(name)),
                RangeResolver = rangeResolver == null
                    ? (Func<RangeSubstitution, decimal?>)null
                    : range => ConvertToDecimal(rangeResolver(range)),
                DetectUnaryMinusInPowersAtStartOfFormula = detectUnaryMinusInPowersAtStartOfFormula
            });
            return new CalculationResult
            {
                IsValid = result.IsValid,
                Result = result.IsValid
                    ? ConvertToDouble(result.Result.Value)
                    : double.NaN,
                ErrorPosition = result.ErrorPosition,
                ErrorMessage = result.ErrorMessage
            };
        }

        private static double ConvertToDouble(decimal value)
        {
            // Older runtimes can round decimal-to-double casts twice.
            return double.Parse(value.ToString("G29", CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
        }

        private static decimal? ConvertToDecimal(double? value)
        {
            if (!value.HasValue)
            {
                return null;
            }
            if (!decimal.TryParse(value.Value.ToString("G15", CultureInfo.InvariantCulture),
                NumberStyles.Float, CultureInfo.InvariantCulture, out var converted))
            {
                throw new DecimalEvaluationException(-1,
                    "The substitution or range value must be finite and within the decimal range.");
            }
            return converted;
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
