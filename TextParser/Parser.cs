using System.Collections.Generic;
using System.IO;

namespace TextParser
{
    public static class Parser
    {
        public static TokenTree ParseString(string text)
        {
            return Parse(new StringReader(text));
        }

        public static TokenTree ParseFile(string fileName)
        {
            return Parse(new StreamReader(fileName));
        }

        public static TokenTree Parse(TextReader textReader)
        {
            Reader reader = new Reader(textReader);
            Splitter splitter = new Splitter();
            Dictionary<int, TokenTree> lastAtLevel = new Dictionary<int, TokenTree>
            {
                [-1] = new TokenTree()
            };
            foreach (Line line in reader)
            {
                TokenTree tokenTree = splitter.Split(line.Content);
                TokenTree parent = lastAtLevel[line.Offset - 1];
                lastAtLevel[line.Offset] = tokenTree;
                parent.Children.Add(tokenTree);
            }
            return lastAtLevel[lastAtLevel[-1].Children.Count == 1 ? 0 : -1];
        }
    }
}
