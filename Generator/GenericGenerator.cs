using TextParser;

namespace Generator
{
    /// <summary>
    /// Takes a vtt data string and a vtt data template and creates a string from them.
    /// </summary>
    public class GenericGenerator
    {
        /// <summary>
        /// Generate the output specified by the input data.
        /// </summary>
        /// <param name="input">The data to output.</param>
        /// <param name="template">The template used to output the data.</param>
        /// <param name="outputKey">The key identifying the token that controls the output.</param>
        /// <returns></returns>
        public string Generate(string input, string template, string outputKey = "Output")
        {
            TokenTree templateTree = Parser.ParseString(template);
            TokenTreeList trees = new TokenTreeList {Parser.ParseString(input), templateTree};
            return templateTree.FindFirst(outputKey).Value.Evaluate(trees, true).ToString();
        }
    }
}
