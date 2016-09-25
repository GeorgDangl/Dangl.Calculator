using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Dfa;
using Antlr4.Runtime.Sharpen;

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
        /// Sets <see cref="IsValid"/> to false
        /// </summary>
        public override void ReportAmbiguity(Parser recognizer, DFA dfa, int startIndex, int stopIndex, bool exact, BitSet ambigAlts, ATNConfigSet configs)
        {
            IsValid = false;
        }

        /// <summary>
        /// Sets <see cref="IsValid"/> to false
        /// </summary>
        public override void ReportAttemptingFullContext(Parser recognizer, DFA dfa, int startIndex, int stopIndex, BitSet conflictingAlts, SimulatorState conflictState)
        {
            IsValid = false;
        }

        /// <summary>
        /// Sets <see cref="IsValid"/> to false
        /// </summary>
        public override void ReportContextSensitivity(Parser recognizer, DFA dfa, int startIndex, int stopIndex, int prediction, SimulatorState acceptState)
        {
            IsValid = false;
        }

        /// <summary>
        /// Sets <see cref="IsValid"/> to false and <see cref="ErrorLocation"/> and <see cref="ErrorMessage"/> to the values provided by the parser
        /// </summary>
        public override void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            IsValid = false;
            ErrorLocation = ErrorLocation == -1 ? charPositionInLine : ErrorLocation;
            ErrorMessage = msg;
        }
    }
}
