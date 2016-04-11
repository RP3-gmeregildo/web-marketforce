using Rp3.AgendaComercial.Models;
using Rp3.AgendaComercial.Models.General;
using Rp3.AgendaComercial.Models.Ruta;
using Rp3.AgendaComercial.Web.Ruta;
using Rp3.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Rp3.AgendaComercial.Web.Ruta.Controllers
{
    public class GestionDetalladoController : Rp3.Web.Mvc.Controllers.BaseController<Models.ContextService>
    {
        //
        // GET: /Ruta/GestionDetallado/
        private void InicializarEdit()
        {
            ViewBag.RutasSelectList = DataBase.Rutas.Get(p => p.Estado == Models.Constantes.Estado.Activo, includeProperties: "Agentes").ToSelectList(includeNullItem: true, displayMember: "Etiqueta");
        }

        [Rp3.Web.Mvc.Authorize("GESTIONDETALL", "QUERY", "AGENDACOMERCIAL")]
        public ActionResult Index()
        {
            InicializarEdit();

            var model = new GestionDetalladoConsulta();

            //model.FechaInicioCalendario = GetCurrentDateTime().Date;
            //model.FechaFinCalendario = model.FechaInicioCalendario.Value.AddDays(1).AddSeconds(-1);

            var fecha = GetCurrentDateTime().Date;
            model.FechaInicioCalendario = new DateTime(fecha.Year, fecha.Month, 1);
            model.FechaFinCalendario = new DateTime(fecha.Year, fecha.Month, 1).AddMonths(1).AddSeconds(-1);

            model.Gestiones = new List<GestionDetallado>();

            return View(model);
        }

        [Rp3.Web.Mvc.Authorize("GESTIONDETALL", "QUERY", "AGENDACOMERCIAL")]
        [HttpPost]
        [PreventSpam(Order = 0)]
        public ActionResult Index(GestionDetalladoConsulta model)
        {
            InicializarEdit();

            //model.FechaFinCalendario = model.FechaInicioCalendario.Value.Date.AddDays(1).AddSeconds(-1);
            model.FechaFinCalendario = model.FechaFinCalendario.Value.Date.AddDays(1).AddSeconds(-1);

            model.Gestiones = new List<GestionDetallado>();

            if(model.IdRuta != null)
            {
                var agendas = DataBase.Agendas.Get(p => p.IdRuta == model.IdRuta && 
                    p.FechaInicio >= model.FechaInicioCalendario && p.FechaInicio <= model.FechaFinCalendario,
                    includeProperties: "ClienteDireccion, AgendaTareas");

                foreach (var agenda in agendas)
                {
                    var gestion = new GestionDetallado() { 
                        IdRuta = agenda.IdRuta,
                        IdAgenda = agenda.IdAgenda,
                        FechaInicio = agenda.FechaInicio,
                        FechaFin = agenda.FechaFin,
                        FechaInicioGestion = agenda.FechaInicioGestion,
                        FechaFinGestion = agenda.FechaFinGestion,
                        IdCliente = agenda.IdCliente,
                        IdClienteDireccion = agenda.IdClienteDireccion,
                        Cliente = agenda.ClienteDireccion.Cliente.NombresCompletos,
                        Direccion = agenda.ClienteDireccion.Etiqueta,
                        Observacion = agenda.Observacion,
                        EstadoAgenda = agenda.EstadoAgenda,
                        EstadoAgendaTabla = agenda.EstadoAgendaTabla,
                        EstadoAgendaGeneralValue = agenda.EstadoAgendaGeneralValue,
                        //Latitud = agenda.Latitud,
                        //Longitud = agenda.Longitud
                        Latitud = agenda.ClienteDireccion.Latitud,
                        Longitud = agenda.ClienteDireccion.Longitud
                    };

                    gestion.Tareas = String.Format("{0} de {1}", agenda.AgendaTareas.Where(p=>p.EstadoTarea == Constantes.EstadoTarea.Realizada).Count(), agenda.AgendaTareas.Count);
                    
                    model.Gestiones.Add(gestion);                    
                }

                SetMarkers(model.Gestiones);
            }

            ViewBag.MapRoute = true;

            return View(model);
        }

        private void SetMarkers(List<GestionDetallado> ubicaciones)
        {
            int markerIndex = 1;

            foreach (var item in ubicaciones.OrderBy(p=>p.FechaInicio))
            {
                item.MarkerIndex = markerIndex;
                markerIndex++;

                if (markerIndex >= 100)
                    markerIndex = 100;
            }
        }

        public ActionResult DetalleTarea(int idRuta, long idAgenda)
        {
            var list = DataBase.AgendaTareas.Get(p => p.IdRuta == idRuta && p.IdAgenda == idAgenda).ToList();

            return PartialView("_DetalleTarea", list);
        }

        public ActionResult DetalleActividad(int idRuta, long idAgenda, long idTarea)
        {
            ViewBag.IdRuta = idRuta;
            ViewBag.IdAgenda = idAgenda;
            ViewBag.IdTarea = idTarea;

            return PartialView("_DetalleActividad");
        }

        [ValidateInput(false)]
        public ActionResult ActividadTreeListPartial(int idRuta, long idAgenda, long idTarea)
        {
            ViewBag.IdRuta = idRuta;
            ViewBag.IdAgenda = idAgenda;
            ViewBag.IdTarea = idTarea;

            var model = DataBase.AgendaTareaActividades.Get(p => p.IdRuta == idRuta && p.IdAgenda == idAgenda && p.IdTarea == idTarea).ToList();
            ViewBag.TipoActividadList = DataBase.TipoActividades.Get(p => p.Estado == Models.Constantes.Estado.Activo).ToList();
            ViewBag.ReadOnly = true;
            return PartialView("_ActividadTreeListPartial", model);
        }
    }
}