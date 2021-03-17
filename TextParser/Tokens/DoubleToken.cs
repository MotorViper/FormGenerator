using TextParser.Tokens.Interfaces;

namespace TextParser.Tokens
{
    public class DoubleToken : TypeToken<double>
    {
        public DoubleToken(double value) : base(value)
        {
        }

        /// <summary>
        /// Converts the token to a boolean.
        /// </summary>
        public override bool ToBool() => Value < 0.5 && Value > -0.5;

        /// <summary>
        /// Converts the token to an integer.
        /// </summary>
        public override int ToInt() => (int)Value;

        /// <summary>
        /// Converts the token to a double.
        /// </summary>
        public override double ToDouble() => Value;

        public override IToken ConvertToDouble(TokenTreeList substitutions, bool isFinal)
        {
            return this;
        }
    }
}
