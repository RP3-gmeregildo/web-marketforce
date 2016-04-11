using System.Web.Mvc;

namespace Rp3.AgendaComercial.Web.Areas.General
{
    public class GeneralAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "General";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                name: "General_default",
                url: "General/{controller}/{action}/{id}",
                defaults: new { action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "Rp3.AgendaComercial.Web.General.Controllers" }
            );
        }
    }
}
