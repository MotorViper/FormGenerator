using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Helpers;
using TextParser;
using TextParser.Tokens;

namespace VTTConsole
{
    public class ConsoleViewModel : ViewModel
    {
        private readonly NotifyingProperty<bool> _commandEntered = new NotifyingProperty<bool>();
        private readonly NotifyingProperty<string> _history = new NotifyingProperty<string>();
        private readonly NotifyingProperty<string> _input = new NotifyingProperty<string>();
        private readonly NotifyingProperty<string> _output = new NotifyingProperty<string>();

        private ICommand _downCommand;
        private List<string> _historyList = new List<string>();
        private ICommand _inputCommand;
        private int _position;
        private TokenTree _tree = new TokenTree();
        private ICommand _upCommand;

        public bool CommandEntered
        {
            get { return _commandEntered.GetValue(); }
            set { _commandEntered.SetValue(value, this); }
        }

        public ICommand DownCommand
        {
            get { return _downCommand ?? (_downCommand = new RelayCommand(x => DownEntered())); }
        }

        public string History
        {
            get { return _history.GetValue(); }
            set { _history.SetValue(value, this); }
        }

        public string Input
        {
            get { return _input.GetValue(); }
            set { _input.SetValue(value, this); }
        }

        public ICommand InputCommand => _inputCommand ?? (_inputCommand = new RelayCommand(Execute));

        public string Output
        {
            get { return _output.GetValue(); }
            set { _output.SetValue(value, this); }
        }

        public ICommand UpCommand
        {
            get { return _upCommand ?? (_upCommand = new RelayCommand(x => UpEntered())); }
        }

        private void DownEntered()
        {
            _position += 1;
            if (_position >= _historyList.Count)
                _position = 0;
            ShowCommand();
        }

        private void UpEntered()
        {
            _position -= 1;
            if (_position < 0)
                _position = _historyList.Count - 1;
            ShowCommand();
        }

        private void ShowCommand()
        {
            if (_position >= 0 && _position < _historyList.Count)
                Input = _historyList[_position];
        }

        private void Execute(object command)
        {
            string input = command.ToString();

            try
            {
                if (input.StartsWith("#"))
                {
                    string text = input.Substring(1).Trim();
                    if (text == "print" || text == "p")
                    {
                        PrintStructure();
                    }
                    else if (text.StartsWith("print "))
                    {
                        PrintExpression(text.Substring(5).Trim());
                    }
                    else if (text.StartsWith("p "))
                    {
                        PrintExpression(text.Substring(1).Trim());
                    }
                    else if (text.StartsWith("delete "))
                    {
                        DeleteItem(text.Substring(6).Trim());
                    }
                    else if (text.StartsWith("d "))
                    {
                        DeleteItem(text.Substring(1).Trim());
                    }
                    else if (text.StartsWith("deleteall "))
                    {
                        DeleteItems(text.Substring(9).Trim());
                    }
                    else if (text.StartsWith("da "))
                    {
                        DeleteItems(text.Substring(2).Trim());
                    }
                    else if (text.StartsWith("insert "))
                    {
                        InsertItem(text.Substring(6).Trim());
                    }
                    else if (text.StartsWith("i "))
                    {
                        InsertItem(text.Substring(1).Trim());
                    }
                    else if (text.StartsWith("save "))
                    {
                        SaveStructure(text.Substring(4).Trim());
                    }
                    else if (text.StartsWith("s "))
                    {
                        SaveStructure(text.Substring(1).Trim());
                    }
                    else if (text.StartsWith("read "))
                    {
                        ReadStructure(text.Substring(4).Trim());
                    }
                    else if (text.StartsWith("r "))
                    {
                        ReadStructure(text.Substring(1).Trim());
                    }
                    else if (text == "exit" || text == "x")
                    {
                        Application.Current.Shutdown();
                    }
                    else if (text == "clear" || text == "c")
                    {
                        _tree = new TokenTree();
                    }
                    else if (text == "clearhistory" || text == "ch")
                    {
                        ClearHistory();
                    }
                    else if (text == "help" || text == "h")
                    {
                        ShowHelp();
                    }
                    else
                    {
                        int count;
                        if (int.TryParse(text, out count) && count <= _historyList.Count && count > 0)
                        {
                            Execute(_historyList[count - 1]);
                            return;
                        }
                        UpdateOutput($"Invalid command: {text}");
                    }
                }
                else
                {
                    TokenTree tree = Parser.ParseString(input);
                    UpdateOutput($"{tree.Key.Text}: {tree.Value.Evaluate(new TokenTreeList(_tree), false)}");
                    _tree.UpdateFirstLeaf(tree);
                }
                Input = "";
                UpdateHistory(input);
            }
            catch (Exception ex)
            {
                UpdateOutput(ex.Message);
            }
            CommandEntered = true;
        }

        private void InsertItem(string input)
        {
            TokenTree tree = Parser.ParseString(input);
            UpdateOutput($"{tree.Key.Text}: {tree.Value.Evaluate(new TokenTreeList(_tree), false)}");
            _tree.Children.Add(tree);
        }

        private void DeleteItem(string input)
        {
            IToken token = TokenGenerator.Parse(input);
            _tree.Remove(token.Text);
        }

        private void DeleteItems(string input)
        {
            IToken token = TokenGenerator.Parse(input);
            _tree.RemoveAll(token.Text);
        }

        private void ClearHistory()
        {
            _historyList = new List<string>();
            History = "";
            _position = 0;
        }

        private void ShowHelp()
        {
            UpdateOutput(@"expression - add/replace token definition
#n - repeat nth command
#[c]lear - clear the structure
#[ch] clearhistory - clear the history
#[d]elete token - remove item from structure
#[da] deleteall token - remove all items matching token
#[h]elp - this message
#[i]nsert expression - insert item into structure
#[p]rint - print structure
#[p]rint token - evaluate and print token
#[r]ead file - read structure from a file
#[s]ave file - save structure to file
#e[x]it - exit");
        }

        private void ReadStructure(string file)
        {
            TokenTree tree = Parser.ParseFile(file, "C:\\");
            _tree.Children.AddRange(tree.Children);
        }

        private void SaveStructure(string file)
        {
            using (StreamWriter fs = new StreamWriter(file))
            {
                foreach (TokenTree tokenTree in _tree.Children)
                    // ReSharper disable once AccessToDisposedClosure
                    tokenTree.WalkTree((x, y) => fs.WriteLine($"{x}: {y}"));
            }
        }

        private void PrintStructure()
        {
            string delimiter = ("'-' * 32").Evaluate();
            UpdateOutput(delimiter);
            foreach (TokenTree tokenTree in _tree.Children)
                tokenTree.WalkTree((x, y) => UpdateOutput($"{x}:{y}"));
            UpdateOutput(delimiter);
        }

        private void PrintExpression(string text)
        {
            UpdateOutput($"{text}: {TokenGenerator.Parse(text).Evaluate(new TokenTreeList(_tree), false)}");
        }

        private void UpdateHistory(string input)
        {
            _historyList.Add(input);
            History += $"{_historyList.Count}:\t{input}\n";
            _position = _historyList.Count;
        }

        private void UpdateOutput(string text)
        {
            Output += "\n" + text;
        }
    }
}
