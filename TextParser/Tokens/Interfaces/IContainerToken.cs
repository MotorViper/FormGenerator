using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextParser.Tokens.Interfaces
{
    interface IContainerToken : IToken
    {
        bool Contains(IToken token);
    }
}
