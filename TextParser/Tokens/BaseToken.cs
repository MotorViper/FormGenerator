namespace TextParser.Tokens
{
    /// <summary>
    /// Base class for tokens.
    /// </summary>
    public abstract class BaseToken : IToken
    {
        public abstract string Text { get; }
        public abstract TTo Convert<TTo>();

        public IToken Simplify()
        {
            return Evaluate(null, false);
        }

        /// <summary>
        /// Converts the token to a list of tokens if possible and required.
        /// </summary>
        /// <returns>The list of tokens or the original token.</returns>
        public virtual IToken EvaluateList()
        {
            return this;
        }

        /// <summary>
        /// Evaluates the token.
        /// </summary>
        /// <param name="parameters">The parameters to use for substitutions.</param>
        /// <param name="isFinal">Whether this is a final parse.</param>
        /// <returns></returns>
        public virtual IToken Evaluate(TokenTreeList parameters, bool isFinal)
        {
            return this;
        }

        public virtual IToken SubstituteParameters(TokenTree parameters)
        {
            return this;
        }

        /// <summary>
        /// Whether the token contains the input text.
        /// </summary>
        /// <param name="text">The input text.</param>
        /// <returns>True if the current token contains the input text.</returns>
        public virtual bool Contains(string text)
        {
            return Text == text;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return Text;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <returns>
        /// true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj.GetType() != GetType())
                return false;
            IToken token = (IToken)obj;
            return token.Text == Text;
        }

        /// <summary>
        /// Serves as the default hash function. 
        /// </summary>
        /// <returns>
        /// A hash code for the current object.
        /// </returns>
        public override int GetHashCode()
        {
            return (Text + GetType().Name).GetHashCode();
        }
    }
}
