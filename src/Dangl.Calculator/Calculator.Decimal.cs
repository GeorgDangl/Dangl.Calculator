using System;

namespace Dangl.Calculator
{
    public static partial class Calculator
    {
        /// <summary>
        /// Evaluates a formula using decimal arithmetic without converting through double.
        /// </summary>
        /// <param name="formula">The mathematical expression to calculate.</param>
        public static DecimalCalculationResult CalculateDecimal(string formula)
        {
            return CalculateDecimalResult(formula, false, null, null, false);
        }

        /// <summary>
        /// Evaluates a formula using decimal arithmetic and decimal substitutions.
        /// </summary>
        /// <param name="formula">The mathematical expression to calculate.</param>
        /// <param name="substitutionResolver">Resolves substitutions; null values indicate unresolved substitutions.</param>
        public static DecimalCalculationResult CalculateDecimal(string formula, Func<string, decimal?> substitutionResolver)
        {
            return CalculateDecimalResult(formula, false, substitutionResolver, null, false);
        }

        /// <summary>
        /// Evaluates a formula using decimal arithmetic, substitutions and ranges.
        /// </summary>
        /// <param name="formula">The mathematical expression to calculate.</param>
        /// <param name="substitutionResolver">Resolves substitutions; null values indicate unresolved substitutions.</param>
        /// <param name="rangeResolver">Resolves ranges; null values indicate unresolved ranges.</param>
        public static DecimalCalculationResult CalculateDecimal(string formula,
            Func<string, decimal?> substitutionResolver,
            Func<RangeSubstitution, decimal?> rangeResolver)
        {
            return CalculateDecimalResult(formula, false, substitutionResolver, rangeResolver, false);
        }

        /// <summary>
        /// Evaluates a formula using the supplied decimal calculation options.
        /// </summary>
        /// <param name="formula">The mathematical expression to calculate.</param>
        /// <param name="options">Decimal resolvers and operator precedence options.</param>
        public static DecimalCalculationResult CalculateDecimal(string formula, DecimalCalculatorOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            return CalculateDecimalResult(formula, false, options.SubstitutionResolver,
                options.RangeResolver, options.DetectUnaryMinusInPowersAtStartOfFormula);
        }

        private static DecimalCalculationResult CalculateDecimalResult(string formula,
            bool secondRun,
            Func<string, decimal?> substitutionResolver,
            Func<RangeSubstitution, decimal?> rangeResolver,
            bool detectUnaryMinusInPowersAtStartOfFormula)
        {
            if (string.IsNullOrWhiteSpace(formula))
            {
                return new DecimalCalculationResult { IsValid = true, Result = 0m };
            }

            var expression = ParseExpression(formula, out var tokens, out var errorListener);
            if (!errorListener.IsValid)
            {
                if (!secondRun)
                {
                    var retried = CalculateDecimalResult(CleanFormula(tokens), true,
                        substitutionResolver, rangeResolver, detectUnaryMinusInPowersAtStartOfFormula);
                    if (!retried.IsValid)
                    {
                        retried.ErrorPosition = errorListener.ErrorLocation;
                        retried.ErrorMessage = errorListener.ErrorMessage;
                    }
                    return retried;
                }
                return new DecimalCalculationResult
                {
                    ErrorPosition = errorListener.ErrorLocation,
                    ErrorMessage = errorListener.ErrorMessage
                };
            }

            var visitor = new DecimalCalculatorVisitor(substitutionResolver, rangeResolver,
                detectUnaryMinusInPowersAtStartOfFormula);
            try
            {
                return new DecimalCalculationResult { IsValid = true, Result = visitor.Visit(expression) };
            }
            catch (DecimalEvaluationException error)
            {
                return new DecimalCalculationResult
                {
                    ErrorPosition = error.Position,
                    ErrorMessage = error.Message
                };
            }
        }
    }
}
