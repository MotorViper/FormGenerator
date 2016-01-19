using Generator;
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
        protected override void AddProperty(string name, IValue value)
        {
            switch (name)
            {
                case "Content":
                    // Need to make sure that the correct binding is chosen which requires Parameter to be null.
                    // Otherwise changing the selection does not cause an update which goes against the use of a combo box.
                    IValue evaluated = value.ApplyParameters(Parameters);
                    Parameters = null;
                    base.AddProperty("SelectedValue", evaluated);
                    break;
                case "SelectedItem":
                    AddTypedProperty("SelectedValue", new StringToken("{Binding " + value.StringValue + "}"));
                    break;
                case "SelectionOptions":
                    AddTypedProperty("ItemsSource", "{Binding Values, Converter={StaticResource ListConverter}, ConverterParameter=" + value.StringValue + "}");
                    break;
                default:
                    base.AddProperty(name, value);
                    break;
            }
        }
    }
}
