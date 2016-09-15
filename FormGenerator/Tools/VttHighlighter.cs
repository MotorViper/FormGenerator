using System.Collections.Generic;
using System.Windows.Media;
using Helpers;
using TextParser;
using TextParser.Tokens;

namespace FormGenerator.Tools
{
    public class VttHighlighter : IHighlighter
    {
        /// <summary>
        /// Highlights the text of the current block.
        /// </summary>
        /// <param name="formattedText">The text from the current block to highlight</param>
        /// <param name="lastBlockState"></param>
        /// <returns>The current block code</returns>
        public void Highlight(FormattedTextBlock formattedText, BlockState lastBlockState)
        {
            bool inComment = lastBlockState == BlockState.InComment;
            string text = formattedText.Text;
            int offset = 0;
            string[] lines = text.Split('\n');
            foreach (string line in lines)
            {
                string trimmed = line.TrimEnd('\r', ' ', '\t');
                if (trimmed.Length > 0)
                {
                    int length = trimmed.Length;
                    trimmed = trimmed.TrimStart();
                    if (line.TrimStart().StartsWith("//"))
                    {
                        formattedText.SetForegroundBrush(Brushes.Gray, offset, line.Length);
                    }
                    else if (line.StartsWith("/*"))
                    {
                        formattedText.SetForegroundBrush(Brushes.Gray, offset, line.Length);
                        inComment = true;
                    }
                    else if (inComment)
                    {
                        formattedText.SetForegroundBrush(Brushes.Gray, offset, line.Length);
                        inComment = !trimmed.EndsWith("*/");
                    }
                    else
                    {
                        List<string> blocks = line.SplitIntoBlocks();
                        int lineOffset = offset + length - trimmed.Length;
                        foreach (string block in blocks)
                        {
                            if (block.StartsWith("'") || block.StartsWith("\""))
                            {
                                formattedText.SetForegroundBrush(Brushes.Green, lineOffset, block.Length + 1);
                            }
                            else
                            {
                                int blockOffset = lineOffset;
                                string[] parts = trimmed.Split(new[] {':'}, 2);
                                formattedText.SetForegroundBrush(Brushes.Brown, blockOffset, parts[0].Length);
                                blockOffset += parts[0].Length;
                                formattedText.SetForegroundBrush(Brushes.Plum, blockOffset, 1);
                                ++blockOffset;
                                if (parts.Length == 2 && parts[1].Length > 0)
                                {
                                    IToken token = TokenGenerator.Parse(parts[1]);
                                    if (token is StringToken)
                                        formattedText.SetForegroundBrush(Brushes.Green, blockOffset, parts[1].Length);
                                    else if (token is IntToken || token is BoolTooken || token is DoubleToken)
                                        formattedText.SetForegroundBrush(Brushes.Blue, blockOffset, parts[1].Length);
                                    else
                                        formattedText.SetForegroundBrush(Brushes.Red, blockOffset, parts[1].Length);
                                }
                            }
                            lineOffset += block.Length - 1;
                        }
                    }
                }
                offset += line.Length + 1;
            }
            formattedText.State = inComment ? BlockState.InComment : BlockState.Normal;
        }
    }
}
