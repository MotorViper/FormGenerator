using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
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
            string dataFile = ConfigurationManager.AppSettings.Get("DataFile");
            _dataName = ConfigurationManager.AppSettings.Get("DataName");
            if (s_allValues == null)
                s_allValues = Parser.ParseFile(dataFile);
            Keys = new List<string>();
            foreach (TokenTree child in s_allValues.Children)
                Keys.Add(child.Name);
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
                string dataFile = ConfigurationManager.AppSettings.Get("StaticData");
                s_staticData = Parser.ParseFile(dataFile);
                return new XamlGenerator().GenerateXaml(s_staticData);
            }
        }

        private void OnChildrenChanged(object sender, PropertyChangedEventArgs args)
        {
            OnPropertyChanged("Values");
        }
    }
}
