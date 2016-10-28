using System;
using System.Text.RegularExpressions;

namespace TextParser.Tokens
{
    /// <summary>
    /// Class representing a regular expression token.
    /// </summary>
    public class RegExToken : BaseToken
    {
        public enum RegexType
        {
            Regex,
            Sql,
            Wildcard
        }

        private readonly Regex _regex;

        public RegExToken(string text, RegexType regexType)
        {
            Text = text;
            _regex = new Regex(ConvertRegex(text, regexType));
        }

        public override string Text { get; }

        public override TTo Convert<TTo>()
        {
            throw new Exception("Can not convert RegExToken");
        }

        public override bool Contains(string text)
        {
            return _regex.IsMatch(text);
        }

        private static string ConvertFromSqlRegex(string text)
        {
            return "^" + Regex.Escape(text).Replace("\\%", ".*").Replace("\\_", ".") + "$";
        }

        private static string ConvertFromWildcardRegex(string text)
        {
            return "^" + Regex.Escape(text).Replace("\\*", ".*").Replace("\\?", ".") + "$";
        }

        private static string ConvertRegex(string text, RegexType regexType)
        {
            switch (regexType)
            {
                case RegexType.Sql:
                    return ConvertFromSqlRegex(text);
                case RegexType.Wildcard:
                    return ConvertFromWildcardRegex(text);
                default:
                    return text;
            }
        }
    }
}
