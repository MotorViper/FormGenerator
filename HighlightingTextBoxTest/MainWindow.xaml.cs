using System.IO;
using Helpers;
using TextParser;
using VTTUtilities;

namespace Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            TestInputData data = new TestInputData();
            IOCContainer.Instance.Register<IInputData>(data);
            IOCContainer.Instance.Register<IHighlighter, VttHighlighter>().AsSingleton();

            InitializeComponent();
            string testFile = data.MainDataFile;
            string dataDirectory = data.DefaultDirectory;
            testFile = FileUtils.GetFullFileName(testFile, dataDirectory);
            mc_box.Text = File.ReadAllText(testFile);
        }
    }
}
