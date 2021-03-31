using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TextParser.Functions;
using TextParser.Tokens;
using TextParser.Tokens.Interfaces;

namespace TextParserTest.Functions
{
    [TestClass]
    public class ContainsFunctionTests
    {
        [TestMethod]
        public void Test_Perform_String()
        {
            StringToken container = new StringToken("abcd");
            BaseToken toFind = new StringToken("bc");
            ListToken list = new ListToken() { container, toFind };
            ContainsFunction function = new ContainsFunction();
            IToken result = function.Perform(list, null, false);
            Assert.IsInstanceOfType(result, typeof(BoolToken));
            Assert.AreEqual(true, ((BoolToken)result).Value);

            toFind = new StringToken("de");
            list = new ListToken() { container, toFind };
            result = function.Perform(list, null, false);
            Assert.IsInstanceOfType(result, typeof(BoolToken));
            Assert.AreEqual(false, ((BoolToken)result).Value);

            container = new StringToken("ab0cd");
            toFind = new IntToken(0);
            list = new ListToken() { container, toFind };
            result = function.Perform(list, null, false);
            Assert.IsInstanceOfType(result, typeof(BoolToken));
            Assert.AreEqual(true, ((BoolToken)result).Value);

            toFind = new IntToken(1);
            list = new ListToken() { container, toFind };
            result = function.Perform(list, null, false);
            Assert.IsInstanceOfType(result, typeof(BoolToken));
            Assert.AreEqual(false, ((BoolToken)result).Value);
        }

        [TestMethod]
        public void Test_Perform_ListParameters()
        {
            IntToken token1 = new IntToken(0);
            StringToken token2 = new StringToken("1");
            DoubleToken token3 = new DoubleToken(0.1);
            StringToken token4 = new StringToken("2");
            StringToken token5 = new StringToken("0");
            ContainsFunction function = new ContainsFunction();

            BaseToken toFind = new StringToken("3");
            ListToken token = new ListToken() { token1, token2, token3, token4, token5, toFind };
            IToken result = function.Perform(token, null, false);
            Assert.IsInstanceOfType(result, typeof(BoolToken));
            Assert.AreEqual(false, ((BoolToken)result).Value);

            toFind = new IntToken(2);
            token = new ListToken() { token1, token2, token3, token4, token5, toFind };
            result = function.Perform(token, null, false);
            Assert.IsInstanceOfType(result, typeof(BoolToken));
            Assert.AreEqual(false, ((BoolToken)result).Value);

            toFind = new StringToken("2");
            token = new ListToken() { token1, token2, token3, token4, token5, toFind };
            result = function.Perform(token, null, false);
            Assert.IsInstanceOfType(result, typeof(BoolToken));
            Assert.AreEqual(true, ((BoolToken)result).Value);

            toFind = new IntToken(0);
            token = new ListToken() { token1, token2, token3, token4, token5, toFind };
            result = function.Perform(token, null, false);
            Assert.IsInstanceOfType(result, typeof(BoolToken));
            Assert.AreEqual(true, ((BoolToken)result).Value);

            toFind = new StringToken("0");
            token = new ListToken() { token1, token2, token3, token4, token5, toFind };
            result = function.Perform(token, null, false);
            Assert.IsInstanceOfType(result, typeof(BoolToken));
            Assert.AreEqual(true, ((BoolToken)result).Value);

            toFind = new DoubleToken(0);
            token = new ListToken() { token1, token2, token3, token4, token5, toFind };
            result = function.Perform(token, null, false);
            Assert.IsInstanceOfType(result, typeof(BoolToken));
            Assert.AreEqual(false, ((BoolToken)result).Value);
        }

        [TestMethod]
        public void Test_Perform_ListToken()
        {
            IntToken token1 = new IntToken(0);
            StringToken token2 = new StringToken("1");
            DoubleToken token3 = new DoubleToken(0.1);
            StringToken token4 = new StringToken("2");
            StringToken token5 = new StringToken("0");
            ContainsFunction function = new ContainsFunction();

            BaseToken toFind = new StringToken("3");
            ListToken list = new ListToken() { token1, token2, token3, token4, token5 };
            ListToken token = new ListToken() { list, toFind };
            IToken result = function.Perform(token, null, false);
            Assert.IsInstanceOfType(result, typeof(BoolToken));
            Assert.AreEqual(false, ((BoolToken)result).Value);

            toFind = new IntToken(2);
            token = new ListToken() { list, toFind };
            result = function.Perform(token, null, false);
            Assert.IsInstanceOfType(result, typeof(BoolToken));
            Assert.AreEqual(false, ((BoolToken)result).Value);

            toFind = new StringToken("2");
            token = new ListToken() { list, toFind };
            result = function.Perform(token, null, false);
            Assert.IsInstanceOfType(result, typeof(BoolToken));
            Assert.AreEqual(true, ((BoolToken)result).Value);

            toFind = new IntToken(0);
            token = new ListToken() { list, toFind };
            result = function.Perform(token, null, false);
            Assert.IsInstanceOfType(result, typeof(BoolToken));
            Assert.AreEqual(true, ((BoolToken)result).Value);

            toFind = new StringToken("0");
            token = new ListToken() { list, toFind };
            result = function.Perform(token, null, false);
            Assert.IsInstanceOfType(result, typeof(BoolToken));
            Assert.AreEqual(true, ((BoolToken)result).Value);

            toFind = new DoubleToken(0);
            token = new ListToken() { list, toFind };
            result = function.Perform(token, null, false);
            Assert.IsInstanceOfType(result, typeof(BoolToken));
            Assert.AreEqual(false, ((BoolToken)result).Value);
        }
    }
}
