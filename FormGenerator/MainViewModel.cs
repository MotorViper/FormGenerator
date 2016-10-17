using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using FormGenerator.ViewModels;
using Helpers;

namespace FormGenerator
{
    public class MainViewModel : ViewModel, ILogging, ILogControl
    {
        private readonly DataViewModel _dataViewModel = new DataViewModel();
        private readonly NotifyingProperty<bool> _doLogging = new NotifyingProperty<bool>();
        private readonly EditorsViewModel _editorsViewModel = new EditorsViewModel();
        private readonly NotifyingProperty<string> _logData = new NotifyingProperty<string>();

        private ICommand _closeFileCommand;
        private ICommand _exitCommand;
        private ICommand _newFileCommand;
        private ICommand _openFileCommand;
        private ICommand _reloadCommand;
        private ICommand _saveAllFilesCommand;
        private ICommand _saveFileCommand;
        private ICommand _saveXamlCommand;

        private bool _wasLogging;

        public MainViewModel()
        {
            IOCContainer.Instance.Register<ILogging>(this);
            IOCContainer.Instance.Register<ILogControl>(this);
        }

        public ICommand CloseFileCommand
        {
            get { return _closeFileCommand ?? (_closeFileCommand = new RelayCommand(x => _editorsViewModel.CloseFile())); }
        }

        public bool DoLogging
        {
            get { return _doLogging.GetValue(); }
            set { _doLogging.SetValue(value, this); }
        }

        public ICommand ExitCommand
        {
            get { return _exitCommand ?? (_exitCommand = new RelayCommand(x => Exit(x as CancelEventArgs))); }
        }

        public string LogData
        {
            get { return _logData.GetValue(); }
            set { _logData.SetValue(value, this); }
        }

        public ICommand NewFileCommand => _newFileCommand ?? (_newFileCommand = new RelayCommand(x => _editorsViewModel.NewFile()));
        public ICommand OpenFileCommand => _openFileCommand ?? (_openFileCommand = new RelayCommand(x => _editorsViewModel.OpenFile()));
        public ICommand ReloadCommand => _reloadCommand ?? (_reloadCommand = new RelayCommand(x => Reload()));

        public ICommand SaveAllFilesCommand =>
            _saveAllFilesCommand ??
            (_saveAllFilesCommand = new RelayCommand(x => _editorsViewModel.SaveAllFiles(), x => _editorsViewModel.Editors.AnyUnSaved()));

        public ICommand SaveFileCommand =>
            _saveFileCommand ??
            (_saveFileCommand = new RelayCommand(x => _editorsViewModel.SaveFile(), x => !(_editorsViewModel.SelectedEditor?.IsSaved ?? true)));

        public ICommand SaveXamlCommand =>
            _saveXamlCommand ??
            (_saveXamlCommand = new RelayCommand(x => _dataViewModel.SaveXaml()));

        public void SetLogging(bool loggingOn)
        {
            _wasLogging = DoLogging;
            DoLogging = loggingOn;
        }

        public void ResetLoggingToDefault()
        {
            DoLogging = _wasLogging;
        }

        /// <summary>
        /// Log an error.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="overview">Shortened version of the message.</param>
        public void LogError(string message, string overview)
        {
            if (DoLogging)
                LogData += $"{overview}[E]: {message}\n";
        }

        /// <summary>
        /// Log an informational message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="overview">Shortened version of the message.</param>
        public void LogMessage(string message, string overview)
        {
            if (DoLogging)
                LogData += $"{overview}[I]: {message}\n";
        }

        /// <summary>
        /// Reset the logger if relevant.
        /// </summary>
        public void Reset()
        {
            LogData = "";
        }

        private void Exit(CancelEventArgs args)
        {
            if (_editorsViewModel.Editors.AnyUnSaved())
            {
                MessageBoxResult res = MessageBox.Show("There are unsaved changes, do you still want to exit?", "Unsaved content", MessageBoxButton.YesNo,
                    MessageBoxImage.Exclamation);
                if (res == MessageBoxResult.Yes && args == null)
                    Application.Current.Shutdown();
                else if (res == MessageBoxResult.No && args != null)
                    args.Cancel = true;
            }
            else if (args == null)
            {
                Application.Current.Shutdown();
            }
        }

        private static void Reload()
        {
            DataViewModel.Instance.Displayer.DisplayData(DataViewModel.Instance.Xaml);
        }
    }
}
