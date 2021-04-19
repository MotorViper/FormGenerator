using Microsoft.VisualStudio.TestTools.UnitTesting;
using TextParser;
using TextParser.Tokens;
using TextParser.Tokens.Interfaces;

namespace TextParserTest.Tokens
{
    [TestClass]
    public class ChainTokenTests
    {
        [TestMethod]
        public void Test_ProcessChain()
        {
            string text = @"
A: C
B: 2
    C: 3
        D: 4
E: {B.{A}.D}";
            TokenTree parsed = Parser.ParseString(text);
            IToken value = parsed.FindFirst(new StringToken("E")).Value.Simplify();
            value = value.Evaluate(new TokenTreeList(parsed), true);
            Assert.IsInstanceOfType(value, typeof(IntToken));
            Assert.AreEqual(4, value.ToInt());
        }
    }
}
