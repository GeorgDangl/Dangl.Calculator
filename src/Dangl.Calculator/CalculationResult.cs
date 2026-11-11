namespace Dangl.Calculator
{
    /// <summary>
    /// Container class that holds the result of a calculation
    /// </summary>
    public class CalculationResult
    {
        /// <summary>
        ///     Indicates if the formula was valid and a result was calculated. If false, the result will be NaN.
        /// </summary>
        public bool IsValid { get; internal set; }

        /// <summary>
        ///     The position on which the first error was encountered. This is a zero based character index.
        ///     Reports -1 when no formula position is available.
        /// </summary>
        public int ErrorPosition { get; internal set; } = -1;

        /// <summary>
        /// Relays the underlying formula parser or numeric evaluation error message
        /// </summary>
        public string ErrorMessage { get; internal set; }

        /// <summary>
        /// The decimal calculation result converted to double, or NaN for an invalid calculation
        /// </summary>
        public double Result { get; internal set; }
    }
}
