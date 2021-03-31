using Microsoft.VisualStudio.TestTools.UnitTesting;
using TextParser.Functions;
using TextParser.Tokens;
using TextParser.Tokens.Interfaces;

namespace TextParserTest
{
    [TestClass]
    public class DoubleFunctionTests
    {
        [TestMethod]
        public void TestPerform()
        {
            DoubleFunction function = new DoubleFunction();
            DoubleToken dToken = new DoubleToken(4.5);
            IToken result = function.Perform(dToken, null, true);
            Assert.IsInstanceOfType(result, typeof(DoubleToken));
            Assert.AreEqual(4.5, ((DoubleToken)result).Value);

            IntToken iToken = new IntToken(5);
            result = function.Perform(iToken, null, true);
            Assert.IsInstanceOfType(result, typeof(DoubleToken));
            Assert.AreEqual(5.0, ((DoubleToken)result).Value);

            StringToken sToken = new StringToken("4.8");
            result = function.Perform(sToken, null, true);
            Assert.IsInstanceOfType(result, typeof(DoubleToken));
            Assert.AreEqual(4.8, ((DoubleToken)result).Value);

            BoolToken bToken = new BoolToken(false);
            result = function.Perform(bToken, null, true);
            Assert.IsInstanceOfType(result, typeof(DoubleToken));
            Assert.AreEqual(0.0, ((DoubleToken)result).Value);

            ListToken list = new ListToken();
            list.Add(new BoolToken(true));
            list.Add(new IntToken(-7));
            list.Add(new DoubleToken(-7.7));
            list.Add(new StringToken("-8.2"));
            result = function.Perform(list, null, true);
            Assert.IsInstanceOfType(result, typeof(ListToken));
            ListToken lResult = (ListToken)result;
            Assert.AreEqual(4, lResult.Value.Count);
            Assert.IsInstanceOfType(lResult.Value[0], typeof(DoubleToken));
            Assert.AreEqual(1.0, ((DoubleToken)lResult.Value[0]).Value);
            Assert.IsInstanceOfType(lResult.Value[1], typeof(DoubleToken));
            Assert.AreEqual(-7.0, ((DoubleToken)lResult.Value[1]).Value);
            Assert.IsInstanceOfType(lResult.Value[2], typeof(DoubleToken));
            Assert.AreEqual(-7.7, ((DoubleToken)lResult.Value[2]).Value);
            Assert.IsInstanceOfType(lResult.Value[3], typeof(DoubleToken));
            Assert.AreEqual(-8.2, ((DoubleToken)lResult.Value[3]).Value);
        }
    }
}
