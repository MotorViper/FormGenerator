namespace Helpers
{
    public interface IHighlighter
    {
        /// <summary>
        /// Highlights the text of the current block.
        /// </summary>
        /// <param name="formattedText">The text from the current block to highlight</param>
        /// <param name="lastBlockState">The state the previous block was in.</param>
        void Highlight(IFormattedTextBlock formattedText, BlockState lastBlockState);
    }
}
