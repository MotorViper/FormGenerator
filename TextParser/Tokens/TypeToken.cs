using System;
using TextParser.Tokens.Interfaces;

namespace TextParser.Tokens
{
    public abstract class TypeToken<T> : BaseToken, ITypeToken, IConvertibleToken
    {
        protected TypeToken(T value)
        {
            Value = value;
        }

        public T Value { get; }

        public object Data => Value;

        /// <summary>
        /// Converts the token to a boolean.
        /// </summary>
        public override bool ToBool()
        {
            throw new Exception($"Could not convert {typeof(T).Name} to Boolean");
        }

        /// <summary>
        /// Converts the token to an integer.
        /// </summary>
        public override int ToInt()
        {
            throw new Exception($"Could not convert {typeof(T).Name} to Integer");
        }

        /// <summary>
        /// Converts the token to a double.
        /// </summary>
        public override double ToDouble()
        {
            throw new Exception($"Could not convert {typeof(T).Name} to Double");
        }

        /// <summary>
        /// Converts the token to a string.
        /// </summary>
        public override string ToString() => Value.ToString();

        public virtual IToken ConvertToInt(TokenTreeList substitutions, bool isFinal)
        {
            return new IntToken(ToInt());
        }

        public virtual IToken ConvertToDouble(TokenTreeList substitutions, bool isFinal)
        {
            return new DoubleToken(ToDouble());
        }

        //public override IToken ValueToken => this;
    }
}
