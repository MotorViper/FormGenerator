using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Helpers;

namespace TextParser
{
    /// <summary>
    /// Converts the input from a text reader into a set of lines.
    /// Handles ifs, includes and continuations.
    /// </summary>
    public class Reader : IEnumerable<Line>
    {
        private readonly TextReader _reader;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="reader">The text reader.</param>
        public Reader(TextReader reader)
        {
            _reader = reader;
        }

        /// <summary>
        /// Options the reader will use when processing theinput.
        /// </summary>
        public ReaderOptions Options { get; set; } = new ReaderOptions
        {
            DefaultDirectory = "",
            EndComment = "*/",
            SinglelineComment = "//",
            StartComment = "/*",
            Selector = null
        };

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns> An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<Line> GetEnumerator()
        {
            string line;
            string expected = null;
            bool inIf = false;
            int lineNumber = 0;
            while ((line = _reader.ReadLine()) != null)
            {
                ++lineNumber;
                if (expected != null)
                {
                    if (line.TrimEnd().ToLower().EndsWith(expected))
                    {
                        if (expected == "#if")
                            inIf = false;
                        expected = null;
                    }
                }
                else
                {
                    string start = line.TrimStart();
                    string lowerStart = start.ToLower();
                    if (lowerStart.StartsWith("#fi"))
                    {
                        if (inIf)
                            inIf = false;
                        else
                            throw new Exception($"Unmatched #fi at {lineNumber}");
                        line = null;
                    }
                    else if (lowerStart.StartsWith(Options.StartComment))
                    {
                        if (!line.TrimEnd().EndsWith(Options.EndComment))
                            expected = Options.EndComment;
                        line = null;
                    }
                    else if (lowerStart.StartsWith(Options.SinglelineComment))
                    {
                        line = null;
                    }
                    else if (lowerStart.StartsWith("#if "))
                    {
                        string[] bits = start.TrimEnd().Split(new[] {' '}, 3);
                        inIf = bits.Length < 3;
                        line = inIf || bits[1] != Options.Selector ? null : bits[2];
                        expected = bits.Length == 1 || (bits.Length == 2 && bits[1] != Options.Selector) ? "#fi" : null;
                    }
                    if (line != null)
                    {
                        if (lowerStart.StartsWith("#include "))
                        {
                            string[] parts = line.Split(' ');
                            int indentCount = parts.Length - 2;
                            if (start.Length != line.Length)
                            {
                                string rest = line.Replace(start, "");
                                Line indent = new Line(rest, false);
                                indentCount = indent.Offset;
                            }
                            else
                            {
                                for (int i = 2; i < parts.Length; ++i)
                                    yield return new Line(parts[i] + ":") {Offset = i - 2};
                            }
                            Reader reader = new Reader(new StreamReader(FileUtils.GetFullFileName(parts[1], Options.DefaultDirectory)))
                            {
                                Options = Options
                            };
                            foreach (Line includedLine in reader)
                            {
                                if (indentCount > 0)
                                    includedLine.Offset += indentCount;
                                yield return includedLine;
                            }
                        }
                        else
                        {
                            line = AddContinutation(line, ref lineNumber);
                            if (!string.IsNullOrWhiteSpace(line))
                                yield return new Line(line);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns> An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection. </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Process a line to take into account continuation characters.
        /// </summary>
        /// <param name="line">The line to process.</param>
        /// <param name="lineNumber">The current line number.</param>
        /// <returns>The current line including any continuations.</returns>
        private string AddContinutation(string line, ref int lineNumber)
        {
            line = line.TrimEnd();
            while (line.EndsWith("-"))
            {
                line = line.Remove(line.Length - 1);
                string extra;
                if (line.EndsWith("-"))
                {
                    line = line.Remove(line.Length - 1).TrimEnd();
                    line += Environment.NewLine;
                    extra = _reader.ReadLine();
                    ++lineNumber;
                }
                else
                {
                    int length = line.Length;
                    line = line.TrimEnd();
                    bool needWhitespace = line.Length != length;
                    extra = _reader.ReadLine();
                    ++lineNumber;
                    if (extra != null)
                    {
                        length = extra.Length;
                        extra = extra.TrimStart();
                        if (length != extra.Length || needWhitespace)
                            extra = " " + extra;
                    }
                }
                if (extra == null)
                    break;
                extra = extra.TrimEnd();
                line += extra;
            }
            return line;
        }
    }
}
