using System.Collections.Generic;
using System.Linq;
using Helpers;
using TextParser;
using TextParser.Tokens;

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
        public virtual void AddProperty<T>(string name, T value)
        {
            switch (name)
            {
                case "Debug":
                    break;
                default:
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
            bool parametersRemoved = false;

            foreach (TokenTree child in Element.Data.Children.Where(child => !IgnoredProperties().Contains(child.Name)))
            {
                if (IsParameter(child.Name))
                {
                    // This is nasty but until I add scoping to function parameters it will have to do.
                    if (!parametersRemoved)
                    {
                        parametersRemoved = true;
                        List<string> toRemove = Element.Parameters[0].Children.Select(x => x.Name).Where(IsParameter).ToList();
                        foreach (string name in toRemove)
                            Element.Parameters[0].Remove(name);
                    }
                    Element.Parameters[0].Children.Add(child);
                }
                else
                {
                    AddProperty(child);
                }
            }
            Parent?.AddChildProperties(this);
        }

        /// <summary>
        /// Add a single property to the list of those to output.
        /// </summary>
        /// <param name="child">The token containing the parameter.</param>
        protected void AddProperty(TokenTree child)
        {
            AddProperty(child.Name, child.Value);
        }

        /// <summary>
        /// Add a single property to the list of those to output.
        /// </summary>
        /// <param name="name">The property name.</param>
        /// <param name="value">The value to add.</param>
        protected virtual void AddProperty(string name, IToken value)
        {
            AddProperty(name, ProcessTokens(value, Element.Parameters));
        }

        /// <summary>
        /// Evaluates a tokens.
        /// </summary>
        /// <param name="value">The token to evaluate.</param>
        /// <param name="parameters">Calculation parameters.</param>
        /// <returns>The string representing the token in the output.</returns>
        protected abstract string ProcessTokens(IToken value, TokenTreeList parameters);

        /// <summary>
        /// Checks if the property is a function parameter.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <returns>True if this is a function parameter.</returns>
        protected static bool IsParameter(string name)
        {
            if (!name.StartsWith("P"))
                return false;
            int index;
            string sIndex = name.Substring(1);
            return int.TryParse(sIndex, out index) && index.ToString() == sIndex;
        }

        /// <summary>
        /// Any properties that should not be processed.
        /// </summary>
        /// <returns>The list of properties to ignore.</returns>
        protected virtual List<string> IgnoredProperties()
        {
            return new List<string> {"Field", "Across", "Over", "Columns", "Rows", "Header"};
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
