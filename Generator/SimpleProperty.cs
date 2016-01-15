using System.Collections.Generic;

namespace Generator
{
    public class SimpleProperty : IProperty
    {
        public SimpleProperty(string name, IValue value)
        {
            Name = name;
            Values = new[] {value};
        }

        public SimpleProperty(string name, string value) : this(name, new SimpleValue(value))
        {
        }

        public bool IsList => false;
        public string Name { get; }
        public IValue Value => Values[0];
        public IList<IValue> Values { get; }
    }
}
