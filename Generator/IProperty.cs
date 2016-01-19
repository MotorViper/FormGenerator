namespace Generator
{
    /// <summary>
    /// Interface representing a property.
    /// </summary>
    public interface IProperty : IValue
    {
        /// <summary>
        /// The property name.
        /// </summary>
        string Name { get; }
    }
}
