using System;
using System.Linq;
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
            Key = _tokenGenerator.Parse(key).Simplify();
            IToken tokenList = value == null ? new NullToken() : _tokenGenerator.Parse(value)?.Simplify();
            Value = tokenList ?? new StringToken("");
            Children = children ?? new TokenTreeList();
        }

        public TokenTree(IToken key, IToken value, TokenTreeList children = null)
        {
            Key = key;
            Value = value;
            Children = children ?? new TokenTreeList();
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
                    IToken tokens = list[0].Value.Evaluate(_parameters, true);
                    return tokens.Text;
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
                if (child.Value.Text == "ALL")
                {
                    Children.Add(child.Clone());
                }
                else
                {
                    bool found = Children.Any(item => item.Name == child.Name);
                    if (!found)
                        Children.Add(child.Clone());
                }
            }
        }

        public void WalkTree(Action<string, string> walker, string prefix = null)
        {
            string key = prefix == null ? Name : prefix + Name;
            walker(key, Value.Text);
            foreach (TokenTree item in Children)
            {
                item.WalkTree(walker, "\t" + prefix);
            }
        }

        public void UpdateFirstLeaf(TokenTree tree)
        {
            if (tree.Children.Count == 0)
            {
                Children.SetValue(tree.Name, tree.Value);
            }
            else
            {
                TokenTree found = Children.FirstOrDefault(child => child.Name == tree.Name);
                if (found == null)
                    Children.Add(tree);
                else
                    found.UpdateFirstLeaf(tree.Children[0]);
            }
        }
    }
}
