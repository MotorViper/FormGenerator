using System.ComponentModel;
using System.Windows;
using FormGenerator.Models;
using Helpers;

namespace FormGenerator.ViewModels
{
    /// <summary>
    /// Interface for interaction with IOC.
    /// </summary>
    public interface IEditorsView
    {
        EditorList Editors { get; set; }
        Editor SelectedEditor { get; set; }
        void CloseFile();
        void NewFile();
        void OpenFile();
        void SaveAllFiles();
        void SaveFile();
    }

    public class EditorsViewModel : ViewModel, IEditorsView
    {
        private readonly NotifyingProperty<Editor> _selectedEditor = new NotifyingProperty<Editor>();

        public EditorsViewModel()
        {
            if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()) && Editors == null)
            {
                IOCContainer.Instance.Register<IEditorsView>(this);
                Editors = new EditorList();
            }
        }

        public EditorList Editors { get; set; }

        public Editor SelectedEditor
        {
            get { return _selectedEditor.GetValue(); }
            set { _selectedEditor.SetValue(value, this); }
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
