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
        protected override void AddStart()
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
        protected override void AddChildren()
        {
            // Underline cannot have children.
        }
    }
}
