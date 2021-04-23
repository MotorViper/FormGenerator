using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TextParser.Functions;
using TextParser.Tokens;
using TextParser.Tokens.Interfaces;

namespace TextParserTest.Functions
{
    [TestClass]
    public class ComparisonFunctionTests
    {
        [TestMethod]
        public void Test_Perform_ThreeOptions()
        {
            StringToken value1 = new StringToken("v1");
            StringToken value2 = new StringToken("v2");
            StringToken value3 = new StringToken("v3");
            ComparisonFunction function = new ComparisonFunction();

            ListToken parameters = new ListToken() { (IntToken)1, (DoubleToken)0.0, value1, value2, value3 };
            IToken result = function.Perform(parameters, null, false);
            Assert.AreEqual(value1, result);

            parameters = new ListToken() { (StringToken)"0", (IntToken)0, value1, value2, value3 };
            result = function.Perform(parameters, null, false);
            Assert.AreEqual(value2, result);

            parameters = new ListToken() { (StringToken)"0", (DoubleToken)1, value1, value2, value3 };
            result = function.Perform(parameters, null, false);
            Assert.AreEqual(value3, result);

            parameters = new ListToken() { (BoolToken)true, (BoolToken)false, value1, value2, value3 };
            result = function.Perform(parameters, null, false);
            Assert.AreEqual(value1, result);

            try
            {
                parameters = new ListToken() { (RegExToken)"false", (BoolToken)false, value1, value2, value3 };
                result = function.Perform(parameters, null, false);
                Assert.Fail("Should throw exception");
            }
            catch (Exception exception)
            {
                Assert.AreEqual("Must have 2 to 4 values for 'COMPARE': (false|False|v1|v2|v3)", exception.Message);
            }
        }

        [TestMethod]
        public void Test_Perform_TwoOptions()
        {
            StringToken value1 = new StringToken("v1");
            StringToken value2 = new StringToken("v2");
            ComparisonFunction function = new ComparisonFunction();

            ListToken parameters = new ListToken() { (IntToken)1, (DoubleToken)0.0, value1, value2 };
            IToken result = function.Perform(parameters, null, false);
            Assert.AreEqual(value2, result);

            parameters = new ListToken() { (StringToken)"0", (IntToken)0, value1, value2 };
            result = function.Perform(parameters, null, false);
            Assert.AreEqual(value1, result);

            parameters = new ListToken() { (StringToken)"0", (DoubleToken)1, value1, value2 };
            result = function.Perform(parameters, null, false);
            Assert.AreEqual(value2, result);

            parameters = new ListToken() { (BoolToken)true, (BoolToken)false, value1, value2 };
            result = function.Perform(parameters, null, false);
            Assert.AreEqual(value2, result);

            parameters = new ListToken() { (RegExToken)"False", (BoolToken)false, value1, value2 };
            result = function.Perform(parameters, null, false);
            Assert.AreEqual(value1, result);

            parameters = new ListToken() { (RegExToken)"true", (BoolToken)false, value1, value2 };
            result = function.Perform(parameters, null, false);
            Assert.AreEqual(value2, result);
        }

        [TestMethod]
        public void Test_Perform_OneOption()
        {
            StringToken value1 = new StringToken("v1");
            ComparisonFunction function = new ComparisonFunction();

            ListToken parameters = new ListToken() { (IntToken)1, (DoubleToken)0.0, value1 };
            IToken result = function.Perform(parameters, null, false);
            Assert.IsInstanceOfType(result, typeof(NullToken));

            parameters = new ListToken() { (StringToken)"0", (IntToken)0, value1 };
            result = function.Perform(parameters, null, false);
            Assert.AreEqual(value1, result);

            parameters = new ListToken() { (StringToken)"0", (DoubleToken)1, value1 };
            result = function.Perform(parameters, null, false);
            Assert.IsInstanceOfType(result, typeof(NullToken));

            parameters = new ListToken() { (BoolToken)true, (BoolToken)false, value1 };
            result = function.Perform(parameters, null, false);
            Assert.IsInstanceOfType(result, typeof(NullToken));

            parameters = new ListToken() { (RegExToken)"False", (BoolToken)false, value1 };
            result = function.Perform(parameters, null, false);
            Assert.AreEqual(value1, result);

            parameters = new ListToken() { (RegExToken)"true", (BoolToken)false, value1 };
            result = function.Perform(parameters, null, false);
            Assert.IsInstanceOfType(result, typeof(NullToken));
        }
    }
}
