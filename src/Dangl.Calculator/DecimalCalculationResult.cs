namespace Dangl.Calculator
{
    /// <summary>
    /// Holds the result of a decimal calculation.
    /// </summary>
    public class DecimalCalculationResult
    {
        /// <summary>
        /// Indicates whether the formula was valid and a decimal result was calculated.
        /// </summary>
        public bool IsValid { get; internal set; }

        /// <summary>
        /// The zero-based position of the error, or -1 if no position is available.
        /// </summary>
        public int ErrorPosition { get; internal set; } = -1;

        /// <summary>
        /// The syntax, substitution or numeric error message, or null for a valid result.
        /// </summary>
        public string ErrorMessage { get; internal set; }

        /// <summary>
        /// The calculated value, or null if the calculation is invalid.
        /// </summary>
        public decimal? Result { get; internal set; }
    }
}
