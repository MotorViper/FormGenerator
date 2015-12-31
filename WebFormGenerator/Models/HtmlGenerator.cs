using System.Linq;
using Generator;
using TextParser;

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

        public string GenerateHtml(TokenTree data, TokenTree selected, string dataName)
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
            foreach (TokenTree field in fields.SelectMany(child => child.Children))
                _sb.AddChild(field, 0, parameters, _offset, _endOfLine, null, null, values);
            return _sb.Generated;
        }
    }
}
