using System.Linq;
using System.Text;
using FormGenerator.Fields;
using TextParser;

namespace FormGenerator.Tools
{
    public class XamlGenerator
    {
        private readonly string _endOfLine;
        private readonly string _offset;
        private readonly StringBuilder _sb = new StringBuilder();

        public XamlGenerator(string endOfLine = "\n", string offset = "  ")
        {
            _endOfLine = endOfLine;
            _offset = offset;
        }

        public string GenerateXaml(TokenTree data)
        {
            TokenTree parameters = new TokenTree(data.GetChildren("Parameters"));
            DataConverter.Parameters = parameters;
            foreach (TokenTree child in parameters.Children)
            {
                string name = child.Name;
                TokenTree defaults = DataConverter.Parameters.FindFirst("Defaults." + name);
                if (defaults != null)
                    foreach (TokenTree item in child.Children)
                        item.AddMissing(defaults);
            }
            TokenTreeList fields = data.GetAll("Fields");
            foreach (TokenTree field in fields.SelectMany(child => child.Children))
                Field.AddChild(field, 0, parameters, _sb, _offset, _endOfLine);
            return _sb.ToString();
        }
    }
}
