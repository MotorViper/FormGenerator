namespace Helpers
{
    public interface IHighlighter
    {
        /// <summary>
        /// Highlights the text of the current block.
        /// </summary>
        /// <param name="formattedText">The text from the current block to highlight</param>
        /// <param name="lastBlockState">The state the previous block was in.</param>
        /// <param name="highlights">Positions of characters to highlight.</param>
        void Format(IFormattedTextBlock formattedText, BlockState lastBlockState, int[] highlights = null);

        /// <summary>
        /// Given the position of a cursor returns the list of characters that should be shown highlighted.
        /// </summary>
        /// <param name="cursorPosition">The cursor position as the index into the text.</param>
        /// <param name="text">The text to check.</param>
        /// <returns>The list of character positions for highlighting.</returns>
        int[] GetHighlights(int cursorPosition, string text);
    }
}
