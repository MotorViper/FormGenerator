using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using TextParser;
using TextParser.Operators;
using TextParser.Tokens;

namespace FormGenerator.Tools
{
    public class DataConverter : IValueConverter
    {
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
                if (data.Parameters != null)
                {
                    parameters = Parameters.Clone();
                    parameters.Children.AddIfMissing(data.Parameters);
                }
                converted = dataToken.Evaluate(new TokenTreeList {(TokenTree)value, parameters}, true);
            }
            ITypeToken typeToken = converted as ITypeToken;
            if (typeToken != null)
                return typeToken.Data;
            ExpressionToken expression = converted as ExpressionToken;
            if (expression?.Operator is SubstitutionOperator)
                return "";
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

        public static int SetFieldData(IToken data, TokenTree parameters)
        {
            s_fieldData.Add(new ItemData(data, parameters));
            return s_fieldData.Count - 1;
        }

        private struct ItemData
        {
            public readonly IToken Token;
            public readonly TokenTree Parameters;

            public ItemData(IToken token, TokenTree parameters)
            {
                Token = token;
                Parameters = parameters;
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
    }
}
