using System.Collections.Generic;
using System.Linq;
using Helpers;
using TextParser;
using TextParser.Tokens;

namespace Generator
{
    public abstract class BaseField : IField
    {
        private readonly Dictionary<string, string> _properties = new Dictionary<string, string>();

        /// <summary>
        /// Object used to output fields.
        /// </summary>
        protected IFieldWriter Builder { get; private set; }

        /// <summary>
        /// The fields children.
        /// </summary>
        public TokenTreeList Children => Data.Children;

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
        public virtual TokenTree Data { set; protected get; }

        /// <summary>
        /// Outputs the field to the writer.
        /// </summary>
        /// <param name="level">The indentation level.</param>
        /// <param name="parameters">Parameters used for any calculations.</param>
        public void OutputField(int level, TokenTree parameters)
        {
            Builder = IOCContainer.Instance.Resolve<IFieldWriter>();
            Level = level;
            AddStart(parameters);
            AddChildren(parameters);
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
        /// <param name="parameters">Calculation parameters.</param>
        /// <param name="parent">The elements parent.</param>
        /// <param name="selected">The selected output element.</param>
        /// <param name="keys">List of available elements.</param>
        public abstract void AddElement(TokenTree data, int level, TokenTree parameters, IField parent = null, TokenTree selected = null,
            List<string> keys = null);

        /// <summary>
        /// Outputs the start text of a field.
        /// </summary>
        /// <param name="parameters">Calculation parameters.</param>
        protected virtual void AddStart(TokenTree parameters)
        {
            AppendStartOfLine(Level, "<").Append(Name).Append(" ");
            AddProperties(parameters);
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
        /// <param name="parameters">Calculation parameters.</param>
        protected virtual void AddProperties(TokenTree parameters)
        {
            AddProperties(parameters, null);
        }

        /// <summary>
        /// Adds properties to the list of those to output.
        /// </summary>
        /// <param name="parameters">Calculation parameters.</param>
        /// <param name="selected">The selected item, used for </param>
        protected void AddProperties(TokenTree parameters, TokenTree selected)
        {
            bool parametersRemoved = false;

            foreach (TokenTree child in Children.Where(child => !IgnoredProperties().Contains(child.Name)))
            {
                if (IsParameter(child.Name))
                {
                    // This is nasty but until I add scoping to function parameters it will have to do.
                    if (!parametersRemoved)
                    {
                        parametersRemoved = true;
                        List<string> toRemove = parameters.Children.Select(x => x.Name).Where(IsParameter).ToList();
                        foreach (string name in toRemove)
                            parameters.Remove(name);
                    }
                    parameters.Children.Add(child);
                }
                else
                {
                    TokenTreeList list = new TokenTreeList(parameters);
                    if (selected != null)
                        list.Add(selected);
                    AddProperty(child, list);
                }
            }
            Parent?.AddChildProperties(this);
        }

        /// <summary>
        /// Add a single property to the list of those to output.
        /// </summary>
        /// <param name="child">The token containing the parameter.</param>
        /// <param name="parameters">Calculation parameters.</param>
        protected void AddProperty(TokenTree child, TokenTreeList parameters)
        {
            AddProperty(child.Name, child.Value, parameters);
        }

        /// <summary>
        /// Add a single property to the list of those to output.
        /// </summary>
        /// <param name="name">The property name.</param>
        /// <param name="value"></param>
        /// <param name="parameters">Calculation parameters.</param>
        protected virtual void AddProperty(string name, IToken value, TokenTreeList parameters)
        {
            AddProperty(name, ProcessTokens(value, parameters));
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
        /// <param name="parameters">Calculation parameters.</param>
        protected abstract void AddChildren(TokenTree parameters);

        /// <summary>
        /// Get the fields children.
        /// </summary>
        /// <returns>The fields children.</returns>
        protected IEnumerable<TokenTree> GetSubFields()
        {
            return Children.Where(child => child.Name == "Field");
        }

        /// <summary>
        /// Adds the end of a field.
        /// </summary>
        protected virtual void AddEnd()
        {
            AppendStartOfLine(Level, "</").Append(Name).Append(">").AppendLine();
        }
    }
}
