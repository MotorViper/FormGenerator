using TextParser;

namespace WebFormGenerator.Models
{
    /// <summary>
    /// Output an uderline.
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global - used by IOC.
    public class Underline : Field
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public Underline() : base("hr")
        {
        }

        /// <summary>
        /// Outputs the start text of a field.
        /// </summary>
        /// <param name="parameters">Calculation parameters.</param>
        protected override void AddStart(TokenTree parameters)
        {
            AppendStartOfLine(Level, "<hr/>");
        }

        /// <summary>
        /// Adds the end indicator for a field.
        /// </summary>
        protected override void AddEnd()
        {
            // This is a closed field and so has no end delimiter.
        }

        /// <summary>
        /// Outputs the fields children.
        /// </summary>
        /// <param name="parameters">The data used for evaluation.</param>
        protected override void AddChildren(TokenTree parameters)
        {
            // Underline cannot have children.
        }
    }
}
