using System.Collections.Generic;

namespace Generator
{
    /// <summary>
    /// Interface representing a field to output.
    /// </summary>
    public interface IField
    {
        /// <summary>
        /// The fields data, i.e. children and properties.
        /// </summary>
        IElement Element { get; set; }

        /// <summary>
        /// Level of indentation.
        /// </summary>
        int Level { set; }

        /// <summary>
        /// Field name.
        /// </summary>
        string Name { set; }

        /// <summary>
        /// Fields parent.
        /// </summary>
        IField Parent { set; }

        /// <summary>
        /// Outputs the field to the writer.
        /// </summary>
        /// <param name="level">The indentation level.</param>
        void OutputField(int level);

        /// <summary>
        /// Adds any child properties that are linked to the field.
        /// </summary>
        /// <param name="child">The child whose properties are being added.</param>
        void AddChildProperties(IField child);

        /// <summary>
        /// Adds a property to the output.
        /// </summary>
        /// <typeparam name="T">The property type.</typeparam>
        /// <param name="name">The property name.</param>
        /// <param name="value">The property value.</param>
        void AddProperty<T>(string name, T value);

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
