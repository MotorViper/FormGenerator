using System.Collections.Generic;
using Generator;
using TextParser;
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
            Parameter = null;
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
        /// Adds any child properties that are linked to the field.
        /// </summary>
        /// <param name="child">The child whose properties are being added.</param>
        public override void AddChildProperties(IField child)
        {
        }

        /// <summary>
        /// Process each token that is to be used.
        /// </summary>
        /// <param name="value">The token to process.</param>
        /// <param name="parameters">The data used to evaluate the token.</param>
        /// <returns>The value of the token after evaluation.</returns>
        protected override string ProcessTokens(IToken value, TokenTreeList parameters)
        {
            try
            {
                IToken converted = value.Evaluate(parameters, true);
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
        /// <param name="parameters">The data used for evaluation.</param>
        /// <param name="endOfLine">The end of line character.</param>
        protected override void AddChildren(TokenTree parameters, string endOfLine)
        {
            Builder.Append(_content).AppendLine();
            base.AddChildren(parameters, endOfLine);
        }
    }
}
