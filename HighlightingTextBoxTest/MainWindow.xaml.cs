using System.Configuration;
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
            string testFile = ConfigurationManager.AppSettings.Get("TestFile");
            if (testFile != null && !testFile.Contains(":"))
            {
                string dataDirectory = ConfigurationManager.AppSettings.Get("DataDirectory");
                testFile = FileUtils.GetFullFileName(testFile, dataDirectory);
            }
            mc_box.Text = File.ReadAllText(testFile);
        }
    }
}
