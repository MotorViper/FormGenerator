using System.IO;
using TextParser;
using TextParser.Tokens;

namespace Generator
{
    public class GenericGenerator
    {
        public string Generate(string input, string template, string outputKey = "Output")
        {
            TokenTree templateTree = Parser.ParseString(template);
            TokenTreeList trees = new TokenTreeList {Parser.ParseString(input), templateTree};
            return templateTree.FindFirst(outputKey).Value.Evaluate(trees, true).Text;
        }
    }
}
