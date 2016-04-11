using Rp3.AgendaComercial.Models.Consulta.View;
using Rp3.AgendaComercial.Models.General;
using Rp3.AgendaComercial.Models.General.View;
using Rp3.AgendaComercial.Models.Ruta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Rp3.AgendaComercial.Web.Areas.Consulta.Controllers
{
    public class HomeDashboardController : Rp3.AgendaComercial.Web.Controllers.AgendaComercialController
    {
        // GET: Consulta/HomeDashboard
        public ActionResult Index()
        {
            if (Agente != null && Agente.CargoRol != null)
            {
                ViewBag.IdAgente = Agente.IdAgente;
                ViewBag.EsSupervidor = Agente.CargoRol == AgenteCargoRol.Supervisor || Agente.CargoRol == AgenteCargoRol.Gerente;

                return View();
            }
            else
                return RedirectToAction("Index", "Home", new { Area = "" });
        }

        [Rp3.Web.Mvc.ChildAction()]
        public ActionResult GestionChart(string Mode, long Inicio, long Fin, int IdAgente)
        {
            DateTime fechaFin = new DateTime(Fin).Date.AddDays(1).AddSeconds(-1);
            DateTime fechaInicio = new DateTime(Inicio).Date;

            var model = DataBase.Reporte.GetDashboardResumenGestion(fechaInicio, fechaFin, IdAgente, Mode == "A");

            return PartialView("_GestionChart", model);
        }

        [Rp3.Web.Mvc.ChildAction()]
        public ActionResult EfectividadChart(string Mode, long Inicio, long Fin, int IdAgente)
        {
            DateTime fechaFin = new DateTime(Fin).Date.AddDays(1).AddSeconds(-1);
            DateTime fechaInicio = new DateTime(Inicio).Date;

            var model = DataBase.Reporte.GetDashboardResumenGestion(fechaInicio, fechaFin, IdAgente, Mode == "A");

            var value = model.Total > 0 ? ((double)model.Gestionados / (double)model.Total) * 100 : 0;

            return PartialView("_EfectividadChart", Convert.ToInt32(value));
        }

        [Rp3.Web.Mvc.ChildAction()]
        public ActionResult AgenteList(string Mode, long Inicio, long Fin, int IdAgente)
        {
            DateTime fechaFin = new DateTime(Fin).Date.AddDays(1).AddSeconds(-1);
            DateTime fechaInicio = new DateTime(Inicio).Date;

            var model = DataBase.Reporte.GetDashboardResumenGestionAgente(fechaInicio, fechaFin, IdAgente, Mode == "A");

            var agentes = DataBase.Agentes.GetAgentesPermitidos(IdAgente);

            foreach (var agente in agentes)
            {
                if (model.Where(p => p.IdAgente == agente.IdAgente).Count() == 0)
                {
                    model.Add(new DashboardResumenGestionAgente()
                    {
                        IdAgente = agente.IdAgente,
                        Agente = agente.Descripcion,
                        Gestionados = 0,
                        NoGestionados = 0,
                        Proximos = 0,
                        Total = 0
                    });
                }
            }

            int index = 1;

            foreach (var item in model.OrderBy(p => p.Agente))
            {
                item.MarkerIndex = index;
                index++;
            }

            return PartialView("_Agentes", model);
        }

        [Rp3.Web.Mvc.ChildAction()]
        public ActionResult ClienteList(string Mode, int IdAgente)
        {
            DateTime fechaFin = this.GetCurrentDateTime().Date.AddDays(1).AddSeconds(-1);
            DateTime fechaInicio = this.GetCurrentDateTime().Date;

            var agente = DataBase.Agentes.Get(p => p.IdAgente == IdAgente).FirstOrDefault();

            var list = DataBase.Agendas.GetAgendaListado(new List<int> { agente.IdRuta ?? 0 }, fechaInicio, fechaFin).OrderBy(p => p.fechaInicio).ToList();

            Rp3.AgendaComercial.Models.General.Ubicacion.SetMarkers(list);

            return PartialView("_Clientes", list);
        }

        [Rp3.Web.Mvc.ChildAction()]
        public ActionResult Ubicacion(string Mode, int IdAgente)
        {
            var agentes = DataBase.Agentes.GetAgentesPermitidos(IdAgente);
            var idsAgente = agentes.Select(p => p.IdAgente).ToList<int>();

            //var list = (from e in DataBase.AgenteUbicaciones.Get(p => idsAgente.Contains(p.IdAgente))
            //            group e by e.IdAgente into g
            //            select g.OrderByDescending(e => e.Fecha).FirstOrDefault() into r
            //            select r).ToList();

            var list = DataBase.AgenteUltimaUbicacions.Get(p => idsAgente.Contains(p.IdAgente));

            int index = 1;

            foreach (var item in agentes.OrderBy(p => p.Descripcion))
            {
                var ubicacion = list.Where(p => p.IdAgente == item.IdAgente).FirstOrDefault();

                if (ubicacion != null)
                {
                    ubicacion.MarkerIndex = index;
                }

                item.MarkerIndex = index;
                index++;
            }

            //Rp3.AgendaComercial.Models.General.Ubicacion.SetMarkers(list);

            return PartialView("_UbicacionMapMarker", list);
        }

        [Rp3.Web.Mvc.ChildAction()]
        public ActionResult Ruta(string Mode, int IdAgente)
        {
            ViewBag.MapRoute = true;
            //ViewBag.SuppressRouteMarkers = true;

            DateTime fechaFin = this.GetCurrentDateTime().Date.AddDays(1).AddSeconds(-1);
            DateTime fechaInicio = this.GetCurrentDateTime().Date;

            var agente = DataBase.Agentes.Get(p => p.IdAgente == IdAgente).FirstOrDefault();

            var list = DataBase.Agendas.GetAgendaListado(new List<int> { agente.IdRuta ?? 0 }, fechaInicio, fechaFin).OrderBy(p => p.fechaInicio).ToList();

            Rp3.AgendaComercial.Models.General.Ubicacion.SetMarkers(list);

            return PartialView("_UbicacionMapMarker", list);
        }

        [Rp3.Web.Mvc.ChildAction()]
        public ActionResult Recorrido(string Mode, int IdAgente)
        {
            ViewBag.MapRoute = true;
            ViewBag.SuppressRouteMarkers = true;

            DateTime fechaFin = this.GetCurrentDateTime().Date.AddDays(1).AddSeconds(-1);
            DateTime fechaInicio = this.GetCurrentDateTime().Date;

            var agenteUbicaciones = DataBase.AgenteUbicaciones.Get(p =>
                        p.Fecha >= fechaInicio && p.Fecha <= fechaFin
                        && p.IdAgente == IdAgente).OrderBy(p => p.Fecha).ToList();

            DataBase.ParametroHelper.Load();

            var list = Rp3.AgendaComercial.Models.General.Ubicacion.Recorrido(agenteUbicaciones, DataBase.ParametroHelper.RoutedDistance);

            return PartialView("_UbicacionMapMarker", list);
        }
    }
}