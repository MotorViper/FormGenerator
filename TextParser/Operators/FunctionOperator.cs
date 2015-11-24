using System;
using TextParser.Functions;
using TextParser.Tokens;

namespace TextParser.Operators
{
    public class FunctionOperator : BaseOperator
    {
        private IFunction _function;

        public FunctionOperator(IFunction function = null) : base(":")
        {
            _function = function;
        }

        public override bool CanBeBinary => true;
        public override bool CanBeUnary => false;
        public override string Text => _function == null ? base.Text : _function.Name + base.Text;

        public override IToken Evaluate(IToken first, IToken last, TokenTreeList parameters, bool isFinal)
        {
            if (_function == null)
            {
                if (first == null)
                    throw new Exception($"Operation {Text} can not be unary.");

                IToken functionToken = first.Evaluate(parameters, isFinal);
                if (functionToken == null || functionToken is ListToken)
                    throw new Exception($"First element of Operation {Text} is not unique.");

                string function = functionToken.Text;
                switch (function)
                {
                    case AndFunction.ID:
                        _function = new AndFunction();
                        break;
                    case AggregateFunction.ID:
                        _function = new AggregateFunction();
                        break;
                    case ComparisonFunction.ID:
                        _function = new ComparisonFunction();
                        break;
                    case ContainsFunction.ID:
                        _function = new ContainsFunction();
                        break;
                    case CountFunction.ID:
                        _function = new CountFunction();
                        break;
                    case DoubleSumFunction.ID:
                        _function = new DoubleSumFunction();
                        break;
                    case IfFunction.ID:
                        _function = new IfFunction();
                        break;
                    case IntSumFunction.ID:
                    case IntSumFunction.ALT_ID:
                        _function = new IntSumFunction();
                        break;
                    case OrFunction.ID:
                        _function = new OrFunction();
                        break;
                    case OverFunction.ID:
                        _function = new OverFunction();
                        break;
                    case RangeFunction.ID:
                        _function = new RangeFunction();
                        break;
                    case ReverseFunction.ID:
                        _function = new ReverseFunction();
                        break;
                    case UserFunction.ID:
                        _function = new UserFunction();
                        break;
                    default:
                        ListToken newList = new ListToken();
                        newList.Add(new ExpressionToken(null, new SubstitutionOperator(), new StringToken(function)));
                        ListToken oldList = last.Evaluate(parameters, isFinal) as ListToken;
                        if (oldList != null)
                        {
                            foreach (IToken token in oldList.Tokens)
                                newList.Add(token);
                        }
                        else
                        {
                            newList.Add(last);
                        }
                        ExpressionToken expression = new ExpressionToken(null, new FunctionOperator(new UserFunction()), newList);
                        return expression.Evaluate(parameters, isFinal);
                }
            }

            IToken parameterList = last.Evaluate(parameters, !_function.FinalCanBeExpression && isFinal);

            if (parameterList is ExpressionToken)
                return new ExpressionToken(null, new FunctionOperator(_function), parameterList);

            return _function.Perform(parameterList, parameters, isFinal);
        }
    }
}
