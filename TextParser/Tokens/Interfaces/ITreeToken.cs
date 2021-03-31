namespace TextParser.Tokens.Interfaces
{
    public interface ITreeToken : IToken
    {
        void AddChild(PairToken child);

        void ReplaceChild(PairToken child);

        ITreeToken Clone();
    }
}
