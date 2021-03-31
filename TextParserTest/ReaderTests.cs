using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TextParser;

namespace TextParserTest
{
    [TestClass]
    public class ReaderTests
    {
        [TestCleanup]
        public void CleanUpAfterTest()
        {
            Line.OffsetSize = 4;
        }

        [TestMethod]
        public void TestReadsLines()
        {
            Reader reader = new Reader(new StringReader("Line 0\n   Line 1\nLine 2   \n\n\n   Line 3   \n	Line 4		"));
            int lineNumber = 0;
            foreach (Line line in reader)
                Assert.AreEqual($"Line {lineNumber++}", line.Content);
        }

        [TestMethod]
        public void TestsHandlesIncludes()
        {
            Reader reader = new Reader(new StreamReader(@"C:\Development\Projects\FormGenerator\Data\Data.vtt"))
            {
                Options = {DefaultDirectory = @"C:\Development\Projects\FormGenerator\Data"}
            };
            Assert.IsTrue(reader.Count() > 4);
        }

        [TestMethod]
        public void TestReadsLinesWithContinuations()
        {
            Reader reader = new Reader(new StringReader("Line 0-\n   0 \nLine 1    -  \n1\nLine 2 -\n   2 \n   Line 3    -\n    3    "));
            int lineNumber = 0;
            foreach (Line line in reader)
                Assert.AreEqual($"Line {lineNumber} {lineNumber++}", line.Content);
        }

        [TestMethod]
        public void TestReadsLinesWithNewLineContinuations()
        {
            string[] expected = {"Line 0\r\n   0", "Line 1 1", "Line 2\r\n   2", "Line 3\r\n    3"};
            Reader reader = new Reader(new StringReader("Line 0--\n   0 \nLine 1    -  \n1\nLine 2 --\n   2 \n   Line 3    --\n    3    "));
            int lineNumber = 0;
            foreach (Line line in reader)
                Assert.AreEqual(expected[lineNumber++], line.Content);
        }

        [TestMethod]
        public void TestReadsLinesWithContinuationsNoSpaces()
        {
            Reader reader = new Reader(new StringReader("Line 0-\n0\nLine 1-  \n1\nLine 2-\n2 \n  Line 3-\n3   "));
            int lineNumber = 0;
            foreach (Line line in reader)
                Assert.AreEqual($"Line {lineNumber}{lineNumber++}", line.Content);
        }

        [TestMethod]
        public void TestReadsLinesWithOffsets()
        {
            Reader reader = new Reader(new StringReader(@"0Line 0
 1Line 1
  1Line 2
   1Line 3
    1Line 4
     2Line 5
      2Line 6
       2Line 7
		2Line 8
		 3Line 9"));
            int lineNumber = 0;
            foreach (Line line in reader)
                Assert.AreEqual($"{line.Offset}Line {lineNumber++}", line.Content);
        }

        [TestMethod]
        public void TestReadsLinesWithSetOffsets()
        {
            Line.OffsetSize = 2;
            Reader reader = new Reader(new StringReader(@"0Line 0
 1Line 1
  1Line 2
   2Line 3
    2Line 4
     3Line 5
      3Line 6"));
            int lineNumber = 0;
            foreach (Line line in reader)
                Assert.AreEqual($"{line.Offset}Line {lineNumber++}", line.Content);
        }

        [TestMethod]
        public void TestIgnoreComments()
        {
            Reader reader = new Reader(new StringReader(@"

A
// B
C
/*
D

E
*/
/*
A
// C
*/
/* B */
F
// G
H
/* I */
I

/*

J
*/
K
/*
L
*/
M /* N
O /* P */
"));
            List<Line> lines = reader.ToList();
            Assert.AreEqual(8, lines.Count);
            Assert.AreEqual("A", lines[0].Content);
            Assert.AreEqual("C", lines[1].Content);
            Assert.AreEqual("F", lines[2].Content);
            Assert.AreEqual("H", lines[3].Content);
            Assert.AreEqual("I", lines[4].Content);
            Assert.AreEqual("K", lines[5].Content);
            Assert.AreEqual("M /* N", lines[6].Content);
            Assert.AreEqual("O", lines[7].Content);
        }


        [TestMethod]
        public void TestCheckIfs()
        {
            const string testData = @"

A
#IF A B
C
#IF A
D
E
#FI
F
#IF B G
H
#IF B 
I
J
#FI
K
#IF A
L
";
            Reader reader = new Reader(new StringReader(testData)) {Options = {Selector = "A"}};
            List<Line> lines = reader.ToList();
            Assert.AreEqual(9, lines.Count, lines.Aggregate("", (x, y) => x + y.Content + ", "));
            Assert.AreEqual("A", lines[0].Content);
            Assert.AreEqual("B", lines[1].Content);
            Assert.AreEqual("C", lines[2].Content);
            Assert.AreEqual("D", lines[3].Content);
            Assert.AreEqual("E", lines[4].Content);
            Assert.AreEqual("F", lines[5].Content);
            Assert.AreEqual("H", lines[6].Content);
            Assert.AreEqual("K", lines[7].Content);
            Assert.AreEqual("L", lines[8].Content);

            reader = new Reader(new StringReader(testData)) {Options = {Selector = "B"}};
            lines = reader.ToList();
            Assert.AreEqual(8, lines.Count, lines.Aggregate("", (x, y) => x + y.Content + ", "));
            Assert.AreEqual("A", lines[0].Content);
            Assert.AreEqual("C", lines[1].Content);
            Assert.AreEqual("F", lines[2].Content);
            Assert.AreEqual("G", lines[3].Content);
            Assert.AreEqual("H", lines[4].Content);
            Assert.AreEqual("I", lines[5].Content);
            Assert.AreEqual("J", lines[6].Content);
            Assert.AreEqual("K", lines[7].Content);
        }
    }
}
