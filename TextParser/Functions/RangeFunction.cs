using System;
using TextParser.Tokens;

namespace TextParser.Functions
{
    public class RangeFunction : BaseFunction
    {
        public const string ID = "RANGE";

        public RangeFunction() : base(ID)
        {
        }

        public override IToken Perform(IToken token, TokenTreeList parameters, bool isFinal)
        {
            ListToken list = token as ListToken;
            if (list == null)
            {
                list = new ListToken();
                list.Tokens.Add(token);
            }
            ListToken result = new ListToken();
            switch (list.Tokens.Count)
            {
                case 1:
                    for (int i = 1; i <= list.Tokens[0].Convert<int>(); ++i)
                        result.Tokens.Add(new IntToken(i));
                    break;
                case 2:
                    for (int i = list.Tokens[0].Convert<int>(); i <= list.Tokens[1].Convert<int>(); ++i)
                        result.Tokens.Add(new IntToken(i));
                    break;
                case 3:
                    for (int i = list.Tokens[0].Convert<int>(); i <= list.Tokens[1].Convert<int>(); i += list.Tokens[2].Convert<int>())
                        result.Tokens.Add(new IntToken(i));
                    break;
                default:
                    throw new Exception($"Must have between 1 and 3 values for '{ID}': {token}");
            }
            return result;
        }
    }
}
