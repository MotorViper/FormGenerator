using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using TextParser;
using TextParser.Tokens;

namespace FormGenerator
{
    public class DataConverter : IValueConverter
    {
        private static readonly List<IToken> s_fieldData = new List<IToken>();
        public static TokenTree Parameters { get; set; }

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value produced by the binding source.</param><param name="targetType">The type of the binding target property.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IToken dataToken = s_fieldData[int.Parse(parameter.ToString())];
            IToken result = dataToken.Evaluate(new TokenTreeList((TokenTree)value))[0];
            result = result.Evaluate(new TokenTreeList(Parameters))[0];
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

        public static int SetFieldData(IToken data)
        {
            s_fieldData.Add(data);
            return s_fieldData.Count - 1;
        }
    }
}
