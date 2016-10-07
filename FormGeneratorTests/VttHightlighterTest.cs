using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using FormGenerator.Tools;
using Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FormGeneratorTests
{
    [TestClass]
    public class VttHighlighterTests
    {
        private VttHighlighter _highlighter;

        [TestInitialize]
        public void InitialiseTests()
        {
            _highlighter = new VttHighlighter();
        }

        [TestMethod]
        public void TestSimpleLine()
        {
            TestFormattedTextBlock block = new TestFormattedTextBlock("Field: Grid");
            _highlighter.Highlight(block, BlockState.Normal);
            CheckBlock(block.Items[0], 0, 5, VttSection.Key);
            CheckBlock(block.Items[1], 5, 1, VttSection.Separator);
            CheckBlock(block.Items[2], 6, 5, VttSection.String);

            block = new TestFormattedTextBlock(" Columns : Auto|Auto|Auto");
            _highlighter.Highlight(block, BlockState.Normal);
            CheckBlock(block.Items[0], 1, 8, VttSection.Key);
            CheckBlock(block.Items[1], 9, 1, VttSection.Separator);
            CheckBlock(block.Items[2], 10, 15, VttSection.Expression);

            block = new TestFormattedTextBlock(" Columns : 23");
            _highlighter.Highlight(block, BlockState.Normal);
            CheckBlock(block.Items[0], 1, 8, VttSection.Key);
            CheckBlock(block.Items[1], 9, 1, VttSection.Separator);
            CheckBlock(block.Items[2], 10, 3, VttSection.Integer);

            block = new TestFormattedTextBlock(" Columns : 23.5");
            _highlighter.Highlight(block, BlockState.Normal);
            CheckBlock(block.Items[0], 1, 8, VttSection.Key);
            CheckBlock(block.Items[1], 9, 1, VttSection.Separator);
            CheckBlock(block.Items[2], 10, 5, VttSection.Double);

            block = new TestFormattedTextBlock(" Columns : True");
            _highlighter.Highlight(block, BlockState.Normal);
            CheckBlock(block.Items[0], 1, 8, VttSection.Key);
            CheckBlock(block.Items[1], 9, 1, VttSection.Separator);
            CheckBlock(block.Items[2], 10, 5, VttSection.Boolean);

            block = new TestFormattedTextBlock("// Columns : 23.5");
            _highlighter.Highlight(block, BlockState.Normal);
            CheckBlock(block.Items[0], 0, 17, VttSection.Comment);

            block = new TestFormattedTextBlock(" Columns : 23.5(");
            _highlighter.Highlight(block, BlockState.Normal);
            CheckBlock(block.Items[0], 1, 8, VttSection.Key);
            CheckBlock(block.Items[1], 9, 1, VttSection.Separator);
            CheckBlock(block.Items[2], 10, 6, VttSection.Error);

            block = new TestFormattedTextBlock(" Columns : A: 1");
            _highlighter.Highlight(block, BlockState.Normal);
            CheckBlock(block.Items[0], 1, 8, VttSection.Key);
            CheckBlock(block.Items[1], 9, 1, VttSection.Separator);
            CheckBlock(block.Items[2], 10, 3, VttSection.Function);
            CheckBlock(block.Items[3], 13, 2, VttSection.Integer);

            block = new TestFormattedTextBlock(" Columns : {A}");
            _highlighter.Highlight(block, BlockState.Normal);
            CheckBlock(block.Items[0], 1, 8, VttSection.Key);
            CheckBlock(block.Items[1], 9, 1, VttSection.Separator);
            CheckBlock(block.Items[2], 10, 4, VttSection.Substitution);

            block = new TestFormattedTextBlock(" Columns : $A");
            _highlighter.Highlight(block, BlockState.Normal);
            CheckBlock(block.Items[0], 1, 8, VttSection.Key);
            CheckBlock(block.Items[1], 9, 1, VttSection.Separator);
            CheckBlock(block.Items[2], 10, 3, VttSection.Substitution);

            block = new TestFormattedTextBlock(" Columns : $A");
            _highlighter.Highlight(block, BlockState.InComment);
            CheckBlock(block.Items[0], 0, 13, VttSection.Comment);
        }

        private void CheckBlock(TextBlockItem item, int position, int length, VttSection section)
        {
            Assert.AreEqual(position, item.Position);
            Assert.AreEqual(length, item.Length);
            Assert.AreEqual(_highlighter.Options(section).Colour, item.Brush);
        }

        private class TextBlockItem
        {
            public Brush Brush { get; set; }
            public int Length { get; set; }
            public int Position { get; set; }
        }

        private class TestFormattedTextBlock : IFormattedTextBlock
        {
            public TestFormattedTextBlock(string text)
            {
                Text = text;
                Items = new List<TextBlockItem>();
            }

            public List<TextBlockItem> Items { get; }

            public string Text { get; }

            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public BlockState State { get; set; }

            public void SetFontSize(double size, int blockOffset, int length)
            {
            }

            public void SetFontWeight(FontWeight weight, int blockOffset, int length)
            {
            }

            public void SetFontStyle(FontStyle style, int blockOffset, int length)
            {
            }

            public void SetFontStretch(FontStretch stretch, int blockOffset, int length)
            {
            }

            public void SetFontFamily(FontFamily family, int blockOffset, int length)
            {
            }

            public void SetTextDecorations(TextDecorationCollection decorations, int blockOffset, int length)
            {
            }

            public void SetForegroundBrush(Brush colour, int blockOffset, int length)
            {
                Items.Add(new TextBlockItem {Brush = colour, Length = length, Position = blockOffset});
            }
        }
    }
}
