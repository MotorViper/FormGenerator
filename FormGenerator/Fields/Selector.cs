using TextParser;

namespace FormGenerator.Fields
{
    // ReSharper disable once ClassNeverInstantiated.Global - used by IOC.
    public class Selector : ComboBox
    {
        protected override void AddProperties(TokenTree parameters)
        {
            base.AddProperties(parameters);
            AddProperty("SelectedValue", "{Binding Selected}");
            AddProperty("SelectedIndex", 0);
            AddProperty("ItemsSource", "{Binding Keys}");
        }
    }
}
