using System.Web.Mvc;

namespace Rp3.AgendaComercial.Web.Areas.Ruta
{
    public class RutaAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Ruta";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Ruta_default",
                "Ruta/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new string[] { "Rp3.AgendaComercial.Web.Ruta.Controllers" }
            );
        }
    }
}
