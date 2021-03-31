using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TextParser.Tokens;

namespace TextParserTest.Tokens
{
    [TestClass]
    public class RegExTokenTests
    {
        [TestMethod]
        public void TestMatchesWithPerCent()
        {
            //RegExToken token = new RegExToken("%J", RegExToken.RegexType.Sql);
            //Assert.AreEqual(true, token.Matches("RAJ"));
            //Assert.AreEqual(false, token.Matches("RAJA"));
            //Assert.AreEqual(false, token.Matches("RAZ"));
            //Assert.AreEqual(true, token.Matches("AJ"));
        }

        [TestMethod]
        public void TestMatchesWithUnderscore()
        {
            //RegExToken token = new RegExToken("_J", RegExToken.RegexType.Sql);
            //Assert.AreEqual(false, token.Matches("RAJ"));
            //Assert.AreEqual(false, token.Matches("RAJA"));
            //Assert.AreEqual(false, token.Matches("RAZ"));
            //Assert.AreEqual(true, token.Matches("AJ"));
        }
    }
}
