using System;

namespace TextParser.Tokens
{
    public class NewLineToken : TypeToken<string>
    {
        public NewLineToken() : base(Environment.NewLine, TokenType.StringToken)
        {
        }
    }
}