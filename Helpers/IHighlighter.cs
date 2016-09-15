using System.Windows.Media;

namespace Helpers
{
    public interface IHighlighter
    {
        /// <summary>
        /// Highlights the text of the current block.
        /// </summary>
        /// <param name="formattedText">The text from the current block to highlight</param>
        void Highlight(FormattedTextBlock formattedText, BlockState lastBlockState);
    }
}
