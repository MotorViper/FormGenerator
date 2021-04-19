using System;
using TextParser.Functions;
using TextParser.Tokens.Interfaces;

namespace TextParser.Tokens
{
    /// <summary>
    /// Base class for tokens.
    /// </summary>
    public abstract class BaseToken : IToken
    {
        //public ITreeToken Parent { get; set; }

        /// <summary>
        /// Converts the token to a boolean.
        /// </summary>
        public virtual bool ToBool()
        {
            throw new Exception($"Cannot convert {GetType().Name} token");
        }

        /// <summary>
        /// Converts the token to an integer.
        /// </summary>
        public virtual int ToInt()
        {
            throw new Exception($"Cannot convert {GetType().Name} token");
        }

        /// <summary>
        /// Converts the token to a double.
        /// </summary>
        public virtual double ToDouble()
        {
            throw new Exception($"Cannot convert {GetType().Name} token");
        }

        /// <summary>
        /// Checks if the token is an expression.
        /// </summary>
        public virtual bool IsExpression => false;

        /// <summary>
        /// Whether the contained value should be treated as a single element.
        /// </summary>
        public virtual bool Verbatim => false;

        public virtual void ModifyParameters(UserFunction function)
        {
        }

        public IToken Simplify()
        {
            return Process(null, false);
        }

        /// <summary>
        /// Converts the token to a list of tokens if possible and required.
        /// </summary>
        /// <returns>The list of tokens or the original token.</returns>
        public virtual IToken Flatten()
        {
            return this;
        }

        /// <summary>
        /// Evaluates the token.
        /// </summary>
        /// <param name="parameters">The parameters to use for substitutions.</param>
        /// <param name="isFinal">Whether this is a final parse.</param>
        /// <returns></returns>
        public IToken Evaluate(TokenTreeList parameters, bool isFinal)
        {
            return Process(parameters, isFinal);
        }

        protected virtual IToken Process(TokenTreeList parameters, bool isFinal)
        {
            return this;
        }

        public virtual IToken SubstituteParameters(TokenTree parameters)
        {
            return this;
        }

        /// <summary>
        /// Whether the token contains the input token.
        /// </summary>
        /// <param name="token">The input token.</param>
        /// <returns>True if the current token contains the input token.</returns>
        public virtual bool HasMatch(IToken token)
        {
            return ToString() == token.ToString();
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
            return token.ToString() == ToString();
        }

        /// <summary>
        /// Serves as the default hash function. 
        /// </summary>
        /// <returns>
        /// A hash code for the current object.
        /// </returns>
        public override int GetHashCode()
        {
            return (ToString() + GetType().Name).GetHashCode();
        }

        //public virtual IToken FindToken(string text, bool checkChildren, bool checkParent)
        //{
        //    return checkParent ? Parent.FindToken(text, checkChildren, checkParent) : null;
        //}

        //public abstract IToken ValueToken { get; }

        //public virtual int CompareTo(IToken toCompare)
        //{
        //    return ToString().CompareTo(toCompare.ToString());
        //}

        //public bool Equals(IToken other)
        //{
        //    if (other == null)
        //        return false;
        //    if (other.GetType() != GetType())
        //        return false;
        //    return other.ToString() == ToString();
        //}

        //public virtual bool ComparisonIsInteger => true;
    }
}
