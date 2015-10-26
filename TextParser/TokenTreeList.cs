using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using TextParser.Annotations;
using TextParser.Tokens;

namespace TextParser
{
    public class TokenTreeList : List<TokenTree>, INotifyPropertyChanged
    {
        private readonly List<string> _funcList = new List<string> {"SUMI", "SUMD", "AGG", "SUM", "COUNT"};
        private readonly TokenGenerator _generator = new TokenGenerator();

        public TokenTreeList()
        {
        }

        public TokenTreeList(TokenTree tree)
        {
            Add(tree);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public TokenTree FindValue(string value)
        {
            return this.FirstOrDefault(child => child.Value.Text == value);
        }

        public TokenTreeList FindMatches(string key, bool all = false)
        {
            TokenTreeList matches = new TokenTreeList();
            string[] parts = key.Split(new[] {'.'}, 2);
            if (parts.Length == 2 && _funcList.Contains(parts[0]))
            {
                string first = parts[0];
                TokenTreeList items = FindMatches(parts[1], true);
                switch (first)
                {
                    case "SUMI":
                    case "SUM":
                    {
                        int sum = items.Sum(token => token.Value.Convert<int>());
                        matches.Add(new TokenTree(new StringToken("SUM"), new IntToken(sum)));
                    }
                        break;
                    case "SUMD":
                    {
                        double sum = items.Sum(token => token.Value.Convert<double>());
                        matches.Add(new TokenTree(new StringToken("SUM"), new DoubleToken(sum)));
                    }
                        break;
                    case "AGG":
                        Dictionary<string, int> found = new Dictionary<string, int>();
                        foreach (TokenTree child in items)
                        {
                            int count;
                            string value = child.Value.Text;
                            found[value] = found.TryGetValue(value, out count) ? ++count : 1;
                        }

                        StringBuilder sb = new StringBuilder();
                        foreach (var item in found)
                            sb.Append(item.Key).Append("(").Append(item.Value).Append(")/");
                        matches.Add(new TokenTree(new StringToken("AGG"), new StringToken(sb.ToString().TrimEnd('/'))));
                        break;
                    case "COUNT":
                        matches.Add(new TokenTree(new StringToken("COUNT"), new IntToken(items.Count)));
                        break;
                }
            }
            else
            {
                if (all)
                {
                    key = "ALL." + key;
                    parts = key.Split(new[] {'.'}, 2);
                }
                string first = parts[0];
                List<TokenTree> tokens = first == "ALL" ? this : this.Where(child => child.Name == first).ToList();

                if (parts.Length == 2)
                {
                    string last = parts[1];
                    foreach (TokenTree tree in tokens)
                        matches.AddRange(tree.Children.FindMatches(last));
                    if (matches.Count == 0 && last == "NAME")
                        matches.AddRange(tokens.Select(tree => new TokenTree("NAME", tree.Name)));
                }
                else
                {
                    matches.AddRange(tokens);
                }
            }
            return matches;
        }

        public void SetValue(string name, string value)
        {
            string[] parts = name.Split(new[] {'.'}, 2);
            TokenTreeList matches = FindMatches(parts[0]);
            if (matches.Count == 0)
                matches.Add(new TokenTree(parts[0], ""));

            if (parts.Length == 2)
            {
                foreach (TokenTree tree in matches)
                    tree.Children.SetValue(parts[1], value);
            }
            else
            {
                foreach (TokenTree tree in matches)
                    tree.Value = _generator.Parse(value);
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
    }
}
