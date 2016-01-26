using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Generator;
using Helpers;
using WebFormGenerator.Models;

namespace WebFormGenerator
{
    public class Global : HttpApplication
    {
        private void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            IOCContainer ioc = IOCContainer.Instance;

            // Register the fields that can be displayed.
            ioc.Register<IField, Underline>("UnderLine");
            ioc.Register<IField, Selector>("Selector");
            ioc.Register<IField, Select>("ComboBox");
            ioc.Register<IField, Grid>("Grid");
            ioc.Register<IField, Grid>("Border");
            ioc.Register<IField, Table>("Table");
            ioc.Register<IField, TextBox>("TextBox");
            ioc.Register<IField, CheckBox>("CheckBox");
            ioc.Register<IField, GenericField>();

            // Register the token data generator.
            ioc.Register<IHtmlTokenData, TokenData>();

            // To use the html in the data directory to test the c# code.
            // Replace the line above with this.
            //IOCContainer.Instance.Register<IHtmlTokenData, PregeneratedHtmlTokenData>();

            ioc.Register<IFieldWriter, StringFieldWriter<GenericField>>().AsSingleton();

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}
