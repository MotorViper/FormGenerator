namespace TextParser.Tokens
{
    public class IntToken : TypeToken<int>
    {
        public IntToken(int value) : base(value, TokenType.IntToken)
        {
        }

        public override TTo Convert<TTo>()
        {
            return typeof(TTo) == typeof(double) ? (TTo)(object)(double)(Value) : base.Convert<TTo>();
        }
    }
}
