using Helpers;
using System;
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

        static FunctionOperator()
        {
            RegisterFunction<AggregateFunction>();
            RegisterFunction<AndFunction>();
            RegisterFunction<CaseFunction>();
            RegisterFunction<ComparisonFunction>();
            RegisterFunction<ContainsFunction>();
            RegisterFunction<CountFunction>();
            RegisterFunction<DebugFunction>();
            RegisterFunction<DebugAllFunction>();
            RegisterFunction<DefaultFunction>();
            RegisterFunction<DoubleFunction>();
            RegisterFunction<FlattenFunction>();
            RegisterFunction<IfFunction>();
            RegisterFunction<IntFunction>();
            RegisterFunction<JoinFunction>();
            RegisterFunction<KeysFunction>();
            RegisterFunction<NotFunction>();
            RegisterFunction<OrFunction>();
            RegisterFunction<OverFunction>();
            RegisterFunction<RangeFunction>();
            RegisterFunction<RegexFunction>();
            RegisterFunction<ReverseFunction>();
            RegisterFunction<SplitFunction>();
            RegisterFunction<SumFunction>();
            RegisterFunction<UniqueFunction>();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="function">The function object if known.</param>
        public FunctionOperator(IFunction function = null) : base(":")
        {
            Function = function;
        }

        public override string Text => Function == null ? base.Text : Function.Name + base.Text;

        public IFunction Function { get; private set; }

        public static void RegisterFunction<T>() where T : IFunction, new()
        {
            T func = new T();
            foreach (string id in func.Ids)
                IOCContainer.Instance.Register<IFunction, T>(id);
        }

        /// <summary>
        /// This method localises function parameters so they don't step on each others toes.
        /// </summary>
        /// <param name="parameters">Contains function definition.</param>
        /// <param name="userFunction">The user function.</param>
        private void ModifyParameters(TokenTreeList parameters, UserFunction userFunction)
        {
            if (parameters != null)
            {
                TokenTreeList functionDefinition = parameters?.FindAllMatches(userFunction.FunctionName);
                if (functionDefinition.Count > 0)
                    functionDefinition[0].Value.ModifyParameters(userFunction);
            }
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
            if (Function == null)
            {
                if (first == null)
                    throw new Exception($"Operation {Text} can not be unary.");

                IToken functionToken = first.Evaluate(parameters, isFinal);
                if (functionToken == null || functionToken is ListToken)
                    throw new Exception($"First element of Operation {Text} is not unique.");

                string function = functionToken.ToString();
                Function = IOCContainer.Instance.Resolve<IFunction>(function);
                if (Function == null && function.EndsWith("F"))
                {
                    Function = IOCContainer.Instance.Resolve<IFunction>(function.Substring(0, function.Length - 1));
                    _flatten = Function != null;
                }
                if (Function == null)
                {
                    UserFunction userFunction = new UserFunction() { FunctionName = function };
                    ModifyParameters(parameters, userFunction);
                    return EvaluateUserFunction(last, parameters, isFinal, userFunction);
                }
            }
            else if (Function is UserFunction userFunction)
            {
                ModifyParameters(parameters, userFunction);
            }

            bool debug = Function.Name == "DEBUGA" || (Function.Name == "DEBUG" && isFinal);
            if (debug)
                LogControl?.SetLogging(true);

            IToken parameterList;
            if (isFinal && Function is UserFunction)
            {
                parameterList = PrepareUserFunction(last, parameters);
            }
            else if (Function.AllowsShortCircuit)
            {
                parameterList = last;
                _flatten = true;
            }
            else
            {
                parameterList = last.Evaluate(parameters, !Function.FinalCanBeExpression && isFinal);
                if (parameterList is ExpressionToken expression)
                {
                    if (isFinal)
                    {
                        IToken substitute = Function.ValueIfFinalValueIsExpression(expression);
                        if (substitute is NullToken)
                            return new ExpressionToken(null, new FunctionOperator(Function), parameterList);
                        parameterList = substitute;
                    }
                    else
                    {
                        return new ExpressionToken(null, new FunctionOperator(Function), parameterList);
                    }
                }
            }

            IToken result = Function.Perform(_flatten ? parameterList.Flatten() : parameterList, parameters, isFinal);

            if (debug)
                LogControl?.ResetLogging();

            return result;
        }

        private static IToken EvaluateUserFunction(IToken last, TokenTreeList parameters, bool isFinal, UserFunction function)
        {
            ListToken newList = new ListToken(new ExpressionToken(null, new SubstitutionOperator(), new StringToken(function.FunctionName)));
            if (last.Evaluate(parameters, isFinal) is ListToken oldList)
            {
                foreach (IToken token in oldList)
                    newList.Add(token);
            }
            else
            {
                newList.Add(last);
            }
            ExpressionToken expression = new ExpressionToken(null, new FunctionOperator(function), newList);
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
                list.Value.Add(toAdd.Value[i].Evaluate(parameters, !Function.FinalCanBeExpression));
            return list;
        }
    }
}
