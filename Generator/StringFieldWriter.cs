using System.Collections.Generic;
using System.Text;
using TextParser;

namespace Generator
{
    /// <summary>
    /// Class that outputs the text generated from the token data.
    /// </summary>
    /// <typeparam name="T">The type of the basic fields that are created.</typeparam>
    public class StringFieldWriter<T> : IFieldWriter where T : IField, new()
    {
        private readonly string _offset;
        private readonly StringBuilder _sb = new StringBuilder();

        // ReSharper disable once UnusedMember.Global - used by IOC.
        /// <summary>
        /// Default constructor.
        /// </summary>
        public StringFieldWriter() : this("  ")
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="offset">The text offset for indentation.</param>
        public StringFieldWriter(string offset)
        {
            _offset = offset;
        }

        /// <summary>
        /// Outputs the start of a new line.
        /// </summary>
        /// <returns></returns>
        public IFieldWriter AppendStart(int level)
        {
            for (int i = 0; i < level; i++)
                _sb.Append(_offset);
            return this;
        }

        /// <summary>
        /// Appends text to the contents.
        /// </summary>
        /// <typeparam name="TValue">The type of the object to append.</typeparam>
        /// <param name="s">The object to append.</param>
        /// <returns>The field writer, to allow chaining.</returns>
        public IFieldWriter Append<TValue>(TValue s)
        {
            _sb.Append(s);
            return this;
        }

        /// <summary>
        /// Appends a new line character.
        /// </summary>
        public void AppendLine()
        {
            _sb.AppendLine();
        }

        /// <summary>
        /// Add an element to the output.
        /// </summary>
        /// <param name="data">The data making up the element.</param>
        /// <param name="level">The indentation level.</param>
        /// <param name="parameters">Calculation parameters.</param>
        /// <param name="selected">The selected output element.</param>
        /// <param name="keys">List of available elements.</param>
        public void AddElement(TokenTree data, int level, TokenTree parameters, TokenTree selected = null, List<string> keys = null)
        {
            TokenTreeList list = new TokenTreeList(parameters);
            if (selected != null)
                list.Add(selected);
            TokenTreeElement tokenTreeElement = new TokenTreeElement(data, list);
            new T().AddElement(tokenTreeElement, level, null, keys);
        }

        /// <summary>
        /// The generated text.
        /// </summary>
        public string Generated => _sb.ToString();
    }
}
