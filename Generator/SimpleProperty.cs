using TextParser;
using TextParser.Tokens;

namespace Generator
{
    public class SimpleProperty : IProperty
    {
        public SimpleProperty(string name, int value) : this(name, value.ToString(), value, true)
        {
        }

        public SimpleProperty(string name, string value) : this(name, value, 0, false)
        {
        }

        public SimpleProperty(string name, IValue value) : this(name, value.StringValue, value.IntValue, value.IsInt, value.Token)
        {
        }

        private SimpleProperty(string name, string s, int i, bool b, IToken t = null)
        {
            Name = name;
            StringValue = s;
            IsInt = b;
            IntValue = i;
            Token = t;
        }

        public string Name { get; }
        public int IntValue { get; }
        public bool IsInt { get; }
        public string StringValue { get; }
        public IToken Token { get; }

        public IValue ApplyParameters(TokenTree parameters)
        {
            return null;
        }
    }
}
