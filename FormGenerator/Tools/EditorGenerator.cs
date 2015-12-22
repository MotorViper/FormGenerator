using System.Windows.Controls;

namespace FormGenerator.Tools
{
    public class EditorGenerator
    {
        public static IEditor CreateEditor(object baseItem)
        {
            RichTextBox richTextBox = baseItem as RichTextBox;
            if (richTextBox != null)
                return new RichTextBoxEditor(richTextBox);
            return null;
        }
    }
}
