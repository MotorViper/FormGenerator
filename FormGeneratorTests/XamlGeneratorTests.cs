using FormGenerator.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TextParser;
using TextParser.Tokens;

namespace FormGeneratorTests
{
    [TestClass]
    public class XamlGeneratorTests
    {
        private const string XAML =
            "<Grid xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">" +
            "  <Label Content=\"Hi\" Width=\"200\" ></Label>" +
            "</Grid>";

        private const string VTL =
            @"
Parameters:
    FieldWidth: 100 + 100
Fields:
    Field: Grid
        Field: Label
            Content: Hi
            Width: $FieldWidth";

        [TestMethod]
        public void TestSimpleGeneration()
        {
            TokenTree toGenerate = new TokenTree
            {
                Key = new StringToken(""),
                Children =
                {
                    new TokenTree
                    {
                        Key = new StringToken("Fields"),
                        Children =
                        {
                            new TokenTree
                            {
                                Key = new StringToken("Field"),
                                Value = new StringToken("Grid"),
                                Children =
                                {
                                    new TokenTree
                                    {
                                        Key = new StringToken("Field"),
                                        Value = new StringToken("Label"),
                                        Children =
                                        {
                                            new TokenTree {Key = new StringToken("Content"), Value = new StringToken("Hi")},
                                            new TokenTree {Key = new StringToken("Width"), Value = new StringToken("200")}
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };
            string generated = new XamlGenerator("").GenerateXaml(toGenerate);
            Assert.AreEqual(XAML.Replace(" ", ""), generated.Replace(" ", ""));
        }

        [TestMethod]
        public void TestVTL()
        {
            TokenTree tokenTree = Parser.ParseString(VTL);
            string generated = new XamlGenerator("").GenerateXaml(tokenTree);
            Assert.AreEqual(XAML.Replace(" ", ""), generated.Replace(" ", ""));
        }
    }
}
