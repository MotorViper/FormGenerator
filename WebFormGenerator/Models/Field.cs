using System.Collections.Generic;
using Generator;
using TextParser;
using TextParser.Operators;
using TextParser.Tokens;

namespace WebFormGenerator.Models
{
    public class Field : BaseField
    {
        private string _content;

        // ReSharper disable once MemberCanBePrivate.Global - used by IOC.
        public Field()
        {
            Parameter = null;
        }

        protected Field(string name) : this()
        {
            Name = name;
        }

        public override void AddChildProperties(IField child)
        {
        }

        protected override string ProcessTokens(IToken value, TokenTreeList parameters)
        {
            try
            {
                IToken converted = value.Evaluate(parameters, true);
                ITypeToken typeToken = converted as ITypeToken;
                if (typeToken != null)
                    return typeToken.Data.ToString();
                ExpressionToken expression = converted as ExpressionToken;
                if (expression?.Operator is SubstitutionOperator && expression.Second is StringToken)
                    return "&nbsp;";
                return converted.ToString();
            }
            catch
            {
                return value.ToString();
            }
        }

        protected override void OutputProperties(Dictionary<string, string> properties)
        {
            foreach (var property in properties)
            {
                switch (property.Key)
                {
                    case "Content":
                    case "Text":
                        _content = property.Value;
                        break;
                    case "Style":
                        Builder.Append("class").Append("=\"").Append(property.Value).Append("\" ");
                        break;
                    default:
                        Builder.Append(property.Key.ToLower()).Append("=\"").Append(property.Value).Append("\" ");
                        break;
                }
            }
        }

        protected override void AddChildren(TokenTree parameters, string endOfLine)
        {
            Builder.Append(_content).AppendLine();
            base.AddChildren(parameters, endOfLine);
        }
    }
}
