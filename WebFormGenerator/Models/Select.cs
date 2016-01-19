using System.Collections.Generic;
using Generator;
using TextParser;

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
        protected override void AddProperty(string name, IValue value)
        {
            switch (name)
            {
                case "SelectedItem":
                    SelectedItem = value.StringValue;
                    break;
                case "SelectionOptions":
                    TokenTree items = new TokenTree(Element.Parameters[0].GetChildren(value.StringValue));
                    foreach (TokenTree item in items.Children)
                        Options.Add(item.Name);
                    break;
                default:
                    base.AddProperty(name, value);
                    break;
            }
        }

        /// <summary>
        /// Outputs the fields children.
        /// </summary>
        protected override void AddChildren()
        {
            foreach (string option in Options)
            {
                Builder.Append("<option ");
                if (option == SelectedItem)
                    Builder.Append("selected =\"selected\" ");
                Builder.Append($"value=\"{option}\">{option}</option>");
            }
            base.AddChildren();
        }
    }
}
