namespace Generator
{
    /// <summary>
    /// Interface representing a property.
    /// </summary>
    public interface IProperty
    {
        /// <summary>
        /// The property name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The property value.
        /// </summary>
        IValue Value { get; }
    }
}
