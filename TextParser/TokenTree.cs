using Helpers;
using System;
using System.IO;
using System.Linq;
using TextParser.Tokens;
using TextParser.Tokens.Interfaces;

namespace TextParser
{
    /// <summary>
    /// Tree representing an expression.
    /// </summary>
    public class TokenTree
    {
        private static bool s_initialising;
        private static TokenTree s_replacements;

        private TokenTreeList _parameters;

        public bool UseStaticCache { get; set; } = true;

        public TokenTree(TokenTreeList children = null) : this("", "", children)
        {
        }

        public TokenTree(string key, string value, TokenTreeList children = null)
        {
            Key = TokenGenerator.Parse(key).Simplify();
            value = ReplaceValue(key, value);
            IToken tokenList = value == null ? new NullToken() : TokenGenerator.Parse(value).Simplify();
            Value = tokenList ?? new StringToken("");
            Children = children ?? new TokenTreeList();
        }

        public TokenTree(IToken key, IToken value, TokenTreeList children = null)
        {
            Key = key;
            Value = value;
            Children = children ?? new TokenTreeList();
        }

        public TokenTree(string key, IToken value = null, TokenTreeList children = null)
        {
            Key = TokenGenerator.Parse(key).Simplify();
            Value = value;
            Children = children ?? new TokenTreeList();
        }

        public TokenTreeList Children { get; private set; }
        public IToken Key { get; set; }
        public string Name => Key.ToString();
        public IToken Value { get; set; }

        /// <summary>
        /// Searches for a string matching the input property.
        /// This is only for xaml interaction the token version should be used where possible.
        /// </summary>
        /// <param name="name">The property name.</param>
        /// <returns></returns>
        public string this[string name]
        {
            get
            {
                return this[new StringToken(name)];
            }
            set
            {
                this[new StringToken(Name)] = value;
            }
        }

        /// <summary>
        /// Searches for a string matching the input property.
        /// </summary>
        /// <param name="name">The property token.</param>
        /// <returns></returns>
        public string this[IToken name]
        {
            get
            {
                TokenTreeList list = GetAll(name);
                if (list.Count > 0)
                {
                    IToken tokens = list[0].Value.Evaluate(_parameters, true);
                    return tokens.ToString();
                }
                return null;
            }
            set
            {
                if (this[name] != value)
                    Children.SetValue(name, value);
            }
        }

        private static void Initialise()
        {
            if (s_replacements == null)
            {
                s_initialising = true;
                s_replacements = new TokenTree();
                IInputData inputData = IOCContainer.Instance.Resolve<IInputData>();
                if (inputData != null)
                {
                    string optionsFile = FileUtils.GetFullFileName(inputData.OptionsFile, inputData.DefaultDirectory);
                    if (File.Exists(optionsFile))
                        s_replacements = Parser.Parse(new StreamReader(optionsFile))
                                    .FindFirst(new StringToken("Replacements", true)) ?? new TokenTree();
                }
                s_initialising = false;
            }
        }

        public string ReplaceValue(string key, string value)
        {
            if (!s_initialising)
            {
                Initialise();
                if (s_replacements.Children.Count > 0)
                {
                    string replacement = string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value)
                        ? null
                        : s_replacements[new ChainToken(key, value)];
                    if (!string.IsNullOrWhiteSpace(replacement))
                        return replacement;
                }
            }
            return value;
        }

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

        public TokenTree FindFirst(IToken name)
        {
            if (!string.IsNullOrWhiteSpace(name.ToString()) && Key.ToString() == name.ToString())
                return this;
            TokenTreeList tokenTreeList = GetAll(name);
            return tokenTreeList.Count > 0 ? tokenTreeList[0] : null;
        }

        public TokenTreeList GetChildren(string name)
        {
            return Children.FindEachMatch(name);
        }

        public TokenTreeList GetAll(IToken name)
        {
            if (Key is ChainToken chain && chain.Value[1].HasMatch(name))
                return new TokenTreeList(new TokenTree(chain.Last, Value, Children));
            return Children.FindMatches(name);
        }

        public void SetParameters(TokenTree values)
        {
            _parameters = new TokenTreeList(this) { values };
        }

        public void Replace(TokenTree inputs)
        {
            Remove(inputs.Name);
            Children.Add(inputs);
        }

        public void Remove(string name)
        {
            TokenTree result = Children.FirstOrDefault(child => child.Name == name);
            if (result != null)
                Children.Remove(result);
        }

        public void RemoveAll(string name)
        {
            Children.RemoveAll(child => child.Name == name);
        }

        public TokenTree Clone()
        {
            return new TokenTree(Key, Value) { Children = Children.Clone() };
        }

        public void AddMissing(TokenTree defaults)
        {
            if (defaults == null)
                return;
            foreach (TokenTree child in defaults.Children)
            {
                bool found = Children.Any(item => item.Name == child.Name);
                if (!found)
                    Children.Add(child.Clone());
            }
        }

        public void WalkTree(Action<string, string> walker, string prefix = null)
        {
            string key = prefix == null ? Name : prefix + Name;
            walker(key, Value.ToString());
            foreach (TokenTree item in Children)
                item.WalkTree(walker, "\t" + prefix);
        }

        public void UpdateFirstLeaf(TokenTree tree)
        {
            if (tree.Children.Count == 0)
            {
                Children.SetValue(tree.Key, tree.Value);
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

        public TokenTree SubstituteParameters(TokenTree tree)
        {
            return new TokenTree(Key.SubstituteParameters(tree), Value.SubstituteParameters(tree)) { Children = Children.SubstituteParameters(tree) };
        }
    }
}
