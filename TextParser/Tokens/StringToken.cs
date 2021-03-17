using Helpers;
using TextParser.Tokens.Interfaces;

namespace TextParser.Tokens
{
    public class StringToken : TypeToken<string>, IReversibleToken, ITokenWithLength
    {
        public StringToken(string text) : base(text)
        {
        }

        /// <summary>
        /// Converts the token to a boolean.
        /// </summary>
        public override bool ToBool()
        {
            if (!string.IsNullOrEmpty(ToString()))
            {
                if (bool.TryParse(ToString(), out bool value))
                    return value;
                if (double.TryParse(ToString(), out double doubleValue))
                    return new DoubleToken(doubleValue).ToBool();
            }
            return false;
        }

        /// <summary>
        /// Converts the token to an integer.
        /// </summary>
        public override int ToInt()
        {
            int value = 0;
            if (!string.IsNullOrEmpty(ToString()))
                value = int.TryParse(ToString(), out value) ? value : 0;
            return value;
        }

        /// <summary>
        /// Converts the token to a double.
        /// </summary>
        public override double ToDouble()
        {
            double value = 0;
            if (!string.IsNullOrEmpty(ToString()))
                value = double.TryParse(ToString(), out value) ? value : 0;
            return value;
        }

        public IntToken Count()
        {
            return new IntToken(Value.Length);
        }

        public virtual IToken Reverse()
        {
            return new StringToken(Value.Reverse());
        }
    }
}
