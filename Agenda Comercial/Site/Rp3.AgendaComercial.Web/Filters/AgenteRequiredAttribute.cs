using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Rp3.AgendaComercial.Web
{
    public class AgenteRequiredAttribute :  FilterAttribute, IAuthorizationFilter 
    {
        public AgenteRequiredAttribute ()
        {

        }

        #region IAuthorizationFilter Members

        public void OnAuthorization(AuthorizationContext filterContext)
        {            
            if (!Rp3.Web.Mvc.Session.IsLogged)
            {
                filterContext.Result = new RedirectResult("~" + Rp3.Configuration.Rp3ConfigurationSection.Current.AccessDeniedRedirect.Url);
                return;
            }            

            AgendaComercial.Web.Controllers.AgendaComercialController controller = (AgendaComercial.Web.Controllers.AgendaComercialController)filterContext.Controller;
            
            if(!controller.Agente.EsAgente)
            {
                var url = new UrlHelper(filterContext.RequestContext);
                string urlString = url.Content("~/Home/NoAgente");

                filterContext.Result = new RedirectResult(urlString);
                return;
            }

            if (controller.Agente.CargoRol == null)
            {
                var url = new UrlHelper(filterContext.RequestContext);
                string urlString = url.Content("~/Home/SeleccionRol");
                
                filterContext.Result = new RedirectResult(urlString);
                return;
            }
                       
        }

        #endregion
    }
}