using Generator;

namespace WebFormGenerator.Models
{
    /// <summary>
    /// Class used to represent generic fields.
    /// </summary>
    public class GenericField : Field, IFieldAdder
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GenericField() : base("")
        {
        }
    }
}
