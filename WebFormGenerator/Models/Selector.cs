namespace WebFormGenerator.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global - used by IOC.
    /// <summary>
    /// Class representing the main element for selecting the displayed item.
    /// </summary>
    public class Selector : Select
    {
        /// <summary>
        /// Adds properties to the list of those to output.
        /// </summary>
        protected override void AddProperties()
        {
            foreach (string child in Keys)
                Options.Add(child);
            SelectedItem = Element.Parameters[1].Name;
            base.AddProperties();
        }
    }
}
