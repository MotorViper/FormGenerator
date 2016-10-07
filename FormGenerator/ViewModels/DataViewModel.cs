using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using FormGenerator.Models;
using FormGenerator.Tools;
using Helpers;
using Microsoft.Win32;
using TextParser;

namespace FormGenerator.ViewModels
{
    public class DataViewModel : ViewModel
    {
        private readonly IXamlTokenData _data = IOCContainer.Instance.Resolve<IXamlTokenData>();
        private readonly string _dataName;
        private readonly NotifyingProperty<string> _selected = new NotifyingProperty<string>();
        private readonly Lazy<ILogging> _logger = IOCContainer.Instance.LazyResolve<ILogging>();

        private ILogging Logger => _logger.Value;

        /// <summary>
        /// Constructor.
        /// </summary>
        public DataViewModel()
        {
            if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                try
                {
                    _dataName = ConfigurationManager.AppSettings.Get("DataName");
                    if (!_data.HasMainData)
                    {
                        _data.DefaultDirectory = ConfigurationManager.AppSettings.Get("DataDirectory");
                        _data.MainDataFile = ConfigurationManager.AppSettings.Get("DataFile");
                    }
                    Keys = new List<string>();
                    foreach (TokenTree child in _data.MainData.Children)
                        Keys.Add(child.Name);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error Reading Data", MessageBoxButton.OK);
                    Application.Current.Shutdown();
                }
                Instance = this;
            }
        }

        /// <summary>
        /// The class that will display the generated xaml.
        /// </summary>
        public IDataDisplayer Displayer { get; set; }

        /// <summary>
        /// Single instance of this class.
        /// </summary>
        public static DataViewModel Instance { get; set; }

        // ReSharper disable once CollectionNeverQueried.Global - used in View
        /// <summary>
        /// The names of the items that can be selected to display.
        /// </summary>
        public List<string> Keys { get; }

        // ReSharper disable once UnusedMember.Global - used in View.
        /// <summary>
        /// The currently selected item's name.
        /// </summary>
        public string Selected
        {
            get { return _selected.GetValue(); }
            set
            {
                if (_data.HasStaticData)
                {
                    Logger?.Reset();
                    if (Values != null)
                        Values.Children.PropertyChanged -= OnChildrenChanged;
                    _selected.SetValue(value, this);
                    Values = _data.MainData.FindFirst(Selected);
                    TokenTree parameters = new TokenTree(_data.StaticData.GetChildren("Parameters"));
                    TokenTree defaults = parameters.FindFirst("Defaults." + _dataName);
                    Values.AddMissing(defaults);
                    Values.SetParameters(parameters);
                    Values.Children.PropertyChanged += OnChildrenChanged;
                    OnPropertyChanged("Values");
                }
            }
        }

        // ReSharper disable once UnusedMember.Global - used in View
        /// <summary>
        /// The items that can be displayed.
        /// </summary>
        public TokenTree Values { get; private set; }

        /// <summary>
        /// The xaml that is to be displayed.
        /// </summary>
        public string Xaml
        {
            get
            {
                _data.DefaultDirectory = ConfigurationManager.AppSettings.Get("DataDirectory");
                _data.StaticDataFile = ConfigurationManager.AppSettings.Get("StaticData");
                return _data.Xaml;
            }
        }

        /// <summary>
        /// Display the xaml.
        /// </summary>
        public void DisplayData()
        {
            Displayer.DisplayData(Xaml);
        }

        /// <summary>
        /// Called when the selected item changes, this will update the screen.
        /// </summary>
        private void OnChildrenChanged(object sender, PropertyChangedEventArgs args)
        {
            OnPropertyChanged("Values");
        }

        /// <summary>
        /// Saves the generated xaml to a file.
        /// </summary>
        public void SaveXaml()
        {
            SaveFileDialog s = new SaveFileDialog
            {
                Filter = "Xaml|*.xaml",
                RestoreDirectory = true
            };
            if (s.ShowDialog() == true)
            {
                string path = s.FileName;
                Regex regex = new Regex("Content=\"\\{Binding Values, Converter=\\{StaticResource DataConverter\\}, Mode=OneWay, ConverterParameter=([0-9]+)\\}");
                string xaml = regex.Replace(Xaml, "Content=\"$1");
                File.WriteAllText(path, xaml);
            }
        }
    }
}
