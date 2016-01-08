using TextParser;

namespace WebFormGenerator.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global - used by IOC.
    public class Selector : Select
    {
        protected override void AddProperties(TokenTree parameters)
        {
            foreach (string child in Keys)
                Options.Add(child);
            SelectedItem = Selected.Name;
            base.AddProperties(parameters);
        }
    }
}
