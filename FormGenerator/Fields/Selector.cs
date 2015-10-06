using TextParser;
using TextParser.Tokens;

namespace FormGenerator.Fields
{
    public class Selector : ComboBox
    {
        public Selector(Field parent, TokenTree data, int level) : base(parent, data, level)
        {
        }

        protected override void AddProperties(TokenTree parameters)
        {
            base.AddProperties(parameters);
            AddProperty("SelectedValue", new StringToken("{Binding Selected}"), parameters);
            AddProperty("SelectedIndex", new StringToken("0"), parameters);
            AddProperty("ItemsSource", new StringToken("{Binding Keys}"), parameters);
        }
    }
}
