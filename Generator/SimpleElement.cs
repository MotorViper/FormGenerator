using System.Collections.Generic;
using TextParser;

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

        /// <summary>
        /// The elements children.
        /// </summary>
        public IEnumerable<IElement> Children { get; }

        /// <summary>
        /// The elements type.
        /// </summary>
        public string ElementType { get; set; }

        /// <summary>
        /// The elements properties.
        /// </summary>
        public IPropertyList Properties => RWProperties;

        public TokenTree Data => null;
        public TokenTreeList Parameters { get; set; }
    }
}
