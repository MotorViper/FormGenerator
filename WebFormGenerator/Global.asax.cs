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

            // Register the fields that can be displayed.
            IOCContainer.Instance.Register<IField, Underline>("UnderLine");
            IOCContainer.Instance.Register<IField, Selector>("Selector");
            IOCContainer.Instance.Register<IField, Select>("ComboBox");
            IOCContainer.Instance.Register<IField, Grid>("Grid");
            IOCContainer.Instance.Register<IField, Grid>("Border");
            IOCContainer.Instance.Register<IField, Table>("Table");
            IOCContainer.Instance.Register<IField, TextBox>("TextBox");
            IOCContainer.Instance.Register<IField, CheckBox>("CheckBox");
            IOCContainer.Instance.Register<IField, Field>();

            // Register the token data generator.
            IOCContainer.Instance.Register<IHtmlTokenData, TokenData>();

            // To use the html in the data directory to test the c# code.
            // Replace the line above with this.
            //IOCContainer.Instance.Register<IHtmlTokenData, PregeneratedHtmlTokenData>();

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}
