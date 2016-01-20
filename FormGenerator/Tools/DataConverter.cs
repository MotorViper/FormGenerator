using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Generator;
using TextParser;
using TextParser.Operators;
using TextParser.Tokens;

namespace FormGenerator.Tools
{
    /// <summary>
    /// Does the work behind the scenes of converting a binding to a value.
    /// </summary>
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
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IToken converted = new NullToken();
            int index = int.Parse(parameter.ToString());
            if (index < s_fieldData.Count)
            {
                ItemData data = s_fieldData[index];
                IToken dataToken = data.Token;
                TokenTree parameters = Parameters;
                if (data.ParameterData != null)
                {
                    parameters = Parameters.Clone();
                    parameters.Children.AddIfMissing(data.ParameterData);
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

        /// <summary>
        /// Sets the data that will be used to do the conversion.
        /// </summary>
        /// <param name="data">The basic data.</param>
        /// <param name="parameters">Extra parameters to be used.</param>
        /// <returns>Id to be used as a parameter in the binding definition.</returns>
        public static int SetFieldData(IToken data, TokenTree parameters)
        {
            s_fieldData.Add(new ItemData(data, parameters));
            return s_fieldData.Count - 1;
        }

        /// <summary>
        /// Gets a list of fields from the parameters.
        /// </summary>
        /// <param name="name">The name of the list.</param>
        /// <returns>The entries in the list.</returns>
        public static IPropertyList GetList(string name)
        {
            SimplePropertyList list = new SimplePropertyList();
            TokenTreeList parameters = Parameters.GetChildren(name);
            list.AddRange(parameters.Select(parameter => new TokenTreeProperty(new TokenTree(parameter.Key, parameter.Key))));
            return list;
        }

        private struct ItemData
        {
            public readonly IToken Token;
            public readonly TokenTree ParameterData;

            public ItemData(IToken token, TokenTree parameterData)
            {
                Token = token;
                ParameterData = parameterData;
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
