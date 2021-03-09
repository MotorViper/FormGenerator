﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Helpers;
using TextParser;
using TextParser.Tokens.Interfaces;

namespace VTTConsole
{
    /// <summary>
    /// ViewModel for the console.
    /// </summary>
    public class ConsoleViewModel : ViewModel
    {
        private readonly NotifyingProperty<bool> _commandEntered = new NotifyingProperty<bool>();
        private readonly NotifyingProperty<string> _cursor = new NotifyingProperty<string>(">");
        private readonly NotifyingProperty<string> _history = new NotifyingProperty<string>();
        private readonly NotifyingProperty<string> _input = new NotifyingProperty<string>();
        private readonly Dictionary<string, Action> _noParameterActions = new Dictionary<string, Action>();
        private readonly NotifyingProperty<string> _output = new NotifyingProperty<string>();
        private readonly Dictionary<string, Action<string>> _parameteredActions = new Dictionary<string, Action<string>>();

        private ICommand _downCommand;
        private string _helpCommand;
        private List<string> _historyList = new List<string>();
        private ICommand _inputCommand;
        private Parser _parser = new Parser();
        private int _position;
        private string _text = "";
        private ICommand _upCommand;

        public ConsoleViewModel()
        {
            _helpCommand = "token: definition - add / replace token definition (- as last character for continuation)";
            _helpCommand += Environment.NewLine + "n - repeat nth command";
            RegisterCommand("[c]lear - clear the structure", () => _parser = new Parser());
            RegisterCommand("[ch] clearhistory - clear the history", ClearHistory);
            RegisterCommand("[cs] clearscreen - clear the history", ClearScreen);
            RegisterCommand("[d]elete <token> - remove item from structure", DeleteItem);
            RegisterCommand("[da] deleteall <token> - remove all items matching token", DeleteItems);
            RegisterCommand("[h]elp - this message", ShowHelp);
            RegisterCommand("[i]nsert <expression> - insert item into structure", InsertItem);
            RegisterCommand("[p]rint - print structure", PrintStructure);
            RegisterCommand("[p]rint <expression> - evaluate and print expression", PrintExpression);
            RegisterCommand("[r]ead <file> - read structure from a file", ReadStructure);
            RegisterCommand("[s]ave <file> - save structure to file", SaveStructure);
            RegisterCommand("e[x]it - exit", Application.Current.Shutdown);
        }

        public bool CommandEntered
        {
            get { return _commandEntered.GetValue(); }
            set { _commandEntered.SetValue(value, this); }
        }

        public string Cursor
        {
            get { return _cursor.GetValue(); }
            set { _cursor.SetValue(value, this); }
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

        public ICommand InputCommand => _inputCommand ?? (_inputCommand = new RelayCommand(x => Execute(x.ToString())));

        public string Output
        {
            get { return _output.GetValue(); }
            set { _output.SetValue(value, this); }
        }

        private TokenTree Tree => _parser.ParsedTree;

        public ICommand UpCommand
        {
            get { return _upCommand ?? (_upCommand = new RelayCommand(x => UpEntered())); }
        }

        private void RegisterCommand(string helpString, Action action)
        {
            _helpCommand += Environment.NewLine + helpString;
            string[] bits = helpString.Split(new[] { '-' }, 2);
            string command = bits[0];
            bits = command.Split(new[] { ' ' }, 2);
            if (!string.IsNullOrEmpty(bits[1]))
            {
                _noParameterActions[bits[0].Trim('[', ']', ' ')] = action;
                _noParameterActions[bits[1].Trim()] = action;
            }
            else
            {
                bits = command.Split(new[] { ']', '[' }, 3);
                _noParameterActions[bits[bits.Length - 2].Trim('[', ']')] = action;
                _noParameterActions[command.Replace("[", "").Replace("]", "".Trim())] = action;
            }
        }

        private void RegisterCommand(string helpString, Action<string> action)
        {
            _helpCommand += Environment.NewLine + helpString;
            string[] bits = helpString.Split(new[] { '<' }, 2);
            string command = bits[0];
            bits = command.Split(new[] { ' ' }, 2);
            if (!string.IsNullOrEmpty(bits[1]))
            {
                _parameteredActions[bits[0].Trim('[', ']', ' ')] = action;
                _parameteredActions[bits[1].Trim()] = action;
            }
            else
            {
                bits = command.Split(new[] { ']' }, 2);
                _parameteredActions[bits[0].Trim('[', ']')] = action;
                _parameteredActions[command.Replace("[", "").Replace("]", "".Trim())] = action;
            }
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

        private void EvaluateLine()
        {
            TokenTree tree = _parser.AddLine(new Line(_text));
            UpdateOutput($"{tree.Key}: {tree.Value.Evaluate(new TokenTreeList(Tree), false)}");
            _text = "";
            Cursor = ">";
        }

        private void Execute(string command)
        {
            string input = command.Trim();

            try
            {
                if (_text != "" || input.Contains(":"))
                {
                    bool hasContinuation = false;
                    if (input.EndsWith("-"))
                    {
                        Cursor = "->";
                        _text += input.Substring(0, input.Length - 1);
                        hasContinuation = true;
                    }
                    else
                    {
                        _text += input;
                    }
                    if (!hasContinuation)
                        EvaluateLine();
                }
                else
                {
                    string text = input.Trim();
                    bool done = false;
                    foreach (var action in _noParameterActions.Where(action => text == action.Key))
                    {
                        action.Value();
                        done = true;
                        break;
                    }
                    if (!done)
                    {
                        foreach (var action in _parameteredActions.Where(action => text.StartsWith(action.Key + " ")))
                        {
                            action.Value(text.Substring(action.Key.Length + 1));
                            done = true;
                            break;
                        }
                    }
                    if (!done)
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
            UpdateOutput($"{tree.Key}: {tree.Value.Evaluate(new TokenTreeList(Tree), false)}");
            Tree.Children.Add(tree);
        }

        private void DeleteItem(string input)
        {
            IToken token = TokenGenerator.Parse(input);
            Tree.Remove(token.ToString());
        }

        private void DeleteItems(string input)
        {
            IToken token = TokenGenerator.Parse(input);
            Tree.RemoveAll(token.ToString());
        }

        private void ClearHistory()
        {
            _historyList = new List<string>();
            History = "";
            _position = 0;
        }

        private void ClearScreen()
        {
            Output = "";
        }

        private void ShowHelp()
        {
            UpdateOutput(_helpCommand);
        }

        private void ReadStructure(string file)
        {
            string fileName = Path.GetFileName(file);
            string directory = Path.GetDirectoryName(file);
            TokenTree tree = Parser.ParseFile(fileName, directory);
            Tree.Children.AddRange(tree.Children);
        }

        private void SaveStructure(string file)
        {
            using (StreamWriter fs = new StreamWriter(file))
            {
                foreach (TokenTree tokenTree in Tree.Children)
                    // ReSharper disable once AccessToDisposedClosure
                    tokenTree.WalkTree((x, y) => fs.WriteLine($"{x}: {y}"));
            }
        }

        private void PrintStructure()
        {
            string delimiter = ("'-' * 32").Evaluate();
            UpdateOutput(delimiter);
            foreach (TokenTree tokenTree in Tree.Children)
                tokenTree.WalkTree((x, y) => UpdateOutput($"{x}:{y}"));
            UpdateOutput(delimiter);
        }

        private void PrintExpression(string text)
        {
            UpdateOutput($"{text}: {TokenGenerator.Parse(text).Evaluate(new TokenTreeList(Tree), false)}");
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
