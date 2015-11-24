using Microsoft.VisualStudio.TestTools.UnitTesting;
using TextParser;

namespace TextParserTest
{
    [TestClass]
    public class ParserTests
    {
        [TestMethod]
        public void TestCreation()
        {
            string VTL = @"
Field: Grid
    Field: Label
        Content: Hi";

            TokenTree tokenTree = Parser.ParseString(VTL);
            Assert.AreEqual("Grid", tokenTree.Value.Text);
            Assert.AreEqual("Label", tokenTree.Children[0].Value.Text);
            Assert.AreEqual("Hi", tokenTree.Children[0].Children[0].Value.Text);
        }


        [TestMethod]
        public void TestHandleMultiPartKey()
        {
            TokenTree tokenTree = Parser.ParseString(@"
A: 1
B: 2
A.B: 3");
            Assert.AreEqual("1", tokenTree.FindFirst("A").Value.Text);
            Assert.AreEqual("2", tokenTree.FindFirst("B").Value.Text);
            Assert.AreEqual("3", tokenTree.FindFirst("A.B").Value.Text);
        }
    }
}
