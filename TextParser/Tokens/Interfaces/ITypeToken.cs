namespace TextParser.Tokens.Interfaces
{
    public interface ITypeToken : IToken //: IValueToken
    {
        object Data { get; }
    }
}
