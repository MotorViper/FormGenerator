using System;
using TextParser.Tokens.Interfaces;

namespace TextParser.Tokens
{
    public class NewLineToken : StringToken
    {
        public NewLineToken() : base(Environment.NewLine)
        {
        }

        public override string ToString()
        {
            return Environment.NewLine;
        }

        public override IToken Reverse()
        {
            return new NewLineToken();
        }
    }
}