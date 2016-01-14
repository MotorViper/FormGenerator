using System.Collections.Generic;

namespace WebFormGenerator.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global - used by IOC.
    /// <summary>
    /// Class to help with output of check box fields.
    /// </summary>
    public class CheckBox : Field
    {
        private string _class;

        /// <summary>
        /// Constructor.
        /// </summary>
        public CheckBox() : base("input")
        {
        }

        /// <summary>
        /// Loops through the properties and outputs each one according to it's content.
        /// </summary>
        /// <param name="properties">The properties to loop over.</param>
        protected override void OutputProperties(Dictionary<string, string> properties)
        {
            base.OutputProperties(properties);
            if (_class != null)
                OutputProperty("class", _class + " form-control checkbox");
            else
                OutputProperty("class", "form-control checkbox");
            OutputProperty("type", "checkbox");
        }

        /// <summary>
        /// Outputs a single property.
        /// </summary>
        /// <param name="key">The property name.</param>
        /// <param name="value">The property value.</param>
        protected override void OutputProperty(string key, string value)
        {
            if (key == "class")
                _class = value;
            else
                base.OutputProperty(key, value);
        }

        /// <summary>
        /// Outputs the content of the field.
        /// </summary>
        /// <param name="content">The value to output.</param>
        protected override void OutputContent(string content)
        {
            if (bool.Parse(content))
                OutputProperty("checked", "checked");
        }

        /// <summary>
        /// Adds the end indicator for a field.
        /// </summary>
        protected override void AddEnd()
        {
            // This is a closed field and so has no end delimiter.
        }
    }
}
