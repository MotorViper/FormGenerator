using System;

namespace TextParser.Tokens
{
    public abstract class TypeToken<T> : BaseToken, ITypeToken
    {
        protected TypeToken(T value, TokenType type)
        {
            Value = value;
            Type = type;
        }

        public T Value { get; }

        public TokenType Type { get; }
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
    }
}
