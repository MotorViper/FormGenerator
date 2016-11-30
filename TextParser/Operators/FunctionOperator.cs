using System;
using TextParser.Functions;
using TextParser.Tokens;

namespace TextParser.Operators
{
    /// <summary>
    /// Runs a function on its parameters.
    /// </summary>
    public class FunctionOperator : BaseOperator
    {
        private IFunction _function;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="function">The function object if known.</param>
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
                    case CaseFunction.ID:
                        _function = new CaseFunction();
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
                    case DoubleFunction.ID:
                        _function = new DoubleFunction();
                        break;
                    case IfFunction.ID:
                        _function = new IfFunction();
                        break;
                    case IntFunction.ID:
                        _function = new IntFunction();
                        break;
                    case JoinFunction.ID:
                        _function = new JoinFunction();
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
                    case RegexFunction.ID:
                        _function = new RegexFunction();
                        break;
                    case ReverseFunction.ID:
                        _function = new ReverseFunction();
                        break;
                    case SplitFunction.ID:
                        _function = new SplitFunction();
                        break;
                    case SumFunction.ID:
                        _function = new SumFunction();
                        break;
                    case UserFunction.ID:
                        _function = new UserFunction();
                        break;
                    default:
                        ListToken newList = new ListToken
                        {
                            new ExpressionToken(null, new SubstitutionOperator(), new StringToken(function))
                        };
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

            IToken parameterList;
            if (isFinal && _function is UserFunction)
            {
                parameterList = PrepareUserFunction(last, parameters);
            }
            else if (_function.IsComparisonFunction)
            {
                parameterList = last.EvaluateList();
            }
            else
            {
                parameterList = last.Evaluate(parameters, !_function.FinalCanBeExpression && isFinal);
                ExpressionToken expression = parameterList as ExpressionToken;
                if (expression != null)
                {
                    if (isFinal)
                    {
                        IToken substitute = _function.ValueIfFinalValueIsExpression(expression);
                        if (substitute is NullToken)
                            return new ExpressionToken(null, new FunctionOperator(_function), parameterList);
                        parameterList = substitute;
                    }
                    else
                    {
                        return new ExpressionToken(null, new FunctionOperator(_function), parameterList);
                    }
                }
            }

            return _function.Perform(parameterList, parameters, isFinal);
        }

        private IToken PrepareUserFunction(IToken last, TokenTreeList parameters)
        {
            ListToken list = new ListToken();
            ListToken toAdd = (ListToken)last;
            IToken toParse = toAdd.Tokens[0];
            IToken method = toParse;
            foreach (TokenTree parameter in parameters)
            {
                TokenTree onlyGlobal = parameter.Clone();

                // Make sure that parameters relevant only to higher levels do not get passed through.
                int i = 1;
                while (onlyGlobal.Children.Remove(i.ToString())) ++i;

                method = toParse.SubstituteParameters(onlyGlobal);
                if (method.Text != toParse.Text)
                    break;
            }
            list.Tokens.Add(method);
            for (int i = 1; i < toAdd.Tokens.Count; ++i)
                list.Tokens.Add(toAdd.Tokens[i].Evaluate(parameters, !_function.FinalCanBeExpression));
            return list;
        }
    }
}
