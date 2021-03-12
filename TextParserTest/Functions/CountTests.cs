﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using TextParser.Functions;
using TextParser.Tokens;
using TextParser.Tokens.Interfaces;

namespace TextParserTest
{
    [TestClass]
    public class CountTests
    {
        [TestMethod]
        public void TestCount()
        {
            CountFunction function = new CountFunction();
            ListToken list = null;
            IToken result = function.Perform(list, null, true);
            Assert.IsInstanceOfType(result, typeof(IntToken));
            Assert.AreEqual(0, ((IntToken)result).Value);
            list = new ListToken();
            result = function.Perform(list, null, true);
            Assert.IsInstanceOfType(result, typeof(IntToken));
            Assert.AreEqual(0, ((IntToken)result).Value);
            list.Add(new BoolToken(true));
            result = function.Perform(list, null, true);
            Assert.IsInstanceOfType(result, typeof(IntToken));
            Assert.AreEqual(1, ((IntToken)result).Value);
            list.Add(new BoolToken(true));
            result = function.Perform(list, null, true);
            Assert.IsInstanceOfType(result, typeof(IntToken));
            Assert.AreEqual(2, ((IntToken)result).Value);
        }

        [TestMethod]
        public void TestCountWithALL()
        {
            CountFunction function = new CountFunction();
            ListToken list = null;
            IToken result = function.Perform(list, null, true);
            list = new ListToken();
            list.Add(new BoolToken(true));
            list.Add(new BoolToken(true));
            result = function.Perform(list, null, true);
            Assert.IsInstanceOfType(result, typeof(IntToken));
            Assert.AreEqual(2, ((IntToken)result).Value);
            list.Add(new StringToken("ALL"));
            result = function.Perform(list, null, true);
            Assert.IsInstanceOfType(result, typeof(IntToken));
            Assert.AreEqual(2, ((IntToken)result).Value);
        }
    }
}