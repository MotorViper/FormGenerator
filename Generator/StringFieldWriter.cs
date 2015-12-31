using System.Text;
using TextParser;
using TextParser.Tokens;

namespace Generator
{
    public class StringFieldWriter : IFieldWriter
    {
        private readonly StringBuilder _sb = new StringBuilder();

        public IFieldWriter Append<T>(T s)
        {
            _sb.Append(s);
            return this;
        }

        public void AppendLine()
        {
            _sb.AppendLine();
        }

        public void AddChild(TokenTree data, int level, TokenTree parameters, string offset, string endOfLine, IField parent, IToken parameter,
            TokenTree selected)
        {
            IField field = FieldFactory.CreateField(data.Value.Text, data, level, parameters, parent);
            field.Parameter = parameter;
            field.Selected = selected;
            field.OutputField(this, level, parameters, offset, endOfLine);
        }

        public string Generated => _sb.ToString();
    }
}
