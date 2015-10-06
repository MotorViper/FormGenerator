using System.Linq;
using TextParser;

namespace FormGenerator.Fields
{
    public class Continuation : Field
    {
        public Continuation(Field parent, TokenTree data = null, int level = -1) : base(parent, "", data, level)
        {
        }

        protected override void AddStart(string endOfLine, TokenTree parameters)
        {
            foreach (var child in Children.Where(child => child.Name == "Inputs"))
                parameters.Replace(child);
        }

        protected override void AddEnd(string endOfLine)
        {
        }

        protected internal override void AddChildProperties(Field child, TokenTree parameters)
        {
            Parent.AddChildProperties(child, parameters);
        }
    }
}
