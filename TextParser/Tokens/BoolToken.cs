namespace TextParser.Tokens
{
    public class BoolToken : TypeToken<bool>
    {
        public BoolToken(bool value) : base(value)
        {
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return Value ? "True" : "False";
        }

        /// <summary>
        /// Converts the token to a boolean.
        /// </summary>
        public override bool ToBool()
        {
            return Value;
        }

        /// <summary>
        /// Converts the token to an integer.
        /// </summary>
        public override int ToInt()
        {
            return Value ? 1 : 0;
        }

        /// <summary>
        /// Converts the token to a double.
        /// </summary>
        public override double ToDouble()
        {
            return Value ? 1 : 0;
        }
    }
}
