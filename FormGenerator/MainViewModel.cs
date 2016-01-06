using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using FormGenerator.ViewModels;
using Helpers;

namespace FormGenerator
{
    public class MainViewModel : ViewModel
    {
        private readonly DataViewModel _dataViewModel = new DataViewModel();
        private readonly EditorsViewModel _editorsViewModel = new EditorsViewModel();

        private ICommand _closeFileCommand;
        private ICommand _exitCommand;
        private ICommand _newFileCommand;
        private ICommand _openFileCommand;
        private ICommand _reloadCommand;
        private ICommand _saveAllFilesCommand;
        private ICommand _saveFileCommand;
        private ICommand _saveXamlCommand;

        public ICommand CloseFileCommand
        {
            get { return _closeFileCommand ?? (_closeFileCommand = new RelayCommand(x => _editorsViewModel.CloseFile())); }
        }

        public ICommand ExitCommand
        {
            get { return _exitCommand ?? (_exitCommand = new RelayCommand(x => Exit(x as CancelEventArgs))); }
        }

        public ICommand NewFileCommand => _newFileCommand ?? (_newFileCommand = new RelayCommand(x => _editorsViewModel.NewFile()));
        public ICommand OpenFileCommand => _openFileCommand ?? (_openFileCommand = new RelayCommand(x => _editorsViewModel.OpenFile()));
        public ICommand ReloadCommand => _reloadCommand ?? (_reloadCommand = new RelayCommand(x => Reload()));

        public ICommand SaveAllFilesCommand
            =>
                _saveAllFilesCommand ??
                (_saveAllFilesCommand = new RelayCommand(x => _editorsViewModel.SaveAllFiles(), x => _editorsViewModel.Editors.AnyUnSaved()));

        public ICommand SaveFileCommand
            =>
                _saveFileCommand ??
                (_saveFileCommand = new RelayCommand(x => _editorsViewModel.SaveFile(), x => !(_editorsViewModel.SelectedEditor?.IsSaved ?? true)));

        public ICommand SaveXamlCommand
            =>
                _saveXamlCommand ??
                (_saveXamlCommand = new RelayCommand(x => _dataViewModel.SaveXaml()));

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
