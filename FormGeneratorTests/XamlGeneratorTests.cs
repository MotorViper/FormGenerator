using System;
using FormGenerator.Fields;
using FormGenerator.Tools;
using Generator;
using Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TextParser;
using TextParser.Tokens;

namespace FormGeneratorTests
{
    [TestClass]
    public class XamlGeneratorTests
    {
        private const string XAML =
            "<Border HorizontalAlignment=\"Stretch\" VerticalAlignment=\"Stretch\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">" +
            "  <Grid>" +
            "    <Label Content=\"Hi\" Width=\"200\" ></Label>" +
            "  </Grid>" +
            "</Border>";

        private const string VTL =
            @"
Parameters:
    FieldWidth: 100 + 100
Fields:
    Field: Grid
        Field: Label
            Content: Hi
            Width: $FieldWidth";

        [TestInitialize]
        public void Initialisation()
        {
            IOCContainer.Instance.Register<IFieldWriter>(new StringFieldWriter<Field>(""));
        }

        [TestMethod]
        public void TestSimpleGeneration()
        {
            IOCContainer.Instance.Register<IField, Field>();
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
            string generated = new XamlGenerator().GenerateXaml(toGenerate);
            Assert.AreEqual(XAML.Replace(" ", ""), generated.Replace(" ", "").Replace(Environment.NewLine, ""));
        }

        [TestMethod]
        public void TestVTL()
        {
            IOCContainer.Instance.Register<IField, Field>();
            TokenTree tokenTree = Parser.ParseString(VTL);
            string generated = new XamlGenerator().GenerateXaml(tokenTree);
            Assert.AreEqual(XAML.Replace(" ", ""), generated.Replace(" ", "").Replace(Environment.NewLine, ""));
        }
    }
}
