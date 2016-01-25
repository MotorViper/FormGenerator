using System.Collections.Generic;

namespace Generator
{
    /// <summary>
    /// Interface representing an element to output.
    /// </summary>
    public interface IElement
    {
        /// <summary>
        /// The elements children.
        /// </summary>
        IEnumerable<IElement> Children { get; }

        /// <summary>
        /// The elements name.
        /// </summary>
        string ElementName { get; }

        /// <summary>
        /// The elements type.
        /// </summary>
        string ElementType { get; set; }

        /// <summary>
        /// Parameters used for any calculations.
        /// </summary>
        IParameters Parameters { get; set; }

        /// <summary>
        /// The elements properties.
        /// </summary>
        IPropertyList Properties { get; }
    }
}
