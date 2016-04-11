using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Rp3.Data.Models.Security;
using Rp3.Web.Mvc.Application.Security.Models;
using Rp3.Web.Mvc;
using Rp3.AgendaComercial.Models.Ruta;
using Rp3.Web.Mvc.Html;
using Rp3.AgendaComercial.Models.General;
using DevExpress.Web.Mvc;
using System.Globalization;
using Rp3.AgendaComercial.Models;
using DevExpress.Web.ASPxScheduler;

namespace Rp3.AgendaComercial.Web.Ruta.Controllers
{
    public class ProgramacionRutaController : Rp3.Web.Mvc.Controllers.BaseController<Models.ContextService>
    {
        //
        // GET: /Ruta/ProgramacionRuta/

        [Rp3.Web.Mvc.Authorize("PROGRAMARUTA", "QUERY", "AGENDACOMERCIAL")]
        public ActionResult Index()
        {
            return View(GetListIndex());
        }
        public ActionResult GridViewIndex()
        {
            return PartialView("_GridViewIndex", GetListIndex());
        }

        public List<ProgramacionRuta> GetListIndex()
        {
            return DataBase.ProgramacionRutas.Get(includeProperties: "EstadoGeneralValue, DuracionGeneralValue").ToList();
        }

        private void InicializarEdit()
        {
            ViewBag.RutasSelectList = DataBase.Rutas.Get(p => p.Estado == Models.Constantes.Estado.Activo).ToSelectList(includeNullItem: true);
        }

        [Rp3.Web.Mvc.Authorize("PROGRAMARUTA", "NEW", "AGENDACOMERCIAL")]
        public ActionResult Create()
        {
            InicializarEdit();

            ProgramacionRuta model = new ProgramacionRuta();

            model.FechaInicio = GetCurrentDateTime().Date;
            model.FechaFin = model.FechaInicio;

            ViewBag.Ubicaciones = GetRuta(model.IdRuta);
            return View(model);
        }

        private ProgramacionRuta GetModel(long id)
        {
            ProgramacionRuta result = DataBase.ProgramacionRutas.Get(p => p.IdProgramacionRuta == id, includeProperties: "Ruta, ProgramacionRutaDetalles").SingleOrDefault();            

            return result;
        }

        [Rp3.Web.Mvc.Authorize("PROGRAMARUTA", "EDIT", "AGENDACOMERCIAL")]
        public ActionResult Edit(long id)
        {
            InicializarEdit();

            var model = GetModel(id);

            ViewBag.Ubicaciones = GetRuta(model.IdRuta);
            return View(model);
        }

        [Rp3.Web.Mvc.Authorize("PROGRAMARUTA", "DETAIL", "AGENDACOMERCIAL")]
        public ActionResult Detail(long id)
        {
            var model = GetModel(id);
            model.ReadOnly = true;

            ViewBag.Ubicaciones = GetRuta(model.IdRuta);
            return View(model);
        }

        [Rp3.Web.Mvc.Authorize("PROGRAMARUTA", "DELETE", "AGENDACOMERCIAL")]
        public ActionResult Delete(long id)
        {
            var model = GetModel(id);
            model.ReadOnly = true;

            ViewBag.Ubicaciones = GetRuta(model.IdRuta);
            return View(model);
        }


        [Rp3.Web.Mvc.Authorize("PROGRAMARUTA", "CONFIGURATOR", "AGENDACOMERCIAL")]
        public ActionResult Configurator(long id)
        {
            var model = GetModel(id);
            model.ReadOnly = true;

            return View(model);
        }

        [Rp3.Web.Mvc.Authorize("PROGRAMARUTA", "PROCESS", "AGENDACOMERCIAL")]
        public ActionResult Process(long id)
        {
            var programacion = GetModel(id);

            var model = new GeneraAgenda();

            model.IdProgramacionRuta = programacion.IdProgramacionRuta;
            //model.IdRuta = programacion.IdRuta;

            var currentDate = GetCurrentDateTime().Date;

            if (currentDate >= programacion.FechaInicio.Value.Date)
                model.FechaInicio = currentDate;
            else
                model.FechaInicio = programacion.FechaInicio.Value.Date;

            model.FechaFin = new DateTime(model.FechaInicio.Value.Year, model.FechaInicio.Value.Month, 1).AddMonths(1).AddSeconds(-1);

            return View(model);
        }

        //[HttpPost]
        //[PreventSpam(Order = 0)]
        //[Rp3.Web.Mvc.Authorize("PROGRAMARUTA", "PROCESS", "AGENDACOMERCIAL")]
        //public ActionResult Process(GeneraAgenda model)
        //{
        //    try
        //    {
        //        var programacion = GetModel(model.IdProgramacionRuta);

        //        if (!programacion.EsIndefinida && model.FechaFin > programacion.FechaFin)
        //            model.FechaFin = programacion.FechaFin;

        //        if (model.FechaInicio <= model.FechaFin)
        //        {                   
        //            model.FechaFin = model.FechaFin.Value.AddDays(1).AddSeconds(-1);

        //            ProcessAgenda(model);

        //            this.AddSuccessMessage(@Rp3.AgendaComercial.Resources.LegendFor.AgendaProcesada);
        //        }
        //        else
        //            this.AddErrorMessage(Rp3.AgendaComercial.Resources.ErrorMessageValidation.FechaInicioMayorFechaFin);
        //    }
        //    catch
        //    {
        //        this.AddDefaultErrorMessage();
        //    }

        //    return View(model);
        //}

        //private void ProcessAgenda(GeneraAgenda model)
        //{
        //    var currentDate = GetCurrentDateTime().Date;
        //    var programacion = GetProgramacionRuta(model.IdProgramacionRuta);
        //    var date = model.FechaInicio.Value.Date;

        //    var listDelete = DataBase.Agendas.Get(p => p.IdRuta == model.IdRuta && 
        //        p.FechaInicio >= model.FechaInicio && p.FechaInicio <= model.FechaFin &&
        //        p.EstadoAgenda == Rp3.AgendaComercial.Models.Constantes.EstadoAgenda.Pendiente, includeProperties: "AgendaTareas, AgendaTareas.AgendaTareaActividades");

        //    foreach (var modelDelete in listDelete)
        //        DataBase.Agendas.Delete(modelDelete);

        //    var tareas = (from p in DataBase.ProgramacionTareas.Get(p => p.Estado == Rp3.AgendaComercial.Models.Constantes.Estado.Activo &&
        //                      (p.FechaInicio <= model.FechaInicio || p.EsIndefinida || p.FechaFin >= model.FechaFin))
        //                  join r in DataBase.ProgramacionTareaRutas.Get(p => p.IdRuta == model.IdRuta) on new { p.IdProgramacionTarea } equals new { r.IdProgramacionTarea }
        //                  join d in DataBase.ProgramacionTareaDetalles.Get() on new { p.IdProgramacionTarea } equals new { d.IdProgramacionTarea }
        //                  join t in DataBase.Tareas.Get(p => p.Estado == Rp3.AgendaComercial.Models.Constantes.Estado.Activo) on new { d.IdTarea } equals new { t.IdTarea }
        //                  select new { t.IdTarea, p.FechaInicio, p.EsIndefinida, p.FechaFin, p.IdProgramacionTarea, p.IdFrecuencia, p.Frecuencia.Modo, p.Frecuencia.Valor, t.TareaActividades } ).Distinct();

        //    long max = Agenda.Max(model.IdRuta);

        //    while (date <= model.FechaFin)
        //    {
        //        var dayOfWeek = date.DayOfWeek;

        //        var detalles = programacion.ProgramacionRutaDetalles.Where(p => p.FechaInicio.Value.DayOfWeek == dayOfWeek);

        //        foreach (var det in detalles)
        //        {
        //            var agenda = new Agenda() { 
        //                IdAgenda = max,
        //                IdRuta = model.IdRuta, 
        //                IdCliente = det.IdCliente,
        //                IdClienteDireccion = det.IdClienteDireccion,
        //                FechaInicio = date.Date.AddHours(det.FechaInicio.Value.Hour).AddMinutes(det.FechaInicio.Value.Minute).AddSeconds(det.FechaInicio.Value.Second),
        //                FechaFin = date.Date.AddHours(det.FechaFin.Value.Hour).AddMinutes(det.FechaFin.Value.Minute).AddSeconds(det.FechaFin.Value.Second),
        //                EstadoAgendaTabla = Rp3.AgendaComercial.Models.Constantes.EstadoAgenda.Tabla,
        //                EstadoAgenda = Rp3.AgendaComercial.Models.Constantes.EstadoAgenda.Pendiente,
        //                UsrIng = this.UserLogonName,
        //                FecIng = currentDate,
        //                AgendaTareas = new List<AgendaTarea>()
        //            };

        //            foreach (var tarea in tareas.Where(p=> p.FechaInicio <= date && (p.EsIndefinida || p.FechaFin >= date)))
        //                if (agenda.AgendaTareas.Where(p => p.IdTarea == tarea.IdTarea).Count() == 0)
        //                {
        //                    var agendaTarea = new AgendaTarea()
        //                    {
        //                        IdRuta = agenda.IdRuta,
        //                        IdAgenda = agenda.IdAgenda,
        //                        IdTarea = tarea.IdTarea,
        //                        IdProgramacionTarea = tarea.IdProgramacionTarea,
        //                        EstadoTareaTabla = Constantes.EstadoTarea.Tabla,
        //                        EstadoTarea = Constantes.EstadoTarea.Pendiente,
        //                        AgendaTareaActividades = new List<AgendaTareaActividad>()
        //                    };

        //                    //agendaTarea.LoadActividades(tarea.TareaActividades);                            
        //                    agenda.AgendaTareas.Add(agendaTarea);
        //                }

        //            DataBase.Agendas.Insert(agenda);

        //        max++;
        //        }

        //        date = date.AddDays(1);
        //    }

        //    DataBase.Save();
        //}

        [HttpPost]
        [PreventSpam(Order = 0)]
        [Rp3.Web.Mvc.Authorize("PROGRAMARUTA", "NEW", "AGENDACOMERCIAL", Order = 1)]
        public ActionResult Create(ProgramacionRuta model)
        {
            return View(model);
        }

        [HttpPost]
        [PreventSpam(Order = 0)]
        [Rp3.Web.Mvc.Authorize("PROGRAMARUTA", "EDIT", "AGENDACOMERCIAL", Order = 1)]
        public ActionResult Edit(ProgramacionRuta model)
        {
            ProgramacionRuta modelUpdate = GetModel(model.IdProgramacionRuta);

            InicializarEdit();
            ViewBag.Ubicaciones = GetRuta(model.IdRuta);
            return View(model);
        }

        [HttpPost]
        [PreventSpam(Order = 0)]
        [Rp3.Web.Mvc.Authorize("PROGRAMARUTA", "DELETE", "AGENDACOMERCIAL")]
        public ActionResult Delete(ProgramacionRuta model)
        {
            try
            {
                ProgramacionRuta modelDelete = GetModel(model.IdProgramacionRuta);

                DataBase.ProgramacionRutas.Delete(modelDelete);
                DataBase.Save();

                return RedirectToAction("Index");
            }
            catch
            {
                this.AddDefaultErrorMessage();
            }

            return RedirectToAction("Index", null, null, true);
        }

        public List<Ubicacion> GetRuta(long? idRuta)
        {
            List<Ubicacion> ubicaciones = new List<Ubicacion>();

            if (idRuta != null)
            {
                var clientes = DataBase.RutaDetalles.Get(p => p.IdRuta == idRuta).Select(p => p.IdCliente).ToList<int>();

                var direcciones = DataBase.ClienteDirecciones.Get(p => clientes.Contains(p.IdCliente) && p.EsPrincipal).ToList();

                foreach (var item in direcciones)
                    ubicaciones.Add(new Ubicacion() { Titulo = item.Cliente.NombresCompletos, Latitud = item.Latitud, Longitud = item.Longitud });
            }

            return ubicaciones;
        }

        public ActionResult UbicacionMapMarker(int? idRuta)
        {
            return PartialView("_UbicacionMapMarker", GetRuta(idRuta));
        }

        public ProgramacionRuta GetProgramacionRuta(long IdProgramacionRuta)
        {
            return DataBase.ProgramacionRutas.Get(p => p.IdProgramacionRuta == IdProgramacionRuta, includeProperties: "ProgramacionRutaDetalles").SingleOrDefault();
        }

        public List<ClienteDireccion> GetClientes(int IdRuta)
        {
            return (from r in DataBase.RutaDetalles.Get(p => p.IdRuta == IdRuta)
                    join c in DataBase.ClienteDirecciones.Get() on new { r.IdCliente, r.IdClienteDireccion } equals new { c.IdCliente, c.IdClienteDireccion }
                    select c).OrderBy(p => p.Cliente.Apellido1).ToList();
        }

        public List<ClienteDireccion> GetClientesPorProgramar(int IdRuta, List<ClienteDireccion> programados)
        {
            return (from r in DataBase.RutaDetalles.Get(p => p.IdRuta == IdRuta)
                    join c in DataBase.ClienteDirecciones.Get() on new { r.IdCliente, r.IdClienteDireccion } equals new { c.IdCliente, c.IdClienteDireccion }
                    select c).Except(programados).ToList();
        }



        public ActionResult RutaSchedulerPartial(long IdProgramacionRuta, int Duracion)
        {


            return PartialView("_RutaSchedulerPartial");
        }

        public ActionResult CreateAppointment(long IdProgramacionRuta, int Duracion, string start, string idClienteDireccion)
        {
            try
            {
                DateTime fechaInicio = DateTime.Parse(start, CultureInfo.InvariantCulture);
                DateTime fechaFin = fechaInicio.AddMinutes(Duracion).AddSeconds(-1);

                string[] keyParts = idClienteDireccion.Split('-');

                var model = new ProgramacionRutaDetalle()
                {
                    IdProgramacionRuta = (int)IdProgramacionRuta,
                    IdCliente = Convert.ToInt32(keyParts[0]),
                    IdClienteDireccion = Convert.ToInt32(keyParts[1]),
                    FechaInicio = fechaInicio,
                    FechaFin = fechaFin
                };

                model.AsignarId();

                DataBase.ProgramacionRutaDetalles.Insert(model);
                DataBase.Save();

                ViewData["id"] = model.IdProgramacionRutaDetalle;
            }
            catch (Exception e)
            {
                ViewData["SchedulerErrorText"] = e.Message;
            }


            ViewBag.IdProgramacionRuta = IdProgramacionRuta;
            ViewBag.Duracion = Duracion;            

            return RutaSchedulerPartial(IdProgramacionRuta, Duracion);
        }

        public ActionResult RutaSchedulerPartialEditAppointment(long IdProgramacionRuta, int Duracion)
        {
            //var programacion = GetProgramacionRuta(IdProgramacionRuta);
            ////System.Collections.IEnumerable AppointmentBinding = programacion.ProgramacionRutaDetalles;
            //System.Collections.IEnumerable ResourceBinding = GetClientes(programacion.IdRuta);

            //try
            //{
            //    RutaSchedulerSettings.UpdateEditableDataObject(DataBase, IdProgramacionRuta, Duracion, AppointmentBinding, ResourceBinding);
            //}
            //catch (Exception e)
            //{
            //    ViewData["SchedulerErrorText"] = e.Message;
            //}

            ////SetMarkers((List<ProgramacionRutaDetalle>)AppointmentBinding);

            ////ViewData["Appointments"] = AppointmentBinding;
            //ViewData["Resources"] = ResourceBinding;

            //ViewBag.IdProgramacionRuta = IdProgramacionRuta;
            //ViewBag.Duracion = Duracion;

            return PartialView("_RutaSchedulerPartial");
        }

        private void SetMarkers(List<ProgramacionRutaDetalle> detalle)
        {
            var fechas = detalle.Select(p => p.FechaInicio.Value.Date).ToList();

            foreach (var fecha in fechas)
            {
                int index = 1;

                foreach (var app in detalle.Where(p => p.FechaInicio.Value.Date == fecha.Date).OrderBy(p => p.FechaInicio))
                {
                    app.MarkerIndex = index;
                    index++;
                }
            }
        }

        public List<Ubicacion> GetUbicaciones(long idProgramacionRuta, DateTime fecha)
        {
            List<Ubicacion> ubicaciones = new List<Ubicacion>();

            DateTime fechaInicio = fecha.Date;
            DateTime fechaFin = fecha.Date.AddDays(1).AddSeconds(-1);

            var direcciones = (from r in DataBase.ProgramacionRutaDetalles.Get(p => p.IdProgramacionRuta == idProgramacionRuta && p.FechaInicio >= fechaInicio && p.FechaInicio <= fechaFin)
                               join c in DataBase.ClienteDirecciones.Get() on new { r.IdCliente, r.IdClienteDireccion } equals new { c.IdCliente, c.IdClienteDireccion }
                               select new { Nombre = c.Cliente.NombresCompletos, Latitud = c.Latitud, Longitud = c.Longitud, r.FechaInicio }).OrderBy(p => p.FechaInicio);

            foreach (var item in direcciones)
                ubicaciones.Add(new Ubicacion() { Titulo = item.Nombre, Latitud = item.Latitud, Longitud = item.Longitud, MarkerIndex = 0 });

            SetMarkers(ubicaciones);

            return ubicaciones;
        }

        private void SetMarkers(List<Ubicacion> ubicaciones)
        {
            int markerIndex = 1;

            foreach (var item in ubicaciones)
            {
                item.MarkerIndex = markerIndex;
                markerIndex++;

                if (markerIndex >= 100)
                    markerIndex = 100;
            }
        }

        public ActionResult UbicacionMapMarkerDay(long idProgramacionRuta, string fecha)
        {
            DateTime fechaMapa = DateTime.Parse(fecha, CultureInfo.InvariantCulture);

            ViewBag.MapRoute = true;

            return PartialView("_UbicacionMapMarker", GetUbicaciones(idProgramacionRuta, fechaMapa));
        }
    }
    public class RutaSchedulerSettings
    {
        static DevExpress.Web.Mvc.MVCxAppointmentStorage appointmentStorage;
        public static DevExpress.Web.Mvc.MVCxAppointmentStorage AppointmentStorage
        {
            get
            {
                if (appointmentStorage == null)
                {
                    appointmentStorage = new DevExpress.Web.Mvc.MVCxAppointmentStorage();
                    appointmentStorage.Mappings.AppointmentId = "IdProgramacionRutaDetalle";
                    appointmentStorage.Mappings.Start = "FechaInicio";
                    appointmentStorage.Mappings.End = "FechaFin";
                    appointmentStorage.Mappings.Subject = "Asunto";
                    appointmentStorage.Mappings.Description = "";
                    appointmentStorage.Mappings.Location = "Ubicacion";
                    appointmentStorage.Mappings.AllDay = "";
                    appointmentStorage.Mappings.Type = "IdTipo";
                    appointmentStorage.Mappings.RecurrenceInfo = "";
                    appointmentStorage.Mappings.ReminderInfo = "";
                    appointmentStorage.Mappings.Label = "IdEtiqueta";
                    appointmentStorage.Mappings.Status = "IdEstado";
                    appointmentStorage.Mappings.ResourceId = "IdRecurso";
                    appointmentStorage.CustomFieldMappings.Add(new DevExpress.Web.ASPxScheduler.ASPxAppointmentCustomFieldMapping("MarkerIndex", "MarkerIndex"));
                }
                return appointmentStorage;
            }
        }

        static DevExpress.Web.Mvc.MVCxResourceStorage resourceStorage;
        public static DevExpress.Web.Mvc.MVCxResourceStorage ResourceStorage
        {
            get
            {
                if (resourceStorage == null)
                {
                    resourceStorage = new DevExpress.Web.Mvc.MVCxResourceStorage();
                    resourceStorage.Mappings.ResourceId = "Key";
                    resourceStorage.Mappings.Caption = "EtiquetaCliente";
                }
                return resourceStorage;
            }
        }

        public static void UpdateEditableDataObject(ContextService DataBase, long IdProgramacionRuta, int Duracion, System.Collections.IEnumerable appointments, System.Collections.IEnumerable resources)
        {
            InsertAppointments(DataBase, IdProgramacionRuta, Duracion, appointments, resources);
            UpdateAppointments(DataBase, IdProgramacionRuta, Duracion, appointments, resources);
            DeleteAppointments(DataBase, IdProgramacionRuta, appointments, resources);
        }

        static void InsertAppointments(ContextService DataBase, long IdProgramacionRuta, int Duracion, System.Collections.IEnumerable appointments, System.Collections.IEnumerable resources)
        {
            var newAppointments = DevExpress.Web.Mvc.SchedulerExtension.GetAppointmentsToInsert<ProgramacionRutaDetalle>("RutaScheduler", appointments, resources,
                AppointmentStorage, ResourceStorage);

            bool save = false;

            foreach (var appointment in newAppointments)
            {
                if (appointment != null)
                {
                    var model = new ProgramacionRutaDetalle();

                    model.IdProgramacionRuta = (int)IdProgramacionRuta;
                    model.AsignarId();

                    model.IdCliente = appointment.IdCliente;
                    model.IdClienteDireccion = appointment.IdClienteDireccion;

                    model.FechaInicio = appointment.FechaInicio;
                    model.FechaFin = model.FechaInicio.Value.AddMinutes(Duracion).AddSeconds(-1);

                    DataBase.ProgramacionRutaDetalles.Insert(model);

                    save = true;
                }
            }

            if (save)
                DataBase.Save();
        }
        static void UpdateAppointments(ContextService DataBase, long IdProgramacionRuta, int Duracion, System.Collections.IEnumerable appointments, System.Collections.IEnumerable resources)
        {
            var updAppointments = DevExpress.Web.Mvc.SchedulerExtension.GetAppointmentsToUpdate<ProgramacionRutaDetalle>("RutaScheduler", appointments, resources,
                AppointmentStorage, ResourceStorage);

            bool save = false;

            foreach (var appointment in updAppointments)
            {
                if (appointment != null)
                {
                    var model = DataBase.ProgramacionRutaDetalles.Get(p => p.IdProgramacionRuta == IdProgramacionRuta && p.IdProgramacionRutaDetalle == appointment.IdProgramacionRutaDetalle).SingleOrDefault();

                    //model.ClienteDireccion = null;
                    //model.ClienteDireccion = DataBase.ClienteDirecciones.Get(p => p.IdCliente == appointment.IdCliente && p.IdClienteDireccion == appointment.IdClienteDireccion).SingleOrDefault();
                    model.IdCliente = appointment.IdCliente;
                    model.IdClienteDireccion = appointment.IdClienteDireccion;

                    model.FechaInicio = appointment.FechaInicio;
                    model.FechaFin = appointment.FechaFin;//model.FechaInicio.Value.AddMinutes(Duracion).AddSeconds(-1);

                    DataBase.ProgramacionRutaDetalles.Update(model);

                    save = true;
                }
            }

            if (save)
                DataBase.Save();
        }

        static void DeleteAppointments(ContextService DataBase, long IdProgramacionRuta, System.Collections.IEnumerable appointments, System.Collections.IEnumerable resources)
        {
            var delAppointments = DevExpress.Web.Mvc.SchedulerExtension.GetAppointmentsToRemove<ProgramacionRutaDetalle>("RutaScheduler", appointments, resources,
                AppointmentStorage, ResourceStorage);

            bool save = false;

            foreach (var appointment in delAppointments)
            {
                if (appointment != null)
                {
                    var model = DataBase.ProgramacionRutaDetalles.Get(p => p.IdProgramacionRuta == IdProgramacionRuta && p.IdProgramacionRutaDetalle == appointment.IdProgramacionRutaDetalle).SingleOrDefault();
                    DataBase.ProgramacionRutaDetalles.Delete(model);

                    save = true;
                }
            }

            if (save)
                DataBase.Save();
        }
    }

    public class CustomAppointmentTemplateContainer : AppointmentFormTemplateContainer
    {
        public CustomAppointmentTemplateContainer(MVCxScheduler scheduler)
            : base(scheduler)
        {

        }
    }

}
