using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TextParser.Functions;
using TextParser.Tokens;
using TextParser.Tokens.Interfaces;

namespace TextParserTest.Functions
{
    [TestClass]
    public class AggregateFunctionTests
    {
        [TestMethod]
        public void Test_Perform_NonList()
        {
            IntToken token = new IntToken(0);
            AggregateFunction function = new AggregateFunction();
            IToken result = function.Perform(token, null, false);
            Assert.IsInstanceOfType(result, typeof(ListToken));
            ListToken list = result as ListToken;
            Assert.AreEqual(1, list.Count);
            Assert.IsInstanceOfType(list.Value[0], typeof(ListToken));
            list = list.Value[0] as ListToken;
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(token, list.Value[0]);
            Assert.AreEqual(1, list.Value[1].ToInt());
        }

        [TestMethod]
        public void Test_Perform_List()
        {
            IntToken token1 = new IntToken(0);
            StringToken token2 = new StringToken("1");
            DoubleToken token3 = new DoubleToken(0.1);
            StringToken token4 = new StringToken("2");
            StringToken token5 = new StringToken("0");
            ListToken token = new ListToken() { token1, token2, token3, token1, token4, token1, token2, token5 };
            AggregateFunction function = new AggregateFunction();
            IToken result = function.Perform(token, null, false);
            Assert.IsInstanceOfType(result, typeof(ListToken));
            ListToken resultList = result as ListToken;
            Assert.AreEqual(4, resultList.Count);

            Assert.IsInstanceOfType(resultList.Value[0], typeof(ListToken));
            ListToken list = resultList.Value[0] as ListToken;
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("0", list.Value[0].ToString());
            Assert.AreEqual(4, list.Value[1].ToInt());

            list = resultList.Value[1] as ListToken;
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("1", list.Value[0].ToString());
            Assert.AreEqual(2, list.Value[1].ToInt());

            list = resultList.Value[2] as ListToken;
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("0.1", list.Value[0].ToString());
            Assert.AreEqual(1, list.Value[1].ToInt());

            list = resultList.Value[3] as ListToken;
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("2", list.Value[0].ToString());
            Assert.AreEqual(1, list.Value[1].ToInt());
        }
    }
}
