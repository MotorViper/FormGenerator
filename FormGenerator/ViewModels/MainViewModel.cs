﻿using FormGenerator.Tools;
using Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using TextParser;
using TextParser.Tokens;
using TextParser.Tokens.Interfaces;

namespace FormGenerator.ViewModels
{
    public class MainViewModel : ViewModel, ILogging, ILogControl
    {
        private readonly DataViewModel _dataViewModel = new DataViewModel();
        private readonly NotifyingProperty<bool> _doLogging = new NotifyingProperty<bool>();
        private readonly Lazy<IEditorsView> _editorsViewModel = IOCContainer.Instance.LazyResolve<IEditorsView>();
        private readonly NotifyingProperty<string> _logData = new NotifyingProperty<string>();

        private ICommand _closeFileCommand;
        private ICommand _exitCommand;
        private ICommand _newFileCommand;
        private ICommand _openFileCommand;
        private ICommand _reloadCommand;
        private ICommand _saveAllFilesCommand;
        private ICommand _saveFileCommand;
        private ICommand _saveXamlCommand;
        private ICommand _testExpressionsCommand;

        private readonly Stack<bool> _loggingManagement = new Stack<bool>();

        public MainViewModel()
        {
            IOCContainer.Instance.Register<ILogging>(this);
            IOCContainer.Instance.Register<ILogControl>(this);
        }

        public ICommand CloseFileCommand
        {
            get { return _closeFileCommand ?? (_closeFileCommand = new RelayCommand(x => EditorsView.CloseFile())); }
        }

        public bool DoLogging
        {
            get { return _doLogging.GetValue(); }
            set { _doLogging.SetValue(value, this); }
        }

        /// <summary>
        /// Interface to editors view.
        /// </summary>
        private IEditorsView EditorsView => _editorsViewModel.Value;

        public ICommand ExitCommand
        {
            get { return _exitCommand ?? (_exitCommand = new RelayCommand(x => Exit(x as CancelEventArgs))); }
        }

        public string LogData
        {
            get { return _logData.GetValue(); }
            set { _logData.SetValue(value, this); }
        }

        public ICommand NewFileCommand => _newFileCommand ?? (_newFileCommand = new RelayCommand(x => EditorsView.NewFile()));
        public ICommand OpenFileCommand => _openFileCommand ?? (_openFileCommand = new RelayCommand(x => EditorsView.OpenFile()));
        public ICommand ReloadCommand => _reloadCommand ?? (_reloadCommand = new RelayCommand(x => Reload()));

        public ICommand SaveAllFilesCommand =>
            _saveAllFilesCommand ??
            (_saveAllFilesCommand = new RelayCommand(x => EditorsView.SaveAllFiles(), x => EditorsView.Editors.AnyUnSaved()));

        public ICommand SaveFileCommand =>
            _saveFileCommand ??
            (_saveFileCommand = new RelayCommand(x => EditorsView.SaveFile(), x => !(EditorsView.SelectedEditor?.IsSaved ?? true)));

        public ICommand SaveXamlCommand =>
            _saveXamlCommand ??
            (_saveXamlCommand = new RelayCommand(x => _dataViewModel.SaveXaml()));

        public ICommand TestExpressionsCommand
        {
            get { return _testExpressionsCommand ?? (_testExpressionsCommand = new RelayCommand(x => TestExpressions())); }
        }

        public void SetLogging(bool loggingOn)
        {
            _loggingManagement.Push(DoLogging);
            DoLogging = loggingOn;
        }

        public void ResetLogging()
        {
            DoLogging = _loggingManagement.Pop();
        }

        /// <summary>
        /// Log an error.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="overview">Shortened version of the message.</param>
        public void LogError(string message, string overview)
        {
            if (DoLogging)
                LogData += $"{DateTime.Now.TimeOfDay}: {overview}[E]: {message}\n";
        }

        /// <summary>
        /// Log an informational message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="overview">Shortened version of the message.</param>
        public void LogMessage(string message, string overview, int offset = 0)
        {
            if (DoLogging)
                OutputLogMessage(message, overview, offset);
        }

        /// <summary>
        /// Reset the logger if relevant.
        /// </summary>
        public void Reset()
        {
            LogData = "";
        }

        private string _offsetSpaces = "";

        /// <summary>
        /// Outputs the log message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="overview">Shortened version of the message.</param>
        private void OutputLogMessage(string message, string overview, int offset)
        {
            Dispatcher dispatchObject = Application.Current.Dispatcher;
            if (dispatchObject == null || dispatchObject.CheckAccess())
            {
                if (offset > 0)
                    _offsetSpaces += "  ";
                LogData += $"{DateTime.Now.TimeOfDay}: {overview}[I]: {_offsetSpaces}{message}\n";
                if (offset < 0 && _offsetSpaces.Length > 1)
                    _offsetSpaces = _offsetSpaces.Substring(0, _offsetSpaces.Length - 2);
            }
            else
            {
                dispatchObject.Invoke(() => OutputLogMessage(message, overview, offset));
            }
        }

        /// <summary>
        /// Test any expressions marked with a Test flag.
        /// </summary>
        private void TestExpressions()
        {
            try
            {
                TokenTree data = DataViewModel.Instance.TestValues;
                foreach (TokenTree tokenTree in data.Children)
                {
                    string doTest = tokenTree[new StringToken("Test", true)] ?? "false";
                    if (bool.Parse(doTest))
                    {
                        bool logIt = bool.Parse(tokenTree[new StringToken("Debug", true)] ?? "true");
                        SetLogging(logIt);
                        IToken simplified = tokenTree.Value.Simplify();
                        OutputLogMessage($"{tokenTree.Key} = {simplified}", "Simplify Result", 0);
                        IToken result = simplified.Evaluate(new TokenTreeList { data, DataConverter.Parameters }, true);
                        OutputLogMessage($"{tokenTree.Key} = {result}", "Test Result", 0);
                        ResetLogging();
                    }
                }
            }
            catch (Exception ex)
            {
                SetLogging(true);
                LogError(ex.Message, "Exception Thrown");
                ResetLogging();
            }
        }

        private void Exit(CancelEventArgs args)
        {
            if (EditorsView.Editors.AnyUnSaved())
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
            string current = DataViewModel.Instance.Selected;
            DataViewModel.Instance.InvalidateData();
            DataViewModel.Instance.Displayer.DisplayData(DataViewModel.Instance.Xaml);
            DataViewModel.Instance.Selected = current;
        }
    }
}
