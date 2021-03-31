using Microsoft.VisualStudio.TestTools.UnitTesting;
using TextParser.Tokens;
using TextParser.Tokens.Interfaces;

namespace TextParserTest.Tokens
{
    [TestClass]
    public class TreeTokenTests
    {
        //[TestMethod]
        //public void Test_FindToken_Simple()
        //{
        //    PairToken child = PairToken.Create("c: d");
        //    TreeToken token = TreeToken.Create("a: b", children: new ChildListToken() { child });

        //    IToken found = token.FindToken("a", false, false);
        //    Assert.AreEqual(token, found);

        //    found = token.FindToken("b", false, false);
        //    Assert.AreEqual(null, found);

        //    found = token.FindToken("c", false, false);
        //    Assert.AreEqual(null, found);

        //    found = token.FindToken("c", true, false);
        //    Assert.AreEqual(child, found);
        //}

        //public void Test_FindToken_Nested()
        //{
        //    PairToken child = PairToken.Create("c: d");
        //    TreeToken token = TreeToken.Create("a: b", children: new ChildListToken() { child });

        //    IToken found = token.FindToken("a.b", false);
        //    Assert.AreEqual(token, found);

        //    found = token.FindToken("a.c", false);
        //    Assert.AreEqual(child, found);

        //    found = token.FindToken("b.c", false);
        //    Assert.AreEqual(null, found);
        //}
    }
}
