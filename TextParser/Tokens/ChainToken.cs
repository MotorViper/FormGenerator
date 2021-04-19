using System.Text;
using TextParser.Tokens.Interfaces;

namespace TextParser.Tokens
{
    public class ChainToken : ListToken
    {
        public ChainToken() : base()
        {
        }

        public ChainToken(IToken value1, IToken value2) : base(value1, value2)
        {
        }

        public ChainToken(string value1, string value2) : base((StringToken)value1, (StringToken)value2)
        {
        }

        public override string ToString()
        {
            if (Value.Count == 0)
                return "";

            StringBuilder sb = new StringBuilder();
            foreach (IToken token in Value)
                sb.Append(token.ToString()).Append(".");
            string text = sb.ToString();
            return text.Substring(0, text.Length - 1);
        }

        /// <summary>
        /// Evaluates the token.
        /// </summary>
        /// <param name="parameters">The parameters to use for substitutions.</param>
        /// <param name="isFinal">Whether this is a final parse.</param>
        /// <returns></returns>
        protected override IToken Process(TokenTreeList parameters, bool isFinal)
        {
            ChainToken list = new ChainToken();
            foreach (IToken token in Value)
            {
                IToken item = token.Evaluate(parameters, isFinal);
                list.Value.Add(item);
            }
            return list;
        }

        /// <summary>
        /// All but the first element in the chain.
        /// </summary>
        public IToken Last
        {
            get
            {
                if (Value.Count == 2)
                {
                    return Value[1];
                }
                else
                {
                    ChainToken chain = new ChainToken();
                    for (int i = 1; i < Value.Count; ++i)
                        chain.Value.Add(Value[i]);
                    return chain;
                }
            }
        }

        /// <summary>
        /// First token in the chain.
        /// </summary>
        public IToken First
        {
            get
            {
                return Value[0];
            }
        }

        /// <summary>
        /// Whether the token contains the input token.
        /// </summary>
        /// <param name="token">The input token.</param>
        /// <returns>True if the current token contains the input token.</returns>
        public override bool HasMatch(IToken token)
        {
            return Value[0].HasMatch(token);
        }
    }
}
