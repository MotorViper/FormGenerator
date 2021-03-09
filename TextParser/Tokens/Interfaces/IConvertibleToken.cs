using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextParser.Tokens.Interfaces
{
    public interface IConvertibleToken
    {
        /// <summary>
        /// Converts the token to an integer.
        /// </summary>
        IToken ConvertToInt(TokenTreeList substitutions, bool isFinal);

        /// <summary>
        /// Converts the token to a double.
        /// </summary>
        IToken ConvertToDouble(TokenTreeList substitutions, bool isFinal);
    }
}
