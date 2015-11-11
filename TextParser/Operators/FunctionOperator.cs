using System;
using System.Collections.Generic;
using System.Text;
using TextParser.Tokens;

namespace TextParser.Operators
{
    public class FunctionOperator : ListProcessingOperator
    {
        public FunctionOperator() : base(":")
        {
        }

        public override bool CanBeBinary => true;
        public override bool CanBeUnary => false;

        protected override IToken Evaluate(IToken first, IToken last, Func<IToken, TokenTreeList, IToken> converter, TokenTreeList parameters)
        {
            if (first == null)
                throw new Exception($"Operation {Text} can not be unary.");

            IToken firstList = converter(first, parameters);
            if (firstList == null || firstList is ListToken)
                throw new Exception($"First element of Operation {Text} is not unique.");

            string function = firstList.Text.ToUpper();
            IToken lastList = converter(last, parameters);

            if (lastList is ExpressionToken)
                return new ExpressionToken(new StringToken(function), new FunctionOperator(), lastList);

            switch (function)
            {
                case "OVER":
                    return Over(lastList as ListToken, converter, parameters);
                case "CONTAINS":
                    return Contains(lastList as ListToken, converter, parameters);
                case "COUNT":
                    return Count(lastList as ListToken, converter);
                case "AGG":
                    return Aggregate(lastList as ListToken, converter, parameters);
                case "SUMI":
                case "SUM":
                    return Sum(lastList as ListToken, converter, parameters);
                case "SUMD":
                    return DSum(lastList as ListToken, converter, parameters);
                default:
                    throw new Exception($"{function} is not recognised for {Text}");
            }
        }

        private IToken Over(ListToken listToken, Func<IToken, TokenTreeList, IToken> converter, TokenTreeList parameters)
        {
            if (listToken == null)
                throw new Exception($"Last token must be list for {Text}");

            List<IToken> lastList = listToken.Tokens;
            int count = lastList.Count;
            IToken iterand = lastList[count - 2];
            IToken method = lastList[count - 1];
            ListToken tokens = new ListToken();
            for (int i = 0; i < count - 2; i++)
            {
                IToken token = converter(lastList[i], parameters);
                ListToken list = token as ListToken;
                if (list == null)
                {
                    list = new ListToken();
                    list.Tokens.Add(token);
                }
                foreach (IToken item in list.Tokens)
                {
                    TokenTreeList treeList = parameters?.Clone() ?? new TokenTreeList();
                    TokenTree tree = new TokenTree();
                    tree.Children.Add(new TokenTree(iterand, item));
                    treeList.Add(tree);
                    IToken parsed = method.Evaluate(treeList);
                    if (parsed is ExpressionToken)
                    {
                        if (parameters == null)
                            return new ExpressionToken(new StringToken("OVER"), new FunctionOperator(), listToken);
                    }
                    else
                    {
                        tokens.Add(parsed);
                    }
                }
            }
            return tokens;
        }

        private IToken Contains(ListToken listToken, Func<IToken, TokenTreeList, IToken> converter, TokenTreeList parameters)
        {
            if (listToken == null)
                throw new Exception($"Last token must be list for {Text}");

            List<IToken> lastList = listToken.Tokens;
            int count = lastList.Count;
            IToken toFind = converter(lastList[count - 1], parameters);
            for (int i = 0; i < count - 1; i++)
            {
                IToken token = converter(lastList[i], parameters);
                if (token is ExpressionToken)
                    return new ExpressionToken(new StringToken("CONTAINS"), new FunctionOperator(), listToken);
                ListToken list = token as ListToken;
                if (list != null && list.Tokens.Contains(toFind))
                    return new BoolTooken(true);
                if (token.Text == toFind.Text)
                    return new BoolTooken(true);
            }
            return new BoolTooken(false);
        }

        private IToken Count(ListToken listToken, Func<IToken, TokenTreeList, IToken> converter)
        {
            if (listToken == null)
                throw new Exception($"Last token must be list for {Text}");

            List<IToken> lastList = listToken.Tokens;
            return new IntToken(lastList.Count);
        }

        private IToken Sum(ListToken listToken, Func<IToken, TokenTreeList, IToken> converter, TokenTreeList parameters)
        {
            if (listToken == null)
                throw new Exception($"Last token must be list for {Text}");

            int sum = 0;
            foreach (IToken token in listToken.Tokens)
            {
                IToken converted = converter(token, parameters);
                if (converted is ExpressionToken)
                    return new ExpressionToken(new StringToken("SUM"), new FunctionOperator(), listToken);
                sum += converted.Convert<int>();
            }
            return new IntToken(sum);
        }

        private IToken DSum(ListToken listToken, Func<IToken, TokenTreeList, IToken> converter, TokenTreeList parameters)
        {
            if (listToken == null)
                throw new Exception($"Last token must be list for {Text}");

            double sum = 0;
            foreach (IToken token in listToken.Tokens)
            {
                IToken converted = converter(token, parameters);
                if (converted is ExpressionToken)
                    return new ExpressionToken(new StringToken("SUMD"), new FunctionOperator(), listToken);
                sum += converted.Convert<double>();
            }
            return new DoubleToken(sum);
        }

        private IToken Aggregate(ListToken listToken, Func<IToken, TokenTreeList, IToken> converter, TokenTreeList parameters)
        {
            if (listToken == null)
                throw new Exception($"Last token must be list for {Text}");

            Dictionary<string, int> found = new Dictionary<string, int>();
            foreach (IToken child in listToken.Tokens)
            {
                IToken converted = converter(child, parameters);
                if (converted is ExpressionToken)
                    return new ExpressionToken(new StringToken("AGG"), new FunctionOperator(), listToken);
                int count;
                string value = converted.Text;
                found[value] = found.TryGetValue(value, out count) ? ++count : 1;
            }

            StringBuilder sb = new StringBuilder();
            foreach (var item in found)
                sb.Append(item.Key).Append("(").Append(item.Value).Append(")/");
            return new StringToken(sb.ToString().TrimEnd('/'));
        }
    }
}
