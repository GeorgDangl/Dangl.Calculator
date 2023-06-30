using Antlr4.Runtime;
using System.IO;

namespace Dangl.Calculator
{
    /// <summary>
    ///     This class overrides some functionalities of the ANTLR4 BaseErrorListener.
    ///     It will create error objects upon encountering errors in the expression.
    /// </summary>
    public class CalculatorErrorListener : BaseErrorListener
    {
        /// <summary>
        /// Returns false if any errors have been reported
        /// </summary>
        public bool IsValid { get; private set; } = true;

        /// <summary>
        /// Returns the location an encountered syntax error, -1 otherwise
        /// </summary>
        public int ErrorLocation { get; private set; } = -1;

        /// <summary>
        /// Returns the message of an encountered syntax error
        /// </summary>
        public string ErrorMessage { get; private set; }

        /// <summary>
        /// Sets <see cref="IsValid"/> to false and <see cref="ErrorLocation"/> and <see cref="ErrorMessage"/> to the values provided by the parser
        /// </summary>
        public override void SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            IsValid = false;
            ErrorLocation = ErrorLocation == -1 ? charPositionInLine : ErrorLocation;
            ErrorMessage = msg;
        }

        /// <summary>
        /// Sets <see cref="IsValid"/> to false and <see cref="ErrorLocation"/> to the value provided by the caller. The <see cref="ErrorMessage"/> will be set
        /// to indicate which substitution value was not found.
        /// </summary>
        /// <param name="errorLocation"></param>
        /// <param name="substitution"></param>
        public void ReportSubstitutionNotFound(int errorLocation, string substitution)
        {
            IsValid = false;
            ErrorLocation = errorLocation;
            ErrorMessage = $"The substitution '{substitution}' could not be resolved";
        }

        /// <summary>
        /// Sets <see cref="IsValid"/> to false and <see cref="ErrorLocation"/> to the value provided by the caller. The <see cref="ErrorMessage"/> will be set
        /// to indicate which range value was not found.
        /// </summary>
        /// <param name="errorLocation"></param>
        /// <param name="range"></param>
        public void ReportRangeNotFound(int errorLocation, string range)
        {
            IsValid = false;
            ErrorLocation = errorLocation;
            ErrorMessage = $"The range '{range}' could not be resolved";
        }
    }
}
