using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TextParser;
using TextParser.Operators;
using TextParser.Tokens;
using TextParser.Tokens.Interfaces;

namespace TextParserTest.Operators
{
    [TestClass]
    public class SubstitutionOperatorTests
    {
        [TestMethod]
        public void Test_Evaluate_WithException()
        {
            SubstitutionOperator op = new SubstitutionOperator();
            try
            {
                op.Evaluate(new StringToken(""), new StringToken(""), new TokenTreeList(), false);
                Assert.Fail("Should have thrown exception");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Operation $ is unary.", ex.Message);
            }

            try
            {
                op.Evaluate(null, null, new TokenTreeList(), false);
                Assert.Fail("Should have thrown exception");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Operation $ needs a variable.", ex.Message);
            }
        }

        [TestMethod]
        public void Test_Evaluate_Simple()
        {
            StringToken stringToken = new StringToken("a");
            TokenTreeList list = new TokenTreeList(new TokenTree("", "", new TokenTreeList(new TokenTree("a", "b"))));
            SubstitutionOperator substitutionOperator = new SubstitutionOperator();
            IToken result = substitutionOperator.Evaluate(null, stringToken, list, true);
            Assert.IsInstanceOfType(result, typeof(StringToken));
            Assert.AreEqual("b", ((StringToken)result).Value);
        }
    }
}
