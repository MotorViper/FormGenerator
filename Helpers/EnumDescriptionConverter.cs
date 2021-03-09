using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;

namespace Helpers
{
    class EnumDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string valueString = value?.ToString();
            FieldInfo fi = value?.GetType().GetField(valueString);
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi?.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes?.Length > 0)
            {
                string description = attributes[0].Description;
                if (description.Contains("|"))
                    description = description.Split('|')[int.Parse(parameter?.ToString() ?? "0")];
                return string.IsNullOrEmpty(description) ? valueString : description;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
