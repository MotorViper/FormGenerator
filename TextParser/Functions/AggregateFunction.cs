using System.Collections.Generic;
using TextParser.Tokens;

namespace TextParser.Functions
{
    /// <summary>
    /// Converts a list of items into a new list with each individual item plus it's number of occurrences.
    /// </summary>
    public class AggregateFunction : BaseFunction
    {
        public const string ID = "AGG";

        /// <summary>
        /// Constructor.
        /// </summary>
        public AggregateFunction() : base(ID)
        {
        }

        /// <summary>
        /// Evaluate the function.
        /// </summary>
        /// <param name="parameters">The tokens that make up the function parameter list.</param>
        /// <param name="substitutions">The tokens that can be used for substitutions.</param>
        /// <param name="isFinal">Whether a result needs to be returned.</param>
        /// <returns></returns>
        public override IToken Perform(IToken parameters, TokenTreeList substitutions, bool isFinal)
        {
            ListToken listToken = parameters as ListToken;

            if (listToken == null)
                return new ListToken {new ListToken {parameters, new IntToken(1)}};

            Dictionary<string, int> found = new Dictionary<string, int>();
            foreach (IToken child in listToken.Tokens)
                if (!AddToken(found, child))
                    return UnParsed(listToken);

            ListToken list = new ListToken();
            foreach (var item in found)
                list.Add(new ListToken {new StringToken(item.Key), new IntToken(item.Value)});
            return list;
        }

        /// <summary>
        /// Recursively adds tokens to the found list.
        /// </summary>
        /// <param name="found">The list of found tokens and their count.</param>
        /// <param name="child">The token to add.</param>
        /// <returns></returns>
        private static bool AddToken(IDictionary<string, int> found, IToken child)
        {
            ListToken list = child as ListToken;
            if (list != null)
            {
                foreach (IToken token in list)
                    if (!AddToken(found, token))
                        return false;
            }
            else
            {
                if (child is ExpressionToken)
                    return false;
                int count;
                string value = child.Text;
                found[value] = found.TryGetValue(value, out count) ? ++count : 1;
            }
            return true;
        }
    }
}
