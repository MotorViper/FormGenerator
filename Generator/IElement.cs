using System.Collections.Generic;
using TextParser;

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

        #region Temporary properties

        TokenTreeList Parameters { get; set; }

        #endregion

        /// <summary>
        /// The elements properties.
        /// </summary>
        IPropertyList Properties { get; }
    }
}
