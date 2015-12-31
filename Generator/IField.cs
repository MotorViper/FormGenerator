using TextParser;
using TextParser.Tokens;

namespace Generator
{
    public interface IField
    {
        TokenTreeList Children { get; }
        TokenTree Data { set; }
        int Level { set; }
        string Name { set; }
        IToken Parameter { set; }
        IField Parent { set; }
        TokenTree Selected { get; set; }
        void OutputField(IFieldWriter builder, int level, TokenTree parameters, string offset, string endOfLine);
        void AddChildProperties(IField child);
        void AddProperty<T>(string name, T value);
    }
}
