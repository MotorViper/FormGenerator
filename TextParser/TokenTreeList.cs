using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using TextParser.Annotations;
using TextParser.Tokens.Interfaces;

namespace TextParser
{
    /// <summary>
    /// A list of TokenTree objects.
    /// </summary>
    public class TokenTreeList : List<TokenTree>, INotifyPropertyChanged
    {
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

        public TokenTreeList FindMatches(string key, bool all = false)
        {
            TokenTreeList matches = new TokenTreeList();
            string[] parts = key.Split(new[] {'.'}, 2);
            if (all)
            {
                key = "ALL." + key;
                parts = key.Split(new[] {'.'}, 2);
            }
            string first = parts[0];
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
            else
            {
                if (first == "ALL")
                {
                    tokens = this;
                }
                else
                {
                    tokens = this.Where(child => child.Key.Contains(first)).ToList();
                    if (tokens.Count == 0)
                        tokens = this.Where(child => child.Name == "ALL").ToList();
                }
            }

            if (parts.Length == 2)
            {
                string last = parts[1];
                foreach (TokenTree tree in tokens)
                    matches.AddRange(tree.GetAll(last));
                if (matches.Count == 0 && last == "NAME")
                    matches.AddRange(tokens.Select(tree => new TokenTree("NAME", tree.Name)));
            }
            else
            {
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
            string[] parts = name.Split(new[] {'.'}, 2);
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
