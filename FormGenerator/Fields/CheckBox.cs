using TextParser;
using TextParser.Tokens;

namespace FormGenerator.Fields
{
    // ReSharper disable once ClassNeverInstantiated.Global - used by IOC.
    public class CheckBox : Field
    {
        public CheckBox() : base("CheckBox")
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
