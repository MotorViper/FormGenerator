﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextParser.Tokens.Interfaces
{
    public interface IReversibleToken
    {
        IToken Reverse();
    }
}
