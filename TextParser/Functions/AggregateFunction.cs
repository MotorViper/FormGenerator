using System.Collections.Generic;
using System.Text;
using TextParser.Tokens;

namespace TextParser.Functions
{
    public class AggregateFunction : BaseFunction
    {
        public override IToken Perform(IToken token, TokenTreeList parameters, bool isFinal)
        {
            ListToken listToken = token as ListToken;

            if (listToken == null)
                return token;

            Dictionary<string, int> found = new Dictionary<string, int>();
            foreach (IToken child in listToken.Tokens)
            {
                if (child is ExpressionToken)
                    return UnParsed(listToken);
                int count;
                string value = child.Text;
                found[value] = found.TryGetValue(value, out count) ? ++count : 1;
            }

            StringBuilder sb = new StringBuilder();
            foreach (var item in found)
                sb.Append(item.Key).Append("(").Append(item.Value).Append(")/");
            return new StringToken(sb.ToString().TrimEnd('/'));
        }
    }
}
