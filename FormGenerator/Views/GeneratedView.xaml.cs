using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using FormGenerator.Tools;
using FormGenerator.ViewModels;

namespace FormGenerator.Views
{
    /// <summary>
    /// Interaction logic for GeneratedView.xaml
    /// </summary>
    public partial class GeneratedView : IDataDisplayer
    {
        public GeneratedView()
        {
            InitializeComponent();
            if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                try
                {
                    DataViewModel.Instance.Displayer = this;
                    DataViewModel.Instance.DisplayData();
                }
                catch (Exception e)
                {
                    try
                    {
                        DisplayData($"<Label xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Content=\"{e.Message}\" " +
                                    "Foreground=\"Red\"/>");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        public void DisplayData(string xaml)
        {
            using (MemoryStream stream = new MemoryStream())
            using (StreamWriter writer = new StreamWriter(stream))
            {
                writer.Write(xaml);
                writer.Flush();
                stream.Position = 0;
                UIElement rootElement = (UIElement)XamlReader.Load(stream);
                mc_window.Content = rootElement;
            }
        }
    }
}
