namespace TextParser.Tokens
{
    public class StringToken : TypeToken<string>
    {
        public StringToken(string text) : base(text, TokenType.StringToken)
        {
        }

        public override TTo Convert<TTo>()
        {
            if (typeof(TTo) == typeof(double))
            {
                double value;
                return (TTo)(object)(double.TryParse(Text, out value) ? value : double.NaN);
            }

            if (typeof(TTo) == typeof(int))
            {
                int value;
                return (TTo)(object)(int.TryParse(Text, out value) ? value : int.MinValue);
            }

            return base.Convert<TTo>();
        }
    }
}
