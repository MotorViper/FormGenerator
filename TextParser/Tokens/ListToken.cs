using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TextParser.Tokens
{
    public class ListToken : BaseToken, IEnumerable<IToken>
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

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<IToken> GetEnumerator()
        {
            return Tokens.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

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

        /// <summary>
        /// Whether the token contains the input text.
        /// </summary>
        /// <param name="text">The input text.</param>
        /// <returns>True if the current token contains the input text.</returns>
        public override bool Contains(string text)
        {
            return Tokens.Any(token => token.Contains(text));
        }
    }
}
