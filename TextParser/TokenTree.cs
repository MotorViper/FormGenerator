using System.Collections.Generic;
using System.Linq;
using System.Text;
using TextParser.Tokens;

namespace TextParser
{
    public class TokenTree
    {
        private readonly TokenGenerator _tokenGenerator = new TokenGenerator();
        private TokenTreeList _parameters;

        public TokenTree(TokenTreeList children = null) : this("", "", children)
        {
        }

        public TokenTree(string key, string value, TokenTreeList children = null)
        {
            Key = _tokenGenerator.Parse(key).Simplify()[0];
            Value = _tokenGenerator.Parse(value)?.Simplify()[0] ?? new StringToken("");
            Children = children ?? new TokenTreeList();
        }

        public TokenTree(IToken key, IToken value)
        {
            Key = key;
            Value = value;
            Children = new TokenTreeList();
        }

        public TokenTreeList Children { get; private set; }

        public string this[string name]
        {
            get
            {
                TokenTreeList list = GetAll(name);
                if (list.Count == 0)
                    return name == "NAME" ? Name : null;

                if (list.Count > 0)
                {
                    TokenList tokens = list[0].Value.Evaluate(_parameters);
                    if (tokens.Count == 1)
                        return tokens[0].Text;

                    Dictionary<string, int> all = new Dictionary<string, int>();
                    foreach (IToken child in tokens)
                    {
                        int count;
                        string value = child.Text;
                        all[value] = all.TryGetValue(value, out count) ? ++count : 1;
                    }

                    StringBuilder sb = new StringBuilder();
                    foreach (var item in all)
                        sb.Append(item.Key).Append("(").Append(item.Value).Append(")/");
                    return sb.ToString().TrimEnd('/');
                }
                return null;
            }
            set
            {
                if (this[name] != value)
                    Children.SetValue(name, value);
            }
        }

        public IToken Key { get; set; }
        public string Name => Key.Text;
        public IToken Value { get; set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return $"TokenTree: {Name}={Value}";
        }

        public TokenTree FindFirst(string name)
        {
            TokenTreeList tokenTreeList = GetAll(name);
            return tokenTreeList.Count > 0 ? tokenTreeList[0] : null;
        }

        public TokenTreeList GetChildren(string name)
        {
            return GetAll(name + ".ALL");
        }

        public TokenTreeList GetAll(string name)
        {
            return Children.FindMatches(name);
        }

        public void SetParameters(TokenTree values)
        {
            _parameters = new TokenTreeList(this) {values};
        }

        public void Replace(TokenTree inputs)
        {
            TokenTree result = Children.FirstOrDefault(child => child.Name == inputs.Name);
            if (result != null)
                Children.Remove(result);
            Children.Add(inputs);
        }

        public TokenTree Clone()
        {
            return new TokenTree(Key, Value) {Children = Children.Clone()};
        }

        public void AddMissing(TokenTree defaults)
        {
            foreach (TokenTree child in defaults.Children)
            {
                bool found = Children.Any(item => item.Name == child.Name);
                if (!found)
                    Children.Add(child.Clone());
            }
        }
    }
}
