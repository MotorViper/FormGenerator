namespace TextParser.Tokens
{
    public abstract class BaseToken : IToken
    {
        public abstract string Text { get; }
        public abstract TTo Convert<TTo>();

        public virtual TokenList Simplify()
        {
            return new TokenList(this);
        }

        public virtual TokenList Evaluate(TokenTreeList parameters)
        {
            return new TokenList(this);
        }
    }
}
