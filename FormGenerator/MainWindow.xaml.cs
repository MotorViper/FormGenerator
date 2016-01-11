using FormGenerator.Fields;
using FormGenerator.Models;
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

            // Register the fields that can be displayed.
            ioc.Register<IField, CheckBox>("CheckBox");
            ioc.Register<IField, ComboBox>("ComboBox");
            ioc.Register<IField, Grid>("Grid");
            ioc.Register<IField, Selector>("Selector");
            ioc.Register<IField, Table>("Table");
            ioc.Register<IField, TextBox>("TextBox");
            ioc.Register<IField, Field>();

            // Register the token data generator.
            ioc.Register<IXamlTokenData, TokenData>();

            // To use the xaml in the data directory to test the c# code.
            // Replace the line above with this.
            //ioc.Register<IXamlTokenData, PregeneratedXamlTokenData>();

            InitializeComponent();
        }
    }
}
