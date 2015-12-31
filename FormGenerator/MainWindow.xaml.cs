using FormGenerator.Fields;
using Generator;
using Helpers;

namespace FormGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            IOCContainer ioc = IOCContainer.Instance;
            ioc.Register<IField, CheckBox>("CheckBox");
            ioc.Register<IField, ComboBox>("ComboBox");
            ioc.Register<IField, Continuation>("Continuation");
            ioc.Register<IField, Grid>("Grid");
            ioc.Register<IField, Selector>("Selector");
            ioc.Register<IField, Table>("Table");
            ioc.Register<IField, TextBox>("TextBox");
            ioc.Register<IField, Field>();

            InitializeComponent();
        }
    }
}
