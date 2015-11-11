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
        private struct ItemData
        {
            public readonly IToken Token;
            public readonly IToken Parameter;

            public ItemData(IToken token, IToken parameter)
            {
                Token = token;
                Parameter = parameter;
            }

            /// <summary>
            /// Returns a string that represents the current object.
            /// </summary>
            /// <returns>
            /// A string that represents the current object.
            /// </returns>
            public override string ToString()
            {
                return Token.Text;
            }
        }

        private static readonly List<ItemData> s_fieldData = new List<ItemData>();
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
            IToken converted = new NullToken();
            int index = int.Parse(parameter.ToString());
            if (index < s_fieldData.Count)
            {
                ItemData data = s_fieldData[index];
                IToken dataToken = data.Token;
                TokenTree parameters = Parameters;
                if (data.Parameter != null)
                {
                    parameters = Parameters.Clone();
                    parameters.Children.AddIfMissing(new TokenTree(new StringToken("Item"), data.Parameter));
                }
                converted = dataToken.Evaluate(new TokenTreeList { (TokenTree)value, parameters });
            }
            BoolTooken boolTooken = converted as BoolTooken;
            if (boolTooken != null)
                return boolTooken.Value;
            return converted;
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

        public static int SetFieldData(IToken data, IToken parameter)
        {
            s_fieldData.Add(new ItemData(data, parameter));
            return s_fieldData.Count - 1;
        }
    }
}
