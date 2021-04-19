using FormGenerator.Fields;
using Generator;
using Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TextParser;
using TextParser.Tokens;
using TextParser.Tokens.Interfaces;

namespace TextParserTest
{
    /// <summary>
    /// Tests for the Parser class.
    /// </summary>
    [TestClass]
    public class ParserTests
    {
        public ParserTests()
        {
            IOCContainer.Instance.Register<ILogging>(new ConsoleLogger());
        }

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
            Assert.AreEqual("a", ((StringToken)tokenTree.Key).Value);
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
                    Assert.AreEqual("1", tokenTree.FindFirst(new StringToken("A", true)).Value.ToString());
                    Assert.AreEqual("2", tokenTree.FindFirst(new StringToken("B")).Value.ToString());
                    Assert.AreEqual("3", tokenTree.FindFirst(new ChainToken("A", "B")).Value.ToString());
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
SubTotal:INT:COMP:($1|0|0|COMP:($2|Poor|PoorValue:$1|GoodValue:$1))
TotalValue: SUM:OVER:(KEYS:Items|cl|INT:COMP:(COUNT:{Level.Which={cl}}|0|0|COMP:({Items.{cl}.{1}}|Poor|COUNT:{Level.Which={cl}}|2 + COUNT:{Level.Which={cl}})))
TotalValue1: SUM:OVER:(KEYS:Items|cl|SubTotal:(COUNT:{Level.Which={cl}}|{Items.{cl}.{1}}))
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
            IToken value = parsed.FindFirst(new StringToken("F3", true)).Value.Simplify();
            Assert.AreEqual("5", value.Evaluate(new TokenTreeList(parsed), true).ToString());
            value = parsed.FindFirst(new StringToken("Sum", true)).Value.Simplify();
            Assert.AreEqual("4", value.Evaluate(new TokenTreeList(parsed), true).ToString());
            value = parsed.FindFirst(new StringToken("Sum1")).Value.Simplify();
            Assert.AreEqual("4", value.Evaluate(new TokenTreeList(parsed), true).ToString());
        }

        [TestMethod]
        public void Test_ParseString_ForChainToken()
        {
            string text = "A: b.'c.d'.e";
            TokenTree parsed = Parser.ParseString(text);
            IToken value = parsed.FindFirst(new StringToken("A")).Value;
            Assert.IsInstanceOfType(value, typeof(ChainToken));
            Assert.AreEqual("b", ((ChainToken)value).Value[0].ToString());
            Assert.AreEqual("c.d", ((ChainToken)value).Value[1].ToString());
            Assert.AreEqual("e", ((ChainToken)value).Value[2].ToString());

            text = "A: b.{d}.e";
            parsed = Parser.ParseString(text);
            value = parsed.FindFirst(new StringToken("A")).Value;
            Assert.IsInstanceOfType(value, typeof(ChainToken));
            Assert.AreEqual("b", ((ChainToken)value).Value[0].ToString());
            Assert.AreEqual("($d)", ((ChainToken)value).Value[1].ToString());
            Assert.AreEqual("e", ((ChainToken)value).Value[2].ToString());
        }

        [TestMethod]
        public void Test_ParseString_ForListToken()
        {
            string text = "Field: Dex.|Modifier";
            TokenTree parsed = Parser.ParseString(text);
            IToken value = parsed.FindFirst(new StringToken("Field")).Value;
            Assert.IsInstanceOfType(value, typeof(ListToken));
            Assert.AreEqual("Dex.", ((ListToken)value).Value[0].ToString());
            Assert.AreEqual("Modifier", ((ListToken)value).Value[1].ToString());

            text = "Columns: 6.5cm|8cm|6.5cm";
            parsed = Parser.ParseString(text);
            value = parsed.FindFirst(new StringToken("Columns")).Value;
            Assert.IsInstanceOfType(value, typeof(ListToken));
            Assert.AreEqual("6.5cm", ((ListToken)value).Value[0].ToString());
            Assert.AreEqual("8cm", ((ListToken)value).Value[1].ToString());
            Assert.AreEqual("6.5cm", ((ListToken)value).Value[2].ToString());
        }
    }
}
