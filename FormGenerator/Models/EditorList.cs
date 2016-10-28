using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Microsoft.Win32;

namespace FormGenerator.Models
{
    /// <summary>
    /// List of editors currently open.
    /// </summary>
    public class EditorList : ObservableCollection<Editor>
    {
        public bool AnyUnSaved()
        {
            return this.Any(editor => !editor.IsSaved);
        }

        public void Close(Editor editor)
        {
            if (!editor.IsSaved)
            {
                MessageBoxResult result = MessageBox.Show("Do you wish to save the data?", "Data Unsaved", MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    editor.Save();
                    Remove(editor);
                }
                else if (result == MessageBoxResult.No)
                {
                    Remove(editor);
                }
            }
            else
            {
                Remove(editor);
            }
        }

        public Editor New()
        {
            Editor editor = new Editor();
            Add(editor);
            return editor;
        }

        public Editor Open()
        {
            OpenFileDialog o = new OpenFileDialog
            {
                Filter = "VTT|*.vtt|All files|*.*",
                Multiselect = false
            };
            if (o.ShowDialog() == true)
            {
                Editor editor = new Editor(o.FileName);
                string fileName = editor.FileName;
                Editor found = Items.FirstOrDefault(item => item.FileName == fileName);
                if (found == null)
                {
                    Add(editor);
                }
                else if (MessageBox.Show("Do you wish to reload the data?", "File Open", MessageBoxButton.YesNo, MessageBoxImage.Question) ==
                         MessageBoxResult.Yes)
                {
                    Remove(found);
                    Add(editor);
                }
                else
                {
                    editor = found;
                }
                return editor;
            }
            return null;
        }

        public void Save()
        {
            foreach (Editor editor in this.Where(editor => !editor.IsSaved))
                editor.Save();
        }
    }
}
