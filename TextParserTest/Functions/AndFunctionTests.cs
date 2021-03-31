using Microsoft.VisualStudio.TestTools.UnitTesting;
using TextParser.Functions;
using TextParser.Tokens;

namespace TextParserTest.Functions
{
    [TestClass]
    public class AndFunctionTests
    {
        [TestMethod]
        public void Test_Perform()
        {
            BoolToken trueToken = new BoolToken(true);
            BoolToken falseToken = new BoolToken(false);
            ListToken list = new ListToken(trueToken);
            Assert.AreEqual(new AndFunction().Perform(list, null, true).ToBool(), true);
            list.Add(trueToken);
            Assert.AreEqual(new AndFunction().Perform(list, null, true).ToBool(), true);
            list.Add(falseToken);
            Assert.AreEqual(new AndFunction().Perform(list, null, true).ToBool(), false);
        }
    }
}
