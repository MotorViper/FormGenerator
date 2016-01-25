using System.Collections.Generic;

namespace Generator
{
    public class SimpleElement : IElement
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="elementType">The element type.</param>
        public SimpleElement(string elementType)
        {
            Children = new List<IElement>();
            ElementType = elementType;
            Properties = new SimplePropertyList();
        }

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
        public IPropertyList Properties { get; }

        /// <summary>
        /// The elements name.
        /// </summary>
        public string ElementName { get; set; }

        /// <summary>
        /// Parameters used for any calculations.
        /// </summary>
        public IParameters Parameters { get; set; }
    }
}
