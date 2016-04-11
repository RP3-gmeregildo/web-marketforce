using System.Web.Mvc;

namespace Rp3.AgendaComercial.Web.Areas.Oportunidad
{
    public class OportunidadAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Oportunidad";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                name:  "Oportunidad_default",
                url: "Oportunidad/{controller}/{action}/{id}",
                defaults: new { action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "Rp3.AgendaComercial.Web.Oportunidad.Controllers" }
            );
        }
    }
}