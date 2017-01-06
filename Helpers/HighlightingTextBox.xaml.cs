using System;
using System.Collections.Generic;
using System.Linq;
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
        private uint _maxLineCountInBlock;
        private DrawingControl _renderCanvas;
        private int _totalLineCount;

        public HighlightingTextBox()
        {
            InitializeComponent();

            MaxLineCountInBlock = 50;
            LineHeight = FontSize * 1.3;
            _totalLineCount = 1;
            _blocks = new List<FormattedTextBlock>();

            Loaded += (s, e) => ItemHasLoaded();
            SizeChanged += (s, e) => SizeHasChanged(e);
            TextChanged += (s, e) => TextHasChanged(e);
            SelectionChanged += (s, e) => SelectionHasChanged(e);
        }

        private static IHighlighter CurrentHighlighter => IOCContainer.Instance.Resolve<IHighlighter>();

        /// <summary>
        /// The height of each line of text.
        /// </summary>
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

        /// <summary>
        /// Maximum number of lines in a text block.
        /// </summary>
        public uint MaxLineCountInBlock
        {
            get { return _maxLineCountInBlock; }
            set
            {
                _maxLineCountInBlock = value;
                _blockHeight = _maxLineCountInBlock * LineHeight;
            }
        }

        /// <summary>
        /// When the text box is loaded formats its text.
        /// </summary>
        private void ItemHasLoaded()
        {
            _renderCanvas = (DrawingControl)Template.FindName("PART_RenderCanvas", this);
            ScrollViewer scrollViewer = (ScrollViewer)Template.FindName("PART_ContentHost", this);

            scrollViewer.ScrollChanged += (s, e) => ScrollHasChanged(e);

            InvalidateBlocks(0);
            InvalidateVisual();
        }

        /// <summary>
        /// When the text box is scrolled make sure any data that becomes visible is formatted.
        /// </summary>
        private void ScrollHasChanged(ScrollChangedEventArgs e)
        {
            if (Math.Abs(e.VerticalChange) > TOLERANCE)
                UpdateBlocks();
            InvalidateVisual();
        }

        /// <summary>
        /// When the cursor moves redo any highlighting.
        /// </summary>
        private void SelectionHasChanged(RoutedEventArgs e)
        {
            int[] positions = CurrentHighlighter?.GetHighlights(CaretIndex, Text);
            if (positions != null)
            {
                MatchBrackets(positions);
                InvalidateVisual();
            }
            else
            {
                bool needsInvalidate = false;
                for (int index = 0; index < _blocks.Count; index++)
                {
                    FormattedTextBlock block = _blocks[index];
                    if (block.HasHighlight)
                    {
                        ReformatBlock(index);
                        needsInvalidate = true;
                    }
                }
                if (needsInvalidate)
                    InvalidateVisual();
            }
        }

        /// <summary>
        /// Highlights the text at the positions given.
        /// </summary>
        /// <param name="positions">The list of positions that need highlighting.</param>
        private void MatchBrackets(IEnumerable<int> positions)
        {
            Dictionary<int, List<int>> blocksToUpdate = new Dictionary<int, List<int>>();
            foreach (int position in positions)
            {
                for (int index = 0; index < _blocks.Count; index++)
                {
                    FormattedTextBlock block = _blocks[index];
                    if (block.FirstCharPosition <= position && position <= block.LastCharPosition + 1)
                    {
                        List<int> ints;
                        if (!blocksToUpdate.TryGetValue(index, out ints))
                        {
                            ints = new List<int>();
                            blocksToUpdate[index] = ints;
                        }
                        ints.Add(position);
                        break;
                    }
                }
            }
            for (int i = 0; i < _blocks.Count; ++i)
            {
                if (!blocksToUpdate.ContainsKey(i))
                    ReformatBlock(i);
            }
            foreach (KeyValuePair<int, List<int>> pair in blocksToUpdate)
            {
                int[] highlights = pair.Value.Select(x => x - _blocks[pair.Key].FirstCharPosition).ToArray();
                ReformatBlock(pair.Key, highlights);
            }
        }

        /// <summary>
        /// Reformats a single block of text.
        /// </summary>
        /// <param name="index">The position of the block in the list of blocks.</param>
        /// <param name="highlightPositions">The positions of any characters that need highlighting.</param>
        private void ReformatBlock(int index, int[] highlightPositions = null)
        {
            if (index >= 0 && index < _blocks.Count)
            {
                CurrentHighlighter?.Format(_blocks[index], index == 0 ? BlockState.Normal : _blocks[index - 1].State, highlightPositions);
                _blocks[index].HasHighlight = highlightPositions != null;
            }
        }

        /// <summary>
        /// When the text changes reformat any text blocks that are effected.
        /// </summary>
        private void TextHasChanged(TextChangedEventArgs e)
        {
            _totalLineCount = Text.LineCount();
            InvalidateBlocks(e.Changes.First().Offset);
            InvalidateVisual();
        }

        /// <summary>
        /// When the text box size changes make sure all visible blocks are formatted.
        /// </summary>
        private void SizeHasChanged(SizeChangedEventArgs e)
        {
            if (!e.HeightChanged)
                return;
            UpdateBlocks();
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            DrawBlocks();
            base.OnRender(drawingContext);
        }

        /// <summary>
        /// Reformats all visible blocks.
        /// </summary>
        private void UpdateBlocks()
        {
            if (_blocks.Count == 0)
                return;

            // While something is visible after last block...
            while (!_blocks.Last().IsLast && _blocks.Last().Position + _blockHeight - VerticalOffset < ActualHeight)
            {
                int firstLineIndex = _blocks.Last().LastLineNumber + 1;
                int lastLineIndex = firstLineIndex + (int)MaxLineCountInBlock - 1;
                lastLineIndex = lastLineIndex <= _totalLineCount - 1 ? lastLineIndex : _totalLineCount - 1;

                int firstCharIndex = _blocks.Last().LastCharPosition + 1;
                int lastCharIndex = Text.GetPositionOfLineEnd(lastLineIndex);

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

        /// <summary>
        /// Adds a block to the list of blocks and formats it.
        /// </summary>
        /// <param name="block"></param>
        private void AddBlock(FormattedTextBlock block)
        {
            _blocks.Add(block);
            CurrentHighlighter?.Format(block, _blocks.Count == 1 ? BlockState.Normal : _blocks[_blocks.Count - 2].State);
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
                    FormattedTextBlock block =
                        CreateTextBlock(charStart, i, lineStart, lineStart + (int)MaxLineCountInBlock - 1);

                    if (_blocks.Any(b => b.FirstLineNumber == block.FirstLineNumber))
                        throw new Exception();

                    AddBlock(block);

                    charStart = i + 1;
                    lineStart += (int)MaxLineCountInBlock;
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
                foreach (FormattedTextBlock block in _blocks.Where(x => x.IsVisible(VerticalOffset, ActualHeight, _blockHeight)))
                    block.Draw(dc, HorizontalOffset, VerticalOffset);
        }

        /// <summary>
        /// Returns the index of the last visible text line.
        /// </summary>
        private int GetIndexOfLastVisibleLine()
        {
            return Math.Min((int)(VerticalOffset + ViewportHeight / _lineHeight), _totalLineCount - 1);
        }
    }
}
