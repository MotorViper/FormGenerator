namespace Generator
{
    public interface IParameters
    {
        /// <summary>
        /// The currently selected item being displayed.
        /// </summary>
        string SelectedItem { get; }

        /// <summary>
        /// Gets a list of fields from the parameters.
        /// </summary>
        /// <param name="name">The name of the list.</param>
        /// <returns>The entries in the list.</returns>
        IPropertyList GetList(string name);

        /// <summary>
        /// Add a new property to the parameters.
        /// </summary>
        /// <param name="property">The property to add.</param>
        /// <returns>The parameters with the new property added.</returns>
        IParameters Add(IProperty property);
    }
}
