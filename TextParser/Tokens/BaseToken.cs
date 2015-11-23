namespace TextParser.Tokens
{
    public abstract class BaseToken : IToken
    {
        public abstract string Text { get; }
        public abstract TTo Convert<TTo>();

        public IToken Simplify()
        {
            return Evaluate(null, false);
        }

        public virtual IToken Evaluate(TokenTreeList parameters, bool isFinal)
        {
            return this;
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
