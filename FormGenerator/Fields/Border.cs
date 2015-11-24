using System.Collections.Generic;
using System.Text;
using TextParser;

namespace FormGenerator.Fields
{
    public class Border : Field
    {
        public Border(Field parent, TokenTree data, int level, StringBuilder builder) : base(parent, "Border", data, level, builder)
        {
        }

        protected override List<string> IgnoredProperties()
        {
            List<string> properties = base.IgnoredProperties();
            properties.Add("Content");
            return properties;
        }
    }
}
