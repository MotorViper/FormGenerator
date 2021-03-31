using Helpers;
using System;
using TextParser.Tokens.Interfaces;

namespace TextParser.Tokens
{
    public class PairToken : BaseToken
    {
        //public static PairToken Create(string text, bool ignoreErrors = false)
        //{
        //    int pos = text.FirstNotInBlock(':', new[] { '\'', '"', '^' });
        //    if (pos < 0)
        //    {
        //        if (ignoreErrors)
        //            return new TreeToken((StringToken)text.Trim(), (StringToken)"");
        //        throw new Exception($"Invalid format: Missing ':' on line: {text}");
        //    }

        //    IKeyToken first = TokenGenerator.Parse(text.Substring(0, pos).Trim()) as IKeyToken;
        //    IValueToken second = TokenGenerator.Parse(text.Substring(pos + 1).Trim()) as IValueToken;
        //    return new TreeToken(first, second);
        //}

        //public PairToken(IKeyToken key, IToken value = null)
        //{
        //    key.Parent = Parent;
        //    value.Parent = Parent;
        //    Key = key;
        //    Value = value;
        //}

        public IKeyToken Key { get; set; }

        public IToken Value { get; private set; }

        //public override IToken FindToken(string text, bool checkChildren, bool checkParent = true)
        //{
        //    return Key.Matches(text) ? this : null;
        //}

        //public override IToken ValueToken => Value;

        public virtual void WalkTree(Action<string, string> walker, string prefix = null)
        {
            string key = prefix == null ? Key.ToString() : prefix + Key.ToString();
            walker(key, Value.ToString());
        }
    }
}
