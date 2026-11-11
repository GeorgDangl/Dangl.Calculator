using System;

namespace Dangl.Calculator
{
    /// <summary>
    /// Options for decimal calculations.
    /// </summary>
    public class DecimalCalculatorOptions
    {
        /// <summary>
        /// Resolves substitutions directly to decimal values. An unresolved substitution is invalid.
        /// </summary>
        public Func<string, decimal?> SubstitutionResolver { get; set; }

        /// <summary>
        /// Resolves ranges directly to decimal values. An unresolved range is invalid.
        /// </summary>
        public Func<RangeSubstitution, decimal?> RangeResolver { get; set; }

        /// <summary>
        /// If true, a leading unary minus in a power is applied after exponentiation.
        /// Defaults to false, matching <see cref="CalculatorOptions.DetectUnaryMinusInPowersAtStartOfFormula"/>.
        /// </summary>
        public bool DetectUnaryMinusInPowersAtStartOfFormula { get; set; }
    }
}
