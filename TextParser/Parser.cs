using System.Collections.Generic;
using System.IO;
using Helpers;

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
                TokenTree parent = lastAtLevel[line.Offset - 1];
                lastAtLevel[line.Offset] = tokenTree;
                parent.Children.Add(tokenTree);
            }
            return lastAtLevel[lastAtLevel[-1].Children.Count == 1 ? 0 : -1];
        }
    }
}
