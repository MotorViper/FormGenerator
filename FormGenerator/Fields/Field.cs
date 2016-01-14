using System.Collections.Generic;
using FormGenerator.Tools;
using Generator;
using Helpers;
using TextParser;
using TextParser.Operators;
using TextParser.Tokens;

namespace FormGenerator.Fields
{
    public class Field : BaseField
    {
        // ReSharper disable once MemberCanBePrivate.Global - used by IOC.
        /// <summary>
        /// Default constructor.
        /// </summary>
        public Field()
        {
            Parameter = null;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The field name.</param>
        protected Field(string name) : this()
        {
            Name = name;
        }

        /// <summary>
        /// Parameters used for any necessary evaluation.
        /// </summary>
        protected IToken Parameter { get; set; }

        /// <summary>
        /// Adds the fields children.
        /// </summary>
        /// <param name="parameters">Calculation parameters.</param>
        protected override void AddChildren(TokenTree parameters)
        {
            IEnumerable<TokenTree> fields = GetSubFields();
            foreach (TokenTree child in fields)
                AddElement(child, Level + 1, parameters, this);
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
        public override void AddElement(TokenTree data, int level, TokenTree parameters, IField parent = null, TokenTree selected = null,
            List<string> keys = null)
        {
            Field field = (Field)FieldFactory.CreateField(data.Value.Text, data, level, parameters, parent);
            field.Parameter = Parameter;
            field.OutputField(level, parameters);
        }

        /// <summary>
        /// Evaluates a tokens.
        /// </summary>
        /// <param name="value">The token to evaluate.</param>
        /// <param name="parameters">Calculation parameters.</param>
        /// <returns>The string representing the token in the output.</returns>
        protected override string ProcessTokens(IToken value, TokenTreeList parameters)
        {
            IToken evaluated = value.Evaluate(parameters, false);
            ExpressionToken expression = evaluated as ExpressionToken;

            // Simple string so can just output it.
            if (expression == null)
                return evaluated.Text;

            // A simple expression with no table parameter so can use the [] operator.
            if (Parameter == null && (expression.Operator is SubstitutionOperator && expression.Second is StringToken))
                return "{Binding Values[" + ((StringToken)expression.Second).Text + "]}";

            // This uses a table parameter or is a complicated expression so need to use the converter.
            int id = DataConverter.SetFieldData(evaluated, Parameter);
            return "{Binding Values, Converter={StaticResource DataConverter}, Mode=OneWay, ConverterParameter=" + id + "}";
        }

        /// <summary>
        /// Outputs the fields properties.
        /// </summary>
        /// <param name="properties">The properties to output.</param>
        protected override void OutputProperties(Dictionary<string, string> properties)
        {
            foreach (var property in properties)
                Builder.Append(property.Key.ToCamelCase()).Append("=\"").Append(property.Value).Append("\" ");
        }

        /// <summary>
        /// Adds a property to the output.
        /// </summary>
        /// <typeparam name="T">The property type.</typeparam>
        /// <param name="name">The property name.</param>
        /// <param name="value">The property value.</param>
        public override void AddProperty<T>(string name, T value)
        {
            switch (name)
            {
                case "Style":
                    base.AddProperty("Style", "{StaticResource " + value + "}");
                    break;
                default:
                    base.AddProperty(name, value);
                    break;
            }
        }
    }
}
