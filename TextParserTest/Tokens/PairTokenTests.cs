using Microsoft.VisualStudio.TestTools.UnitTesting;
using TextParser.Tokens;
using TextParser.Tokens.Interfaces;

namespace TextParserTest.Tokens
{
    [TestClass]
    public class PairTokenTests
    {
        //[TestMethod]
        //public void TestCreate()
        //{
        //    Test("   a   : string   ", "a", "string");
        //    Test("a:string", "a", "string");
        //    Test("   'a:b'   : string   ", "a:b", "string");
        //    Test(@"a: b
        //c", "a", @"b
        //c");
        //}

        //private static void Test(string toSplit, string key, string value)
        //{
        //    PairToken tokenTree = PairToken.Create(toSplit);
        //    Assert.AreEqual(key, tokenTree.Key.ToString(), toSplit);
        //    Assert.AreEqual(value, tokenTree.Value.ToString(), toSplit);
        //}

        //[TestMethod]
        //public void TestFindToken()
        //{
        //    PairToken token = PairToken.Create("a: b");
        //    IToken found = token.FindToken("a", false, false);
        //    Assert.AreEqual(token, found);

        //    found = token.FindToken("b", false, false);
        //    Assert.AreEqual(null, found);
        //}
    }
}
