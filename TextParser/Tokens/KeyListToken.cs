using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextParser.Tokens.Interfaces;

namespace TextParser.Tokens
{
    public class KeyListToken //: ListToken<IKeyToken>, IKeyToken
    {
        public bool Matches(string text)
        {
            //foreach (IKeyToken token in this)
            //{
            //    if (token.Matches(text))
            //        return true;
            //}
            return false;
        }
    }
}
