using TextParser;
using TextParser.Tokens;

namespace FormGenerator.Fields
{
    public class TextBox : Field
    {
        public TextBox(Field parent, TokenTree data = null, int level = -1) : base(parent, "TextBox", data, level)
        {
        }

        protected override void AddProperty(string name, IToken value, TokenTree parameters)
        {
            switch (name)
            {
                case "Content":
                    name = "Text";
                    break;
            }
            base.AddProperty(name, value, parameters);
        }
    }
}
