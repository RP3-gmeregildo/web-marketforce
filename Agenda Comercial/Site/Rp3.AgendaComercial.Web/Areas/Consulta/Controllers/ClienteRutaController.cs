using Rp3.AgendaComercial.Models.Ruta.View;
using Rp3.AgendaComercial.Web.Ruta;
using Rp3.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Rp3.AgendaComercial.Web.Areas.Consulta.Controllers
{
    public class ClienteRutaController : Rp3.AgendaComercial.Web.Controllers.AgendaComercialController
    {
        // GET: Consulta/ClienteRuta
        [Rp3.Web.Mvc.Authorize("CLIENTERUTA", "QUERY", "AGENDACOMERCIAL", Order = 0)]
        public ActionResult Index()
        {
            InicializarIndex();

            return View();
        }

        private void InicializarIndex()
        {
            ViewBag.Rutas = DataBase.Rutas.Get(p => p.Estado == Rp3.AgendaComercial.Models.Constantes.Estado.Activo).OrderBy(p=>p.Descripcion).ToList();
        }

        [ChildAction(Order = 1)]
        [Rp3.Web.Mvc.Authorize("CLIENTERUTA", "QUERY", "AGENDACOMERCIAL", Order = 2)]
        [HttpGet]
        public ActionResult ConsultaRutaDetalle(string ruta, string pagina, string numreg, string isfilter, string buscar)
        {
            var data = DataBase.Rutas.ConsultaRutaDetalleSP(ruta, null, null, null, pagina, numreg, isfilter, buscar, ruta == "0", ruta == "-1");
            RutaConsulta rutaConsulta = new RutaConsulta();
            List<RutaDetalleGV> detalleRuta = new List<RutaDetalleGV>();
            detalleRuta = data.ToList();
            rutaConsulta.RutaDetalleGVs = detalleRuta;

            return PartialView("_RutaDetalle", rutaConsulta);
        }
    }
}