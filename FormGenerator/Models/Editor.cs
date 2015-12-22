using System.IO;
using Helpers;
using Microsoft.Win32;

namespace FormGenerator.Models
{
    public class Editor : ViewModel
    {
        private readonly NotifyingProperty<string> _fileName = new NotifyingProperty<string>();
        private readonly NotifyingProperty<string> _fileText = new NotifyingProperty<string>();
        private readonly NotifyingProperty<bool> _isCurrent = new NotifyingProperty<bool>();

        private string _path;

        public Editor()
        {
            FileName = "?";
            _path = null;
            FileText = "";
            IsSaved = true;
            IsCurrent = true;
        }

        public Editor(string path)
        {
            FileName = new FileInfo(path).Name;
            _path = path;
            FileText = File.ReadAllText(path);
            IsSaved = true;
            IsCurrent = true;
        }

        public string FileName
        {
            get { return _fileName.GetValue(); }
            set { _fileName.SetValue(value, this); }
        }

        public string FileText
        {
            get { return _fileText.GetValue(); }
            set
            {
                IsSaved = false;
                _fileText.SetValue(value, this);
            }
        }

        public bool IsCurrent
        {
            get { return _isCurrent.GetValue(); }
            set { _isCurrent.SetValue(value, this); }
        }

        public bool IsSaved { get; private set; }

        public void Save()
        {
            if (_path == null)
            {
                SaveFileDialog s = new SaveFileDialog
                {
                    Filter = "VTT|*.vtt",
                    RestoreDirectory = true
                };
                if (s.ShowDialog() == true)
                {
                    FileName = new FileInfo(s.FileName).Name;
                    _path = s.FileName;
                }
            }
            File.WriteAllText(_path, FileText);
            IsSaved = true;
        }
    }
}
