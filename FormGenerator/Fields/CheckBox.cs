using System.Text;
using TextParser;
using TextParser.Tokens;

namespace FormGenerator.Fields
{
    public class CheckBox : Field
    {
        public CheckBox(Field parent, TokenTree data, int level, StringBuilder builder) : base(parent, "CheckBox", data, level, builder)
        {
        }

        protected override void AddProperty(string name, IToken value, TokenTreeList parameters)
        {
            switch (name)
            {
                case "Content":
                    base.AddProperty("IsChecked", value, parameters);
                    break;
                default:
                    base.AddProperty(name, value, parameters);
                    break;
            }
        }
    }
}
