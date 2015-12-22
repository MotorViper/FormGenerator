using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace FormGenerator.Tools
{
    public class RichTextBoxEditor : IEditor
    {
        private readonly RichTextBox _editor;
        private readonly List<Range> _ranges = new List<Range>();

        public RichTextBoxEditor(RichTextBox editor)
        {
            _editor = editor;
        }

        public void Save(FileStream stream)
        {
            TextRange range = new TextRange(_editor.Document.ContentStart, _editor.Document.ContentEnd);
            range.Save(stream, DataFormats.Text);
        }

        public void Clear()
        {
            _editor.Document.Blocks.Clear();
        }

        public void Load(FileStream stream)
        {
            TextRange range = new TextRange(_editor.Document.ContentStart, _editor.Document.ContentEnd);
            range.Load(stream, DataFormats.Text);
        }

        public void Format()
        {
            for (int i = 0; i < _ranges.Count; i++)
            {
                TextRange range = new TextRange(_ranges[i].StartPosition, _ranges[i].EndPosition);
                range.ApplyPropertyValue(TextElement.ForegroundProperty, _ranges[i].Foreground);
                range.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
            }
            _ranges.Clear();
        }

        public bool IsEmpty => _editor.Document == null;

        public void FormatAtStart()
        {
            TextPointer navigator = _editor.Document.ContentStart;
            // ReSharper disable once PossibleNullReferenceException - resharper error
            while (navigator.CompareTo(_editor.Document.ContentEnd) < 0)
            {
                TextPointerContext context = navigator.GetPointerContext(LogicalDirection.Backward);
                Run run = navigator.Parent as Run;
                if (context == TextPointerContext.ElementStart && run != null)
                    CheckWords(run.Text, run.ContentStart);
                navigator = navigator.GetNextContextPosition(LogicalDirection.Forward);
            }
        }

        public void FormatOnFly()
        {
            TextPointer pos = _editor.CaretPosition;
            TextPointer start = pos.GetLineStartPosition(0);
            TextPointer end = pos.GetLineStartPosition(1);
            end = (end ?? pos.DocumentEnd).GetInsertionPosition(LogicalDirection.Backward);
            TextRange range = new TextRange(start, end);
            range.ClearAllProperties();
            CheckWords(range.Text, start);
        }

        private void CheckWords(string text, TextPointer start)
        {
            List<SyntaxProvider.Range> range = SyntaxProvider.CheckWords(text);
            foreach (SyntaxProvider.Range syntaxRange in range)
            {
                Range item = new Range
                {
                    StartPosition = start.GetPositionAtOffset(syntaxRange.Start),
                    EndPosition = start.GetPositionAtOffset(syntaxRange.End),
                    Foreground = new SolidColorBrush(syntaxRange.Foreground)
                };
                _ranges.Add(item);
            }
        }

        public struct Range
        {
            public TextPointer StartPosition;
            public TextPointer EndPosition;
            public Brush Foreground;
        }
    }
}
