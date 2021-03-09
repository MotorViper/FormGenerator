using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Helpers
{
    /// <summary>
    /// String utilities class.
    /// </summary>
    public static class StringUtils
    {
        /// <summary>
        /// Enum specifying how to include delimiters.
        /// See SplitIntoBlocks below.
        /// </summary>
        public enum DelimiterInclude
        {
            IncludeInline,
            IncludeSeparately,
            DontInclude
        }

        /// <summary>
        /// Splits a string into blocks.
        /// </summary>
        /// <param name="parent">The string to split.</param>
        /// <param name="delimiters">The delimiters to split at, defaults to ''' and '"'.</param>
        /// <param name="paired">Whether the delimiters are paired, i.e. '[' with ']' or not, i.e. '"'.</param>
        /// <param name="includeDelimiters">How to include delimiters in the output.</param>
        /// <returns>The string split into blocks.</returns>
        public static List<string> SplitIntoBlocks(this string parent, char[] delimiters = null, bool paired = false,
            DelimiterInclude includeDelimiters = DelimiterInclude.IncludeInline)
        {
            int delimiter = -1;
            int count = 0;
            bool escape = false;
            List<char> ends;
            List<char> starts;
            CreateDelimiterArrays((char)0, delimiters, paired, out starts, out ends);
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
                                blocks.Add(c.ToString());
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
                        if (!string.IsNullOrEmpty(current))
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
                            blocks.Add(c.ToString());
                    }
                    else
                    {
                        current += c;
                    }
                }
            }
            if (delimiter != -1)
                throw new Exception($"Delimiters are not balanced in {parent}");
            if (!string.IsNullOrEmpty(current))
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
        
        /// <summary>
        /// Reverses a string.
        /// </summary>
        /// <param name="toReverse">The string to reverse.</param>
        /// <returns>The reversed string.</returns>
        public static string Reverse(this string toReverse)
        {
            char[] array = toReverse.ToCharArray();
            Array.Reverse(array);
            return new string(array);
        }

        /// <summary>
        /// Finds the first instance of a character that is not included within delimited blocks.
        /// </summary>
        /// <param name="parent">The string to split.</param>
        /// <param name="toFind">The character to find.</param>
        /// <param name="delimiters">The delimiters to split at, defaults to ''' and '"', without the relevant one if toFind is one of them.</param>
        /// <param name="paired">Whether the delimiters are paired, i.e. '[' with ']' or not, i.e. ','.</param>
        /// <returns>The position of the first character that matches the criteria.</returns>
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

        /// <summary>
        /// Counts the number of instances of a string included in another string.
        /// </summary>
        /// <param name="parent">The string to check.</param>
        /// <param name="text">The string to look for.</param>
        /// <returns>The number of instances</returns>
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

        /// <summary>
        /// Creates a string that is a concatenation of multiple copies of a single string.
        /// </summary>
        /// <param name="seed">The string to create the new string from.</param>
        /// <param name="count">The number of instances of the seed.</param>
        /// <returns>The generated string.</returns>
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

        /// <summary>
        /// Creates a string that can be used in regexes.
        /// </summary>
        /// <param name="text">The string to convert.</param>
        /// <returns>The regex string.</returns>
        public static string CreateRegexpString(this string text)
        {
            return text.Replace("\\", "\\\\").Replace("*", "\\*");
        }

        /// <summary>
        /// Converts a hyphenated string to CamelCase.
        /// </summary>
        /// <param name="text">The text to convert.</param>
        /// <returns>The converted string.</returns>
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
                    result += Char.ToUpper(c);
                    capitalise = false;
                }
                else
                {
                    result += c;
                }
            }
            return result;
        }

        /// <summary>
        /// Converts a string in CamelCase to a hyphenated string.
        /// </summary>
        /// <param name="text">The string to convert.</param>
        /// <param name="toLower">Whether the output should be all lower case.</param>
        /// <returns>The converted string.</returns>
        public static string CamelCaseToHyphenated(this string text, bool toLower = true)
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
            return toLower ? result.ToLower() : result;
        }

        /// <summary>
        /// Returns the number of instances of a character in a string.
        /// </summary>
        public static int CountInstances(this string text, char toCount)
        {
            return text.Count(t => t == toCount);
        }

        /// <summary>
        /// Returns the number of lines in a string.
        /// </summary>
        public static int LineCount(this string text)
        {
            return 1 + text.CountInstances('\n');
        }

        public static int NthInstance(this string text, char item, int n)
        {
            if (n > 0)
            {
                int count = 0;
                for (int i = 0; i < text.Length; i++)
                {
                    if (text[i] == item)
                    {
                        count++;
                        if (count == n)
                            return i;
                    }
                }
                return -2;
            }
            return -1;
        }

        /// <summary>
        /// Returns the zero-based position of the first character of the specified line.
        /// If the line number is greater than the current line count, the method returns the position of the last character.
        /// </summary>
        public static int GetPositionOfLineStart(this string text, int lineIndex)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));
            int index = text.NthInstance('\n', lineIndex);
            switch (index)
            {
                case -1:
                    return 0;
                case -2:
                    return Math.Max(text.Length - 1, 0);
                default:
                    return Math.Min(text.Length - 1, index + 1);
            }
        }

        /// <summary>
        /// Returns the zero-based position of the last character of the specified line.
        /// If the line number is greater than the current line count, the method returns the position of the last character.
        /// </summary>
        public static int GetPositionOfLineEnd(this string text, int lineIndex)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));
            int index = text.NthInstance('\n', lineIndex + 1);
            switch (index)
            {
                case -1:
                    return 0;
                case -2:
                    return Math.Max(text.Length - 1, 0);
                default:
                    return index;
            }
        }

        public static IndexOfResult IndexOfItem(this string text, string toFind)
        {
            int index = text.IndexOf(toFind);
            return new IndexOfResult(index >= 0, index, toFind.Length);
        }

        public static IndexOfResult IndexOfPattern(this string parent, string pattern)
        {
            Match match = new Regex(pattern).Match(parent);
            return new IndexOfResult(match.Success, match.Index, match.Length);
        }

        public static string RemoveFirst(this string text, string toRemove)
        {
            int index = text.IndexOf(toRemove);
            return text.Substring(index + toRemove.Length);
        }

        public static List<string> Split(this string parent, char[] characters, DelimiterInclude includeDelimiters)
        {
            List<string> blocks = new List<string>();
            string current = "";

            foreach (char c in parent)
            {
                if (characters.Contains(c))
                {
                    if (includeDelimiters == DelimiterInclude.IncludeInline)
                        current += c;
                    blocks.Add(current);
                    if (includeDelimiters == DelimiterInclude.IncludeSeparately)
                        blocks.Add(c.ToString());
                    current = "";
                }
                else
                {
                    current += c;
                }
            }
            if (!string.IsNullOrEmpty(current))
                blocks.Add(current);
            return blocks;
        }

        public static Tuple<string, string> SplitAt(this string data, string split)
        {
            int index = data.IndexOf(split);
            return index >= 0
                ? Tuple.Create(data.Substring(0, index), data.Substring(index + split.Length))
                : Tuple.Create((string)null, data);
        }


        public static string Start(this string text, uint end)
        {
            return text.Substring((int)end);
        }

        public static string StartAfter(this string text, uint end, string ignore)
        {
            return text.Substring((int)end + ignore.Length);
        }

        public static string StartAfter(this string text, IndexOfResult index)
        {
            return text.Substring((int)index.Index, index.Length);
        }

        public static string Substring(this string text, uint start, uint end)
        {
            return text.Substring((int)start, (int)end);
        }

        public static string AtMost(this string text, int length)
        {
            return text.Substring(0, Math.Min(length, text.Length));
        }

        public class IndexOfResult
        {
            public IndexOfResult(bool success, int position, int length)
            {
                if (success)
                {
                    Index = (uint)position;
                    Length = length;
                }
                else
                {
                    Index = uint.MaxValue;
                    Length = 0;
                }
            }

            public uint Index { get; private set; }
            public int Length { get; private set; }
        }
    }
}
