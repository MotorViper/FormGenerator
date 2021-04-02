using Microsoft.VisualStudio.TestTools.UnitTesting;
using TextParser.Functions;
using TextParser.Tokens;
using TextParser.Tokens.Interfaces;

namespace TextParserTest.Functions
{
    [TestClass]
    public class CaseFunctionTests
    {
        [TestMethod]
        public void Test_Perform()
        {
            StringToken key1 = new StringToken("key");
            StringToken value1 = new StringToken("v1");
            IntToken key2 = new IntToken(1);
            StringToken value2 = new StringToken("v2");
            BoolToken key3 = new BoolToken(true);
            StringToken value3 = new StringToken("v3");
            DoubleToken defaultValue = new DoubleToken(1.5);
            CaseFunction function = new CaseFunction();

            ListToken parameters = new ListToken() { new StringToken("key"), key1, value1, key2, value2, key3, value3 };
            IToken result = function.Perform(parameters, null, false);
            Assert.AreEqual(value1, result);

            parameters = new ListToken() { new StringToken("1"), key1, value1, key2, value2, key3, value3 };
            result = function.Perform(parameters, null, false);
            Assert.AreEqual(value2, result);

            parameters = new ListToken() { new StringToken("True"), key1, value1, key2, value2, key3, value3 };
            result = function.Perform(parameters, null, false);
            Assert.AreEqual(value3, result);

            parameters = new ListToken() { new StringToken("false"), key1, value1, key2, value2, key3, value3 };
            result = function.Perform(parameters, null, false);
            Assert.IsInstanceOfType(result, typeof(NullToken));

            parameters = new ListToken() { new StringToken("other"), key1, value1, key2, value2, key3, value3 };
            result = function.Perform(parameters, null, false);
            Assert.IsInstanceOfType(result, typeof(NullToken));

            parameters = new ListToken() { new StringToken("other"), key1, value1, key2, value2, key3, value3, defaultValue };
            result = function.Perform(parameters, null, false);
            Assert.AreEqual(defaultValue, result);
        }
    }
}
