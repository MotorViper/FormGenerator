using TextParser;
using TextParser.Tokens;

namespace Generator
{
    public interface IFieldWriter
    {
        string Generated { get; }
        IFieldWriter Append<T>(T s);
        void AppendLine();

        void AddChild(TokenTree data, int level, TokenTree parameters, string offset, string endOfLine, IField parent = null, IToken parameter = null,
            TokenTree selected = null);
    }
}
