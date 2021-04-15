using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using TextParser;
using TextParser.Tokens;
using TextParser.Tokens.Interfaces;

namespace FormGenerator.Tools
{
    public class ListConverter : IValueConverter
    {
        private static readonly Dictionary<string, HashSet<string>> s_lists = new Dictionary<string, HashSet<string>>();

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value produced by the binding source.</param><param name="targetType">The type of the binding target property.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string dataType = parameter.ToString();

            HashSet<string> result = new HashSet<string>();
            string name = null;
            if (dataType.StartsWith("^"))
            {
                dataType = dataType.Substring(1);
                int pos = dataType.IndexOf("^");
                name = dataType.Substring(0, pos);
                dataType = dataType.Substring(pos + 2);
                if (s_lists.TryGetValue(name, out HashSet<string> stored))
                    result.UnionWith(stored);
            }

            // If the data is an expression then do extra work to evaluate it.
            if (dataType.Contains("$") || dataType.Contains("{") || dataType.Contains(":"))
            {
                TokenTree tree = new Parser().AddLine(new Line("Content: " + dataType));
                IToken converted = tree.Value.Evaluate(new TokenTreeList { (TokenTree)value, DataConverter.Parameters }, true);
                if (converted is ListToken list)
                    foreach (IToken token in list.Value)
                        result.Add(token.ToString());
            }
            else
            {
                TokenTree items = new TokenTree(DataConverter.Parameters?.GetChildren(dataType));
                foreach (TokenTree item in items.Children)
                    result.Add(item.Name);
            }

            if (name != null)
                s_lists[name] = result;

            return result;
        }

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value that is produced by the binding target.</param><param name="targetType">The type to convert to.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
