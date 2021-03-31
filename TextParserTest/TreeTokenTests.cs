using Microsoft.VisualStudio.TestTools.UnitTesting;
using TextParser;
using TextParser.Tokens;

namespace TextParserTest
{
    /// <summary>
    /// Tests for the TokenTree class.
    /// </summary>
    [TestClass]
    public class TreeTokenTests
    {
        //[TestMethod]
        //public void TestMatching()
        //{
        //    TokenTreeList children = new TokenTreeList
        //    {
        //        new TokenTree(new ListToken(new StringToken("C"), new StringToken("D")), new StringToken("E")),
        //        new TokenTree(new StringToken("A"), new StringToken("B")),
        //        new TokenTree(new RegExToken(".*F", RegExToken.RegexType.Regex), new StringToken("G")),
        //        new TokenTree(new RegExToken("*H", RegExToken.RegexType.Wildcard), new StringToken("I")),
        //        new TokenTree(new RegExToken("%J", RegExToken.RegexType.Sql), new StringToken("K"))
        //    };
        //    TokenTree toSearch = new TokenTree(children);
        //    Assert.AreEqual("B", toSearch["A"]);
        //    Assert.AreEqual("E", toSearch["C"]);
        //    Assert.AreEqual("E", toSearch["D"]);
        //    Assert.AreEqual("G", toSearch["IF"]);
        //    Assert.AreEqual("I", toSearch["EACH"]);
        //    Assert.AreEqual("K", toSearch["RAJ"]);
        //}
    }
}
