using System.Collections.Generic;

namespace Generator
{
    public interface IProperty
    {
        bool IsList { get; }
        string Name { get; }
        IValue Value { get; }
        IList<IValue> Values { get; }
    }
}
