using System.Collections.Generic;
using System.Windows.Media;
using Helpers;
using TextParser.Functions;

namespace FormGenerator
{
    internal static class SyntaxProvider
    {
        static SyntaxProvider()
        {
            string[] strs =
            {
                AndFunction.ID,
                AggregateFunction.ID,
                ComparisonFunction.ID,
                ContainsFunction.ID,
                CountFunction.ID,
                DoubleSumFunction.ID,
                IfFunction.ID,
                IntSumFunction.ID,
                IntSumFunction.ALT_ID,
                OrFunction.ID,
                OverFunction.ID,
                RangeFunction.ID,
                ReverseFunction.ID,
                UserFunction.ID
            };
            Tags = new List<string>(strs);

            char[] chrs =
            {
                '.',
                ')',
                '(',
                '[',
                ']',
                '>',
                '<',
                ':',
                ';',
                '\n',
                '\t'
            };
            Specials = new List<char>(chrs);
        }

        public static List<char> Specials { get; }

        public static List<string> Tags { get; }

        public static bool IsKnownTag(string tag)
        {
            return Tags.Exists(s => s == tag);
        }

        public static List<Range> CheckWords(string text)
        {
            List<Range> ranges = new List<Range>();
            int originalLength = text.Length;
            text = text.TrimStart(' ', '\t');
            int trimmedLength = text.Length;
            int numSpacesAtStart = originalLength - trimmedLength;
            int colonPosition = text.FirstNotInBlock(':');
            if (colonPosition == -1)
            {
                ranges.Add(new Range {Start = 0, End = text.Length, Foreground = Colors.Red});
                return ranges;
            }
            ranges.Add(new Range {Start = numSpacesAtStart, End = numSpacesAtStart + colonPosition, Foreground = Colors.Blue});
            ranges.Add(new Range {Start = numSpacesAtStart + colonPosition, End = numSpacesAtStart + colonPosition + 1, Foreground = Colors.OrangeRed});
            //for (int i = 0; i < text.Length; ++i)
            //{
            //    if (text[i] == ':')
            //    {
            //        ranges.Add(new Range
            //        {
            //            Start = i,
            //            End = i + 1,
            //            Foreground = Colors.Green
            //        });
            //    }
            //}

            //int sIndex = 0;
            //for (int i = 0; i < text.Length; i++)
            //{
            //    if (char.IsWhiteSpace(text[i]) || Specials.Contains(text[i]))
            //    {
            //        if (i > 0 && !(char.IsWhiteSpace(text[i - 1]) | Specials.Contains(text[i - 1])))
            //        {
            //            string word = text.Substring(sIndex, i - sIndex);

            //            if (IsKnownTag(word))
            //            {
            //                Range t = new Range
            //                {
            //                    Start = sIndex,
            //                    End = sIndex + word.Length,
            //                    Foreground = Colors.Blue
            //                };
            //                ranges.Add(t);
            //            }
            //        }
            //        sIndex = i + 1;
            //    }
            //}

            //string lastWord = text.Substring(sIndex, text.Length - sIndex);
            //if (IsKnownTag(lastWord))
            //{
            //    Range t = new Range
            //    {
            //        Start = sIndex,
            //        End = sIndex + lastWord.Length,
            //        Foreground = Colors.Blue
            //    };
            //    ranges.Add(t);
            //}

            return ranges;
        }

        public class Range
        {
            public int End { get; set; }
            public Color Foreground { get; set; }
            public int Start { get; set; }
        }
    }
}
