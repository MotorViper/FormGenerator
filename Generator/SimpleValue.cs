namespace Generator
{
    public class SimpleValue : IValue
    {
        public SimpleValue(string value)
        {
            StringValue = value;
            IsInt = false;
            IntValue = 0;
        }

        public SimpleValue(int value)
        {
            StringValue = value.ToString();
            IsInt = true;
            IntValue = value;
        }

        public int IntValue { get; }
        public bool IsInt { get; }
        public string StringValue { get; }
    }
}
