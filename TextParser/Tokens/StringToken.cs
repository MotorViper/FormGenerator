﻿using Helpers;
using TextParser.Tokens.Interfaces;

namespace TextParser.Tokens
{
    public class StringToken : TypeToken<string>, IReversibleToken, ITokenWithLength, IContainerToken//, IKeyToken
    {
        public StringToken(string text, bool verbatim = false) : base(text)
        {
            Verbatim = verbatim;
        }

        /// <summary>
        /// Whether the contained value should be treated as a single element.
        /// </summary>
        public override bool Verbatim { get; }

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

        public static implicit operator StringToken(string s) => new StringToken(s);

        /// <summary>
        /// Converts the token to an integer.
        /// </summary>
        public override int ToInt()
        {
            string sValue = ToString();
            return string.IsNullOrEmpty(sValue) ? 0 : (int.TryParse(sValue, out int value) ? value : 0);
        }

        /// <summary>
        /// Converts the token to a double.
        /// </summary>
        public override double ToDouble()
        {
            string sValue = ToString();
            return string.IsNullOrEmpty(sValue) ? 0 : (double.TryParse(sValue, out double value) ? value : 0);
        }

        public IntToken Count()
        {
            return new IntToken(Value.Length);
        }

        public virtual IToken Reverse()
        {
            return new StringToken(Value.Reverse());
        }

        public bool Contains(IToken token)
        {
            return ToString().Contains(token.ToString());
        }
    }
}
