using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace Helpers
{
    public class FormattedTextBlock : FormattedText
    {
        public FormattedTextBlock(string text, int charStart, int charEnd, int lineStart, int lineEnd, double lineHeight, Typeface typeface,
            double fontSize, bool isLast = false) :
                base(text.Substring(charStart, charEnd - charStart + 1), CultureInfo.InvariantCulture, FlowDirection.LeftToRight, typeface, fontSize,
                    Brushes.Black)
        {
            FirstCharPosition = charStart;
            LastCharPosition = charEnd;
            FirstLineNumber = lineStart;
            LastLineNumber = lineEnd;
            IsLast = isLast;
            Trimming = TextTrimming.None;
            LineHeight = lineHeight;
            Position = FirstLineNumber * LineHeight;
            State = BlockState.Normal;
        }

        public int FirstCharPosition { get; }
        public int FirstLineNumber { get; }

        public bool IsLast { get; set; }

        public int LastCharPosition { get; }
        public int LastLineNumber { get; }

        public double Position { get; }

        public BlockState State { get; set; }

        public bool IsVisible(double offset, double height, double blockHeight)
        {
            double yPosition = Position;
            double top = yPosition - offset;
            double bottom = top + blockHeight;
            return top < height && bottom > 0;
        }

        public void Draw(DrawingContext dc, double horizontalOffset, double verticalOffset)
        {
            try
            {
                dc.DrawText(this, new Point(2 - horizontalOffset, Position - verticalOffset));
            }
            catch
            {
                // Don't know why this exception is raised sometimes.
                // Reproduce steps:
                // - Sets a valid syntax highlighter on the box.
                // - Copy a large chunk of code in the clipboard.
                // - Paste it using ctrl+v and keep these buttons pressed.
            }
        }
    }
}
