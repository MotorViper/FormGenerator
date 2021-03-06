﻿using System;
using System.Collections.Generic;
using System.Linq;
using Generator;

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
            GridData positions = new GridData(Element.Properties, "10");
            List<string> columns = positions.Columns;
            List<IElement> fields = Element.Children.ToList();
            Builder.Append("<tr>").AppendLine();
            int i = 0;
            foreach (IElement child in fields)
            {
                string columnData = columns[i];
                IElement header = child.Properties.FindChild("Header");
                if (columnData != "Auto")
                    Builder.Append("<th style=\"width: ").Append(columnData).Append("px\">").AppendLine();
                else
                    Builder.Append("<th>").AppendLine();
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
                    AddElement(header, Level + 1, this);
                }
                Builder.Append("</th>").AppendLine();
                ++i;
            }
            Builder.Append("</tr>").AppendLine();
            IList<IProperty> over = Element.Properties.Find("Content");
            if (over == null || over.Count == 0)
                throw new Exception("Tried to create table with no Content.");
            AddRows(fields, over.Count == 1 ? Element.Parameters.GetList(over[0].StringValue) : (IEnumerable<IProperty>)over);
        }

        /// <summary>
        /// Adds the rows of the table.
        /// </summary>
        /// <param name="fields">The fields that will be displayed.</param>
        /// <param name="rows">The rows to output.</param>
        private void AddRows(IReadOnlyCollection<IElement> fields, IEnumerable<IProperty> rows)
        {
            foreach (IProperty item in rows)
                AddRow(fields, item.StringValue);
        }

        /// <summary>
        /// Adds a single row of the table.
        /// </summary>
        /// <param name="fields">The fields that will be displayed.</param>
        /// <param name="item">The selected item.</param>
        private void AddRow(IEnumerable<IElement> fields, string item)
        {
            IParameters parameters = Element.Parameters.Add(new SimpleProperty("1", item));
            Builder.Append("<tr>").AppendLine();
            foreach (IElement child in fields)
            {
                Builder.Append("<td>");
                child.Parameters = parameters;
                AddElement(child, Level + 1, this);
                Builder.Append("</td>");
            }
            Builder.Append("<tr>").AppendLine();
        }
    }
}
