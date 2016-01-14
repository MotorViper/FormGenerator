namespace WebFormGenerator.Models
{
    /// <summary>
    /// Class to help with output of text box fields.
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global - used by IOC.
    public class TextBox : Field
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public TextBox() : base("input")
        {
        }

        /// <summary>
        /// Outputs a single property.
        /// </summary>
        /// <param name="key">The property name.</param>
        /// <param name="value">The property value.</param>
        protected override void OutputProperty(string key, string value)
        {
            if (key == "class")
                value += " form-control text-box";
            base.OutputProperty(key, value);
        }

        /// <summary>
        /// Outputs the content of the field.
        /// </summary>
        /// <param name="content">The value to output.</param>
        protected override void OutputContent(string content)
        {
            OutputProperty("value", content);
        }

        /// <summary>
        /// Adds the end indicator for a field.
        /// </summary>
        protected override void AddEnd()
        {
            // This is a closed field so has no end delimiter.
        }
    }
}
