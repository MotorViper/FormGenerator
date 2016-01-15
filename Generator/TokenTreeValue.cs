using TextParser.Tokens;

namespace Generator
{
    public class TokenTreeValue : IValue
    {
        private readonly IToken _data;

        public TokenTreeValue(IToken data)
        {
            _data = data;
        }

        public bool IsInt => _data is IntToken;
        public int IntValue => (_data as IntToken)?.Value ?? 0;
        public string StringValue => _data.Text;
    }
}
