using Microsoft.VisualStudio.TestTools.UnitTesting;
using TextParser.Tokens;

namespace TextParserTest.Tokens
{
    [TestClass]
    public class BoolTokenTests
    {
        [TestMethod]
        public void TestToString()
        {
            BoolToken token = new BoolToken(true);
            Assert.AreEqual("True", token.ToString());
            token = new BoolToken(false);
            Assert.AreEqual("False", token.ToString());
        }

        [TestMethod]
        public void TestToBool()
        {
            BoolToken token = new BoolToken(true);
            Assert.AreEqual(true, token.ToBool());
            token = new BoolToken(false);
            Assert.AreEqual(false, token.ToBool());
        }

        [TestMethod]
        public void TestToInt()
        {
            BoolToken token = new BoolToken(true);
            Assert.AreEqual(1, token.ToInt());
            token = new BoolToken(false);
            Assert.AreEqual(0, token.ToInt());
        }

        [TestMethod]
        public void TestToDouble()
        {
            BoolToken token = new BoolToken(true);
            Assert.AreEqual(1.0, token.ToDouble());
            token = new BoolToken(false);
            Assert.AreEqual(0.0, token.ToDouble());
        }
    }
}
