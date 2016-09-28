using Microsoft.VisualStudio.TestTools.UnitTesting;
using TextParser;

namespace TextParserTest
{
    [TestClass]
    public class LineTests
    {
        [TestMethod]
        public void TestCreateLines()
        {
            int offset = 0;
            for (int i = 0; i < 10; ++i)
            {
                Line line = new Line(new string(' ', i) + "T");
                Assert.AreEqual("T", line.Content);
                Assert.AreEqual(offset, line.Offset);
                if (i % 4 == 0)
                    ++offset;
            }
        }

        [TestMethod]
        public void TestLineStart()
        {
            Assert.AreEqual(0, new Line("aaa").LineStart);
            Assert.AreEqual(1, new Line(" aaa").LineStart);
            Assert.AreEqual(1, new Line("\taaa").LineStart);
            Assert.AreEqual(3, new Line(" \t aaa  \t  ").LineStart);
        }
    }
}
