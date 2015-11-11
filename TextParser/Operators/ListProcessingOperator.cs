using System;
using TextParser.Tokens;

namespace TextParser.Operators
{
    public abstract class ListProcessingOperator : BaseOperator
    {
        protected ListProcessingOperator(string text) : base(text)
        {
        }

        public override bool CanBeBinary => true;
        public override bool CanBeUnary => false;

        public override IToken Evaluate(IToken first, IToken last, TokenTreeList parameters)
        {
            return Evaluate(first, last, (x, y) => x?.Evaluate(y), parameters);
        }

        public override IToken Simplify(IToken first, IToken last)
        {
            return Evaluate(first, last, (x, y) => x?.Simplify(), null);
        }

        protected abstract IToken Evaluate(IToken first, IToken last, Func<IToken, TokenTreeList, IToken> converter, TokenTreeList parameters);
    }
}
