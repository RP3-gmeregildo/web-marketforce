using System.Web.Mvc;

namespace Rp3.Web.Application.Definition
{
    public class DefinitionAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Definition";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Definition_default",
                "Definition/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new string[] { "Rp3.Web.Mvc.Application.Definition.Controllers" }
            );
        }
    }
}
