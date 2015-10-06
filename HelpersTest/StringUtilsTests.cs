using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Helpers.StringUtils;

namespace HelpersTest
{
    [TestClass]
    public class StringUtilsTests
    {
        [TestMethod]
        public void TestFirstNotInBlock()
        {
            Dictionary<string, int> toTest = new Dictionary<string, int>
            {
                {":", 0},
                {" :", 1},
                {"a:", 1},
                {"a:junk:", 1},
                {"junk ", -1},
                {"':'junk:junk:", 7},
                {"'junk:'junk:junk:", 11},
                {"\":\"junk:junk:", 7},
                {"rubbish\"junk:\"junk:junk:", 18},
                {"\"junk:junk:junk:", 5},
                {"\"junk':junk:junk:", 6},
                {"here's\":junk:junk:", 7},
                {"'one:'there's:'four:", 19},
                {"\"here's some code:\"here:", 23}
            };
            TestAll(toTest, ':');

            toTest = new Dictionary<string, int>
            {
                {"'", 0},
                {" '", 1},
                {"a'", 1},
                {"1'junk'", 1},
                {"junk ", -1},
                {"':'junk:junk:", 0},
                {"'junk:'junk:junk:", 0},
                {"\"'\"1234'junk'", 7},
                {"rubbish\"junk'\"1234'junk:", 18},
                {"\"1234'1234'1234'", 5},
                {"\"junk''junk'1234:", 5},
                {"here's\":junk:junk:", 4},
                {"'one:'there's:'four:", 0},
                {"\"here's some code:\"here'", 23}
            };
            TestAll(toTest, '\'');
        }

        private static void TestAll(Dictionary<string, int> toTest, char toFind)
        {
            foreach (KeyValuePair<string, int> pair in toTest)
            {
                int firstNotInString = pair.Key.FirstNotInBlock(toFind);
                Assert.AreEqual(pair.Value, firstNotInString, $"Error in '{pair.Key}'");
            }
        }

        [TestMethod]
        public void TestSplit()
        {
            Dictionary<string, List<string>> tests = new Dictionary<string, List<string>>
            {
                ["123"] = new List<string> {"123"},
                ["\"123\""] = new List<string> {"\"123\""},
                ["'123'"] = new List<string> {"'123'"},
                ["'12\\'3'"] = new List<string> {"'12\\'3'"},
                ["1234"] = new List<string> {"1234"},
                ["\"123\"4"] = new List<string> {"\"123\"", "4"},
                ["'123'4"] = new List<string> {"'123'", "4"},
                ["'12\\'3'4"] = new List<string> {"'12\\'3'", "4"}
            };

            TestSplits(tests, DelimiterInclude.IncludeInline);

            tests = new Dictionary<string, List<string>>
            {
                ["123"] = new List<string> {"123"},
                ["\"123\""] = new List<string> {"123"},
                ["'123'"] = new List<string> {"123"},
                ["'12\\'3'"] = new List<string> {"12'3"}
            };

            TestSplits(tests, DelimiterInclude.DontInclude);

            tests = new Dictionary<string, List<string>>
            {
                ["123"] = new List<string> {"", "123", ""},
                ["\"123\""] = new List<string> {"\"", "123", "\""},
                ["'123'"] = new List<string> {"'", "123", "'"},
                ["'12\\'3'"] = new List<string> {"'", "12'3", "'"}
            };

            TestSplits(tests, DelimiterInclude.IncludeSeparately);

            tests = new Dictionary<string, List<string>>
            {
                ["(123)"] = new List<string> {"123"},
                ["123"] = new List<string> {"123"},
                ["(1)2(3)"] = new List<string> {"1", "2", "3"},
                ["((1)2)(3)"] = new List<string> {"(1)2", "3"},
                ["\\((1)2\\)3"] = new List<string> {"(", "1", "2)3"}
            };

            TestSplits(tests, DelimiterInclude.DontInclude, true);
        }

        private static void TestSplits(Dictionary<string, List<string>> tests, DelimiterInclude includeDelimiter, bool brackets = false)
        {
            foreach (var test in tests)
            {
                List<string> blocks = test.Key.SplitIntoBlocks(brackets ? new[] {'(', ')'} : null, brackets, includeDelimiter);
                List<string> expected = test.Value;
                for (int i = 0; i < blocks.Count; ++i)
                    Assert.AreEqual(expected[i], blocks[i], test.Key);
                Assert.AreEqual(expected.Count, blocks.Count, test.Key);
            }
        }

        [TestMethod]
        public void TestCount()
        {
            Assert.AreEqual(1, "ABC".CountInstances("B"));
            Assert.AreEqual(4, "ABBBBC".CountInstances("B"));
            Assert.AreEqual(3, "ABABABC".CountInstances("B"));
            Assert.AreEqual(4, "ABCABCABCABCAABAACAAA".CountInstances("BCA"));
        }

        [TestMethod]
        public void TestCreate()
        {
            Assert.AreEqual("", CreateString("+-", 0));
            Assert.AreEqual("+-+-+-", CreateString("+-", 3));
            Assert.AreEqual("+-+-+-+", CreateString("+-", 3.5));
            Assert.AreEqual("", CreateString("1234567890", 0));
            Assert.AreEqual("1", CreateString("1234567890", 0.1));
            Assert.AreEqual("12", CreateString("1234567890", 0.2));
            Assert.AreEqual("123", CreateString("1234567890", 0.3));
            Assert.AreEqual("1234", CreateString("1234567890", 0.4));
            Assert.AreEqual("12345", CreateString("1234567890", 0.5));
            Assert.AreEqual("123456", CreateString("1234567890", 0.6));
            Assert.AreEqual("1234567", CreateString("1234567890", 0.7));
            Assert.AreEqual("12345678", CreateString("1234567890", 0.8));
            Assert.AreEqual("123456789", CreateString("1234567890", 0.9));
            Assert.AreEqual("1234567890", CreateString("1234567890", 1.0));
            Assert.AreEqual("12345678901", CreateString("1234567890", 1.1));
        }
    }
}
