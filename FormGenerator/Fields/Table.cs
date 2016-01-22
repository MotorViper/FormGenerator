using System;
using System.Collections.Generic;
using System.Linq;
using FormGenerator.Tools;
using Generator;
using TextParser;
using TextParser.Tokens;

namespace FormGenerator.Fields
{
    // ReSharper disable once ClassNeverInstantiated.Global - used by IOC.
    /// <summary>
    /// Class representing a grid.
    /// </summary>
    public class Table : Grid
    {
        /// <summary>
        /// Any properties that should not be processed.
        /// </summary>
        /// <returns>The list of properties to ignore.</returns>
        protected override List<string> IgnoredProperties()
        {
            List<string> properties = base.IgnoredProperties();
            // Content is handled differently, see below.
            properties.Add("Content");
            return properties;
        }

        /// <summary>
        /// Adds the fields children.
        /// </summary>
        protected override void AddChildren()
        {
            BeginAddChildren();
            IEnumerable<IElement> fields = Element.Children.ToList();

            // Add the headers.
            foreach (IElement child in fields)
            {
                TokenTree header = child.Data.FindFirst("Header");
                if (header != null)
                {
                    if (header.Children.Count == 0)
                    {
                        TokenTree label = new TokenTree {Value = new StringToken("Label")};
                        label.Children.Add(new TokenTree("Content", header.Value.Text ?? ""));
                        header = label;
                    }
                    if (header.Value is NullToken)
                        header.Value = new StringToken("Label");
                    AddElement(new TokenTreeElement(header, Element.Parameters), Level + 1, this);
                }
            }

            // Add a set of children for every item in the content list.
            IList<IProperty> over = Element.Properties.Find("Content");
            if (over == null || over.Count == 0)
                throw new Exception("Tried to create table with no Content.");
            if (over.Count == 1)
            {
                IPropertyList items = DataConverter.GetList(over[0].StringValue);
                foreach (IProperty item in items)
                    AddElements(fields, item);
            }
            else
            {
                foreach (IProperty item in over)
                    AddElements(fields, item);
            }
            EndAddChildren();
        }

        /// <summary>
        /// Adds one child element per field.
        /// </summary>
        /// <param name="fields">The fields to add.</param>
        /// <param name="parameter">The table parameter for distinguishing each element.</param>
        private void AddElements(IEnumerable<IElement> fields, IValue parameter)
        {
            Parameters = parameter.CreateProperty("TABLEITEM");
            foreach (IElement child in fields)
                AddElement(child, Level + 1, this);
        }
    }
}
