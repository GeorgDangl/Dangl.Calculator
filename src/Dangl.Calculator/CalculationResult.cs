namespace Dangl.Calculator
{
    /// <summary>
    /// Container class that holds the result of a calculation
    /// </summary>
    public class CalculationResult
    {
        /// <summary>
        ///     Indicates if the formula was valid and a result was calculated. If false, the result will be either NaN or Infinity
        /// </summary>
        public bool IsValid { get; internal set; }

        /// <summary>
        ///     The position on which the first error was encountered. This is a zero based index. Will report -1 for
        ///     division by zero, but result will be reported as invalid
        /// </summary>
        public int ErrorPosition { get; internal set; } = -1;

        /// <summary>
        /// Relays the underlying formula parsers error message
        /// </summary>
        public string ErrorMessage { get; internal set; }

        /// <summary>
        /// The calculated result
        /// </summary>
        public double Result { get; internal set; }
    }
}
