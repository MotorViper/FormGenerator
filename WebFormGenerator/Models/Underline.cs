using TextParser;

namespace WebFormGenerator.Models
{
    public class Underline : Field
    {
        public Underline() : base("hr")
        {
        }

        protected override void AddStart(string endOfLine, TokenTree parameters)
        {
            AppendStartOfLine(Level, "<hr/>");
        }

        protected override void AddEnd(string endOfLine)
        {
        }

        protected override void AddChildren(TokenTree parameters, string endOfLine)
        {
        }
    }
}
