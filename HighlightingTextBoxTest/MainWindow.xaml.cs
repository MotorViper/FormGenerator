using System.IO;
using FormGenerator.Tools;
using Helpers;

namespace Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            IOCContainer.Instance.Register<IHighlighter, VttHighlighter>().AsSingleton();
            InitializeComponent();
            mc_box.Text = File.ReadAllText(@"C:\Development\Projects\FormGenerator\FormGenerator\Data\Fields.vtt");
        }
    }
}
