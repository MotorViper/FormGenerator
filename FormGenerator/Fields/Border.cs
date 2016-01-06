using System.Collections.Generic;

namespace FormGenerator.Fields
{
    public class Border : Field
    {
        public Border() : base("Border")
        {
        }

        protected override List<string> IgnoredProperties()
        {
            List<string> properties = base.IgnoredProperties();
            properties.Add("Content");
            properties.Add("HeaderBorder");
            properties.Add("HeaderStyle");
            return properties;
        }
    }
}
