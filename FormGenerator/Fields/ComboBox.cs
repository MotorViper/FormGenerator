using TextParser;
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
                    // Need to make sure that the correct binding is chosen which requires Parameter to be null.
                    // Otherwise changing the selection does not cause an update which goes against the use of a combo box.
                    IToken evaluated = value;
                    if (Parameter != null)
                    {
                        TokenTree tree = new TokenTree();
                        tree.Children.Add(new TokenTree("TABLEITEM", Parameter));
                        evaluated = evaluated.SubstituteParameters(tree);
                    }
                    Parameter = null;
                    base.AddProperty("SelectedValue", evaluated);
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
