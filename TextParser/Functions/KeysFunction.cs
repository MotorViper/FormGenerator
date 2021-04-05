using TextParser.Tokens;
using TextParser.Tokens.Interfaces;

namespace TextParser.Functions
{
    class KeysFunction : BaseFunction
    {
        public KeysFunction() : base("K(EYS)")
        {
        }

        public override IToken Perform(IToken parameters, TokenTreeList substitutions, bool isFinal)
        {
            if (substitutions == null && !isFinal)
                return UnParsed(parameters);
            TokenTreeList list = substitutions.FindAllMatches(parameters.ToString());
            ListToken result = new ListToken();
            foreach (TokenTree tree in list)
                foreach (TokenTree subTree in tree.Children)
                    result.Add(subTree.Key);
            return result;
        }
    }
}
