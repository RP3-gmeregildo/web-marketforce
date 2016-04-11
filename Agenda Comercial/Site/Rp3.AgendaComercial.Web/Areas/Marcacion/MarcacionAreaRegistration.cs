using System.Web.Mvc;

namespace Rp3.AgendaComercial.Web.Areas.Marcacion
{
    public class MarcacionAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Marcacion";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                name: "Marcacion_default",
                url: "Marcacion/{controller}/{action}/{id}",
                defaults: new { action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "Rp3.AgendaComercial.Web.Marcacion.Controllers" }
            );
        }
    }
}