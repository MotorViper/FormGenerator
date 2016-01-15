using TextParser.Tokens;

namespace FormGenerator.Fields
{
    /// <summary>
    /// Class representing a combo box.
    /// </summary>
    public class ComboBox : Field
    {
        // ReSharper disable once MemberCanBeProtected.Global - used by IOC.
        /// <summary>
        /// Constructor.
        /// </summary>
        public ComboBox() : base("ComboBox")
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
                    base.AddProperty("SelectedValue", value);
                    break;
                case "SelectedItem":
                    base.AddProperty("SelectedValue", new StringToken("{Binding " + value.Text + "}"));
                    break;
                case "SelectionOptions":
                    AddProperty("ItemsSource", "{Binding Values, Converter={StaticResource ListConverter}, ConverterParameter=" + value.Text + "}");
                    break;
                default:
                    base.AddProperty(name, value);
                    break;
            }
        }
    }
}
