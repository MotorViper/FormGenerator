using System.Collections.Generic;
using TextParser.Tokens.Interfaces;

namespace TextParser
{
    public static class TokenCache
    {
        private static readonly Dictionary<string, IToken> s_staticCache = new Dictionary<string, IToken>();
        private static Dictionary<string, IToken> s_currentCache = new Dictionary<string, IToken>();
        private static readonly Dictionary<string, Dictionary<string, IToken>> s_entriesCache =
            new Dictionary<string, Dictionary<string, IToken>>();

        public static void UseCache(string name)
        {
            if (s_entriesCache.ContainsKey(name))
            {
                s_currentCache = s_entriesCache[name];
            }
            else
            {
                s_currentCache = new Dictionary<string, IToken>();
                s_entriesCache[name] = s_currentCache;
            }
        }

        public static bool FindToken(string key, out IToken token)
        {
            if (s_currentCache.TryGetValue(key, out token))
                return true;
            if (s_staticCache.TryGetValue(key, out token))
                return true;
            return false;
        }

        internal static void UpdateCache(bool useStatic, string key, IToken token)
        {
            if (useStatic)
                s_staticCache[key] = token;
            else
                s_currentCache[key] = token;
        }
    }
}
