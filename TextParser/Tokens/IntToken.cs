using TextParser.Tokens.Interfaces;

namespace TextParser.Tokens
{
    public class IntToken : TypeToken<int>
    {
        public IntToken(int value) : base(value)
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

        public static implicit operator IntToken(int i) => new IntToken(i);

        public override IToken ConvertToInt(TokenTreeList substitutions, bool isFinal)
        {
            return this;
        }

        //public override int CompareTo(IToken toCompare)
        //{
        //    if (toCompare is DoubleToken dToken)
        //        return -dToken.Value.CompareTo(Value);
        //    if (toCompare is IntToken iToken)
        //        return Value.CompareTo(iToken.Value);
        //    return base.CompareTo(toCompare);
        //}
    }
}
