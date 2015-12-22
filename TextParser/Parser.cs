using System.Collections.Generic;
using System.IO;
using Helpers;
using TextParser.Tokens;

namespace TextParser
{
    public class Parser
    {
        public Parser()
        {
            LastAtLevel = new Dictionary<int, TokenTree>
            {
                [-1] = new TokenTree()
            };
        }

        private Dictionary<int, TokenTree> LastAtLevel { get; }

        public TokenTree ParsedTree => LastAtLevel[LastAtLevel[-1].Children.Count == 1 ? 0 : -1];

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
            Parser parser = new Parser();
            Reader reader = new Reader(textReader) {DefaultDirectory = defaultDirectory};
            foreach (Line line in reader)
                parser.AddLine(line);
            return parser.ParsedTree;
        }

        public TokenTree AddLine(Line line)
        {
            TokenTree tokenTree = Splitter.Split(line.Content);
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
            TokenTree parent = LastAtLevel[line.Offset - 1];
            LastAtLevel[line.Offset] = tokenTree;
            parent.Children.Add(tokenTree);
            return tokenTree;
        }
    }
}
