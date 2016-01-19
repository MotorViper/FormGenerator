using Generator;

namespace FormGenerator.Fields
{
    // ReSharper disable once ClassNeverInstantiated.Global - used by IOC.
    /// <summary>
    /// Class representing a text box.
    /// </summary>
    public class TextBox : Field
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public TextBox() : base("TextBox")
        {
        }

        /// <summary>
        /// Add a single property to the list of those to output.
        /// </summary>
        /// <param name="name">The property name.</param>
        /// <param name="value">The value to add.</param>
        protected override void AddProperty(string name, IValue value)
        {
            switch (name)
            {
                case "Content":
                    name = "Text";
                    break;
            }
            base.AddProperty(name, value);
        }
    }
}
