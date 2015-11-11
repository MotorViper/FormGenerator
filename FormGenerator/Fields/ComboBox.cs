using System.Text;
using TextParser;
using TextParser.Tokens;

namespace FormGenerator.Fields
{
    public class ComboBox : Field
    {
        public ComboBox(Field parent, TokenTree data, int level, StringBuilder builder) : base(parent, "ComboBox", data, level, builder)
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
