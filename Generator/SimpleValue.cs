namespace Generator
{
    /// <summary>
    /// Simple implementation of the IValue interface holding a string.
    /// </summary>
    public class SimpleValue : IValue
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="value">String holding the value.</param>
        public SimpleValue(string value)
        {
            StringValue = value;
        }

        /// <summary>
        /// Integer value of property.
        /// </summary>
        public int IntValue => 0;

        /// <summary>
        /// Whether the property is an integer.
        /// </summary>
        public bool IsInt => false;

        /// <summary>
        /// String value of property.
        /// </summary>
        public string StringValue { get; }

        /// <summary>
        /// StringValue including quotes if this is a verbatim string.
        /// </summary>
        public string QualifiedStringValue { get; }

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
        public IValue Evaluate(IElement element, bool isFinal)
        {
            return null;
        }
    }
}
