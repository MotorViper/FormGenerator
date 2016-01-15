namespace Generator
{
    public interface IValue
    {
        int IntValue { get; }
        bool IsInt { get; }
        string StringValue { get; }
    }
}
