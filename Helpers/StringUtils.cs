using System;
using System.Collections.Generic;

namespace Helpers
{
    public static class StringUtils
    {
        public enum DelimiterInclude
        {
            IncludeInline,
            IncludeSeparately,
            DontInclude
        }

        public static List<string> SplitIntoBlocks(this string parent, char[] delimiters = null, bool paired = false,
            DelimiterInclude includeDelimiters = DelimiterInclude.IncludeInline)
        {
            int delimiter = -1;
            int count = 0;
            bool escape = false;
            List<char> ends;
            List<char> starts;
            CreateDelimiterArrays('1', delimiters, paired, out starts, out ends);
            List<string> blocks = new List<string>();
            string current = "";

            foreach (char c in parent)
            {
                if (escape)
                {
                    escape = false;
                    if ((starts.Contains(c) || ends.Contains(c)) && includeDelimiters == DelimiterInclude.IncludeInline)
                        current += "\\";
                    current += c;
                }
                else if (c == '\\')
                {
                    escape = true;
                }
                else if (delimiter != -1)
                {
                    if (ends[delimiter] == c)
                    {
                        if (count == 0)
                        {
                            delimiter = -1;
                            if (includeDelimiters == DelimiterInclude.IncludeInline)
                                current += c;
                            blocks.Add(current);
                            if (includeDelimiters == DelimiterInclude.IncludeSeparately)
                                blocks.Add("" + c);
                            current = "";
                        }
                        else if (paired)
                        {
                            --count;
                            current += c;
                        }
                    }
                    else if (paired && starts[delimiter] == c)
                    {
                        ++count;
                        current += c;
                    }
                    else
                    {
                        current += c;
                    }
                }
                else
                {
                    if (starts.Contains(c))
                    {
                        delimiter = starts.IndexOf(c);
                        if (!string.IsNullOrWhiteSpace(current))
                        {
                            if (includeDelimiters == DelimiterInclude.IncludeSeparately)
                                blocks.Add("");
                            blocks.Add(current);
                            if (includeDelimiters == DelimiterInclude.IncludeSeparately)
                                blocks.Add("");
                        }
                        current = "";
                        if (includeDelimiters == DelimiterInclude.IncludeInline)
                            current += c;
                        else if (includeDelimiters == DelimiterInclude.IncludeSeparately)
                            blocks.Add("" + c);
                    }
                    else
                    {
                        current += c;
                    }
                }
            }
            if (!string.IsNullOrWhiteSpace(current))
            {
                if (includeDelimiters == DelimiterInclude.IncludeSeparately)
                    blocks.Add("");
                blocks.Add(current);
                if (includeDelimiters == DelimiterInclude.IncludeSeparately)
                    blocks.Add("");
            }
            return blocks;
        }

        private static void CreateDelimiterArrays(char toFind, char[] delimiters, bool paired, out List<char> starts, out List<char> ends)
        {
            starts = new List<char>();
            ends = new List<char>();
            if (delimiters == null)
            {
                if (toFind != '"')
                {
                    starts.Add('"');
                    ends.Add('"');
                }
                if (toFind != '\'')
                {
                    starts.Add('\'');
                    ends.Add('\'');
                }
            }
            else if (paired)
            {
                bool start = true;
                foreach (char c in delimiters)
                {
                    if (start)
                        starts.Add(c);
                    else
                        ends.Add(c);
                    start = !start;
                }
            }
            else
            {
                foreach (char c in delimiters)
                {
                    starts.Add(c);
                    ends.Add(c);
                }
            }
        }

        public static int FirstNotInBlock(this string parent, char toFind, char[] delimiters = null, bool paired = false)
        {
            int position = -1;
            int maybe = -1;
            int index = 0;
            int delimiter = -1;
            bool escape = false;
            List<char> starts;
            List<char> ends;
            CreateDelimiterArrays(toFind, delimiters, paired, out starts, out ends);

            foreach (char c in parent)
            {
                if (escape)
                {
                    escape = false;
                }
                else if (c == '\\')
                {
                    escape = true;
                }
                else if (delimiter != -1)
                {
                    if (ends.Contains(c))
                    {
                        if (ends[delimiter] == c)
                        {
                            delimiter = -1;
                            maybe = -1;
                        }
                    }
                    else if (toFind == c && maybe == -1)
                    {
                        maybe = index;
                    }
                }
                else
                {
                    if (starts.Contains(c))
                    {
                        delimiter = starts.IndexOf(c);
                    }
                    else if (toFind == c && position == -1)
                    {
                        position = index;
                    }
                }
                ++index;
            }
            return position == -1 ? maybe : position;
        }

        public static int CountInstances(this string parent, string text)
        {
            int count = 0;
            int pos;
            while ((pos = parent.IndexOf(text)) != -1)
            {
                ++count;
                parent = parent.Substring(pos + text.Length);
            }
            return count;
        }

        public static string CreateString(string seed, double count)
        {
            string result = "";
            int fullCount = (int)Math.Floor(count);
            for (int i = 0; i < fullCount; ++i)
                result += seed;
            double rest = count - fullCount;
            int length = (int)(seed.Length * rest);
            result += seed.Substring(0, length);
            return result;
        }

        public static string CreateRegexpString(this string text)
        {
            return text.Replace("\\", "\\\\").Replace("*", "\\*");
        }

        public static string ToCamelCase(this string text)
        {
            string result = "";
            bool capitalise = true;
            foreach (char c in text)
            {
                if (c == '_')
                {
                    capitalise = true;
                }
                else if (capitalise)
                {
                    result += char.ToUpper(c);
                    capitalise = false;
                }
                else
                {
                    result += c;
                }
            }
            return result;
        }

        public static string CamelCaseToHyphenated(this string text)
        {
            string result = "";
            bool isFirst = true;
            foreach (char c in text)
            {
                if (char.IsUpper(c) && !isFirst)
                {
                    result += "-";
                }
                result += c;
                isFirst = false;
            }
            return result;
        }
    }
}
