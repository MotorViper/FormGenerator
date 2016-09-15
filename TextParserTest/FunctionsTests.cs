using System.Runtime.Remoting.Lifetime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TextParser.Functions;
using TextParser.Tokens;

namespace TextParserTest
{
    [TestClass]
    public class FunctionsTests
    {
        [TestMethod]
        public void TestCount()
        {
            CountFunction function = new CountFunction();
            ListToken list = null;
            IToken result = function.Perform(list, null, true);
            Assert.IsInstanceOfType(result, typeof(IntToken));
            Assert.AreEqual(0, ((IntToken)result).Value);
            list = new ListToken();
            result = function.Perform(list, null, true);
            Assert.IsInstanceOfType(result, typeof(IntToken));
            Assert.AreEqual(0, ((IntToken)result).Value);
            list.Add(new BoolTooken(true));
            result = function.Perform(list, null, true);
            Assert.IsInstanceOfType(result, typeof(IntToken));
            Assert.AreEqual(1, ((IntToken)result).Value);
            list.Add(new BoolTooken(true));
            result = function.Perform(list, null, true);
            Assert.IsInstanceOfType(result, typeof(IntToken));
            Assert.AreEqual(2, ((IntToken)result).Value);
        }
    }
}