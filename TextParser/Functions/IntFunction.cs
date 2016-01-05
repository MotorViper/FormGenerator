using System;
using TextParser.Tokens;

namespace TextParser.Functions
{
    public class IntFunction : BaseFunction
    {
        public const string ID = "INT";

        public IntFunction() : base(ID)
        {
        }

        public override IToken Perform(IToken dataToken, TokenTreeList parameters, bool isFinal)
        {
            ListToken listToken = dataToken as ListToken;
            if (listToken != null)
            {
                ListToken returnList = new ListToken();
                foreach (IToken token in listToken.Tokens)
                    returnList.Add(Perform(token, parameters, isFinal));
                return returnList;
            }

            ITypeToken typeToken = dataToken as ITypeToken;
            if (typeToken != null)
                return new IntToken(typeToken.Convert<int>());

            if (dataToken is NullToken)
                return dataToken;

            throw new Exception($"Token must be list or convertible to int for {ID}");
        }
    }
}
