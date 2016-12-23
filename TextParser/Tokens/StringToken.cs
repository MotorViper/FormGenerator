namespace TextParser.Tokens
{
    public class StringToken : TypeToken<string>
    {
        public StringToken(string text) : base(text, TokenType.StringToken)
        {
        }

        /// <summary>
        /// Converts the token to a boolean.
        /// </summary>
        public override bool ToBool()
        {
            if (!string.IsNullOrEmpty(ToString()))
            {
                bool value;
                if (bool.TryParse(ToString(), out value))
                    return value;
                double doubleValue;
                if (double.TryParse(ToString(), out doubleValue))
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
    }
}
