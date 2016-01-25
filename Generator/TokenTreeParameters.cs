using System.Linq;
using TextParser;

namespace Generator
{
    public class TokenTreeParameters : TokenTreeList, IParameters
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="list">The tokens that make up the parameters.</param>
        public TokenTreeParameters(TokenTreeList list)
        {
            foreach (TokenTree tree in list)
            {
                Add(tree);
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        private TokenTreeParameters()
        {
        }

        /// <summary>
        /// Gets a list of fields from the parameters.
        /// </summary>
        /// <param name="name">The name of the list.</param>
        /// <returns>The entries in the list.</returns>
        public IPropertyList GetList(string name)
        {
            SimplePropertyList list = new SimplePropertyList();
            TokenTreeList children = this[0].GetChildren(name);
            list.AddRange(children.Select(child => new SimpleProperty("item", child.Name)));
            return list;
        }

        /// <summary>
        /// The currently selected item being displayed.
        /// </summary>
        public string SelectedItem => this[1]?.Name;

        /// <summary>
        /// Add a new property to the parameters.
        /// </summary>
        /// <param name="property">The property to add.</param>
        /// <returns>The parameters with the new property added.</returns>
        public IParameters Add(IProperty property)
        {
            TokenTree parameters = this[0].Clone();
            parameters.Children.AddIfMissing(new TokenTree(property.Name, property.StringValue));
            TokenTreeParameters treeParameters = new TokenTreeParameters {parameters};
            for (int i = 1; i < Count; ++i)
                treeParameters.Add(this[i]);
            return treeParameters;
        }
    }
}
