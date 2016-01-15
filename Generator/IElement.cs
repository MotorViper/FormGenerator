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
        /// The elements type.
        /// </summary>
        string ElementType { get; set; }

        /// <summary>
        /// The elements properties.
        /// </summary>
        IPropertyList Properties { get; }

        #region Temporary properties

        TokenTree Data { get; }
        TokenTreeList Parameters { get; set; }

        #endregion
    }
}
