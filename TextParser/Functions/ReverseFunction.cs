using TextParser.Tokens;

namespace TextParser.Functions
{
    public class ReverseFunction : BaseFunction
    {
        public override IToken Perform(IToken token, TokenTreeList parameters, bool isFinal)
        {
            ListToken list = token as ListToken;
            if (list == null)
            {
                list = new ListToken();
                list.Tokens.Add(token);
            }
            ListToken result = new ListToken();
            result.Tokens.AddRange(list.Tokens);
            result.Tokens.Reverse();
            return result;
        }
    }
}
