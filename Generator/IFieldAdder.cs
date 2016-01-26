using System.Collections.Generic;

namespace Generator
{
    /// <summary>
    /// Interface for class that can add elements to the output.
    /// </summary>
    public interface IFieldAdder
    {
        /// <summary>
        /// Add an element to the output.
        /// </summary>
        /// <param name="data">The data making up the element.</param>
        /// <param name="level">The indentation level.</param>
        /// <param name="parent">The elements parent.</param>
        /// <param name="keys">List of available elements.</param>
        void AddElement(IElement data, int level, IField parent = null, List<string> keys = null);
    }
}
