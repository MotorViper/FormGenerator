using System.Collections.Generic;
using System.Linq;

namespace Generator
{
    public class SimplePropertyList : List<IProperty>, IPropertyList
    {
        public IProperty Find(string name)
        {
            return FindAll(name).FirstOrDefault();
        }

        public IList<IProperty> FindAll(string name)
        {
            return this.Where(x => x.Name == name).ToList();
        }

        public IElement FindChild(string name)
        {
            return new SimpleElement(Find(name).Value.StringValue);
        }
    }
}
