using System.Collections.Generic;
using System.Linq;

namespace Generator
{
    public class SimplePropertyList : List<IProperty>, IPropertyList
    {
        /// <summary>
        /// Finds a property in the list.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <returns>List of properties matching the name.</returns>
        public IList<IProperty> Find(string name)
        {
            return this.Where(x => x.Name == name).ToList();
        }

        /// <summary>
        /// Finds a property that represents an element.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <returns></returns>
        public IElement FindChild(string name)
        {
            return new SimpleElement(Find(name)[0].Value.StringValue);
        }
    }
}
