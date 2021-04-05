using Microsoft.VisualStudio.TestTools.UnitTesting;
using TextParser;
using TextParser.Tokens;
using TextParser.Tokens.Interfaces;

namespace TextParserTest.Functions
{
    [TestClass]
    public class OverFunctionTests
    {
        [TestMethod]
        public void Test_Perform_Simple()
        {
            IToken expression = TokenGenerator.Parse("OVER:(1,2,3,i,$i*$i)");
            IToken result = expression.Evaluate(null, true);
            Assert.IsInstanceOfType(result, typeof(ListToken));
            Assert.AreEqual(4, ((IntToken)((ListToken)result).Value[1]).Value);
        }

        [TestMethod]
        public void Test_Perform_OnALLSpecifier()
        {
            TokenTree parameters = Parser.ParseString(@"Head:
    Head2:
        Parent:
            a: 1
            b: 2");
            IToken expression = TokenGenerator.Parse("OVER:(KEYS:Parent,i,$i)");
            IToken result = expression.Evaluate(parameters.Children, true);
            Assert.IsInstanceOfType(result, typeof(ListToken));
            Assert.AreEqual("b", ((StringToken)((ListToken)result).Value[1]).Value);
        }
    }
}
