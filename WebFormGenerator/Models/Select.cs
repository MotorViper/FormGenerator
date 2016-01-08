using System.Collections.Generic;
using TextParser;
using TextParser.Tokens;

namespace WebFormGenerator.Models
{
    public class Select : Field
    {
        // ReSharper disable once MemberCanBeProtected.Global - used by IOC.
        public Select() : base("Select")
        {
        }

        protected List<string> Options { get; } = new List<string>();
        protected string SelectedItem { get; set; }

        protected override void AddProperty(string name, IToken value, TokenTreeList parameters)
        {
            switch (name)
            {
                case "SelectedItem":
                    SelectedItem = value.Text;
                    break;
                case "SelectionOptions":
                    TokenTree items = new TokenTree(parameters[0].GetChildren(value.Text));
                    foreach (TokenTree item in items.Children)
                        Options.Add(item.Name);
                    break;
                default:
                    base.AddProperty(name, value, parameters);
                    break;
            }
        }

        protected override void AddChildren(TokenTree parameters, string endOfLine)
        {
            foreach (var option in Options)
            {
                Builder.Append("<option ");
                if (option == SelectedItem)
                    Builder.Append("selected =\"selected\" ");
                Builder.Append($"value=\"{option}\">{option}</option>");
            }
            base.AddChildren(parameters, endOfLine);
        }
    }
}
