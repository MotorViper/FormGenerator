using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Helpers;
using TextParser;
using TextParser.Operators;
using TextParser.Tokens;

namespace VTTUtilities
{
    /// <summary>
    /// Specifies the highlighting for VTT files.
    /// </summary>
    public class VttHighlighter : IHighlighter
    {
        private static Dictionary<VttSection, VttTextOptions> s_options;

        /// <summary>
        /// Highlights the text of the current block.
        /// </summary>
        /// <param name="formattedText">The text from the current block to highlight</param>
        /// <param name="lastBlockState">The state the previous block was in.</param>
        /// <param name="highlights">Positions of characters to highlight.</param>
        public void Format(IFormattedTextBlock formattedText, BlockState lastBlockState, int[] highlights = null)
        {
            Initialise();
            bool inComment = lastBlockState == BlockState.InComment;
            string text = formattedText.Text;
            int offset = 0;
            string[] lines = text.Split('\n');
            foreach (string line in lines)
            {
                inComment = FormatLine(formattedText, line, offset, inComment, highlights);
                offset += line.Length + 1;
            }
            formattedText.State = inComment ? BlockState.InComment : BlockState.Normal;
        }

        /// <summary>
        /// Given the position of a cursor returns the list of characters that should be shown highlighted.
        /// </summary>
        /// <param name="cursorPosition">The cursor position as the index into the text.</param>
        /// <param name="text">The text to check.</param>
        /// <returns>The list of character positions for highlighting.</returns>
        public int[] GetHighlights(int cursorPosition, string text)
        {
            int[] positions = null;
            if (cursorPosition < text.Length)
            {
                char charAt = text[cursorPosition];
                char prevChar = cursorPosition > 0 ? text[cursorPosition - 1] : ' ';

                if (charAt == '(')
                    positions = MatchBrackets(text, cursorPosition, ')');
                else if (charAt == '{')
                    positions = MatchBrackets(text, cursorPosition, '}');
                else if (prevChar == ')')
                    positions = MatchBrackets(text, '(', cursorPosition - 1);
                else if (prevChar == '}')
                    positions = MatchBrackets(text, '{', cursorPosition - 1);
                else if (prevChar == '(')
                    positions = MatchBrackets(text, cursorPosition - 1, ')');
                else if (prevChar == '{')
                    positions = MatchBrackets(text, cursorPosition - 1, '}');
                else if (charAt == ')')
                    positions = MatchBrackets(text, '(', cursorPosition);
                else if (charAt == '}')
                    positions = MatchBrackets(text, '{', cursorPosition);
            }
            return positions;
        }

        /// <summary>
        /// Given a character position finds the matching item going forward.
        /// </summary>
        /// <param name="text">The text to check.</param>
        /// <param name="start">The position of the character to match.</param>
        /// <param name="toMatch">The character to find.</param>
        /// <returns>The position of the matching character.</returns>
        private static int[] MatchBrackets(string text, int start, char toMatch)
        {
            int end = text.Substring(start + 1).FirstNotInBlock(toMatch, new[] {'\'', '\'', '"', '"', '^', '^', '{', '}', '(', ')'}, true);
            return end >= 0 ? new[] {start, end + start + 1} : null;
        }

        /// <summary>
        /// Given a character position finds the matching item going backward.
        /// </summary>
        /// <param name="text">The text to check.</param>
        /// <param name="toMatch">The character to find.</param>
        /// <param name="end">The position of the character to match.</param>
        /// <returns>The position of the matching character.</returns>
        private static int[] MatchBrackets(string text, char toMatch, int end)
        {
            text = text.Reverse().Substring(text.Length - end);
            int start = text.FirstNotInBlock(toMatch, new[] {'\'', '\'', '"', '"', '^', '^', '}', '{', ')', '('}, true);
            return start >= 0 ? new[] {end - start - 1, end} : null;
        }

        /// <summary>
        /// Initialises the highlighter with data from the options file (if it exists) or with default values.
        /// </summary>
        private static void Initialise()
        {
            if (s_options == null)
            {
                s_options = new Dictionary<VttSection, VttTextOptions>
                {
                    {VttSection.Boolean, new VttTextOptions("Blue")},
                    {VttSection.Comment, new VttTextOptions("Gray | Italic")},
                    {VttSection.Default, new VttTextOptions("Orange")},
                    {VttSection.Double, new VttTextOptions("Blue")},
                    {VttSection.Error, new VttTextOptions("Red | Strikethrough")},
                    {VttSection.Expression, new VttTextOptions("Orange")},
                    {VttSection.Function, new VttTextOptions("OrangeRed")},
                    {VttSection.Highlight, new VttTextOptions("Black | Heavy")},
                    {VttSection.Integer, new VttTextOptions("Blue")},
                    {VttSection.Key, new VttTextOptions("Brown | Bold")},
                    {VttSection.Separator, new VttTextOptions("CadetBlue")},
                    {VttSection.String, new VttTextOptions("Green")},
                    {VttSection.Substitution, new VttTextOptions("Turquoise")}
                };

                IInputData inputData = IOCContainer.Instance.Resolve<IInputData>();
                string optionsFile = FileUtils.GetFullFileName(inputData.OptionsFile, inputData.DefaultDirectory);

                if (File.Exists(optionsFile))
                {
                    TokenTree options = Parser.Parse(new StreamReader(optionsFile)).FindFirst("Highlighting");
                    foreach (TokenTree child in options.Children)
                    {
                        VttSection section = child.Key.ToString().ParseEnum<VttSection>();
                        IToken value = child.Value;
                        ListToken list = value as ListToken;
                        if (list != null)
                        {
                            VttTextOptions values = new VttTextOptions();
                            foreach (IToken token in list)
                                values.Add(token.ToString());
                            s_options[section] = values;
                        }
                        else
                        {
                            string values = child.Value.ToString();
                            s_options[section] = new VttTextOptions(values);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Formats a line of text.
        /// </summary>
        /// <param name="formattedText">The text block containing the line of text.</param>
        /// <param name="line">The line of text.</param>
        /// <param name="offset">The offset into the text box of the line.</param>
        /// <param name="inComment">Whether a comment has been started and not finished.</param>
        /// <param name="highlights">The position of any characters to highlight.</param>
        /// <returns>Whether a comment has been started and not finished.</returns>
        private static bool FormatLine(IFormattedTextBlock formattedText, string line, int offset, bool inComment, int[] highlights)
        {
            string trimmed = line.TrimEnd('\r', ' ', '\t');
            if (trimmed.Length > 0)
            {
                int length = trimmed.Length;
                trimmed = trimmed.TrimStart();
                if (line.TrimStart().StartsWith("//"))
                {
                    FormatItem(VttSection.Comment, formattedText, offset, line, highlights);
                }
                else if (line.StartsWith("/*"))
                {
                    FormatItem(VttSection.Comment, formattedText, offset, line, highlights);
                    inComment = true;
                }
                else if (inComment)
                {
                    FormatItem(VttSection.Comment, formattedText, offset, line, highlights);
                    inComment = !trimmed.EndsWith("*/");
                }
                else
                {
                    offset += length - trimmed.Length;
                    string[] parts = trimmed.Split(new[] {':'}, 2);
                    FormatItem(VttSection.Key, formattedText, offset, parts[0], highlights);
                    offset += parts[0].Length;
                    FormatItem(VttSection.Separator, formattedText, offset, ":", highlights);
                    if (parts.Length == 2 && parts[1].Length > 0)
                        FormatValue(formattedText, parts[1], offset + 1, highlights);
                }
            }
            return inComment;
        }

        private static void StoreFormattedToken(IList<FormattedToken> tokens, IToken token, int blockOffset, string block)
        {
            tokens.Insert(0, new FormattedToken(token, block, blockOffset));
        }

        /// <summary>
        /// Formats the value part of a key/value pair.
        /// </summary>
        /// <param name="formattedText">The text block containing the line of text.</param>
        /// <param name="text">The text of the value.</param>
        /// <param name="offset">The offset into the text box of the line.</param>
        /// <param name="highlights">The position of any characters to highlight.</param>
        private static void FormatValue(IFormattedTextBlock formattedText, string text, int offset, int[] highlights)
        {
            try
            {
                List<FormattedToken> formattedTokens = new List<FormattedToken>();
                IToken token = TokenGenerator.Parse(text, offset, (x, y, z) => StoreFormattedToken(formattedTokens, x, y, z));

                // Formats the line as a whole.
                FormatToken(formattedText, token, offset, text, highlights);

                // Format each individual token in the line.
                foreach (FormattedToken formattedToken in formattedTokens)
                    FormatToken(formattedText, formattedToken.Token, formattedToken.Offset, formattedToken.Text, highlights);
            }
            catch (Exception)
            {
                FormatItem(VttSection.Error, formattedText, offset, text, highlights);
            }
        }

        /// <summary>
        /// Formats a token.
        /// </summary>
        /// <param name="formattedText">The text block containing the line of text.</param>
        /// <param name="token">The token that is being parsed.</param>
        /// <param name="tokenOffset">The offset into the text box of the token.</param>
        /// <param name="tokenText">The text that was parsed to form the token.</param>
        /// <param name="highlights">The position of any characters to highlight.</param>
        private static void FormatToken(IFormattedTextBlock formattedText, IToken token, int tokenOffset, string tokenText, int[] highlights)
        {
            if (token is StringToken)
                FormatItem(VttSection.String, formattedText, tokenOffset, tokenText, highlights);
            else if (token is IntToken)
                FormatItem(VttSection.Integer, formattedText, tokenOffset, tokenText, highlights);
            else if (token is BoolTooken)
                FormatItem(VttSection.Boolean, formattedText, tokenOffset, tokenText, highlights);
            else if (token is DoubleToken)
                FormatItem(VttSection.Double, formattedText, tokenOffset, tokenText, highlights);
            else if (token is ExpressionToken)
                FormatExpression(formattedText, token, tokenOffset, tokenText, highlights);
            else
                FormatItem(VttSection.Default, formattedText, tokenOffset, tokenText, highlights);
        }

        /// <summary>
        /// Formats an expression token.
        /// </summary>
        /// <param name="formattedText">The text block containing the line of text.</param>
        /// <param name="token">The token that is being parsed.</param>
        /// <param name="tokenOffset">The offset into the text box of the token.</param>
        /// <param name="tokenText">The text that was parsed to form the token.</param>
        /// <param name="highlights">The position of any characters to highlight.</param>
        private static void FormatExpression(IFormattedTextBlock formattedText, IToken token, int tokenOffset, string tokenText, int[] highlights)
        {
            ExpressionToken expression = (ExpressionToken)token;
            IOperator op = expression.Operator;
            if (op is SubstitutionOperator)
                FormatItem(VttSection.Substitution, formattedText, tokenOffset, tokenText, highlights);
            else if (op is FunctionOperator)
                FormatFunction(formattedText, expression, tokenOffset, tokenText, highlights);
            else
                FormatItem(VttSection.Expression, formattedText, tokenOffset, tokenText, highlights);
        }

        /// <summary>
        /// Formats a function token.
        /// </summary>
        /// <param name="formattedText">The text of the function.</param>
        /// <param name="token">The token representing the function.</param>
        /// <param name="blockOffset">The offset from the start of the format block.</param>
        /// <param name="block">The format block containing the function text.</param>
        /// <param name="highlights">The characters to highlight.</param>
        private static void FormatFunction(IFormattedTextBlock formattedText, ExpressionToken token, int blockOffset, string block, int[] highlights)
        {
            if (token.First is StringToken)
            {
                // The first item of the expression is a string so split it up and format the parts.
                string name = token.First.ToString();
                int index = block.IndexOf(name);
                index = block.IndexOf(':', index + name.Length);
                FormatItem(VttSection.Function, formattedText, blockOffset, block.Substring(0, index + 1), highlights);
                FormatToken(formattedText, token.Second, blockOffset + index + 1, block.Substring(index + 1), highlights);
            }
            else
            {
                // This is a more complex function so format it as a whole (for now).
                FormatItem(VttSection.Function, formattedText, blockOffset, block, highlights);
            }
        }

        public VttTextOptions Options(VttSection section)
        {
            VttTextOptions options;
            return s_options.TryGetValue(section, out options) ? options : null;
        }

        /// <summary>
        /// Sets the formatting options for a block of text that may contain highlights.
        /// </summary>
        /// <param name="section">The type of the data that is being formatted.</param>
        /// <param name="formattedText">The text block containing the line of text.</param>
        /// <param name="blockOffset">The offset into the text box of the text.</param>
        /// <param name="block">The text in the block.</param>
        /// <param name="highlights">The position of any characters to highlight.</param>
        private static void FormatItem(VttSection section, IFormattedTextBlock formattedText, int blockOffset, string block, int[] highlights)
        {
            Dictionary<int, Tuple<string, VttSection>> blocks = new Dictionary<int, Tuple<string, VttSection>>
            {
                [blockOffset] = Tuple.Create(block, section)
            };

            // If there are highlighted characters then this block needs to be split up into multiple blocks so that they can be highlighted.
            if (highlights != null)
            {
                foreach (int position in highlights)
                {
                    foreach (KeyValuePair<int, Tuple<string, VttSection>> pair in blocks)
                    {
                        int offset = pair.Key;
                        string text = pair.Value.Item1;
                        if (position == offset)
                        {
                            blocks[offset] = Tuple.Create(text.Substring(0, 1), VttSection.Highlight);
                            blocks[offset + 1] = Tuple.Create(text.Substring(1), section);
                            break;
                        }
                        if (position == offset + text.Length - 1)
                        {
                            blocks[offset] = Tuple.Create(text.Substring(0, text.Length - 1), section);
                            blocks[position] = Tuple.Create(text.Substring(text.Length - 1), VttSection.Highlight);
                            break;
                        }
                        if (position > offset && position < offset + text.Length - 1)
                        {
                            blocks[offset] = Tuple.Create(text.Substring(0, position - offset), section);
                            blocks[position] = Tuple.Create(text.Substring(position - offset, 1), VttSection.Highlight);
                            blocks[position + 1] = Tuple.Create(text.Substring(position - offset + 1), section);
                            break;
                        }
                    }
                }
            }
            foreach (KeyValuePair<int, Tuple<string, VttSection>> pair in blocks)
                FormatItem(pair.Value.Item2, formattedText, pair.Key, pair.Value.Item1);
        }

        /// <summary>
        /// Sets the formatting options for a block of text.
        /// </summary>
        /// <param name="section">The type of the data that is being formatted.</param>
        /// <param name="formattedText">The text block containing the line of text.</param>
        /// <param name="blockOffset">The offset into the text box of the text.</param>
        /// <param name="block">The text in the block.</param>
        private static void FormatItem(VttSection section, IFormattedTextBlock formattedText, int blockOffset, string block)
        {
            VttTextOptions options;
            if (blockOffset + block.Length <= formattedText.Text.Length && s_options.TryGetValue(section, out options))
            {
                int length = block.Length;
                if (options.Colour != null)
                    formattedText.SetForegroundBrush(options.Colour, blockOffset, length);
                if (options.Decorations != null)
                    formattedText.SetTextDecorations(options.Decorations, blockOffset, length);
                if (options.Family != null)
                    formattedText.SetFontFamily(options.Family, blockOffset, length);
                if (options.Size > 0)
                    formattedText.SetFontSize(options.Size, blockOffset, length);
                if (options.Stretch != FontStretches.Medium)
                    formattedText.SetFontStretch(options.Stretch, blockOffset, length);
                if (options.Style != FontStyles.Normal)
                    formattedText.SetFontStyle(options.Style, blockOffset, length);
                if (options.Weight != FontWeights.Normal)
                    formattedText.SetFontWeight(options.Weight, blockOffset, length);
            }
        }

        private struct FormattedToken
        {
            public FormattedToken(IToken token, string text, int offset)
            {
                Token = token;
                Text = text;
                Offset = offset;
            }

            public IToken Token { get; }
            public string Text { get; }
            public int Offset { get; }
        }
    }
}
