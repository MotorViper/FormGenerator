using TextParser.Tokens;

namespace FormGenerator.Fields
{
    // ReSharper disable once ClassNeverInstantiated.Global - used by IOC.
    /// <summary>
    /// Class to represent a check box.
    /// </summary>
    public class CheckBox : Field
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public CheckBox() : base("CheckBox")
        {
        }

        /// <summary>
        /// Add a single property to the list of those to output.
        /// </summary>
        /// <param name="name">The property name.</param>
        /// <param name="value">The value to add.</param>
        protected override void AddProperty(string name, IToken value)
        {
            switch (name)
            {
                case "Content":
                    base.AddProperty("IsChecked", value);
                    break;
                default:
                    base.AddProperty(name, value);
                    break;
            }
        }
    }
}
