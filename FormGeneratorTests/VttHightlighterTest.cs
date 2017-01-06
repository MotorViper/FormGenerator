using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TextParser;
using VTTUtilities;

namespace FormGeneratorTests
{
    [TestClass]
    public class VttHighlighterTests
    {
        private VttHighlighter _highlighter;

        [TestInitialize]
        public void InitialiseTests()
        {
            IOCContainer.Instance.Register<IInputData, TestInputData>();
            _highlighter = new VttHighlighter();
        }

        [TestMethod]
        public void TestSimpleLine()
        {
            TestFormattedTextBlock block = new TestFormattedTextBlock("Field: Grid");
            _highlighter.Format(block, BlockState.Normal);
            CheckBlock(block.Items, 0, 5, VttSection.Key);
            CheckBlock(block.Items, 5, 1, VttSection.Separator);
            CheckBlock(block.Items, 6, 5, VttSection.String);

            block = new TestFormattedTextBlock(" Columns : Auto|Auto|Auto");
            _highlighter.Format(block, BlockState.Normal);
            CheckBlock(block.Items, 1, 8, VttSection.Key);
            CheckBlock(block.Items, 9, 1, VttSection.Separator);
            CheckBlock(block.Items, 10, 15, VttSection.Expression);

            block = new TestFormattedTextBlock(" Columns : 23");
            _highlighter.Format(block, BlockState.Normal);
            CheckBlock(block.Items, 1, 8, VttSection.Key);
            CheckBlock(block.Items, 9, 1, VttSection.Separator);
            CheckBlock(block.Items, 10, 3, VttSection.Integer);

            block = new TestFormattedTextBlock(" Columns : 23.5");
            _highlighter.Format(block, BlockState.Normal);
            CheckBlock(block.Items, 1, 8, VttSection.Key);
            CheckBlock(block.Items, 9, 1, VttSection.Separator);
            CheckBlock(block.Items, 10, 5, VttSection.Double);

            block = new TestFormattedTextBlock(" Columns : True");
            _highlighter.Format(block, BlockState.Normal);
            CheckBlock(block.Items, 1, 8, VttSection.Key);
            CheckBlock(block.Items, 9, 1, VttSection.Separator);
            CheckBlock(block.Items, 10, 5, VttSection.Boolean);

            block = new TestFormattedTextBlock("// Columns : 23.5");
            _highlighter.Format(block, BlockState.Normal);
            CheckBlock(block.Items, 0, 17, VttSection.Comment);

            block = new TestFormattedTextBlock(" Columns : 23.5(");
            _highlighter.Format(block, BlockState.Normal);
            CheckBlock(block.Items, 1, 8, VttSection.Key);
            CheckBlock(block.Items, 9, 1, VttSection.Separator);
            CheckBlock(block.Items, 10, 6, VttSection.Error);

            block = new TestFormattedTextBlock(" Columns : A: 1");
            _highlighter.Format(block, BlockState.Normal);
            CheckBlock(block.Items, 1, 8, VttSection.Key);
            CheckBlock(block.Items, 9, 1, VttSection.Separator);
            CheckBlock(block.Items, 10, 3, VttSection.Function);
            CheckBlock(block.Items, 13, 2, VttSection.Integer);

            block = new TestFormattedTextBlock(" Columns : {AB}");
            _highlighter.Format(block, BlockState.Normal);
            CheckBlock(block.Items, 1, 8, VttSection.Key);
            CheckBlock(block.Items, 9, 1, VttSection.Separator);
            CheckBlock(block.Items, 10, 5, VttSection.Substitution);

            block = new TestFormattedTextBlock(" Columns : {AB}");
            _highlighter.Format(block, BlockState.Normal, new[] {11, 14});
            CheckBlock(block.Items, 1, 8, VttSection.Key);
            CheckBlock(block.Items, 9, 1, VttSection.Separator);
            CheckBlock(block.Items, 10, 1, VttSection.Substitution);
            CheckBlock(block.Items, 11, 1, VttSection.Highlight);
            CheckBlock(block.Items, 12, 2, VttSection.Substitution);
            CheckBlock(block.Items, 14, 1, VttSection.Highlight);

            block = new TestFormattedTextBlock(" Columns : $A");
            _highlighter.Format(block, BlockState.Normal);
            CheckBlock(block.Items, 1, 8, VttSection.Key);
            CheckBlock(block.Items, 9, 1, VttSection.Separator);
            CheckBlock(block.Items, 10, 3, VttSection.Substitution);

            block = new TestFormattedTextBlock(" Columns : $A");
            _highlighter.Format(block, BlockState.InComment);
            CheckBlock(block.Items, 0, 13, VttSection.Comment);
        }

        private void CheckBlock(IReadOnlyDictionary<int, Brush> item, int position, int length, VttSection section)
        {
            Brush colour = _highlighter.Options(section).Colour;
            for (int i = 0; i < length; ++i)
                Assert.AreEqual(colour, item[i + position]);
        }

        private class TestFormattedTextBlock : IFormattedTextBlock
        {
            public TestFormattedTextBlock(string text)
            {
                Text = text;
                Items = new Dictionary<int, Brush>();
            }

            public Dictionary<int, Brush> Items { get; }

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
                for (int i = 0; i < length; ++i)
                    Items[i + blockOffset] = colour;
            }
        }
    }
}
