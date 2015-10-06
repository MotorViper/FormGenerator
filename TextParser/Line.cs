namespace TextParser
{
    public class Line
    {
        public Line(string line)
        {
            line = line.TrimEnd().Replace("\t", new string(' ', TabSize));
            Content = line.TrimStart();
            Offset = (line.Length - Content.Length + OffsetSize - 1) / OffsetSize;
        }

        public string Content { get; }
        public int Offset { get; internal set; }
        public static int OffsetSize { get; set; } = 4;
        public static int TabSize { get; set; } = 4;
    }
}
