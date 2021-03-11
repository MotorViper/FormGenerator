using System;
using System.IO;
using System.Linq;
using Helpers;
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

        public IToken Key { get; set; }
        public string Name => Key.ToString();
        public IToken Value { get; set; }

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
                        s_replacements = Parser.Parse(new StreamReader(optionsFile)).FindFirst("Replacements") ?? new TokenTree();
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
                    string replacement = string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value) ? null : s_replacements[$"{key}.{value}"];
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

        public TokenTree FindFirst(string name)
        {
            if (!string.IsNullOrWhiteSpace(name) && Key.ToString() == name)
                return this;
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
            return new TokenTree(Key, Value) {Children = Children.Clone()};
        }

        public void AddMissing(TokenTree defaults)
        {
            foreach (TokenTree child in defaults.Children)
            {
                if (child.Value.ToString() == "ALL")
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
            walker(key, Value.ToString());
            foreach (TokenTree item in Children)
                item.WalkTree(walker, "\t" + prefix);
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

        public TokenTree SubstituteParameters(TokenTree tree)
        {
            return new TokenTree(Key.SubstituteParameters(tree), Value.SubstituteParameters(tree)) {Children = Children.SubstituteParameters(tree)};
        }
    }
}
