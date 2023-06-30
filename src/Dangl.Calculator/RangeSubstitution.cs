namespace Dangl.Calculator
{
    /// <summary>
    /// This class is used when performing a callback to try to resolve a range in a formula,
    /// e.g. the range '#A..#B' will report '#A' and '#B' as start and end values.
    /// </summary>
    public class RangeSubstitution
    {
        /// <summary>
        /// The constructor must be called with both start and end values
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public RangeSubstitution(string start, string end)
        {
            Start = start;
            End = end;
        }

        /// <summary>
        /// The start substitution
        /// </summary>
        public string Start { get; }

        /// <summary>
        /// The end substitution
        /// </summary>
        public string End { get; }
    }
}
