﻿using Generator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TextParser;
using TextParser.Tokens;

namespace GeneratorsTest
{
    [TestClass]
    public class TokenTreeElementTests
    {
        [TestMethod]
        public void TestElementType()
        {
            string data = @"
Macro1: TypeM
Macro2: TypeA
Field1: TypeF
Field2: Macro1: Parameter1
Field3: Macro2
";
            TokenTree tokenTree = Parser.ParseString(data);
            TokenTreeList list = new TokenTreeList(tokenTree);
            TokenTree tree = tokenTree.FindFirst(new StringToken("Field1", true));
            TokenTreeElement element = new TokenTreeElement(tree);
            Assert.AreEqual("TypeF", element.ElementType);

            tree = tokenTree.FindFirst(new StringToken("Field2", true));
            element = new TokenTreeElement(tree, list);
            Assert.AreEqual("TypeM", element.ElementType);

            tree = tokenTree.FindFirst(new StringToken("Field3"));
            element = new TokenTreeElement(tree, list);
            Assert.AreEqual("TypeA", element.ElementType);
        }
    }
}
