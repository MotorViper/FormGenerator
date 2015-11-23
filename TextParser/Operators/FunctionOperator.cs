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
                    case "OVER":
                        _function = new OverFunction();
                        break;
                    case "CONTAINS":
                        _function = new ContainsFunction();
                        break;
                    case "COUNT":
                        _function = new CountFunction();
                        break;
                    case "AGG":
                        _function = new AggregateFunction();
                        break;
                    case "SUMI":
                    case "SUM":
                        _function = new IntSumFunction();
                        break;
                    case "SUMD":
                        _function = new DoubleSumFunction();
                        break;
                    case "IF":
                        _function = new IfFunction();
                        break;
                    case "COMP":
                        _function = new ComparisonFunction();
                        break;
                    case "OR":
                        _function = new OrFunction();
                        break;
                    case "REVERSE":
                        _function = new ReverseFunction();
                        break;
                    case "RANGE":
                        _function = new RangeFunction();
                        break;
                    case "FUNC":
                        _function = new UserFunction();
                        break;
                    default:
                        ListToken newList = new ListToken();
                        newList.Add(new ExpressionToken(null, new SubstitutionOperator(), new StringToken(function)));
                        newList.Add(last);
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
