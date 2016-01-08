using System.Collections.Generic;
using FormGenerator.Tools;
using Generator;
using Helpers;
using TextParser;
using TextParser.Operators;
using TextParser.Tokens;

namespace FormGenerator.Fields
{
    public class Field : BaseField
    {
        // ReSharper disable once MemberCanBePrivate.Global - used by IOC.
        public Field()
        {
            Parameter = null;
        }

        protected Field(string name) : this()
        {
            Name = name;
        }

        protected override string ProcessTokens(IToken value, TokenTreeList parameters)
        {
            IToken evaluated = value.Evaluate(parameters, false);
            ExpressionToken expression = evaluated as ExpressionToken;

            if (expression == null)
                return evaluated.Text;

            if (Parameter == null && (expression.Operator is SubstitutionOperator && expression.Second is StringToken))
                return "{Binding Values[" + ((StringToken)expression.Second).Text + "]}";

            int id = DataConverter.SetFieldData(evaluated, Parameter);
            return "{Binding Values, Converter={StaticResource DataConverter}, Mode=OneWay, ConverterParameter=" + id + "}";
        }

        protected override void OutputProperties(Dictionary<string, string> properties)
        {
            foreach (var property in properties)
                Builder.Append(property.Key.ToCamelCase()).Append("=\"").Append(property.Value).Append("\" ");
        }

        public override void AddProperty<T>(string name, T value)
        {
            switch (name)
            {
                case "Style":
                    base.AddProperty("Style", "{StaticResource " + value + "}");
                    break;
                default:
                    base.AddProperty(name, value);
                    break;
            }
        }
    }
}
