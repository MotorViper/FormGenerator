using Microsoft.VisualStudio.TestTools.UnitTesting;
using TextParser;
using TextParser.Tokens;
using TextParser.Tokens.Interfaces;

namespace TextParserTest.Functions
{
    [TestClass]
    public class KeysFunctionTests
    {
        [TestMethod]
        public void Test_Perform()
        {
            TokenTree parameters = Parser.ParseString(@"Head:
    Head2:
        Parent:
            a: 1
            b: 2");
            IToken expression = TokenGenerator.Parse("KEYS:Parent");
            IToken result = expression.Evaluate(parameters.Children, true);
            Assert.IsInstanceOfType(result, typeof(ListToken));
            Assert.AreEqual("b", ((StringToken)((ListToken)result).Value[1]).Value);
        }
    }
}
