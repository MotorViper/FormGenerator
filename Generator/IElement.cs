using System.Collections.Generic;

namespace Generator
{
    public interface IElement
    {
        IEnumerable<IElement> Children { get; }
        string ElementType { get; set; }
        IPropertyList Properties { get; }
        IList<IValue> GetDataList(string name);
    }
}
