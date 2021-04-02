using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using TextParser;
using TextParser.Tokens;
using TextParser.Tokens.Interfaces;

namespace TextParserTest
{
    [TestClass]
    public class TokenGeneratorTests
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Global - used externally
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void TestSimple()
        {
            IToken token = TokenGenerator.Parse("word");
            Assert.IsTrue(token is StringToken);
            Assert.AreEqual("word", token.ToString());
            Assert.AreEqual(0, token.ToInt());
        }

        [TestMethod]
        public void TestNumber()
        {
            IToken token = TokenGenerator.Parse("4");
            Assert.IsTrue(token is TypeToken<int>);
            Assert.AreEqual("4", token.ToString());
            Assert.AreEqual(4, token.ToInt());

            token = TokenGenerator.Parse("4.5");
            Assert.IsTrue(token is TypeToken<double>);
            Assert.AreEqual("4.5", token.ToString());
            Assert.AreEqual(4.5, token.ToDouble());
        }

        [TestMethod]
        public void TestPlus()
        {
            IToken token = TokenGenerator.Parse("2 + 2");
            Assert.IsTrue(token is ExpressionToken, token.GetType().Name);
            Assert.AreEqual("(2+2)", token.ToString());

            token = token.Simplify();
            Assert.IsTrue(token is TypeToken<int>, token.GetType().Name);
            Assert.AreEqual("4", token.ToString());
            Assert.AreEqual(4, token.ToInt());

            token = TokenGenerator.Parse("\"2+2\"");
            Assert.IsTrue(token is StringToken);
            Assert.AreEqual("2+2", token.ToString());
        }

        [TestMethod]
        public void TestIntegerMath()
        {
            Dictionary<string, int> tests = new Dictionary<string, int>
            {
                ["6 + -2"] = 4,
                ["8 / -(2 + 2)"] = -2,
                ["6 + 2"] = 8,
                ["6 * 2"] = 12,
                ["6 - 2"] = 4,
                ["6 / 2"] = 3,
                ["2 + 6 + 2"] = 10,
                ["2 + 6 * 2"] = 14,
                ["2 + 6 - 2"] = 6,
                ["2 + 6 / 2"] = 5,
                ["6 + 2 + 2"] = 10,
                ["6 * 2 + 2"] = 14,
                ["6 - 2 + 2"] = 6,
                ["6 / 2 + 2"] = 5,
                ["6 + (2 + 2)"] = 10,
                ["6 * (2 + 2)"] = 24,
                ["6 - (2 + 2)"] = 2,
                ["6 - (-2 + 2)"] = 6,
                ["(-2 + 2) - 6"] = -6,
                ["8 / (2 + 2)"] = 2,
                ["10 + (12 - 10) / 2"] = 11
            };

            foreach (var test in tests)
            {
                IToken token = TokenGenerator.Parse(test.Key).Simplify();
                Assert.IsTrue(token is TypeToken<int>, test.Key + ": " + token.ToString());
                Assert.AreEqual(test.Value, token.ToInt(), test.Key);
            }
        }

        [TestMethod]
        public void TestFloatMath()
        {
            Dictionary<string, double> tests = new Dictionary<string, double>
            {
                ["6.1 + 2.1"] = 8.2,
                ["6.1 + 2"] = 8.1,
                ["6 + 2.1"] = 8.1,
                ["6.1 * 2.0"] = 12.2,
                ["6 * 2.0"] = 12.0,
                ["6.1 * 2"] = 12.2,
                ["6.4 - 2.3"] = 4.1,
                ["6.4 - 2"] = 4.4,
                ["6 - 2.3"] = 3.7,
                ["6.1 / 2.0"] = 3.05,
                ["6.1 / 2"] = 3.05,
                ["6 / 2.0"] = 3,
                ["5.4 / -1.8"] = -3,
                ["-5.4 / 1.8"] = -3
            };

            foreach (var test in tests)
            {
                IToken token = TokenGenerator.Parse(test.Key).Simplify();
                Assert.IsTrue(token is TypeToken<double>, test.Key + ": " + token.ToString());
                Assert.AreEqual(test.Value, token.ToDouble(), 0.001, test.Key);
            }
        }

        [TestMethod]
        public void TestStringMath()
        {
            Dictionary<string, string> tests = new Dictionary<string, string>
            {
                ["'+-' + '-+'"] = "+--+",
                ["'123456' - '123'"] = "456",
                ["'1232323456' / '23'"] = "3"
            };

            foreach (var test in tests)
            {
                IToken token = TokenGenerator.Parse(test.Key).Simplify();
                Assert.AreEqual(test.Value, token.ToString(), test.Key);
            }
        }

        [TestMethod]
        public void TestMixedStringMath()
        {
            Dictionary<string, string> tests = new Dictionary<string, string>
            {
                ["'+-' * 2"] = "+-+-",
                ["'+-' * 2.5"] = "+-+-+",
                ["'+-' + 2"] = "+-2",
                ["'+-' + 2.3"] = "+-2.3",
                ["2 * '+-'"] = "+-+-",
                ["2.5 * '+-'"] = "+-+-+",
                ["2 + '+-'"] = "2+-",
                ["2.3 + '+-'"] = "2.3+-",
                ["'123' + 2 * 2 + '567'"] = "1234567"
            };

            foreach (var test in tests)
            {
                IToken token = TokenGenerator.Parse(test.Key).Simplify();
                Assert.IsTrue(token is TypeToken<string>, test.Key + ": " + token.ToString());
                Assert.AreEqual(test.Value, token.ToString(), test.Key);
            }
        }

        //[TestMethod]
        //[DataSource("System.Data.Odbc",
        //    "Dsn=Excel Files; dbq=|DataDirectory|\\TokeniserTestData.xlsx;defaultdir=C:\\Development\\Projects\\FormGenerator\\TextParserTest\\TestData; " +
        //    "driverid=1046;maxbuffersize=2048;pagetimeout=5",
        //    "TokeniserTestData$", DataAccessMethod.Sequential)]
        //[DeploymentItem("TestData\\TokeniserTestData.xlsx")]
        //public void TestFromFile()
        //{
        //    DataRow row = TestContext.DataRow;
        //    string input = row[0].ToString().Trim();
        //    string output = row[1].ToString().Trim();
        //    IToken parsed = TokenGenerator.Parse(input);
        //    IToken simplified = parsed.Simplify();
        //    Assert.AreEqual(output, simplified.ToString());
        //}

        [TestMethod]
        public void TestSubstitution()
        {
            Dictionary<string, string> tests = new Dictionary<string, string>
            {
                ["$($e + '.c')"] = "d",
                ["$a + $b"] = "7",
                ["$$d"] = "4",
                ["{b}"] = "4",
                ["${d}"] = "4",
                ["$($d)"] = "4",
                ["'a' $b + $d 'd'"] = "a4bd",
                ["{$e.$f}"] = "d",
                ["$a"] = "3",
                ["$($e + '.c') + $b"] = "d4",
                ["$a.c + $b"] = "d4",
                ["{'a' + 'b'}"] = "5",
                ["{$d + $e}"] = "12"
            };

            TokenTree parameters = new TokenTree
            {
                Children =
                {
                    new TokenTree("a", "3")
                    {
                        Children = {new TokenTree("c", "d")}
                    },
                    new TokenTree("b", "4"),
                    new TokenTree("ab", "5"),
                    new TokenTree("ba", "12"),
                    new TokenTree("d", "b"),
                    new TokenTree("e", "a"),
                    new TokenTree("f", "c")
                }
            };

            int passedCount = 0;
            string resultString = "";
            foreach (var test in tests)
            {
                try
                {
                    IToken token = TokenGenerator.Parse(test.Key);
                    token = token.Evaluate(new TokenTreeList(parameters), true);
                    bool passed = test.Value == token.ToString();
                    string result = passed ? "passed" : $"failed {token.ToString()}";
                    if (passed)
                        ++passedCount;
                    resultString += Environment.NewLine + $"'{test.Key}' {result}.";
                }
                catch (Exception ex)
                {
                    resultString += Environment.NewLine + $"'{test.Key}' {ex.Message}";
                }
            }
            Assert.AreEqual(tests.Count, passedCount, resultString);
        }
    }
}
