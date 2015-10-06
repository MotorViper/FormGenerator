using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Helpers;
using TextParser.Tokens;

namespace TextParser
{
    public class TokenGenerator
    {
        private static readonly Dictionary<string, Func<string[], IToken>> s_tokens = new Dictionary<string, Func<string[], IToken>>
        {
            ["(.*)([\\+-])([^\\+-]*)"] = tokens => PerformOperation(tokens),
            ["(.*)([\\*/×÷])([^\\*/×÷]*)"] = tokens => PerformOperation(tokens),
            ["(.*)\\$([A-Za-z][A-Za-z0-9]*(\\.[A-Za-z][A-Za-z0-9]*)*)?([^$]*)"] = tokens => PerformSubstitutionOperation(tokens),
            ["(0|[1-9][0-9]*\\.[0-9]+)"] = tokens => new DoubleToken(double.Parse(tokens[0])),
            ["(0|[1-9][0-9]*)"] = tokens => new IntToken(int.Parse(tokens[0])),
            [".*"] = tokens => new StringToken(tokens[0])
        };

        private static IToken PerformSubstitutionOperation(IReadOnlyList<string> tokens)
        {
            IToken token = null;
            if (!string.IsNullOrWhiteSpace(tokens[1]))
            {
                token = new StringToken(tokens[1]);
            }
            IToken result = new ExpressionToken(null, new SubstitutionToken(), token);
            if (!string.IsNullOrWhiteSpace(tokens[0]))
            {
                token = ParseExpressionNoBrackets(tokens[0].Trim());
                ExpressionToken expression = token as ExpressionToken;
                if (expression?.NeedsSecond ?? false)
                    result = expression.SetSecond(result);
                else
                    result = new ExpressionToken(token, new StringPlusToken(), result);
            }
            if (!string.IsNullOrWhiteSpace(tokens[3]))
            {
                token = ParseExpressionNoBrackets(tokens[3].Trim());
                result = new ExpressionToken(result, new StringPlusToken(), token);
            }
            return result;
        }

        private static IToken PerformOperation(IReadOnlyList<string> tokens)
        {
            IToken first = ParseExpressionNoBrackets(tokens[0].Trim());
            OperatorToken op = OperatorToken.CreateOperatorToken(tokens[1]);
            IToken second = ParseExpressionNoBrackets(tokens[2].Trim());

            ExpressionToken firstExpression = first as ExpressionToken;
            if (firstExpression != null && firstExpression.NeedsSecond && op.CanBeUnary)
            {
                firstExpression.SetSecond(new ExpressionToken(null, op, second));
                return first;
            }

            return new ExpressionToken(first, op, second);
        }

        public IToken Parse(string text)
        {
            List<string> blocks = text.SplitIntoBlocks(new[] { '\'', '\'', '"', '"', '{', '}', '(', ')' }, true, StringUtils.DelimiterInclude.IncludeSeparately);

            string simplifed = "";
            List<IToken> subResults = new List<IToken>();

            for (int i = 0; i < blocks.Count / 3; i++)
            {
                int index = i * 3;
                string start = blocks[index];
                string entry = blocks[index + 1];
                IToken subResult;
                switch (start)
                {
                    case "\"":
                    case "'":
                        subResult = new StringToken(entry);
                        simplifed += $"█{subResults.Count}█";
                        subResults.Add(subResult);
                        break;
                    case "{":
                        subResult = new ExpressionToken(null, new SubstitutionToken(), Parse(entry));
                        simplifed += $"█{subResults.Count}█";
                        subResults.Add(subResult);
                        break;
                    case "(":
                        subResult = Parse(entry);
                        simplifed += $"█{subResults.Count}█";
                        subResults.Add(subResult);
                        break;
                    default:
                        simplifed += entry;
                        break;
                }
            }

            IToken result = ParseExpressionNoBrackets(simplifed);
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
                            : new ExpressionToken(new StringToken(bits[i]), new StringPlusToken(), substituted);
                        result = result == null 
                            ? partial 
                            : new ExpressionToken(result, new StringPlusToken(), partial);
                    }
                    if (!string.IsNullOrWhiteSpace(bits[bits.Length - 1]))
                        result = new ExpressionToken(result, new StringPlusToken(), new StringToken(bits[bits.Length - 1]));
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

        private static IToken ParseExpressionNoBrackets(string text)
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
                    result = item.Value(values);
                    break;
                }
            }
            return result;
        }
    }
}
