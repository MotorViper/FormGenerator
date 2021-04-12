using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using TextParser.Annotations;
using TextParser.Tokens;
using TextParser.Tokens.Interfaces;

namespace TextParser
{
    /// <summary>
    /// A list of TokenTree objects.
    /// </summary>
    public class TokenTreeList : List<TokenTree>, INotifyPropertyChanged
    {
        public bool UseStaticCache { get; set; } = true;

        public TokenTreeList()
        {
        }

        public TokenTreeList(TokenTree tree)
        {
            Add(tree);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool Remove(string value)
        {
            TokenTree found = this.FirstOrDefault(child => child.Key.ToString() == value);
            if (found != null)
                Remove(found);
            return found != null;
        }

        public TokenTreeList FindAllMatches(string key)
        {
            return PerformFind(key, this);
        }

        public TokenTreeList FindMatches(string key)
        {
            string[] parts = key.Split(new[] { '.' }, 2);
            string first = parts[0];
            string last = parts.Length == 2 ? parts[1] : null;
            List<TokenTree> tokens;
            if (first.Contains('='))
            {
                tokens = new List<TokenTree>();
                string[] searchCriteria = first.Split('=');
                foreach (TokenTree item in this)
                {
                    if (item.Key.ToString() == searchCriteria[0] && item.Value.ToString() == searchCriteria[1])
                        tokens.Add(item);
                }
            }
            else if (first.Contains("::"))
            {
                // This handles templates. Not final version but works for now.
                tokens = new List<TokenTree>();
                string[] options = first.TrimStart('(').TrimEnd(')').Split(':');
                foreach (string option in options)
                {
                    tokens = this.Where(child => child.Key.Contains(option)).ToList();
                    if (tokens.Count > 1)
                        throw new Exception("Too many possible returns for template");
                    if (tokens.Count == 1)
                    {
                        TokenTreeList found = PerformFind(last, tokens);
                        if (found.Count > 0)
                            return found;
                    }
                }
                return new TokenTreeList();
            }
            else
            {
                tokens = this.Where(child => child.Key.Contains(first)).ToList();
            }

            return PerformFind(last, tokens);
        }

        public TokenTreeList FindEachMatch(string key)
        {
            List<TokenTree> tokens = this.Where(child => child.Key.Contains(key)).ToList();
            TokenTreeList matches = new TokenTreeList();
            foreach (TokenTree tree in tokens)
            {
                foreach (TokenTree child in tree.Children)
                {
                    if (!child.UseStaticCache)
                    {
                        matches.UseStaticCache = false;
                        break;
                    }
                }
                matches.AddRange(tree.Children);
            }
            return matches;
        }

        private TokenTreeList PerformFind(string key, List<TokenTree> tokens)
        {
            TokenTreeList matches = new TokenTreeList();
            if (key != null)
            {
                foreach (TokenTree tree in tokens)
                {
                    TokenTreeList found = tree.GetAll(key);
                    if (found.Count > 0 && !tree.UseStaticCache)
                        matches.UseStaticCache = false;
                    matches.AddRange(found);
                }
                if (matches.Count == 0 && tokens.Count == 1 && tokens[0].Value != null &&
                    tokens[0].Value is ExpressionToken value && value.Second is StringToken token)
                    matches = FindMatches(token.Value + "." + key);
            }
            else
            {
                foreach (TokenTree tree in tokens)
                {
                    if (!tree.UseStaticCache)
                    {
                        matches.UseStaticCache = false;
                        break;
                    }
                }
                matches.AddRange(tokens);
            }
            return matches;
        }

        public void SetValue(string name, string value)
        {
            SetValue(name, TokenGenerator.Parse(value));
        }

        public void SetValue(string name, IToken value)
        {
            string[] parts = name.Split(new[] { '.' }, 2);
            TokenTreeList matches = FindMatches(parts[0]);
            if (matches.Count == 0)
            {
                TokenTree child = new TokenTree(parts[0], "");
                matches.Add(child);
                Add(child);
            }

            if (parts.Length == 2)
            {
                foreach (TokenTree tree in matches)
                    tree.Children.SetValue(parts[1], value);
            }
            else
            {
                foreach (TokenTree tree in matches)
                    tree.Value = value;
                OnPropertyChanged(name);
            }
        }

        public TokenTreeList Clone()
        {
            TokenTreeList list = new TokenTreeList();
            list.AddRange(this.Select(tokenTree => tokenTree.Clone()));
            return list;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void AddIfMissing(TokenTree child)
        {
            if (this.All(item => child.Name != item.Name))
                Add(child);
        }

        public TokenTreeList SubstituteParameters(TokenTree tree)
        {
            TokenTreeList list = new TokenTreeList();
            list.AddRange(this.Select(tokenTree => tokenTree.SubstituteParameters(tree)));
            return list;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (TokenTree tokenTree in this)
                sb.Append(tokenTree).Append(" ");
            return sb.ToString();
        }
    }
}
