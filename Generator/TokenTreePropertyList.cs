using System.Collections.Generic;
using System.Linq;
using TextParser;

namespace Generator
{
    public class TokenTreePropertyList : IPropertyList
    {
        private readonly List<TokenTree> _data;
        private readonly TokenTree _parameters;
        private readonly Dictionary<string, IList<IProperty>> _values = new Dictionary<string, IList<IProperty>>();

        public TokenTreePropertyList(TokenTree data, TokenTree parameters)
        {
            _data = data.Children.Where(x => x.Name != "Field").ToList();
            _parameters = parameters;
        }

        public IProperty Find(string name)
        {
            return FindAll(name).FirstOrDefault();
        }

        public IList<IProperty> FindAll(string name)
        {
            IList<IProperty> value;
            if (!_values.TryGetValue(name, out value))
            {
                value = _data.Where(child => child.Name == name).Select(x => (IProperty)new TokenTreeProperty(x, _parameters)).ToList();
                _values[name] = value;
            }
            return value;
        }

        public IElement FindChild(string name)
        {
            return null;
        }

        public int Count => _data.Count;
    }
}
