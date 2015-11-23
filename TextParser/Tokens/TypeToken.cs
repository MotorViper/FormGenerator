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
        public override string Text => Value.ToString();
        public TokenType Type { get; }
        public object Data => Value;

        public override TTo Convert<TTo>()
        {
            TypeToken<TTo> converted = this as TypeToken<TTo>;
            if (converted != null)
                return converted.Value;
            throw new Exception($"Could not convert {typeof(T).Name} to {typeof(TTo).Name}");
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
