using System;
using Helpers;

namespace Generator
{
    /// <summary>
    /// Factory for creating field objects.
    /// </summary>
    public static class FieldFactory
    {
        /// <summary>
        /// Creates a field from an element.
        /// </summary>
        /// <param name="element">The element representing the field.</param>
        /// <param name="level">The indentation level, for formatting output</param>
        /// <param name="parent">The parent field.</param>
        /// <returns>The new field.</returns>
        public static IField CreateField(IElement element, int level, IField parent)
        {
            string fieldName = element.ElementType;
            if (string.IsNullOrWhiteSpace(fieldName))
                throw new Exception("No field name given.");

            IField field = IOCContainer.Instance.Resolve<IField>(element.ElementType);
            if (field == null)
            {
                field = IOCContainer.Instance.Resolve<IField>();
                field.Name = fieldName;
            }
            field.Level = level;
            field.Parent = parent;
            field.Element = element;
            return field;
        }
    }
}
