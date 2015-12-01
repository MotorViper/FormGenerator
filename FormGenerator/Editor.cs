using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Microsoft.Win32;

namespace FormGenerator
{
    public class Editor
    {
        private static Editor s_instance;

        private readonly List<Range> _ranges = new List<Range>();
        private bool _isFormatted;
        public string CurrentFileName { get; private set; } = string.Empty;

        public static Editor Instance => s_instance ?? (s_instance = new Editor());

        public bool IsSaved { get; private set; } = true;

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

        private void SaveTheFile(RichTextBox richTextBox)
        {
            using (FileStream stream = File.OpenWrite(CurrentFileName))
            {
                TextRange range = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
                range.Save(stream, DataFormats.Text);
            }
            IsSaved = true;
        }

        public void SaveFile(RichTextBox richTextBox)
        {
            if (CurrentFileName == string.Empty)
            {
                SaveFileDialog s = new SaveFileDialog
                {
                    Filter = "VTT|*.vtt",
                    RestoreDirectory = true
                };
                if (s.ShowDialog() == true)
                {
                    SaveTheFile(richTextBox);
                }
            }
            else
            {
                SaveTheFile(richTextBox);
            }
        }

        public void NewFile(RichTextBox box)
        {
            bool create = true;
            if (!IsSaved)
            {
                MessageBoxResult res = MessageBox.Show("Do you want to save changes?", "Unsaved content", MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Exclamation);
                switch (res)
                {
                    case MessageBoxResult.Yes:
                        SaveFile(box);
                        break;
                    case MessageBoxResult.Cancel:
                        create = false;
                        break;
                }
            }
            if (create)
            {
                _isFormatted = false;
                CurrentFileName = string.Empty;
                box.Document.Blocks.Clear();
            }
        }

        private void LoadFile(RichTextBox richTextBox)
        {
            using (FileStream stream = File.OpenRead(CurrentFileName))
            {
                TextRange range = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
                range.Load(stream, DataFormats.Text);
                stream.Close();
            }
        }

        public void OpenFile(RichTextBox box)
        {
            OpenFileDialog o = new OpenFileDialog
            {
                Filter = "VTT|*.vtt|All files|*.*",
                Multiselect = false
            };
            if (o.ShowDialog() == true)
            {
                _isFormatted = false;
                CurrentFileName = o.FileName;
                LoadFile(box);
            }
        }

        public void UpdateFormats(RichTextBox richTextBox)
        {
            if (!_isFormatted)
            {
                TextPointer navigator = richTextBox.Document.ContentStart;
                // ReSharper disable once PossibleNullReferenceException - resharper error
                while (navigator.CompareTo(richTextBox.Document.ContentEnd) < 0)
                {
                    TextPointerContext context = navigator.GetPointerContext(LogicalDirection.Backward);
                    Run run = navigator.Parent as Run;
                    if (context == TextPointerContext.ElementStart && run != null)
                        CheckWords(run.Text, run.ContentStart);
                    navigator = navigator.GetNextContextPosition(LogicalDirection.Forward);
                }
                _isFormatted = true;
            }
            else
            {
                TextPointer pos = richTextBox.CaretPosition;
                TextPointer start = pos.GetLineStartPosition(0);
                TextPointer end = pos.GetLineStartPosition(1);
                end = (end ?? pos.DocumentEnd).GetInsertionPosition(LogicalDirection.Backward);
                TextRange range = new TextRange(start, end);
                range.ClearAllProperties();
                CheckWords(range.Text, start);
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
