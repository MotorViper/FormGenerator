﻿using System;
using System.Collections.Generic;
using System.Linq;
using Generator;
using TextParser;
using TextParser.Tokens;

namespace WebFormGenerator.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global - used by IOC.
    /// <summary>
    /// Class for displaying a table.
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
            properties.Add("Content");
            return properties;
        }

        /// <summary>
        /// Outputs the fields children.
        /// </summary>
        protected override void AddChildren()
        {
            List<IElement> fields = Element.Children.ToList();
            Builder.Append("<tr>").AppendLine();
            foreach (IElement child in fields)
            {
                IElement header = child.Properties.FindChild("Header");
                if (header != null)
                {
                    if (header.Properties.Count == 0)
                    {
                        SimpleElement label = new SimpleElement("Label");
                        label.Properties.Add(new SimpleProperty("Content", new SimpleValue(header.ElementName)));
                        header = label;
                    }
                    if (string.IsNullOrWhiteSpace(header.ElementType))
                        header.ElementType = "Label";
                    Builder.Append("<th>").AppendLine();
                    AddElement(header, Level + 1, this);
                    Builder.Append("</th>").AppendLine();
                }
            }
            Builder.Append("</tr>").AppendLine();
            IList<IProperty> over = Element.Properties.Find("Content");
            if (over == null || over.Count == 0)
                throw new Exception("Tried to create table with no Content.");
            if (over.Count == 1)
            {
                TokenTree items = new TokenTree(Element.Parameters[0].GetChildren(over[0].StringValue));
                foreach (TokenTree item in items.Children)
                    AddRow(fields, item.Key);
            }
            else
            {
                foreach (IProperty item in over)
                    AddRow(fields, new StringToken(item.StringValue));
            }
        }

        /// <summary>
        /// Adds a single row of the table.
        /// </summary>
        /// <param name="fields">The fields that will be displayed.</param>
        /// <param name="item">The selected item.</param>
        private void AddRow(List<IElement> fields, IToken item)
        {
            TokenTree parameters = Element.Parameters[0].Clone();
            parameters.Children.AddIfMissing(new TokenTree("TABLEITEM", item));
            Builder.Append("<tr>").AppendLine();
            foreach (IElement child in fields)
            {
                Builder.Append("<td>");
                TokenTreeList tokenTreeList = new TokenTreeList(parameters);
                if (Element.Parameters.Count > 1)
                    tokenTreeList.Add(Element.Parameters[1]);
                child.Parameters = tokenTreeList;
                AddElement(child, Level + 1, this);
                Builder.Append("</td>");
            }
            Builder.Append("<tr>").AppendLine();
        }
    }
}
