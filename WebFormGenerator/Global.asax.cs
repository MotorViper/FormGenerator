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
            IOCContainer.Instance.Register<IField, Field>("Continuation");
            IOCContainer.Instance.Register<IField, Grid>("Grid");
            IOCContainer.Instance.Register<IField, Field>();

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}
