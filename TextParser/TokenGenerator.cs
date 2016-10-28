using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Helpers;
using TextParser.Operators;
using TextParser.Tokens;

namespace TextParser
{
    /// <summary>
    /// Parses a string and converts it into tokens.
    /// </summary>
    public static class TokenGenerator
    {
        private static readonly Dictionary<string, Func<string[], int, Action<IToken, int, string>, IToken>> s_tokens =
            new Dictionary<string, Func<string[], int, Action<IToken, int, string>, IToken>>
            {
                ["([Tt][Rr][Uu][Ee]|[Ff][Aa][Ll][Ss][Ee])"] = (tokens, x, y) => new BoolTooken(bool.Parse(tokens[0])),
                ["(.*)(\\|)([^\\|]*)"] = (tokens, x, y) => PerformOperation(tokens, x, y),
                ["(.*)([\\+-])([^\\+-]*)"] = (tokens, x, y) => PerformOperation(tokens, x, y),
                ["(.*)([\\*/×÷])([^\\*/×÷]*)"] = (tokens, x, y) => PerformOperation(tokens, x, y),
                ["(.*)(#)([^#]*)"] = (tokens, x, y) => PerformOperation(tokens, x, y),
                ["([^:]*)(:)(.*)"] = (tokens, x, y) => PerformOperation(tokens, x, y),
                ["(.*)\\$(([A-Za-z█][A-Za-z0-9█]*|[1-9][0-9]*)(\\.[A-Za-z█][A-Za-z0-9█]*)*)?([^$]*)"] = (tokens, x, y) =>
                    PerformSubstitutionOperation(tokens, x, y),
                ["([1-9][0-9]*\\.[0-9]+)"] = (tokens, x, y) => new DoubleToken(double.Parse(tokens[0])),
                ["(0|[1-9][0-9]*)"] = (tokens, x, y) => CreateIntToken(tokens, x, y),
                ["\\r\\n"] = (tokens, x, y) => new NewLineToken(),
                [".*"] = (tokens, x, y) => CreateStringToken(tokens, x, y)
            };

        public static string Evaluate(this string input)
        {
            return Parse(input).Simplify().Text;
        }

        private static IToken CreateStringToken(IReadOnlyList<string> tokens, int startPosition, Action<IToken, int, string> callback)
        {
            string item = tokens[0];
            IToken token = new StringToken(item);
            //if (!item.Contains("█"))
            //    callback?.Invoke(token, startPosition, item);
            return token;
        }

        private static IToken CreateIntToken(IReadOnlyList<string> tokens, int startPosition, Action<IToken, int, string> callback)
        {
            string item = tokens[0];
            IToken token = new IntToken(int.Parse(item));
            //callback?.Invoke(token, startPosition, item);
            return token;
        }

        private static IToken PerformSubstitutionOperation(IReadOnlyList<string> tokens, int startPosition, Action<IToken, int, string> callback)
        {
            IToken token = null;
            string firstToken = tokens[1];
            if (!string.IsNullOrWhiteSpace(firstToken))
                token = new StringToken(firstToken);

            IToken result = new ExpressionToken(null, new SubstitutionOperator(), token);
            //if (token != null)
            //    callback?.Invoke(result, startPosition + tokens[0].Length, firstToken);
            if (!string.IsNullOrWhiteSpace(tokens[0]))
            {
                token = ParseExpressionNoBrackets(tokens[0].Trim(), startPosition, null);
                ExpressionToken expression = token as ExpressionToken;
                if (expression?.NeedsSecond ?? false)
                    result = expression.SetSecond(result);
                else
                    result = new ExpressionToken(token, new StringPlusOperator(), result);
            }
            if (!string.IsNullOrWhiteSpace(tokens[4]))
            {
                token = ParseExpressionNoBrackets(tokens[4].Trim(), startPosition + tokens[0].Length + tokens[3].Length, null);
                result = new ExpressionToken(result, new StringPlusOperator(), token);
            }
            return result;
        }

        private static IToken PerformOperation(IReadOnlyList<string> tokens, int startPosition, Action<IToken, int, string> callback)
        {
            string token = tokens[0].TrimStart();
            startPosition += tokens[0].Length - token.Length;
            IToken first = ParseExpressionNoBrackets(token.TrimEnd(), startPosition, null);
            startPosition += tokens[0].Length;
            BaseOperator op = BaseOperator.CreateOperatorToken(tokens[1]);
            startPosition += tokens[1].Length;
            token = tokens[2].TrimStart();
            startPosition += tokens[2].Length - token.Length;
            IToken second = ParseExpressionNoBrackets(token.TrimEnd(), startPosition, null);

            ExpressionToken firstExpression = first as ExpressionToken;
            if (firstExpression != null && firstExpression.NeedsSecond && op.CanBeUnary)
            {
                firstExpression.SetSecond(new ExpressionToken(null, op, second));
                return first;
            }

            return new ExpressionToken(first, op, second);
        }

        public static IToken Parse(string text, int startPosition = 0, Action<IToken, int, string> callback = null)
        {
            if (string.IsNullOrWhiteSpace(text))
                return new NullToken();

            if (text.StartsWith("'") && text.IndexOf('\'', 1) <= 0)
                return new StringToken(text.Substring(1));

            List<string> blocks = text.SplitIntoBlocks(new[] {'\'', '\'', '"', '"', '^', '^', '{', '}', '(', ')'}, true,
                StringUtils.DelimiterInclude.IncludeSeparately);

            string simplifed = "";
            List<IToken> subResults = new List<IToken>();

            int currentPosition = startPosition;
            for (int i = 0; i < blocks.Count; i += 3)
            {
                string start = blocks[i];
                string entry = blocks[i + 1];
                IToken subResult = null;
                switch (start)
                {
                    case "\"":
                    case "'":
                        subResult = new StringToken(entry);
                        callback?.Invoke(subResult, currentPosition, "'" + entry + "'");
                        break;
                    case "^":
                        subResult = Parse(entry);
                        callback?.Invoke(subResult, currentPosition, "'" + entry + "'");
                        break;
                    case "{":
                        subResult = new ExpressionToken(null, new SubstitutionOperator(), Parse(entry));
                        callback?.Invoke(subResult, currentPosition, "{" + entry + "}");
                        break;
                    case "(":
                        subResult = Parse(entry, currentPosition + 1, callback);
                        break;
                    default:
                        simplifed += entry;
                        break;
                }
                if (subResult != null)
                {
                    simplifed += $"█{subResults.Count}█";
                    subResults.Add(subResult);
                }
                if (callback != null)
                    currentPosition += start.Length + entry.Length + blocks[i + 2].Length;
            }

            IToken result = ParseExpressionNoBrackets(simplifed, startPosition, callback);
            result = SubstituteValues(result, subResults);

            return result ?? new StringToken(text);
        }

        private static IToken SubstituteValues(IToken parsed, IReadOnlyList<IToken> subResults)
        {
            IToken result = parsed;
            StringToken stringToken = parsed as StringToken;
            if (stringToken != null)
            {
                if (stringToken.Value.Contains("█"))
                {
                    result = null;
                    string[] bits = stringToken.Value.Split('█');
                    for (int i = 0; i < bits.Length - 1; i += 2)
                    {
                        int position = int.Parse(bits[i + 1]);
                        IToken substituted = subResults[position];
                        IToken partial = string.IsNullOrWhiteSpace(bits[i])
                            ? substituted
                            : new ExpressionToken(new StringToken(bits[i]), new StringPlusOperator(), substituted);
                        result = result == null
                            ? partial
                            : new ExpressionToken(result, new StringPlusOperator(), partial);
                    }
                    if (!string.IsNullOrWhiteSpace(bits[bits.Length - 1]))
                        result = new ExpressionToken(result, new StringPlusOperator(), new StringToken(bits[bits.Length - 1]));
                }
            }
            else
            {
                ExpressionToken expression = parsed as ExpressionToken;
                if (expression != null)
                    result = new ExpressionToken(SubstituteValues(expression.First, subResults), expression.Operator,
                        SubstituteValues(expression.Second, subResults));
            }
            return result;
        }

        private static IToken ParseExpressionNoBrackets(string text, int startPosition, Action<IToken, int, string> callback)
        {
            if (string.IsNullOrEmpty(text))
                return null;

            IToken result = null;
            foreach (var item in s_tokens)
            {
                Regex regex = new Regex("^\\s*" + item.Key + "\\s*$");
                Match match = regex.Match(text);
                if (match.Success)
                {
                    string[] values;
                    int count = match.Groups.Count;
                    if (count == 1)
                    {
                        values = new[] {match.Groups[0].Value};
                    }
                    else
                    {
                        values = new string[count - 1];
                        for (int i = 1; i < count; ++i)
                            values[i - 1] = match.Groups[i].Value;
                    }
                    result = item.Value(values, startPosition, callback);
                    break;
                }
            }
            return result;
        }
    }
}
