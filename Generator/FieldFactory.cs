using System.Collections.Generic;
using Helpers;
using TextParser;
using TextParser.Operators;
using TextParser.Tokens;

namespace Generator
{
    public static class FieldFactory
    {
        public static IField CreateField(string fieldName, TokenTree data, int level, TokenTree parameters, IField parent)
        {
            if (string.IsNullOrWhiteSpace(fieldName))
                fieldName = "Continuation";

            IField field = IOCContainer.Instance.Resolve<IField>(fieldName);
            if (field != null)
            {
                field.Level = level;
                field.Data = data;
                field.Parent = parent;
            }
            else
            {
                ExpressionToken expression = data.Value as ExpressionToken;
                if (expression != null)
                {
                    // There should be a better way of doing this using the language features.
                    FunctionOperator function = expression.Operator as FunctionOperator;
                    if (function != null)
                    {
                        ListToken list = (ListToken)expression.Second;
                        List<IToken> tokens = list.Tokens;
                        fieldName = ((ExpressionToken)tokens[0]).Second.Text;
                        for (int i = 1; i < tokens.Count; ++i)
                            data.Children.Add(new TokenTree("P" + i, tokens[i].Text));
                    }
                    else
                    {
                        fieldName = expression.Evaluate(new TokenTreeList(parameters), false).Text;
                    }
                }

                TokenTree replacement = parameters?.FindFirst(fieldName);
                if (replacement != null)
                {
                    fieldName = replacement.Value.Text;
                    foreach (TokenTree child in replacement.Children)
                    {
                        if (child.Name == "Field")
                            data.Children.Add(child);
                        else
                            data.Children.AddIfMissing(child);
                    }
                    field = CreateField(fieldName, data, level, parameters, parent);
                }
                else
                {
                    field = IOCContainer.Instance.Resolve<IField>();
                    field.Name = fieldName;
                    field.Level = level;
                    field.Data = data;
                    field.Parent = parent;
                }
            }
            return field;
        }
    }
}
