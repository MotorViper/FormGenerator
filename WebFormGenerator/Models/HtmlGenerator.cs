using Generator;
using Helpers;
using System.Collections.Generic;
using System.Linq;
using TextParser;
using TextParser.Tokens;
using TextParser.Tokens.Interfaces;

namespace WebFormGenerator.Models
{
    public class HtmlGenerator
    {
        private readonly IFieldWriter _sb = IOCContainer.Instance.Resolve<IFieldWriter>();

        /// <summary>
        /// Generates the html that is to be displayed.
        /// </summary>
        /// <param name="data">The data the html is constructed from.</param>
        /// <param name="selected">The selected item for displaying.</param>
        /// <param name="dataName">The name of the data structure that contains item information.</param>
        /// <param name="keys">List of selectable items.</param>
        /// <returns>The html representation of the input.</returns>
        public string GenerateHtml(TokenTree data, TokenTree selected, string dataName, List<string> keys)
        {
            TokenTree parameters = new TokenTree(data.GetChildren("Parameters"));
            foreach (TokenTree child in parameters.Children)
            {
                string name = child.Name;
                TokenTree defaults = parameters.FindFirst(new ChainToken("Defaults", name));
                if (defaults != null)
                    foreach (TokenTree item in child.Children)
                        item.AddMissing(defaults);
            }
            _sb.Clear();
            TokenTree values = selected.Clone();
            TokenTree defaultValues = parameters.FindFirst(new ChainToken("Defaults", dataName));
            if (defaultValues != null)
                values.AddMissing(defaultValues);
            values.SetParameters(parameters);

            TokenTreeList fields = data.GetAll(new StringToken("Fields", true));
            TokenTreeList styles = data.GetAll(new StringToken("Styles", true)).FindAllMatches(new StringToken("Style", true));
            AddStyles(styles, parameters);
            foreach (TokenTree field in fields.SelectMany(child => child.Children).Where(x => x.Name == "Field"))
                _sb.AddElement(field, 0, parameters, values, keys);
            return _sb.Generated;
        }

        /// <summary>
        /// Adds style information.
        /// </summary>
        /// <param name="styles">List of styles to add.</param>
        /// <param name="parameters">Calculation parameters.</param>
        protected void AddStyles(IReadOnlyList<TokenTree> styles, TokenTree parameters)
        {
            _sb.Append("<style>").AppendLine();
            foreach (TokenTree tokenTree in styles)
            {
                IToken targetType = tokenTree.Value;
                if (!string.IsNullOrWhiteSpace(targetType.ToString()))
                {
                    _sb.Append(tokenTree.Value);
                }
                else
                {
                    string key = tokenTree[new StringToken("Name", true)];
                    if (!string.IsNullOrWhiteSpace(key))
                        _sb.Append(".").Append(key);
                    else
                    {
                        key = tokenTree[new StringToken("ID", true)];
                        _sb.Append("#").Append(key);
                    }
                }
                _sb.Append(" {").AppendLine();
                AddProperties(styles, parameters, tokenTree);
                _sb.Append("}").AppendLine();
            }
            _sb.Append("</style>").AppendLine();
        }

        /// <summary>
        /// Add the list of properties for a style.
        /// </summary>
        /// <param name="styles">The styles, needed in case the current style has a BasedOn property.</param>
        /// <param name="parameters">Calculation parameters.</param>
        /// <param name="style">The style being added.</param>
        private void AddProperties(IReadOnlyList<TokenTree> styles, TokenTree parameters, TokenTree style)
        {
            foreach (TokenTree child in style.Children.Where(x => x.Name != "Name"))
                AddProperty(parameters, child, styles);
        }

        /// <summary>
        /// Adds a property for a styles.
        /// </summary>
        /// <param name="parameters">Calculation parameters.</param>
        /// <param name="property">The property being added.</param>
        /// <param name="styles">The styles, needed in case the current style has a BasedOn property.</param>
        private void AddProperty(TokenTree parameters, TokenTree property, IReadOnlyList<TokenTree> styles)
        {
            if (property.Name == "BasedOn")
            {
                property = styles.FirstOrDefault(x => x[new StringToken("Name", true)] == property.Value.ToString());
                AddProperties(styles, parameters, property);
            }
            else
            {
                _sb.Append("  ")
                    .Append(property.Name.CamelCaseToHyphenated())
                    .Append(": ")
                    .Append(ProcessTokens(property.Value, new TokenTreeList(parameters)))
                    .Append(";")
                    .AppendLine();
            }
        }

        /// <summary>
        /// Evaluates a token.
        /// </summary>
        /// <param name="value">The token to process.</param>
        /// <param name="parameters">Calculation parameters.</param>
        /// <returns>The processed token.</returns>
        private static string ProcessTokens(IToken value, TokenTreeList parameters)
        {
            return value.Evaluate(parameters, false).ToString();
        }
    }
}
