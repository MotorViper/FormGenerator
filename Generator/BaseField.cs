﻿using Helpers;
using System.Collections.Generic;
using System.Linq;

namespace Generator
{
    /// <summary>
    /// Base class for generated fields.
    /// </summary>
    public abstract class BaseField : IField
    {
        private readonly Dictionary<string, string> _properties = new Dictionary<string, string>();

        /// <summary>
        /// Object used to output fields.
        /// </summary>
        protected IFieldWriter Builder { get; private set; }

        /// <summary>
        /// Level of indentation.
        /// </summary>
        public virtual int Level { protected get; set; }

        /// <summary>
        /// Field name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Fields parent.
        /// </summary>
        public IField Parent { protected get; set; }

        /// <summary>
        /// The fields data, i.e. children and properties.
        /// </summary>
        public virtual IElement Element { set; get; }

        /// <summary>
        /// Outputs the field to the writer.
        /// </summary>
        /// <param name="level">The indentation level.</param>
        public void OutputField(int level)
        {
            Builder = IOCContainer.Instance.Resolve<IFieldWriter>();
            Level = level;
            AddStart();
            AddChildren();
            AddEnd();
        }

        /// <summary>
        /// Adds any child properties that are linked to the field.
        /// </summary>
        /// <param name="child">The child whose properties are being added.</param>
        public virtual void AddChildProperties(IField child)
        {
        }

        /// <summary>
        /// Adds a property to the output.
        /// </summary>
        /// <typeparam name="T">The property type.</typeparam>
        /// <param name="name">The property name.</param>
        /// <param name="value">The property value.</param>
        public virtual void AddTypedProperty<T>(string name, T value)
        {
            switch (name)
            {
                case "Debug":
                    break;
                default:
                    if (_properties.ContainsKey(name))
                        _properties[name] += "," + value;
                    else
                        _properties[name] = value.ToString();
                    break;
            }
        }

        /// <summary>
        /// Add an element to the output.
        /// </summary>
        /// <param name="data">The data making up the element.</param>
        /// <param name="level">The indentation level.</param>
        /// <param name="parent">The elements parent.</param>
        /// <param name="keys">List of available elements.</param>
        public abstract void AddElement(IElement data, int level, IField parent = null, List<string> keys = null);

        /// <summary>
        /// Outputs the start text of a field.
        /// </summary>
        protected virtual void AddStart()
        {
            AppendStartOfLine(Level, "<").Append(Name).Append(" ");
            AddProperties();
            OutputProperties(_properties);
            Builder.Append(">").AppendLine();
        }

        /// <summary>
        /// Outputs the fields properties.
        /// </summary>
        /// <param name="properties">The properties to output.</param>
        protected abstract void OutputProperties(Dictionary<string, string> properties);

        /// <summary>
        /// Adds properties to the list of those to output.
        /// </summary>
        protected virtual void AddProperties()
        {
            foreach (IProperty child in Element.Properties.Where(child => !IgnoredProperties().Contains(child.Name)))
                AddProperty(child.Name, child);
            Parent?.AddChildProperties(this);
        }

        /// <summary>
        /// Add a single property to the list of those to output.
        /// </summary>
        /// <param name="name">The property name.</param>
        /// <param name="value">The value to add.</param>
        protected virtual void AddProperty(string name, IValue value)
        {
            AddTypedProperty(name, ProcessValue(value));
        }

        /// <summary>
        /// Evaluates a tokens.
        /// </summary>
        /// <param name="value">The token to evaluate.</param>
        /// <returns>The string representing the token in the output.</returns>
        protected abstract string ProcessValue(IValue value);

        /// <summary>
        /// Any properties that should not be processed.
        /// </summary>
        /// <returns>The list of properties to ignore.</returns>
        protected virtual List<string> IgnoredProperties()
        {
            return new List<string> { "Field", "Across", "Over", "Columns", "Rows", "Header" };
        }

        /// <summary>
        /// Outputs the start of a line.
        /// </summary>
        /// <param name="start">The initial text.</param>
        /// <returns>The output object.</returns>
        protected IFieldWriter AppendStartOfLine(string start)
        {
            Builder.AppendStart(Level);
            return Builder.Append(start);
        }

        /// <summary>
        /// Outputs the start of a line.
        /// </summary>
        /// <param name="level">The indentation level.</param>
        /// <param name="start">The initial text.</param>
        /// <returns>The output object.</returns>
        protected IFieldWriter AppendStartOfLine(int level, string start)
        {
            Builder.AppendStart(level);
            return Builder.Append(start);
        }

        /// <summary>
        /// Adds the fields children.
        /// </summary>
        protected abstract void AddChildren();

        /// <summary>
        /// Adds the end of a field.
        /// </summary>
        protected virtual void AddEnd()
        {
            AppendStartOfLine(Level, "</").Append(Name).Append(">").AppendLine();
        }
    }
}
