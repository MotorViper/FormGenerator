using System.ComponentModel;
using System.Windows;
using FormGenerator.Models;
using Helpers;

namespace FormGenerator.ViewModels
{
    public class EditorsViewModel : ViewModel
    {
        private static readonly NotifyingProperty<Editor> s_selectedEditor = new NotifyingProperty<Editor>();
        private static EditorList s_editors;

        public EditorsViewModel()
        {
            if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()) && s_editors == null)
                s_editors = new EditorList();
        }

        public EditorList Editors
        {
            get { return s_editors; }
            set { s_editors = value; }
        }

        public Editor SelectedEditor
        {
            get { return s_selectedEditor.GetValue(); }
            set { s_selectedEditor.SetValue(value, this); }
        }

        public void CloseFile()
        {
            Editors.Close(SelectedEditor);
        }

        public void NewFile()
        {
            SelectedEditor = Editors.New();
        }

        public void OpenFile()
        {
            Editor editor = Editors.Open();
            if (editor != null)
                SelectedEditor = editor;
        }

        public void SaveFile()
        {
            SelectedEditor.Save();
        }

        public void SaveAllFiles()
        {
            Editors.Save();
        }
    }
}
