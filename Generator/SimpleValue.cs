namespace Generator
{
    public class SimpleValue : IValue
    {
        public SimpleValue(int value) : this(value.ToString(), value, true)
        {
        }

        public SimpleValue(string value) : this(value, 0, false)
        {
        }

        private SimpleValue(string s, int i, bool b)
        {
            StringValue = s;
            IsInt = b;
            IntValue = i;
        }

        /// <summary>
        /// Integer value of property.
        /// </summary>
        public int IntValue { get; }

        /// <summary>
        /// Whether the property is an integer.
        /// </summary>
        public bool IsInt { get; }

        /// <summary>
        /// String value of property.
        /// </summary>
        public string StringValue { get; }

        /// <summary>
        /// Does the value represent an expression.
        /// </summary>
        public bool IsExpression => false;

        /// <summary>
        /// Does the value represent a simple substitution expression.
        /// </summary>
        public bool IsVariableExpression => false;

        /// <summary>
        /// The variable name if IsVariableExpression is true.
        /// </summary>
        public string VariableName => null;

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
        /// Creates a property from the value.
        /// </summary>
        /// <param name="name">The property name.</param>
        /// <returns>The new property.</returns>
        public IProperty CreateProperty(string name)
        {
            return new SimpleProperty(name, this);
        }

        /// <summary>
        /// Evaluates a value using information from the element.
        /// </summary>
        /// <param name="element">The element that owns the parameter.</param>
        /// <param name="isFinal">Is this the final time the value will be evaluated.</param>
        /// <returns>The evaluated value.</returns>
        public IValue Evaluate(IElement parameters, bool isFinal)
        {
            return null;
        }
    }
}
