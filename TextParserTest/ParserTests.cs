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
    }
}
