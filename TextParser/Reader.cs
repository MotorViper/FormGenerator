using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Helpers;

namespace TextParser
{
    public class Reader : IEnumerable<Line>
    {
        private readonly TextReader _reader;
        private string _endComment;
        private Regex _endRegex;
        private string _singlelineComment;
        private Regex _singleRegex;
        private string _startComment;
        private Regex _startRegex;

        public Reader(TextReader reader, string startComment = "/*", string endComment = "*/", string singleLineComment = "//")
        {
            _reader = reader;
            StartComment = startComment;
            EndComment = endComment;
            SinglelineComment = singleLineComment;
        }

        public string DefaultDirectory
        {
            get; set;
        }

        public string EndComment
        {
            get { return _endComment; }
            set
            {
                _endComment = value;
                _endRegex = new Regex(_endComment.CreateRegexpString() + "\\s*$");
            }
        }

        public string SinglelineComment
        {
            get { return _singlelineComment; }
            set
            {
                _singlelineComment = value;
                _singleRegex = new Regex("^\\s*" + _singlelineComment.CreateRegexpString());
            }
        }

        public string StartComment
        {
            get { return _startComment; }
            set
            {
                _startComment = value;
                _startRegex = new Regex("^\\s*" + _startComment.CreateRegexpString());
            }
        }

        public IEnumerator<Line> GetEnumerator()
        {
            string line;
            while ((line = _reader.ReadLine()) != null)
            {
                line = IgnoreComments(line);
                if (line == null)
                    break;
                if (line.ToLower().StartsWith("@include "))
                {
                    string[] parts = line.Split(' ');
                    int indentCount = parts.Length - 2;
                    for (int i = 2; i < parts.Length; ++i)
                        yield return new Line(parts[i] + ":") {Offset = i - 2};
                    Reader reader = new Reader(new StreamReader(FileUtils.GetFullFileName(parts[1], DefaultDirectory)), _startComment, _endComment, _singlelineComment)
                    {
                        DefaultDirectory = DefaultDirectory
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
                    line = AddContinutation(line);
                    if (!string.IsNullOrWhiteSpace(line))
                        yield return new Line(line);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private string AddContinutation(string line)
        {
            line = line.TrimEnd();
            while (line.EndsWith("-"))
            {
                line = line.Remove(line.Length - 1);
                int length = line.Length;
                line = line.TrimEnd();
                bool needWhitespace = line.Length != length;
                string extra = _reader.ReadLine();
                if (extra == null)
                    break;
                extra = extra.TrimEnd();
                length = extra.Length;
                extra = extra.TrimStart();
                if (length != extra.Length || needWhitespace)
                    extra = " " + extra;
                line += extra;
            }
            return line;
        }

        private string IgnoreComments(string line)
        {
            bool inComment = _startRegex.IsMatch(line);
            while (inComment || _singleRegex.IsMatch(line))
            {
                if (inComment)
                    inComment = !_endRegex.IsMatch(line);
                line = _reader.ReadLine()?.TrimEnd();
                if (line == null)
                    break;
                if (_endRegex.IsMatch(line))
                {
                    line = _reader.ReadLine();
                    if (line == null)
                        break;
                    inComment = _startRegex.IsMatch(line);
                }
            }
            return line;
        }
    }
}
