using System.Collections.Generic;
using Generator;
using TextParser.Operators;
using TextParser.Tokens;

namespace WebFormGenerator.Models
{
    /// <summary>
    /// An html field to be displayed.
    /// </summary>
    public class Field : BaseField
    {
        private string _content;

        // ReSharper disable once MemberCanBePrivate.Global - used by IOC.
        /// <summary>
        /// Constructor - only used by IOC.
        /// </summary>
        public Field()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the field to be output.</param>
        protected Field(string name) : this()
        {
            Name = name;
        }

        /// <summary>
        /// List of selectable items.
        /// </summary>
        protected List<string> Keys { get; private set; }

        /// <summary>
        /// Adds any child properties that are linked to the field.
        /// </summary>
        /// <param name="child">The child whose properties are being added.</param>
        public override void AddChildProperties(IField child)
        {
        }

        /// <summary>
        /// Process each token that is to be used.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>The value of the token after evaluation.</returns>
        protected override string ProcessTokens(IToken value)
        {
            try
            {
                IToken converted = value.Evaluate(Element.Parameters, true);
                ITypeToken typeToken = converted as ITypeToken;
                if (typeToken != null)
                    return typeToken.Data.ToString();
                ExpressionToken expression = converted as ExpressionToken;
                if (expression?.Operator is SubstitutionOperator && expression.Second is StringToken)
                    return "&nbsp;";
                return converted.ToString();
            }
            catch
            {
                return value.ToString();
            }
        }

        /// <summary>
        /// Loops through the properties and outputs each one according to it's content.
        /// </summary>
        /// <param name="properties">The properties to loop over.</param>
        protected override void OutputProperties(Dictionary<string, string> properties)
        {
            foreach (var property in properties)
            {
                switch (property.Key)
                {
                    case "Content":
                    case "Text":
                        OutputContent(property.Value);
                        break;
                    case "Style":
                        OutputProperty("class", property.Value);
                        break;
                    default:
                        OutputProperty(property.Key, property.Value);
                        break;
                }
            }
        }

        /// <summary>
        /// Outputs the content of the field.
        /// </summary>
        /// <param name="content">The value to output.</param>
        protected virtual void OutputContent(string content)
        {
            _content = content;
        }

        /// <summary>
        /// Outputs a single property.
        /// </summary>
        /// <param name="key">The property name.</param>
        /// <param name="value">The property value.</param>
        protected virtual void OutputProperty(string key, string value)
        {
            Builder.Append(key.ToLower()).Append("=\"").Append(value).Append("\" ");
        }

        /// <summary>
        /// Outputs the fields children.
        /// </summary>
        protected override void AddChildren()
        {
            Builder.Append(_content).AppendLine();
            foreach (IElement child in Element.Children)
                AddElement(child, Level + 1, this, Keys);
        }

        /// <summary>
        /// Add an element to the output.
        /// </summary>
        /// <param name="data">The data making up the element.</param>
        /// <param name="level">The indentation level.</param>
        /// <param name="parent">The elements parent.</param>
        /// <param name="keys">List of available elements.</param>
        public override void AddElement(IElement data, int level, IField parent = null, List<string> keys = null)
        {
            Field field = (Field)FieldFactory.CreateField(data, level, parent);
            field.Keys = keys;
            field.OutputField(level);
        }
    }
}
