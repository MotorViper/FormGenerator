namespace TextParser.Tokens
{
    public class IntToken : TypeToken<int>
    {
        public IntToken(int value) : base(value, TokenType.IntToken)
        {
        }

        /// <summary>
        /// Converts the token to a boolean.
        /// </summary>
        public override bool ToBool() => Value != 0;

        /// <summary>
        /// Converts the token to an integer.
        /// </summary>
        public override int ToInt() => Value;

        /// <summary>
        /// Converts the token to a double.
        /// </summary>
        public override double ToDouble() => Value;
    }
}
