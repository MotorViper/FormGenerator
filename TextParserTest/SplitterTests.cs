using Microsoft.VisualStudio.TestTools.UnitTesting;
using TextParser;

namespace TextParserTest
{
    [TestClass]
    public class SplitterTests
    {
        [TestMethod]
        public void TestSplitter()
        {
            Test("   a   : string   ", "a", "string");
            Test("a:string", "a", "string");
            Test("   'a:b'   : string   ", "a:b", "string");
        }

        private static void Test(string toSplit, string key, string value)
        {
            TokenTree tokenTree = new Splitter().Split(toSplit);
            Assert.AreEqual(key, tokenTree.Name, toSplit);
            Assert.AreEqual(value, tokenTree.Value.Text, toSplit);
        }
    }
}
