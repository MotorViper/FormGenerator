using TextParser;

namespace WebFormGenerator.Models
{
    public class Underline : Field
    {
        public Underline() : base("hr")
        {
        }

        public override void AddStart(string endOfLine, TokenTree parameters)
        {
            AppendStartOfLine(Level, "<hr/>");
        }

        public override void AddEnd(string endOfLine)
        {
        }

        protected override void AddChildren(TokenTree parameters, string endOfLine)
        {
        }
    }
}
