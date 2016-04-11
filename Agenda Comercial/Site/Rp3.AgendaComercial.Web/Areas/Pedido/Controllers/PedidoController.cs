using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Rp3.AgendaComercial.Web.Pedido.Controllers
{
    public class PedidoController : AgendaComercial.Web.Controllers.AgendaComercialController
    {
        // GET: Pedido/Pedido
        [Rp3.Web.Mvc.Authorize("PEDIDO", "QUERY", "AGENDACOMERCIAL")]
        public ActionResult Index()
        {
            return View(GetListIndex());
        }

        [Rp3.Web.Mvc.Authorize("PEDIDO", "EDIT", "AGENDACOMERCIAL")]
        public ActionResult Edit(int id)
        {
            ViewBag.ReadOnly = false;
            return View();
        }

        [Rp3.Web.Mvc.Authorize("PEDIDO", "DETAIL", "AGENDACOMERCIAL")]
        public ActionResult Detail(int id)
        {
            ViewBag.ReadOnly = true;
            AgendaComercial.Models.Pedido.Pedido pedido = DataBase.Pedidos.GetSingleOrDefault(p => p.IdPedido == id);
            return View(pedido);
        }

        [Rp3.Web.Mvc.Authorize("PEDIDO", "NEW", "AGENDACOMERCIAL")]
        public ActionResult Create()
        {
            return View();
        }
        public List<AgendaComercial.Models.Pedido.Pedido> GetListIndex()
        {
            return DataBase.Pedidos.Get().ToList();
        }

        public ActionResult GridViewIndex()
        {
            return PartialView("_GridViewIndex", GetListIndex());
        }
        public ActionResult GridViewIndexDetalle()
        {
            return PartialView("_GridViewIndexDetalle", GetListIndex());
        }
    }
}