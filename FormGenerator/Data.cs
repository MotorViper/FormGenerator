using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Windows;
using Helpers;
using TextParser;

namespace FormGenerator
{
    public class Data : ViewModel
    {
        private static TokenTree s_allValues;
        private static TokenTree s_staticData;
        private readonly string _dataName;
        private readonly NotifyingProperty<string> _selected = new NotifyingProperty<string>();

        public Data()
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

        // ReSharper disable once CollectionNeverQueried.Global - used in View
        public List<string> Keys { get; }

        // ReSharper disable once UnusedMember.Global - used in view.
        public string Selected
        {
            get { return _selected.GetValue(); }
            set
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

        private void OnChildrenChanged(object sender, PropertyChangedEventArgs args)
        {
            OnPropertyChanged("Values");
        }
    }
}
