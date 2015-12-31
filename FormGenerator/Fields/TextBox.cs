using TextParser;
using TextParser.Tokens;

namespace FormGenerator.Fields
{
    // ReSharper disable once ClassNeverInstantiated.Global - used by IOC.
    public class TextBox : Field
    {
        public TextBox() : base("TextBox")
        {
        }

        protected override void AddProperty(string name, IToken value, TokenTreeList parameters)
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
