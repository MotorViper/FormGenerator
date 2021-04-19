using Microsoft.VisualStudio.TestTools.UnitTesting;
using TextParser.Tokens;

namespace TextParserTest.Tokens
{
    [TestClass]
    public class RegExTokenTests
    {
        [TestMethod]
        public void TestMatchesWithPerCent()
        {
            RegExToken token = new RegExToken("%J", RegExToken.RegexType.Sql);
            Assert.AreEqual(true, token.HasMatch((StringToken)"RAJ"));
            Assert.AreEqual(false, token.HasMatch((StringToken)"RAJA"));
            Assert.AreEqual(false, token.HasMatch((StringToken)"RAZ"));
            Assert.AreEqual(true, token.HasMatch((StringToken)"AJ"));
        }

        [TestMethod]
        public void TestMatchesWithUnderscore()
        {
            RegExToken token = new RegExToken("_J", RegExToken.RegexType.Sql);
            Assert.AreEqual(false, token.HasMatch((StringToken)"RAJ"));
            Assert.AreEqual(false, token.HasMatch((StringToken)"RAJA"));
            Assert.AreEqual(false, token.HasMatch((StringToken)"RAZ"));
            Assert.AreEqual(true, token.HasMatch((StringToken)"AJ"));
        }
    }
}
