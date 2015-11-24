using System.Collections.Generic;
using System.IO;
using Helpers;
using TextParser.Tokens;

namespace TextParser
{
    public static class Parser
    {
        public static TokenTree ParseString(string text)
        {
            return Parse(new StringReader(text));
        }

        public static TokenTree ParseFile(string fileName, string defaultDirectory)
        {
            fileName = FileUtils.GetFullFileName(fileName, defaultDirectory);
            return Parse(new StreamReader(fileName), defaultDirectory);
        }

        public static TokenTree Parse(TextReader textReader, string defaultDirectory = null)
        {
            Reader reader = new Reader(textReader) {DefaultDirectory = defaultDirectory};
            Splitter splitter = new Splitter();
            Dictionary<int, TokenTree> lastAtLevel = new Dictionary<int, TokenTree>
            {
                [-1] = new TokenTree()
            };
            foreach (Line line in reader)
            {
                TokenTree tokenTree = splitter.Split(line.Content);
                StringToken key = tokenTree.Key as StringToken;
                if (key != null && key.Text.Contains("."))
                {
                    string[] parts = key.Text.Split('.');
                    TokenTree tree = new TokenTree(parts[0], null);
                    TokenTree top = tree;
                    for (int i = 1; i < parts.Length - 1; ++i)
                    {
                        TokenTree child = new TokenTree(parts[i], null);
                        tree.Children.Add(child);
                        tree = child;
                    }
                    tree.Children.Add(new TokenTree(new StringToken(parts[parts.Length - 1]), tokenTree.Value, tokenTree.Children));
                    tokenTree = top;
                }
                TokenTree parent = lastAtLevel[line.Offset - 1];
                lastAtLevel[line.Offset] = tokenTree;
                parent.Children.Add(tokenTree);
            }
            return lastAtLevel[lastAtLevel[-1].Children.Count == 1 ? 0 : -1];
        }
    }
}
