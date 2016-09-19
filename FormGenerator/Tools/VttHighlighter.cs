using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Helpers;
using TextParser;
using TextParser.Operators;
using TextParser.Tokens;

namespace FormGenerator.Tools
{
    public class VttHighlighter : IHighlighter
    {
        private static Dictionary<VttSection, VttTextOptions> s_options;

        /// <summary>
        /// Highlights the text of the current block.
        /// </summary>
        /// <param name="formattedText">The text from the current block to highlight</param>
        /// <param name="lastBlockState"></param>
        /// <returns>The current block code</returns>
        public void Highlight(FormattedTextBlock formattedText, BlockState lastBlockState)
        {
            Initialise();
            bool inComment = lastBlockState == BlockState.InComment;
            string text = formattedText.Text;
            int offset = 0;
            string[] lines = text.Split('\n');
            foreach (string line in lines)
            {
                inComment = FormatLine(formattedText, line, offset, inComment);
                offset += line.Length + 1;
            }
            formattedText.State = inComment ? BlockState.InComment : BlockState.Normal;
        }

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
                    {VttSection.Integer, new VttTextOptions("Blue")},
                    {VttSection.Key, new VttTextOptions("Brown | Bold")},
                    {VttSection.Separator, new VttTextOptions("CadetBlue")},
                    {VttSection.String, new VttTextOptions("Green")},
                    {VttSection.Substitution, new VttTextOptions("DarkBlue")}
                };
            }
        }

        private static bool FormatLine(FormattedText formattedText, string line, int offset, bool inComment)
        {
            string trimmed = line.TrimEnd('\r', ' ', '\t');
            if (trimmed.Length > 0)
            {
                int length = trimmed.Length;
                trimmed = trimmed.TrimStart();
                if (line.TrimStart().StartsWith("//"))
                {
                    FormatItem(VttSection.Comment, formattedText, offset, line);
                }
                else if (line.StartsWith("/*"))
                {
                    FormatItem(VttSection.Comment, formattedText, offset, line);
                    inComment = true;
                }
                else if (inComment)
                {
                    FormatItem(VttSection.Comment, formattedText, offset, line);
                    inComment = !trimmed.EndsWith("*/");
                }
                else
                {
                    offset += length - trimmed.Length;
                    string[] parts = trimmed.Split(new[] {':'}, 2);
                    FormatItem(VttSection.Key, formattedText, offset, parts[0]);
                    offset += parts[0].Length;
                    FormatItem(VttSection.Separator, formattedText, offset, ":");
                    if (parts.Length == 2 && parts[1].Length > 0)
                        FormatValue(formattedText, parts[1], offset + 1);
                }
            }
            return inComment;
        }

        private static void FormatValue(FormattedText formattedText, string text, int offset)
        {
            string errorString = text;
            int errorPosition = offset;
            try
            {
                List<string> blocks = text.SplitIntoBlocks();
                foreach (string block in blocks)
                {
                    if (block.StartsWith("'") || block.StartsWith("\""))
                    {
                        FormatItem(VttSection.String, formattedText, offset, block + "'");
                    }
                    else
                    {
                        errorString = block;
                        errorPosition = offset;
                        IToken token = TokenGenerator.Parse(block);
                        FormatToken(formattedText, token, offset, block);
                    }
                    offset += block.Length;
                }
            }
            catch (Exception)
            {
                FormatItem(VttSection.Error, formattedText, errorPosition, errorString);
            }
        }

        private static void FormatToken(FormattedText formattedText, IToken token, int blockOffset, string block)
        {
            if (token is StringToken)
                FormatItem(VttSection.String, formattedText, blockOffset, block);
            else if (token is IntToken)
                FormatItem(VttSection.Integer, formattedText, blockOffset, block);
            else if (token is BoolTooken)
                FormatItem(VttSection.Boolean, formattedText, blockOffset, block);
            else if (token is DoubleToken)
                FormatItem(VttSection.Double, formattedText, blockOffset, block);
            else if (token is ExpressionToken)
                FormatExpression(formattedText, token, blockOffset, block);
            else
                FormatItem(VttSection.Default, formattedText, blockOffset, block);
        }

        private static void FormatExpression(FormattedText formattedText, IToken token, int blockOffset, string block)
        {
            ExpressionToken expression = (ExpressionToken)token;
            IOperator op = expression.Operator;
            if (op is SubstitutionOperator)
                FormatItem(VttSection.Substitution, formattedText, blockOffset, block);
            else if (op is FunctionOperator)
                FormatFunction(formattedText, expression, blockOffset, block);
            else
                FormatItem(VttSection.Expression, formattedText, blockOffset, block);
        }

        private static void FormatFunction(FormattedText formattedText, ExpressionToken token, int blockOffset, string block)
        {
            if (token.First is StringToken)
            {
                string name = token.First.Text;
                int index = block.IndexOf(name);
                index = block.IndexOf(':', index + name.Length);
                FormatItem(VttSection.Function, formattedText, blockOffset, block.Substring(0, index + 1));
                FormatToken(formattedText, token.Second, blockOffset + index + 1, block.Substring(index + 1));
            }
            else
            {
                FormatItem(VttSection.Function, formattedText, blockOffset, block);
            }
        }

        private static void FormatItem(VttSection section, FormattedText formattedText, int blockOffset, string block)
        {
            VttTextOptions options;
            if (s_options.TryGetValue(section, out options))
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
    }
}
