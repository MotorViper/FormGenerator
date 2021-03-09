using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;

namespace Helpers
{
    class EnumDescriptionTypeConverter : EnumConverter
    {
        public EnumDescriptionTypeConverter(Type type)  : base(type)
        {
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                string valueString = value?.ToString();
                FieldInfo fi = value?.GetType().GetField(valueString);
                DescriptionAttribute[] attributes = (DescriptionAttribute[])fi?.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attributes?.Length > 0)
                {
                    string description = attributes[0].Description;
                    if (description.Contains("|"))
                        description = description.Split(new[] { '|' }, 2)[0];
                    return string.IsNullOrEmpty(description) ? valueString : description;
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
