namespace Generator
{
    public interface IValue
    {
        /// <summary>
        /// Integer value of property.
        /// </summary>
        int IntValue { get; }

        /// <summary>
        /// Does the value represent an expression.
        /// </summary>
        bool IsExpression { get; }

        /// <summary>
        /// Whether the property is an integer.
        /// </summary>
        bool IsInt { get; }

        /// <summary>
        /// Does the value represent a simple substitution expression.
        /// </summary>
        bool IsVariableExpression { get; }

        /// <summary>
        /// String value of property.
        /// </summary>
        string StringValue { get; }

        /// <summary>
        /// StringValue including quotes if this is a verbatim string.
        /// </summary>
        string QualifiedStringValue { get; }

        /// <summary>
        /// The variable name if IsVariableExpression is true.
        /// </summary>
        string VariableName { get; }

        /// <summary>
        /// Applies the properties that act as parameters.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The value created by applying the parameters.</returns>
        IValue ApplyParameters(IProperty parameters);

        /// <summary>
        /// Evaluates a value using information from the element.
        /// </summary>
        /// <param name="element">The element that owns the parameter.</param>
        /// <param name="isFinal">Is this the final time the value will be evaluated.</param>
        /// <returns>The evaluated value.</returns>
        IValue Evaluate(IElement element, bool isFinal);

        /// <summary>
        /// Creates a property from the value.
        /// </summary>
        /// <param name="name">The property name.</param>
        /// <returns>The new property.</returns>
        IProperty CreateProperty(string name);
    }
}
