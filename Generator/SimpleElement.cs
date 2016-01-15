using System.Collections.Generic;

namespace Generator
{
    public class SimpleElement : IElement
    {
        public SimpleElement(string elementType)
        {
            Children = new List<IElement>();
            ElementType = elementType;
            RWProperties = new SimplePropertyList();
        }

        public SimplePropertyList RWProperties { get; }

        public IEnumerable<IElement> Children { get; }
        public string ElementType { get; set; }
        public IPropertyList Properties => RWProperties;
        public IList<IValue> GetDataList(string name)
        {
            return null;
        }
    }
}
