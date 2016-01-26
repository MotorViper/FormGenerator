﻿namespace TextParser
{
    /// <summary>
    /// Class that represents a line that is input into a reader.
    /// </summary>
    public class Line
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="line">The line to process.</param>
        /// <param name="trim">Whether the end of the line can be trimmed before processing.</param>
        public Line(string line, bool trim = true)
        {
            line = line.Replace("\t", new string(' ', TabSize));
            if (trim)
                line = line.TrimEnd();
            Content = line.TrimStart();
            Offset = (line.Length - Content.Length + OffsetSize - 1) / OffsetSize;
        }

        /// <summary>
        /// The process content of the line.
        /// </summary>
        public string Content { get; }

        /// <summary>
        /// The offset from the start of the line in offset unit.
        /// </summary>
        public int Offset { get; internal set; }

        /// <summary>
        /// The offset unit size.
        /// </summary>
        public static int OffsetSize { get; set; } = 4;

        /// <summary>
        /// How many spaces a tab should represent.
        /// </summary>
        public static int TabSize { get; set; } = 4;
    }
}
