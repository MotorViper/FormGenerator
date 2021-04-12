using TextParser.Tokens;
using TextParser.Tokens.Interfaces;

namespace TextParser.Functions
{
    class UniqueFunction : ListFunction
    {
        public UniqueFunction() : base("U(NIQUE)")
        {
        }

        protected override IToken PerformOnList(ListToken parameters, TokenTreeList substitutions, bool isFinal)
        {
            ListToken toReturn = new ListToken();
            foreach (IToken token in parameters)
            {
                if (!toReturn.Value.Contains(token))
                    toReturn.Add(token);
            }
            return toReturn;
        }
    }
}