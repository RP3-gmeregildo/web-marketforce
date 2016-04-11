using Rp3.Web.Http;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace Rp3.AgendaComercial.Web
{
    // Nota: para obtener instrucciones sobre cómo habilitar el modo clásico de IIS6 o IIS7, 
    // visite http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            Rp3.Web.Mvc.AppStart.Initialize(this);

            ModelBinders.Binders.DefaultBinder = new DevExpress.Web.Mvc.DevExpressEditorsBinder();
            
            AreaRegistration.RegisterAllAreas();

            GlobalConfiguration.Configuration.MessageHandlers.Add(new CompressHandler());
            GlobalConfiguration.Configuration.MessageHandlers.Add(new DecompressionHandler());

            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes); 
        }

        protected void Application_Error()
        {
            Exception lastException = Server.GetLastError();
            //var logger = DependencyResolver.Current.GetService<ILogger>();
            //logger.Fatal(lastException);
        }

    }
}