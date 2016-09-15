using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Helpers
{
    /// <summary>
    /// Text box that highlights code.
    /// With thanks to Aurelien Ribon's SyntaxHighlightBox.
    /// </summary>
    public partial class HighlightingTextBox
    {
        private const double TOLERANCE = 0.000001;

        private readonly List<FormattedTextBlock> _blocks;

        private double _blockHeight;
        private double _lineHeight;
        private int _maxLineCountInBlock;
        private DrawingControl _renderCanvas;
        private int _totalLineCount;

        public HighlightingTextBox()
        {
            InitializeComponent();

            MaxLineCountInBlock = 100;
            LineHeight = FontSize * 1.3;
            _totalLineCount = 1;
            _blocks = new List<FormattedTextBlock>();

            Loaded += (s, e) =>
            {
                _renderCanvas = (DrawingControl)Template.FindName("PART_RenderCanvas", this);
                ScrollViewer scrollViewer = (ScrollViewer)Template.FindName("PART_ContentHost", this);

                scrollViewer.ScrollChanged += (s1, e1) =>
                {
                    if (Math.Abs(e1.VerticalChange) > TOLERANCE)
                        UpdateBlocks();
                    InvalidateVisual();
                };

                InvalidateBlocks(0);
            };

            SizeChanged += (s, e) =>
            {
                if (!e.HeightChanged)
                    return;
                UpdateBlocks();
            };

            TextChanged += (s, e) =>
            {
                _totalLineCount = Text.LineCount();
                InvalidateBlocks(e.Changes.First().Offset);
            };
        }

        public IHighlighter CurrentHighlighter => IOCContainer.Instance.Resolve<IHighlighter>();

        public double LineHeight
        {
            get { return _lineHeight; }
            set
            {
                _lineHeight = value;
                _blockHeight = MaxLineCountInBlock * _lineHeight;
                TextBlock.SetLineStackingStrategy(this, LineStackingStrategy.BlockLineHeight);
                TextBlock.SetLineHeight(this, _lineHeight);
            }
        }

        public int MaxLineCountInBlock
        {
            get { return _maxLineCountInBlock; }
            set
            {
                _maxLineCountInBlock = value > 0 ? value : 0;
                _blockHeight = _maxLineCountInBlock * LineHeight;
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            DrawBlocks();
            base.OnRender(drawingContext);
        }

        private void UpdateBlocks()
        {
            if (_blocks.Count == 0)
                return;

            // While something is visible after last block...
            while (!_blocks.Last().IsLast && _blocks.Last().Position + _blockHeight - VerticalOffset < ActualHeight)
            {
                int firstLineIndex = _blocks.Last().LastLineNumber + 1;
                int lastLineIndex = firstLineIndex + _maxLineCountInBlock - 1;
                lastLineIndex = lastLineIndex <= _totalLineCount - 1 ? lastLineIndex : _totalLineCount - 1;

                int firstCharIndex = _blocks.Last().LastCharPosition + 1;
                int lastCharIndex = Text.GetPositionOfLineEnd(lastLineIndex); // to be optimized (forward search)

                if (lastCharIndex <= firstCharIndex)
                {
                    _blocks.Last().IsLast = true;
                    return;
                }

                FormattedTextBlock block = CreateTextBlock(firstCharIndex, lastCharIndex, _blocks.Last().LastLineNumber + 1, lastLineIndex);
                AddBlock(block);
            }
        }

        private FormattedTextBlock CreateTextBlock(int charStart, int charEnd, int lineStart, int lineEnd, bool isLast = false)
        {
            return new FormattedTextBlock(Text, charStart, charEnd, lineStart, lineEnd,
                LineHeight, new Typeface(FontFamily, FontStyle, FontWeight, FontStretch), FontSize, isLast);
        }

        private void AddBlock(FormattedTextBlock block)
        {
            _blocks.Add(block);
            FormatBlock(block, _blocks.Count == 1 ? BlockState.Normal : _blocks[_blocks.Count - 2].State);
        }

        private void InvalidateBlocks(int changeOffset)
        {
            FormattedTextBlock blockChanged =
                _blocks.FirstOrDefault(block => block.FirstCharPosition <= changeOffset && changeOffset <= block.LastCharPosition + 1);

            if (blockChanged == null && changeOffset > 0)
                blockChanged = _blocks.Last();

            int firstLine = blockChanged?.FirstLineNumber ?? 0;
            int lastLine = GetIndexOfLastVisibleLine();
            int firstChar = blockChanged?.FirstCharPosition ?? 0;
            int lastChar = Text.GetPositionOfLineEnd(lastLine);

            if (blockChanged != null)
                _blocks.RemoveRange(_blocks.IndexOf(blockChanged), _blocks.Count - _blocks.IndexOf(blockChanged));

            int localLineCount = 1;
            int charStart = firstChar;
            int lineStart = firstLine;
            for (int i = firstChar; i < Text.Length; i++)
            {
                if (Text[i] == '\n')
                    localLineCount += 1;
                if (i == Text.Length - 1)
                {
                    string blockText = Text.Substring(charStart);
                    FormattedTextBlock block = CreateTextBlock(charStart, i, lineStart, lineStart + blockText.LineCount() - 1, true);

                    if (_blocks.Any(b => b.FirstLineNumber == block.FirstLineNumber))
                        throw new Exception();

                    AddBlock(block);
                    break;
                }
                if (localLineCount > _maxLineCountInBlock)
                {
                    FormattedTextBlock block = CreateTextBlock(charStart, i, lineStart, lineStart + _maxLineCountInBlock - 1);

                    if (_blocks.Any(b => b.FirstLineNumber == block.FirstLineNumber))
                        throw new Exception();

                    AddBlock(block);

                    charStart = i + 1;
                    lineStart += _maxLineCountInBlock;
                    localLineCount = 1;

                    if (i > lastChar)
                        break;
                }
            }
        }

        private void DrawBlocks()
        {
            if (!IsLoaded || _renderCanvas == null)
                return;

            using (DrawingContext dc = _renderCanvas.GetContext())
                foreach (FormattedTextBlock block in _blocks.Where(x => !x.Formatting && x.IsVisible(VerticalOffset, ActualHeight, _blockHeight)))
                    block.Draw(dc, HorizontalOffset, VerticalOffset);
        }

        /// <summary>
        /// Returns the index of the last visible text line.
        /// </summary>
        private int GetIndexOfLastVisibleLine()
        {
            return Math.Min((int)(VerticalOffset + ViewportHeight / _lineHeight), _totalLineCount - 1);
        }

        /// <summary>
        /// Formats and Highlights the text of a block.
        /// </summary>
        private void FormatBlock(FormattedTextBlock currentBlock, BlockState lastState)
        {
            ThreadPool.QueueUserWorkItem(p =>
            {
                try
                {
                    currentBlock.Formatting = true;
                    CurrentHighlighter?.Highlight(currentBlock, lastState);
                }
                finally
                {
                    currentBlock.Formatting = false;
                    Dispatcher.Invoke(InvalidateVisual);
                }
            });
        }
    }
}
