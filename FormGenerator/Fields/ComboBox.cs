using TextParser;
using TextParser.Tokens;

namespace FormGenerator.Fields
{
    public class ComboBox : Field
    {
        // ReSharper disable once MemberCanBeProtected.Global - used by IOC.
        public ComboBox() : base("ComboBox")
        {
        }

        protected override void AddProperty(string name, IToken value, TokenTreeList parameters)
        {
            switch (name)
            {
                case "Content":
                    base.AddProperty("SelectedValue", value, parameters);
                    break;
                case "SelectedItem":
                    base.AddProperty("SelectedValue", new StringToken("{Binding " + value.Text + "}"), parameters);
                    break;
                case "SelectionOptions":
                    AddProperty("ItemsSource", "{Binding Values, Converter={StaticResource ListConverter}, ConverterParameter=" + value.Text + "}");
                    break;
                default:
                    base.AddProperty(name, value, parameters);
                    break;
            }
        }
    }
}
