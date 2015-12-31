using System.Linq;
using Generator;
using TextParser;

namespace FormGenerator.Fields
{
    // ReSharper disable once ClassNeverInstantiated.Global - used by IOC.
    public class Continuation : Field
    {
        public Continuation() : base("")
        {
        }

        public override void AddStart(string endOfLine, TokenTree parameters)
        {
            foreach (var child in Children.Where(child => IsParameter(child.Name)))
                parameters.Replace(child);
        }

        public override void AddEnd(string endOfLine)
        {
        }

        public override void AddChildProperties(IField child)
        {
            Parent.AddChildProperties(child);
        }
    }
}
