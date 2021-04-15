using Generator;
using System.Linq;
using TextParser.Tokens;

namespace FormGenerator.Fields
{
    /// <summary>
    /// Class representing a combo box.
    /// </summary>
    public class ComboBox : Field
    {
        private static int s_id = 0;

        private readonly int _id;

        // ReSharper disable once MemberCanBeProtected.Global - used by IOC.
        /// <summary>
        /// Constructor.
        /// </summary>
        public ComboBox() : base("ComboBox")
        {
            _id = ++s_id;
        }

        protected override void AddProperties()
        {
            int selectionOptionCount = 0;
            IValue selectionOption = null;
            foreach (IProperty child in Element.Properties.Where(child => !IgnoredProperties().Contains(child.Name)))
            {
                string name = child.Name;
                if (name == "SelectionOptions")
                {
                    ++selectionOptionCount;
                    selectionOption = child;
                }
                else
                    AddProperty(child.Name, child);
            }
            if (selectionOptionCount == 1)
                AddProperty("SelectionOptions", selectionOption);
            else if (selectionOption != null)
                AddProperty("SelectionOptions", new SimpleValue(Element.Properties.FindChild("SelectionOptions").ElementName));
            Parent?.AddChildProperties(this);
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
                    AddTypedProperty("ItemsSource", "{Binding Values, Converter={StaticResource ListConverter}, ConverterParameter=^" +
                        _id + "^:" + value.StringValue + "}");
                    break;
                default:
                    base.AddProperty(name, value);
                    break;
            }
        }
    }
}
