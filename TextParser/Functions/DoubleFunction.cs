using System;
using TextParser.Tokens;

namespace TextParser.Functions
{
    public class DoubleFunction : BaseFunction
    {
        public const string ID = "DBL";

        public DoubleFunction() : base(ID)
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
                return new DoubleToken(typeToken.Convert<double>());

            throw new Exception($"Token must be list or convertible to double for {ID}");
        }
    }
}
