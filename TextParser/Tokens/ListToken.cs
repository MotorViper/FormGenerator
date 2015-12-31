using System;
using System.Collections.Generic;
using System.Text;

namespace TextParser.Tokens
{
    public class ListToken : BaseToken
    {
        public ListToken()
        {
            Tokens = new List<IToken>();
        }

        public override string Text
        {
            get
            {
                if (Tokens.Count == 0)
                    return "";

                StringBuilder sb = new StringBuilder();
                foreach (IToken token in Tokens)
                    sb.Append(token.Text).Append("|");
                string text = sb.ToString();
                return "(" + text.Substring(0, text.Length - 1) + ")";
            }
        }

        public List<IToken> Tokens { get; }

        public void Add(IToken token)
        {
            Tokens.Add(token);
        }

        public override TTo Convert<TTo>()
        {
            if (Tokens.Count == 1)
                return Tokens[0].Convert<TTo>();
            throw new Exception("Could not convert ListToken");
        }

        public override IToken Evaluate(TokenTreeList parameters, bool isFinal)
        {
            ListToken list = new ListToken();
            foreach (IToken token in Tokens)
                list.Tokens.Add(token.Evaluate(parameters, isFinal));
            return list;
        }

        public override IToken SubstituteParameters(TokenTree parameters)
        {
            ListToken list = new ListToken();
            foreach (IToken token in Tokens)
                list.Tokens.Add(token.SubstituteParameters(parameters));
            return list;
        }
    }
}
