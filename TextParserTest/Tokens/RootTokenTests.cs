using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TextParser.Tokens;

namespace TextParserTest.Tokens
{
    [TestClass]
    public class RootTokenTests
    {
        [TestMethod]
        public void TestMatching()
        {
            //RootToken toSearch = new RootToken();
            //toSearch.AddChild(new PairToken(new KeyListToken() { new StringToken("C"), new StringToken("D") }, new StringToken("E")));
            //toSearch.AddChild(new PairToken(new StringToken("A"), new StringToken("B")));
            //toSearch.AddChild(new PairToken(new RegExToken(".*F", RegExToken.RegexType.Regex), new StringToken("G")));
            //toSearch.AddChild(new PairToken(new RegExToken("*H", RegExToken.RegexType.Wildcard), new StringToken("I")));
            //toSearch.AddChild(new PairToken(new RegExToken("%J", RegExToken.RegexType.Sql), new StringToken("K")));
            //Assert.AreEqual("B", toSearch["A"]);
            //Assert.AreEqual("E", toSearch["C"]);
            //Assert.AreEqual("E", toSearch["D"]);
            //Assert.AreEqual("G", toSearch["IF"]);
            //Assert.AreEqual("I", toSearch["EACH"]);
            //Assert.AreEqual("K", toSearch["RAJ"]);
            //Assert.AreEqual(null, toSearch["ZZZZZZ"]);
        }
    }
}
