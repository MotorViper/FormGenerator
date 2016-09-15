using System;

namespace Helpers
{
    public static class EnumParser
    {
        public static T ParseEnum<T>(this string toParse)
        {
            return (T)Enum.Parse(typeof(T), toParse);
        }

        public static T ParseEnum<T>(this string toParse, T defaultValue) where T : struct
        {
            T value;
            return Enum.TryParse(toParse, out value) ? value : defaultValue;
        }
    }
}