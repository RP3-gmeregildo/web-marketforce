using System.Web.Mvc;

namespace Rp3.AgendaComercial.Web.Areas.Consulta
{
    public class ConsultaAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Consulta";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Consulta_default",
                "Consulta/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}