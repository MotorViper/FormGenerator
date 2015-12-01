using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace FormGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                try
                {
                    string xaml = new Data().Xaml;
                    DisplayXaml(xaml);
                }
                catch (Exception e)
                {
                    try
                    {
                        DisplayXaml($"<Label xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Content=\"{e.Message}\" " +
                                    "Foreground=\"Red\"/>");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, ex.Message);
                    }
                }
            }
        }

        private void DisplayXaml(string xaml)
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

        private void TextChangedEventHandler(object sender, TextChangedEventArgs e)
        {
            if (mc_textInput.Document == null)
                return;

            Editor.Instance.UpdateFormats(mc_textInput);
            mc_textInput.TextChanged -= TextChangedEventHandler;
            Editor.Instance.Format();
            mc_textInput.TextChanged += TextChangedEventHandler;
        }
    }
}
