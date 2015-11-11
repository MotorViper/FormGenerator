using System.Linq;
using System.Text;
using TextParser;

namespace FormGenerator.Fields
{
    public class Continuation : Field
    {
        public Continuation(Field parent, TokenTree data, int level, StringBuilder builder) : base(parent, "", data, level, builder)
        {
        }

        protected internal override void AddStart(string endOfLine, TokenTree parameters)
        {
            foreach (var child in Children.Where(child => child.Name == "Inputs"))
                parameters.Replace(child);
        }

        protected internal override void AddEnd(string endOfLine)
        {
        }

        protected internal override void AddChildProperties(Field child, TokenTree parameters)
        {
            Parent.AddChildProperties(child, parameters);
        }
    }
}
