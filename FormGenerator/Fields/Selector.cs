namespace FormGenerator.Fields
{
    // ReSharper disable once ClassNeverInstantiated.Global - used by IOC.
    /// <summary>
    /// Class representing the main element for selecting the displayed item.
    /// </summary>
    public class Selector : ComboBox
    {
        /// <summary>
        /// Adds properties to the list of those to output.
        /// </summary>
        protected override void AddProperties()
        {
            base.AddProperties();

            // Add in the properties that allow user interaction.
            AddProperty("SelectedValue", "{Binding Selected}");
            AddProperty("SelectedIndex", 0);
            AddProperty("ItemsSource", "{Binding Keys}");
        }
    }
}
