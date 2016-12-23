using FormGenerator.Fields;
using Generator;
using Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TextParser;
using TextParser.Tokens;

namespace TextParserTest
{
    /// <summary>
    /// Tests for the Parser class.
    /// </summary>
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

            IOCContainer.Instance.Register<IField, Field>();
            TokenTree tokenTree = Parser.ParseString(VTL);
            Assert.AreEqual("Grid", tokenTree.Value.ToString());
            Assert.AreEqual("Label", tokenTree.Children[0].Value.ToString());
            Assert.AreEqual("Hi", tokenTree.Children[0].Children[0].Value.ToString());
        }

        [TestMethod]
        public void TestContinuations()
        {
            string text = @"a: b --
  c";
            TokenTree tokenTree = Parser.ParseString(text);
            Assert.AreEqual("a", tokenTree.Key);
            Assert.AreEqual(@"b
  c", tokenTree.Value.ToString());
        }

        [TestMethod]
        public void TestHandleMultiPartKey()
        {
            TokenTree tokenTree = Parser.ParseString(@"
A: 1
B: 2
A.B: 3");
            Assert.AreEqual("1", tokenTree.FindFirst("A").Value.ToString());
            Assert.AreEqual("2", tokenTree.FindFirst("B").Value.ToString());
            Assert.AreEqual("3", tokenTree.FindFirst("A.B").Value.ToString());
        }


        [TestMethod]
        public void TestFunctions()
        {
            string text = @"
F1: $1/2
F2: F1:($1 + $2)
F3: F2:(4|6)
PoorValue: $1
GoodValue: 2 + $1
TotalValue: SUM:OVER:({Items.ALL.NAME}|cl|INT:COMP:({Items.{cl}.{1}}|Poor|COUNT:{Level.Which={cl}}|2 + COUNT:{Level.Which={cl}}))
TotalValue1: SUM:OVER:({Items.ALL.NAME}|cl|INT:COMP:({Items.{cl}.{1}}|Poor|PoorValue:COUNT:{Level.Which={cl}}|GoodValue:COUNT:{Level.Which={cl}}))
Sum: TotalValue: Selection
Sum1: TotalValue1: Selection
Items:
    Item1:
    	Selection: Good
    Item2:
	    Selection: Good
    Item3:
	    Selection: Poor
Level : 1
    Which: Item2
Level : 2
	Which: Item3
";
            TokenTree parsed = Parser.ParseString(text);
            IToken value = parsed.FindFirst("F3").Value.Simplify();
            Assert.AreEqual("5", value.Evaluate(new TokenTreeList(parsed), true).ToString());
            value = parsed.FindFirst("Sum").Value.Simplify();
            Assert.AreEqual("4", value.Evaluate(new TokenTreeList(parsed), true).ToString());
            value = parsed.FindFirst("Sum1").Value.Simplify();
            Assert.AreEqual("4", value.Evaluate(new TokenTreeList(parsed), true).ToString());
        }
    }
}
