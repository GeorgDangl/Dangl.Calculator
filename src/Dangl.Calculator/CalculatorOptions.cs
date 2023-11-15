using System;

namespace Dangl.Calculator
{
    /// <summary>
    /// Options for the calculator
    /// </summary>
    public class CalculatorOptions
    {
        /// <summary>
        /// This callback may be used to resolve substitutions. If a null value is returned
        /// by this callback, the formula is considered invalid.
        /// </summary>
        public Func<string, double?> SubstitutionResolver { get; set; }

        /// <summary>
        /// This callback may be used to resolve range substitutions. If a null value is returned by this callback,
        /// the formula is considerd invalid.
        /// </summary>
        public Func<RangeSubstitution, double?> RangeResolver { get; set; }

        /// <summary>
        /// This defaults to false, and it is recommended to keep it to false. If this is set to true, a special case
        /// will be activated for formulas that start with a negative number in a power, like "-2^2". The default parsing
        /// would first resolve the unary minus, resulting in "(-2)^2", which is 4. If this is set to true, the unary minus
        /// at the beginning of a formula is not taken to the power, resulting in the formula being interpreted as -(2^2) to
        /// equal negative 4. Other commonly used formula interpreters (e.g. Microsoft Excel, Google Sheets) conform to the
        /// default setting, so it is recommended to keep it to false.
        /// </summary>
        public bool DetectUnaryMinusInPowersAtStartOfFormula { get; set; }
    }
}
