using System.Collections.Generic;
using System.Linq;
using Generator;
using Helpers;
using TextParser;
using TextParser.Tokens;

namespace WebFormGenerator.Models
{
    public class HtmlGenerator
    {
        private readonly string _endOfLine;
        private readonly string _offset;
        private readonly IFieldWriter _sb = new StringFieldWriter();

        public HtmlGenerator(string endOfLine = "\n", string offset = "  ")
        {
            _endOfLine = endOfLine;
            _offset = offset;
        }

        public string GenerateHtml(TokenTree data, TokenTree selected, string dataName, List<string> keys)
        {
            TokenTree parameters = new TokenTree(data.GetChildren("Parameters"));
            foreach (TokenTree child in parameters.Children)
            {
                string name = child.Name;
                TokenTree defaults = parameters.FindFirst("Defaults." + name);
                if (defaults != null)
                    foreach (TokenTree item in child.Children)
                        item.AddMissing(defaults);
            }

            TokenTree values = selected.Clone();
            TokenTree defaultValues = parameters.FindFirst("Defaults." + dataName);
            values.AddMissing(defaultValues);
            values.SetParameters(parameters);

            TokenTreeList fields = data.GetAll("Fields");
            TokenTreeList styles = fields.FindMatches("Style", true);
            AddStyles(styles, parameters);
            foreach (TokenTree field in fields.SelectMany(child => child.Children).Where(x => x.Name == "Field"))
                _sb.AddChild(field, 0, parameters, _offset, _endOfLine, null, null, values, keys);
            return _sb.Generated;
        }

        protected void AddStyles(IReadOnlyList<TokenTree> styles, TokenTree parameters)
        {
            _sb.Append("<style>").AppendLine();
            foreach (TokenTree tokenTree in styles)
            {
                IToken targetType = tokenTree.Value;
                if (!string.IsNullOrWhiteSpace(targetType.Text))
                {
                    _sb.Append(tokenTree.Value);
                }
                else
                {
                    string key = tokenTree["Name"];
                    if (!string.IsNullOrWhiteSpace(key))
                        _sb.Append(".").Append(key);
                    else
                    {
                        key = tokenTree["ID"];
                        _sb.Append("#").Append(key);
                    }
                }
                _sb.Append(" {").AppendLine();
                AddProperties(styles, parameters, tokenTree);
                _sb.Append("}").AppendLine();
            }
            _sb.Append("</style>").AppendLine();
        }

        private void AddProperties(IReadOnlyList<TokenTree> styles, TokenTree parameters, TokenTree style)
        {
            foreach (TokenTree child in style.Children.Where(x => x.Name != "Name"))
                AddProperty(parameters, child, styles);
        }

        private void AddProperty(TokenTree parameters, TokenTree child, IReadOnlyList<TokenTree> styles)
        {
            if (child.Name == "BasedOn")
            {
                child = styles.FirstOrDefault(x => x["Name"] == child.Value.Text);
                AddProperties(styles, parameters, child);
            }
            else
            {
                _sb.Append("  ")
                    .Append(child.Name.CamelCaseToHyphenated())
                    .Append(": ")
                    .Append(ProcessTokens(child.Value, new TokenTreeList(parameters)))
                    .Append(";")
                    .AppendLine();
            }
        }

        private static string ProcessTokens(IToken value, TokenTreeList parameters)
        {
            return value.Evaluate(parameters, false).Text;
        }
    }
}
