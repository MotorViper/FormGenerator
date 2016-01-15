using System.Collections.Generic;

namespace Generator
{
    public interface IPropertyList
    {
        IProperty Find(string name);
        IList<IProperty> FindAll(string name);
        IElement FindChild(string name);
        int Count { get; }
    }
}
