using Rp3.AgendaComercial.Models.Ruta.View;
using Rp3.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Rp3.AgendaComercial.Web.Ruta.Controllers
{
    public class ReasignacionClienteController : Rp3.AgendaComercial.Web.Controllers.AgendaComercialController
    {
        // GET: Ruta/ReasignacionCliente
         [Rp3.Web.Mvc.Authorize("REASIGCLIENTE", "QUERY", "AGENDACOMERCIAL")]
        public ActionResult Index()
        {
            InicializarIndex();

            return View();
        }

         private void InicializarIndex()
         {
             ViewBag.Rutas = DataBase.Rutas.Get(p => p.Estado == Rp3.AgendaComercial.Models.Constantes.Estado.Activo).OrderBy(p => p.Descripcion).ToList();
         }

         [ChildAction(Order = 1)]
         [Rp3.Web.Mvc.Authorize("REASIGCLIENTE", "QUERY", "AGENDACOMERCIAL", Order = 2)]
         [HttpPost]
         public ActionResult Reasignar(int idRutaOrigen, int idRutaDestino, string[] clientes)
         {
             try
             {
                 string ids = String.Empty;

                 foreach (var item in clientes)
                 {
                     if (ids.Length > 0)
                         ids = String.Format("{0},{1}", ids, item);
                     else
                         ids = item;
                 }

                 DataBase.Rutas.ReasignarClientes(idRutaOrigen, idRutaDestino, ids, this.UserLogonName);

                 this.AddDefaultSuccessMessage();
                 return Json();
                 
             }
             catch (Exception e)
             {
                 this.AddDefaultErrorMessage();
                 return Json();
             }
         }


         [ChildAction(Order = 1)]
         [Rp3.Web.Mvc.Authorize("REASIGCLIENTE", "QUERY", "AGENDACOMERCIAL", Order = 2)]
         [HttpGet]
         public ActionResult ConsultaRutaOrigenDetalle(string ruta, string pagina, string numreg, string isfilter, string buscar)
         {
             var data = DataBase.Rutas.ConsultaRutaDetalleSP(ruta, null, null, null, pagina, numreg, isfilter, buscar, ruta == "0", ruta == "-1");
             RutaConsulta rutaConsulta = new RutaConsulta();
             List<RutaDetalleGV> detalleRuta = new List<RutaDetalleGV>();
             detalleRuta = data.ToList();
             rutaConsulta.RutaDetalleGVs = detalleRuta;

             return PartialView("_RutaOrigenDetalle", rutaConsulta);
         }


         [ChildAction(Order = 1)]
         [Rp3.Web.Mvc.Authorize("REASIGCLIENTE", "QUERY", "AGENDACOMERCIAL", Order = 2)]
         [HttpGet]
         public ActionResult ConsultaRutaDestinoDetalle(string ruta, string pagina, string numreg, string isfilter, string buscar)
         {
             var data = DataBase.Rutas.ConsultaRutaDetalleSP(ruta, null, null, null, pagina, numreg, isfilter, buscar);
             RutaConsulta rutaConsulta = new RutaConsulta();
             List<RutaDetalleGV> detalleRuta = new List<RutaDetalleGV>();
             detalleRuta = data.ToList();
             rutaConsulta.RutaDetalleGVs = detalleRuta;

             return PartialView("_RutaDestinoDetalle", rutaConsulta);
         }
    }
}