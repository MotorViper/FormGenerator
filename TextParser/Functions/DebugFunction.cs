using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextParser.Tokens.Interfaces;

namespace TextParser.Functions
{
    class DebugFunction : BaseFunction
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public DebugFunction() : base("DEB(UG)")
        {
        }

        /// <summary>
        /// Evaluate the function.
        /// </summary>
        /// <param name="parameters">The tokens that make up the function parameter list.</param>
        /// <param name="substitutions">The tokens that can be used for substitutions.</param>
        /// <param name="isFinal">Whether a result needs to be returned.</param>
        /// <returns></returns>
        public override IToken Perform(IToken parameters, TokenTreeList substitutions, bool isFinal)
        {
            return parameters.Evaluate(substitutions, isFinal);
        }
    }
}
