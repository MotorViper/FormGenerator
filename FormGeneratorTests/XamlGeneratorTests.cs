﻿using FormGenerator.Fields;
using FormGenerator.Tools;
using Generator;
using Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TextParser;
using TextParser.Tokens;

namespace FormGeneratorTests
{
    [TestClass]
    public class XamlGeneratorTests
    {
        private const string VTL =
            @"
Parameters:
    FieldWidth: 100 + 100
Fields:
    Field: Grid
        Field: Label
            Content: Hi
            Width: $FieldWidth
            Padding: 3,0";

        private const string XAML =
            "<Border HorizontalAlignment=\"Stretch\" VerticalAlignment=\"Stretch\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">" +
            "  <Grid>" +
            "    <Label Content=\"Hi\" Width=\"200\" Padding=\"3,0\"></Label>" +
            "  </Grid>" +
            "</Border>";

        [TestInitialize]
        public void Initialisation()
        {
            IOCContainer.Instance.Register<IFieldWriter>(new StringFieldWriter<GenericField>(""));
            IOCContainer.Instance.Register<IField, GenericField>();
        }

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
                                            new TokenTree {Key = new StringToken("Width"), Value = new StringToken("200")},
                                            new TokenTree {Key = new StringToken("Padding"), Value = new StringToken("3,0")}
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
            TokenTree tokenTree = Parser.ParseString(VTL);
            string generated = new XamlGenerator().GenerateXaml(tokenTree);
            Assert.AreEqual(XAML.Replace(" ", ""), generated.Replace(" ", "").Replace(Environment.NewLine, ""));
        }
    }
}
