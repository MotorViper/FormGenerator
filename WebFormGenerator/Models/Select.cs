using System.Collections.Generic;
using TextParser;
using TextParser.Tokens;

namespace WebFormGenerator.Models
{
    /// <summary>
    /// Class that allows selection of the current display item.
    /// </summary>
    public class Select : Field
    {
        // ReSharper disable once MemberCanBeProtected.Global - used by IOC.
        public Select() : base("Select")
        {
        }

        /// <summary>
        /// The list of options to selected from.
        /// </summary>
        protected List<string> Options { get; } = new List<string>();

        /// <summary>
        /// The currently selected item.
        /// </summary>
        protected string SelectedItem { get; set; }

        /// <summary>
        /// Add a single property to the list of those to output.
        /// </summary>
        /// <param name="name">The property name.</param>
        /// <param name="value"></param>
        /// <param name="parameters">Calculation parameters.</param>
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

        /// <summary>
        /// Outputs the fields children.
        /// </summary>
        /// <param name="parameters">The data used for evaluation.</param>
        protected override void AddChildren(TokenTree parameters)
        {
            foreach (var option in Options)
            {
                Builder.Append("<option ");
                if (option == SelectedItem)
                    Builder.Append("selected =\"selected\" ");
                Builder.Append($"value=\"{option}\">{option}</option>");
            }
            base.AddChildren(parameters);
        }
    }
}
