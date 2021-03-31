using System.Text.RegularExpressions;
using TextParser.Tokens.Interfaces;

namespace TextParser.Tokens
{
    /// <summary>
    /// Class representing a regular expression token.
    /// </summary>
    public class RegExToken : BaseToken//, IValueToken, IKeyToken
    {
        public enum RegexType
        {
            Regex,
            Sql,
            Wildcard
        }

        private readonly Regex _regex;
        private readonly string _text;

        public RegExToken(string text, RegexType regexType)
        {
            _text = text;
            _regex = new Regex(ConvertRegex(text, regexType));
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return _text;
        }
 
        public override bool Contains(string text)
        {
            return _regex.IsMatch(text);
        }

        //public override int CompareTo(IToken token)
        //{
        //    return Matches(token.ToString()) ? 0 : 1;
        //}

        //public override bool ComparisonIsInteger => false;

        private static string ConvertFromSqlRegex(string text)
        {
            return "^" + Regex.Escape(text).Replace("%", ".*").Replace("_", ".") + "$";
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

        //public bool Matches(string text)
        //{
        //    return _regex.IsMatch(text);
        //}

        //public override IToken ValueToken => this;

        public static implicit operator RegExToken(string s) => new RegExToken(s, RegexType.Wildcard);
    }
}
