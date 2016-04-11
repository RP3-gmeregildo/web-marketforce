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
    public class GestionResumidoController : Rp3.Web.Mvc.Controllers.BaseController<Models.ContextService>
    {
        //
        // GET: /Ruta/GestionResumido/
        private void InicializarEdit()
        {
            ViewBag.RutasSelectList = DataBase.Rutas.Get(p => p.Estado == Models.Constantes.Estado.Activo, includeProperties: "Agentes").ToSelectList(includeNullItem: true, displayMember: "Etiqueta");
        }

        [Rp3.Web.Mvc.Authorize("GESTIONRESUMIDO", "QUERY", "AGENDACOMERCIAL")]
        public ActionResult Index()
        {
            InicializarEdit();

            var model = new GestionResumidoConsulta();

            var fecha = GetCurrentDateTime().Date;
            model.FechaInicioCalendario = new DateTime(fecha.Year, fecha.Month, 1);
            model.FechaFinCalendario = new DateTime(fecha.Year, fecha.Month, 1).AddMonths(1).AddSeconds(-1);

            model.ResumenEstado = new List<ResumenEstado>();
            model.ResumenTareaEstado = new List<ResumenEstado>();

            return View(model);
        }

        [Rp3.Web.Mvc.Authorize("GESTIONRESUMIDO", "QUERY", "AGENDACOMERCIAL")]
        [HttpPost]
        [PreventSpam(Order = 0)]
        public ActionResult Index(GestionResumidoConsulta model)
        {
            InicializarEdit();
            
            model.FechaFinCalendario = model.FechaFinCalendario.Value.Date.AddDays(1).AddSeconds(-1);

            model.ResumenEstado = new List<ResumenEstado>();
            model.ResumenTareaEstado = new List<ResumenEstado>();

            var estados = DataBase.GeneralValues.Get(p => p.Id == Constantes.EstadoAgenda.Tabla);

            var tareaEstados = DataBase.GeneralValues.Get(p => p.Id == Constantes.EstadoTarea.Tabla);   

            var agendas = DataBase.Agendas.Get(p => (model.IdRuta == null || p.IdRuta == model.IdRuta) &&
                   p.FechaInicio >= model.FechaInicioCalendario && p.FechaInicio <= model.FechaFinCalendario);

            var tareas = DataBase.AgendaTareas.Get(p => (model.IdRuta == null || p.IdRuta == model.IdRuta) &&
                  p.Agenda.FechaInicio >= model.FechaInicioCalendario && p.Agenda.FechaInicio <= model.FechaFinCalendario);

            foreach (var estado in estados)
            {
                var resumen = new ResumenEstado()
                {
                    EstadoAgenda = estado.Code,
                    EstadoAgendaContent = estado.Content,
                    Cantidad = 0
                };

                resumen.Cantidad = agendas.Where(p => p.EstadoAgenda == resumen.EstadoAgenda).Count();

                if (resumen.Cantidad > 0)
                    model.ResumenEstado.Add(resumen);
            }

            foreach (var estado in tareaEstados)
            {
                var resumen = new ResumenEstado()
                {
                    EstadoAgenda = estado.Code,
                    EstadoAgendaContent = estado.Content,
                    Cantidad = 0
                };

                resumen.Cantidad = tareas.Where(p => p.EstadoTarea == resumen.EstadoAgenda).Count();

                if (resumen.Cantidad > 0)
                    model.ResumenTareaEstado.Add(resumen);
            }

            return View(model);
        }

    }
}