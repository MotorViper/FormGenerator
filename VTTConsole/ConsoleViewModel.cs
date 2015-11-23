using System;
using System.Collections.Generic;
using System.Windows.Input;
using Helpers;
using TextParser;

namespace VTTConsole
{
    public class ConsoleViewModel : ViewModel
    {
        private readonly NotifyingProperty<bool> _commandEntered = new NotifyingProperty<bool>();
        private readonly TokenGenerator _generator = new TokenGenerator();
        private readonly NotifyingProperty<string> _history = new NotifyingProperty<string>();
        private readonly List<string> _historyList = new List<string>();
        private readonly NotifyingProperty<string> _input = new NotifyingProperty<string>();
        private readonly NotifyingProperty<string> _output = new NotifyingProperty<string>();
        private readonly TokenTree _tree = new TokenTree();

        private ICommand _inputCommand;

        public bool CommandEntered
        {
            get { return _commandEntered.GetValue(); }
            set { _commandEntered.SetValue(value, this); }
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

        private void Execute(object command)
        {
            string input = command.ToString().Trim();

            try
            {
                if (input.StartsWith("#"))
                {
                    string text = input.Substring(1).Trim();
                    bool showHelp = true;
                    if (text.StartsWith("print"))
                    {
                        showHelp = false;
                        text = text.Substring(5).Trim();
                        Output += $"{Environment.NewLine}{text}: {_generator.Parse(text).Evaluate(new TokenTreeList(_tree), false)}";
                    }
                    else
                    {
                        int count;
                        if (int.TryParse(text, out count) && count <= _historyList.Count)
                        {
                            Execute(_historyList[count - 1]);
                            return;
                        }
                    }
                    if (showHelp)
                        Output +=
                            $"{Environment.NewLine}VTT expression - add VTT expression{Environment.NewLine}#print token - evaluate and print token{Environment.NewLine}#n - repeat nth command";
                }
                else
                {
                    TokenTree tree = Parser.ParseString(input);
                    Output += $"{Environment.NewLine}{tree.Key.Text}: {tree.Value.Evaluate(new TokenTreeList(_tree), false)}";
                    _tree.Replace(tree);
                }
                Input = "";
                _historyList.Add(input);
                History += $"{Environment.NewLine}{_historyList.Count}:\t{input}";
            }
            catch (Exception ex)
            {
                Output += $"{Environment.NewLine}{ex.Message}";
            }
            CommandEntered = true;
        }
    }
}
