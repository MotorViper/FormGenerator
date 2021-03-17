using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextParser.Tokens;
using TextParser.Tokens.Interfaces;

namespace TextParser.Functions
{
    public abstract class CheckedCountFunction : ListFunction
    {
        private readonly int _minCount;
        private readonly int _maxCount;

        protected CheckedCountFunction(string idBase, int minCount, int maxCount) : base(idBase)
        {
            _minCount = minCount;
            _maxCount = maxCount;
        }

        protected abstract IToken PerformOnList(int count, ListToken parameters, TokenTreeList substitutions, bool isFinal);

        /// <summary>
        /// Evaluate the function.
        /// </summary>
        /// <param name="parameters">The tokens that make up the function parameter list.</param>
        /// <param name="substitutions">The tokens that can be used for substitutions.</param>
        /// <param name="isFinal">Whether a result needs to be returned.</param>
        /// <returns></returns>
        protected override IToken PerformOnList(ListToken listToken, TokenTreeList substitutions, bool isFinal)
        {
            int count = listToken.Count;

            if (count < _minCount || count > _maxCount)
                if (_maxCount == _minCount)
                    throw new Exception($"Must have {_minCount} values for '{Name}': {listToken}");
                else
                    throw new Exception($"Must have between {_minCount} and {_maxCount} values for '{Name}': {listToken}");

            return PerformOnList(count, listToken, substitutions, isFinal);
        }
    }
}
