using System.Web.Mvc;

namespace Rp3.Web.Application.Base
{
    public class _BaseAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Base";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Base_default",
                "Generals/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new string[] { "Rp3.Web.Mvc.Application.Base.Controllers", "Rp3.Web.Mvc" }
            );
        }
    }
}
