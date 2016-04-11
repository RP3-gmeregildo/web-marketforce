using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Rp3.AgendaComercial.Web.Areas.Pedido
{
    public class PedidoAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Pedido";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                name: "Pedido_default",
                url: "Pedido/{controller}/{action}/{id}",
                defaults: new { action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "Rp3.AgendaComercial.Web.Pedido.Controllers" }
            );
        }
    }
}