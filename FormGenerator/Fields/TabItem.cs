using System.Collections.Generic;

namespace FormGenerator.Fields
{
    /// <summary>
    /// Class used to represent a tab item.
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global - used by IOC.
    public class TabItem : Field
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public TabItem() : base("TabItem")
        {
        }

        /// <summary>
        /// Any properties that should not be processed.
        /// This has been overridden so that the header property will be processed.
        /// </summary>
        /// <returns>The list of properties to ignore.</returns>
        protected override List<string> IgnoredProperties()
        {
            List<string> properties = base.IgnoredProperties();
            properties.Remove("Header");
            return properties;
        }
    }
}
