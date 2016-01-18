using System.Collections.Generic;
using System.Linq;
using Generator;
using Helpers;
using TextParser;
using TextParser.Tokens;

namespace FormGenerator.Tools
{
    public class XamlGenerator
    {
        private readonly IFieldWriter _sb = IOCContainer.Instance.Resolve<IFieldWriter>();

        /// <summary>
        /// Generates the xaml that is to be displayed.
        /// </summary>
        /// <param name="data">The data the xaml is constructed from.</param>
        /// <returns>The xaml representation of the input.</returns>
        public string GenerateXaml(TokenTree data)
        {
            if (_sb.Generated.Length <= 0)
            {
                TokenTree parameters = new TokenTree(data.GetChildren("Parameters"));
                DataConverter.Parameters = parameters;
                foreach (TokenTree child in parameters.Children)
                {
                    string name = child.Name;
                    TokenTree defaults = parameters.FindFirst("Defaults." + name);
                    if (defaults != null)
                        foreach (TokenTree item in child.Children)
                            item.AddMissing(defaults);
                }
                TokenTreeList fields = data.GetAll("Fields");
                TokenTreeList styles = data.GetAll("Styles");
                _sb.Append(
                    "<Border HorizontalAlignment=\"Stretch\" VerticalAlignment=\"Stretch\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">")
                    .AppendLine();
                if (styles != null && styles.Count > 0)
                    AddStyles(styles, parameters);
                foreach (TokenTree field in fields.SelectMany(child => child.Children))
                    _sb.AddElement(field, 1, parameters);
                _sb.Append("</Border>").AppendLine();
            }
            return _sb.Generated;
        }

        /// <summary>
        /// Adds style information.
        /// </summary>
        /// <param name="styles">List of styles to add.</param>
        /// <param name="parameters">Calculation parameters.</param>
        private void AddStyles(IReadOnlyList<TokenTree> styles, TokenTree parameters)
        {
            _sb.Append("  <Border.Resources>").AppendLine();
            foreach (TokenTree tokenTree in styles[0].Children)
            {
                _sb.Append("    <Style ");

                IToken targetType = tokenTree.Value;
                bool hasType = false;
                if (!string.IsNullOrWhiteSpace(targetType.Text))
                {
                    _sb.Append("TargetType=\"{x:Type ").Append(targetType.Text).Append("}\" ");
                    hasType = true;
                }

                IToken key = tokenTree.Key;
                if (!string.IsNullOrWhiteSpace(key.Text))
                    _sb.Append("x:Key=\"").Append(key.Text).Append("\" ");

                string basedOn = tokenTree["BasedOn"];
                if (!string.IsNullOrWhiteSpace(basedOn))
                    _sb.Append("BasedOn=\"{StaticResource ").Append(basedOn).Append("}\" ");

                _sb.Append(">").AppendLine();

                foreach (TokenTree child in tokenTree.Children.Where(x => x.Name != "Name" && x.Name != "BasedOn"))
                {
                    _sb.Append("      <Setter Property=\"");
                    if (!hasType)
                        _sb.Append("Control.");
                    _sb.Append(child.Name).Append("\" Value=\"")
                        .Append(ProcessTokens(child.Value, new TokenTreeList(parameters))).Append("\"/>").AppendLine();
                }
                _sb.Append("    </Style>").AppendLine();
            }
            _sb.Append("  </Border.Resources>").AppendLine();
        }

        /// <summary>
        /// Evaluates a token.
        /// </summary>
        /// <param name="value">The token to process.</param>
        /// <param name="parameters">Calculation parameters.</param>
        /// <returns>The processed token.</returns>
        private static string ProcessTokens(IToken value, TokenTreeList parameters)
        {
            return value.Evaluate(parameters, false).Text;
        }
    }
}
