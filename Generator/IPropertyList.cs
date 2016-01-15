using System.Collections.Generic;

namespace Generator
{
    /// <summary>
    /// Interface representing a list of properties.
    /// </summary>
    public interface IPropertyList : IEnumerable<IProperty>
    {
        /// <summary>
        /// Gets the number of properties.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Finds a property in the list.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <returns>List of properties matching the name.</returns>
        IList<IProperty> Find(string name);

        /// <summary>
        /// Finds a property that represents an element.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <returns></returns>
        IElement FindChild(string name);
    }
}
