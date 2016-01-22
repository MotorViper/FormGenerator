namespace Generator
{
    public class SimpleProperty : IProperty
    {
        private readonly IValue _value;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Property name.</param>
        /// <param name="value">Property value.</param>
        public SimpleProperty(string name, IValue value)
        {
            Name = name;
            _value = value;
        }

        /// <summary>
        /// The property name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Integer value of property.
        /// </summary>
        public int IntValue => _value.IntValue;

        /// <summary>
        /// Whether the property is an integer.
        /// </summary>
        public bool IsInt => _value.IsInt;

        /// <summary>
        /// String value of property.
        /// </summary>
        public string StringValue => _value.StringValue;

        /// <summary>
        /// Creates a property from the value.
        /// </summary>
        /// <param name="name">The property name.</param>
        /// <returns>The new property.</returns>
        public IProperty CreateProperty(string name)
        {
            return _value.CreateProperty(name);
        }

        /// <summary>
        /// Does the value represent an expression.
        /// </summary>
        public bool IsExpression => _value.IsExpression;

        /// <summary>
        /// Does the value represent a simple substitution expression.
        /// </summary>
        public bool IsVariableExpression => _value.IsVariableExpression;

        /// <summary>
        /// The variable name if IsVariableExpression is true.
        /// </summary>
        public string VariableName => _value.VariableName;

        /// <summary>
        /// Applies the properties that act as parameters.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The value created by applying the parameters.</returns>
        public IValue ApplyParameters(IProperty parameters)
        {
            return null;
        }

        /// <summary>
        /// Evaluates a value using information from the element.
        /// </summary>
        /// <param name="element">The element that owns the parameter.</param>
        /// <param name="isFinal">Is this the final time the value will be evaluated.</param>
        /// <returns>The evaluated value.</returns>
        public IValue Evaluate(IElement element, bool isFinal)
        {
            return null;
        }
    }
}
