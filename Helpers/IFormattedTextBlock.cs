using System.Windows;
using System.Windows.Media;

namespace Helpers
{
    public interface IFormattedTextBlock
    {
        BlockState State { set; }
        string Text { get; }
        void SetForegroundBrush(Brush colour, int blockOffset, int length);
        void SetTextDecorations(TextDecorationCollection decorations, int blockOffset, int length);
        void SetFontFamily(FontFamily family, int blockOffset, int length);
        void SetFontSize(double size, int blockOffset, int length);
        void SetFontStretch(FontStretch stretch, int blockOffset, int length);
        void SetFontStyle(FontStyle style, int blockOffset, int length);
        void SetFontWeight(FontWeight weight, int blockOffset, int length);
    }
}
