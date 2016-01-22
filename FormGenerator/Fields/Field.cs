using System.Collections.Generic;
using FormGenerator.Tools;
using Generator;
using Helpers;

namespace FormGenerator.Fields
{
    /// <summary>
    /// Class representing any non-specified field.
    /// </summary>
    public class Field : BaseField
    {
        // ReSharper disable once MemberCanBePrivate.Global - used by IOC.
        /// <summary>
        /// Default constructor.
        /// </summary>
        public Field()
        {
            Parameters = null;
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
        protected IProperty Parameters { get; set; }

        /// <summary>
        /// Adds the fields children.
        /// </summary>
        protected override void AddChildren()
        {
            foreach (IElement child in Element.Children)
                AddElement(child, Level + 1, this);
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
            field.Parameters = Parameters;
            field.OutputField(level);
        }

        /// <summary>
        /// Evaluates a tokens.
        /// </summary>
        /// <param name="value">The token to evaluate.</param>
        /// <returns>The string representing the token in the output.</returns>
        protected override string ProcessValue(IValue value)
        {
            IValue evaluated = value.Evaluate(Element, false);

            // Simple string so can just output it.
            if (!evaluated.IsExpression)
                return evaluated.StringValue;

            // A simple expression with no extra parameters so can use the [] operator.
            if (Parameters == null && evaluated.IsVariableExpression)
                return "{Binding Values[" + evaluated.VariableName + "]}";

            // This uses a table parameter or is a complicated expression so need to use the converter.
            int id = DataConverter.SetFieldData(evaluated, Parameters);
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
        public override void AddTypedProperty<T>(string name, T value)
        {
            switch (name)
            {
                case "Style":
                    base.AddTypedProperty("Style", "{StaticResource " + value + "}");
                    break;
                default:
                    base.AddTypedProperty(name, value);
                    break;
            }
        }
    }
}
