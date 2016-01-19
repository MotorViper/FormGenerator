using TextParser;
using TextParser.Tokens;

namespace Generator
{
    public class SimpleProperty : IProperty
    {
        public SimpleProperty(string name, int value)
        {
            Name = name;
            StringValue = value.ToString();
            IsInt = true;
            IntValue = value;
        }

        public SimpleProperty(string name, string value)
        {
            Name = name;
            StringValue = value;
            IsInt = false;
            IntValue = 0;
        }

        public string Name { get; }
        public int IntValue { get; }
        public bool IsInt { get; }
        public string StringValue { get; }
        public IToken Token => null;
        public TokenTree Tree => null;

        public IValue ApplyParameters(TokenTree parameters)
        {
            return null;
        }
    }
}
