using System;
using Helpers;
using TextParser.Functions;
using TextParser.Tokens;
using TextParser.Tokens.Interfaces;

namespace TextParser.Operators
{
    /// <summary>
    /// Runs a function on its parameters.
    /// </summary>
    public class FunctionOperator : BaseOperator
    {
        private bool _flatten;
        private IFunction _function;

        static FunctionOperator()
        {
            RegisterFunction<AggregateFunction>();
            RegisterFunction<AndFunction>();
            RegisterFunction<CaseFunction>();
            RegisterFunction<ComparisonFunction>();
            RegisterFunction<ContainsFunction>();
            RegisterFunction<CountFunction>();
            RegisterFunction<DoubleFunction>();
            RegisterFunction<FlattenFunction>();
            RegisterFunction<IfFunction>();
            RegisterFunction<IntFunction>();
            RegisterFunction<JoinFunction>();
            RegisterFunction<NotFunction>();
            RegisterFunction<OrFunction>();
            RegisterFunction<OverFunction>();
            RegisterFunction<RangeFunction>();
            RegisterFunction<RegexFunction>();
            RegisterFunction<ReverseFunction>();
            RegisterFunction<SplitFunction>();
            RegisterFunction<SumFunction>();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="function">The function object if known.</param>
        public FunctionOperator(IFunction function = null) : base(":")
        {
            _function = function;
        }

        public override string Text => _function == null ? base.Text : _function.Name + base.Text;

        public static void RegisterFunction<T>() where T : IFunction, new()
        {
            T func = new T();
            foreach (string id in func.Ids)
                IOCContainer.Instance.Register<IFunction, T>(id);
        }

        /// <summary>
        /// Evaluates an operator expression.
        /// </summary>
        /// <param name="first">The first value.</param>
        /// <param name="last">The second value.</param>
        /// <param name="parameters">Any substitution parameters.</param>
        /// <param name="isFinal">Whether this is the final (output) call.</param>
        /// <returns>The evaluated value.</returns>
        public override IToken Evaluate(IToken first, IToken last, TokenTreeList parameters, bool isFinal)
        {
            if (_function == null)
            {
                if (first == null)
                    throw new Exception($"Operation {Text} can not be unary.");

                IToken functionToken = first.Evaluate(parameters, isFinal);
                if (functionToken == null || functionToken is ListToken)
                    throw new Exception($"First element of Operation {Text} is not unique.");

                string function = functionToken.ToString();
                _function = IOCContainer.Instance.Resolve<IFunction>(function);
                if (_function == null && function.EndsWith("F"))
                {
                    _function = IOCContainer.Instance.Resolve<IFunction>(function.Substring(0, function.Length - 1));
                    _flatten = _function != null;
                }
                if (_function == null)
                    return EvaluateUserFunction(last, parameters, isFinal, function);
            }

            IToken parameterList;
            if (isFinal && _function is UserFunction)
            {
                parameterList = PrepareUserFunction(last, parameters);
            }
            else if (_function.AllowsShortCircuit)
            {
                parameterList = last;
                _flatten = true;
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

            return _function.Perform(_flatten ? parameterList.Flatten() : parameterList, parameters, isFinal);
        }

        private static IToken EvaluateUserFunction(IToken last, TokenTreeList parameters, bool isFinal, string function)
        {
            ListToken newList = new ListToken(new ExpressionToken(null, new SubstitutionOperator(), new StringToken(function)));
            ListToken oldList = last.Evaluate(parameters, isFinal) as ListToken;
            if (oldList != null)
            {
                foreach (IToken token in oldList)
                    newList.Add(token);
            }
            else
            {
                newList.Add(last);
            }
            ExpressionToken expression = new ExpressionToken(null, new FunctionOperator(new UserFunction()), newList);
            return expression.Evaluate(parameters, isFinal);
        }

        private IToken PrepareUserFunction(IToken last, TokenTreeList parameters)
        {
            ListToken list = new ListToken();
            ListToken toAdd = (ListToken)last;
            IToken toParse = toAdd.Value[0];
            IToken method = toParse;
            foreach (TokenTree parameter in parameters)
            {
                TokenTree onlyGlobal = parameter.Clone();

                // Make sure that parameters relevant only to higher levels do not get passed through.
                int i = 1;
                while (onlyGlobal.Children.Remove(i.ToString())) ++i;

                method = toParse.SubstituteParameters(onlyGlobal);
                if (method.ToString() != toParse.ToString())
                    break;
            }
            list.Value.Add(method);
            for (int i = 1; i < toAdd.Value.Count; ++i)
                list.Value.Add(toAdd.Value[i].Evaluate(parameters, !_function.FinalCanBeExpression));
            return list;
        }
    }
}
