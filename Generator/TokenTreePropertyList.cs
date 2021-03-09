using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TextParser;
using TextParser.Tokens;
using TextParser.Tokens.Interfaces;

namespace Generator
{
    /// <summary>
    /// Property list based on token data.
    /// </summary>
    public class TokenTreePropertyList : IPropertyList
    {
        private readonly List<TokenTree> _data;
        private readonly TokenTree _parameters;
        private readonly Dictionary<string, IList<IProperty>> _values = new Dictionary<string, IList<IProperty>>();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="data">Parent token tree.</param>
        /// <param name="parameters">Calculation parameters.</param>
        public TokenTreePropertyList(TokenTree data, TokenTree parameters)
        {
            _data = data.Children.Where(x => x.Name != "Field").ToList();
            _parameters = parameters;
        }

        /// <summary>
        /// Finds a property in the list.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <returns>List of properties matching the name.</returns>
        public IList<IProperty> Find(string name)
        {
            IList<IProperty> value;
            if (!_values.TryGetValue(name, out value))
            {
                List<IProperty> values = new List<IProperty>();
                IEnumerable<TokenTree> tokenTrees = _data.Where(child => child.Name == name);
                TokenTreeList parameters = new TokenTreeList(_parameters);
                foreach (TokenTree item in tokenTrees)
                {
                    IToken evaluated = item.Value.Evaluate(parameters, false);
                    ListToken list = evaluated as ListToken;
                    if (list != null)
                    {
                        foreach (IToken token in list)
                            values.Add(new TokenTreeProperty(new TokenTree(name, token)));
                    }
                    else
                    {
                        values.Add(new TokenTreeProperty(new TokenTree(name, evaluated)));
                    }
                }
                value = values;
                _values[name] = value;
            }
            return value;
        }

        /// <summary>
        /// Finds a property that represents an element.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <returns></returns>
        public IElement FindChild(string name)
        {
            TokenTree tokenTree = _data.FirstOrDefault(child => child.Name == name);
            return tokenTree == null ? null : new TokenTreeElement(tokenTree, new TokenTreeList(_parameters));
        }

        /// <summary>
        /// Adds a property to the list.
        /// </summary>
        /// <param name="property">The property to add.</param>
        public void Add(IProperty property)
        {
        }

        /// <summary>
        /// Gets the number of properties.
        /// </summary>
        public int Count => _data.Count;

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<IProperty> GetEnumerator()
        {
            return _data.SelectMany(tree => Find(tree.Name)).GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
