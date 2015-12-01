using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Helpers;
using TextParser;

namespace FormGenerator
{
    public class Data : ViewModel
    {
        private static TokenTree s_allValues;
        private static TokenTree s_staticData;

        private readonly string _dataName;

        private readonly NotifyingProperty<string> _editorName = new NotifyingProperty<string>("Editor");
        private readonly NotifyingProperty<bool> _editorReady = new NotifyingProperty<bool>();
        private readonly NotifyingProperty<string> _selected = new NotifyingProperty<string>();

        private ICommand _closeFileCommand;
        private ICommand _exitCommand;
        private ICommand _newFileCommand;
        private ICommand _openFileCommand;
        private ICommand _reloadCommand;
        private ICommand _saveFileCommand;

        public Data()
        {
            if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                try
                {
                    _dataName = ConfigurationManager.AppSettings.Get("DataName");
                    if (s_allValues == null)
                    {
                        string dataDirectory = ConfigurationManager.AppSettings.Get("DataDirectory");
                        string dataFile = ConfigurationManager.AppSettings.Get("DataFile");
                        s_allValues = Parser.ParseFile(dataFile, dataDirectory);
                    }
                    Keys = new List<string>();
                    foreach (TokenTree child in s_allValues.Children)
                        Keys.Add(child.Name);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error Reading Data", MessageBoxButton.OK);
                    Application.Current.Shutdown();
                }
            }
        }

        public ICommand CloseFileCommand
        {
            get { return _closeFileCommand ?? (_closeFileCommand = new RelayCommand(x => CloseFile(x as RichTextBox))); }
        }

        public string EditorName
        {
            get { return _editorName.GetValue(); }
            set { _editorName.SetValue(value, this); }
        }

        public bool EditorReady
        {
            get { return _editorReady.GetValue(); }
            set { _editorReady.SetValue(value, this); }
        }

        public ICommand ExitCommand
        {
            get { return _exitCommand ?? (_exitCommand = new RelayCommand(x => Exit(x as Window))); }
        }

        // ReSharper disable once CollectionNeverQueried.Global - used in View
        public List<string> Keys { get; }

        public ICommand NewFileCommand => _newFileCommand ?? (_newFileCommand = new RelayCommand(x => NewFile(x as RichTextBox)));
        public ICommand OpenFileCommand => _openFileCommand ?? (_openFileCommand = new RelayCommand(x => OpenFile(x as RichTextBox)));
        public ICommand ReloadCommand => _reloadCommand ?? (_reloadCommand = new RelayCommand(x => Reload()));

        public ICommand SaveFileCommand
            => _saveFileCommand ?? (_saveFileCommand = new RelayCommand(x => SaveFile(x as RichTextBox), x => !Editor.Instance.IsSaved));

        // ReSharper disable once UnusedMember.Global - used in view.
        public string Selected
        {
            get { return _selected.GetValue(); }
            set
            {
                if (s_staticData != null)
                {
                    if (Values != null)
                        Values.Children.PropertyChanged -= OnChildrenChanged;
                    _selected.SetValue(value, this);
                    Values = s_allValues.FindFirst(Selected);
                    TokenTree parameters = new TokenTree(s_staticData.GetChildren("Parameters"));
                    TokenTree defaults = parameters.FindFirst("Defaults." + _dataName);
                    Values.AddMissing(defaults);
                    Values.SetParameters(parameters);
                    Values.Children.PropertyChanged += OnChildrenChanged;
                    OnPropertyChanged("Values");
                }
            }
        }

        // ReSharper disable once UnusedMember.Global - used in View
        public TokenTree Values { get; private set; }

        public string Xaml
        {
            get
            {
                string dataDirectory = ConfigurationManager.AppSettings.Get("DataDirectory");
                string dataFile = ConfigurationManager.AppSettings.Get("StaticData");
                s_staticData = Parser.ParseFile(dataFile, dataDirectory);
                return new XamlGenerator().GenerateXaml(s_staticData);
            }
        }

        private void CloseFile(RichTextBox box)
        {
            Editor.Instance.NewFile(box);
            EditorReady = false;
            EditorName = "Editor";
        }

        private void NewFile(RichTextBox box)
        {
            Editor.Instance.NewFile(box);
            EditorReady = true;
            EditorName = "Editor - ?";
        }

        private void OpenFile(RichTextBox box)
        {
            Editor.Instance.OpenFile(box);
            EditorReady = true;
            EditorName = $"Editor - {new FileInfo(Editor.Instance.CurrentFileName).Name}";
        }

        private void SaveFile(RichTextBox box)
        {
            Editor.Instance.SaveFile(box);
            EditorReady = true;
            EditorName = $"Editor - {new FileInfo(Editor.Instance.CurrentFileName).Name}";
        }

        private static void Exit(Window window)
        {
            if (!Editor.Instance.IsSaved)
            {
                MessageBoxResult res = MessageBox.Show("There are unsaved changes, do you still want to exit?", "Unsaved content", MessageBoxButton.YesNo,
                    MessageBoxImage.Exclamation);
                if (res == MessageBoxResult.Yes)
                    window.Close();
            }
        }

        private void Reload()
        {
        }

        private void OnChildrenChanged(object sender, PropertyChangedEventArgs args)
        {
            OnPropertyChanged("Values");
        }
    }
}
