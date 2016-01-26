using System.Collections.Generic;
using TextParser;

namespace Generator
{
    /// <summary>
    /// Interface for classes that can write out field data.
    /// </summary>
    public interface IFieldWriter
    {
        /// <summary>
        /// The generated text.
        /// </summary>
        string Generated { get; }

        /// <summary>
        /// Outputs the start of a new line.
        /// </summary>
        /// <returns></returns>
        IFieldWriter AppendStart(int level);

        /// <summary>
        /// Appends text to the contents.
        /// </summary>
        /// <typeparam name="TValue">The type of the object to append.</typeparam>
        /// <param name="s">The object to append.</param>
        /// <returns>The field writer, to allow chaining.</returns>
        IFieldWriter Append<TValue>(TValue s);

        /// <summary>
        /// Appends a new line character.
        /// </summary>
        void AppendLine();

        /// <summary>
        /// Add an element to the output.
        /// </summary>
        /// <param name="data">The data making up the element.</param>
        /// <param name="level">The indentation level.</param>
        /// <param name="parameters">Calculation parameters.</param>
        /// <param name="selected">The selected output element.</param>
        /// <param name="keys">List of available elements.</param>
        void AddElement(TokenTree data, int level, TokenTree parameters, TokenTree selected = null, List<string> keys = null);

        /// <summary>
        /// Clear the output.
        /// </summary>
        void Clear();
    }
}
