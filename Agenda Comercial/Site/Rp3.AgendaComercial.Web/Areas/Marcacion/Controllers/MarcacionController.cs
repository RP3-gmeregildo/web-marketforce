using Rp3.AgendaComercial.Models.General;
using Rp3.AgendaComercial.Models.Marcacion.View;
using Rp3.AgendaComercial.Web.Areas.Marcacion.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Rp3.AgendaComercial.Web.Marcacion.Controllers
{
    public class MarcacionController : Rp3.AgendaComercial.Web.Controllers.AgendaComercialController
    {
        // GET: Marcacion/Marcacion
        public ActionResult Index()
        {
            var model = new ReporteMarcacionView() { Fecha = this.GetCurrentDateTime() };

            return View(model);
        }

        [Rp3.Web.Mvc.ChildAction()]
        public ActionResult GetData(long FechaTicks)
        {
            DateTime fecha = new DateTime(FechaTicks).Date;

            var model = DataBase.Reporte.GetReporteMarcacion(this.Agente.IdAgente, fecha);

            DataBase.ParametroHelper.Load();

            foreach (var item in model)
            {
                //if (item.MinutosAtrasoNoJustificado > DataBase.ParametroHelper.MinutoAtrasoDia || item.Ausente)
                //{
                //    item.Eficiencia = 0;
                //}
                //else 
                //{
                //    item.Eficiencia = (1 - (item.MinutosAtrasoNoJustificado / DataBase.ParametroHelper.MinutoAtrasoDia)) * 100;
                //}

                item.Eficiencia = Models.Marcacion.Marcacion.Efectividad(item.MinutosAtrasoNoJustificado);

                if (item.Eficiencia >= DataBase.ParametroHelper.MaxMarcacionEficiencia)
                    item.KpiEficiencia = 1;
                else if (item.Eficiencia >= DataBase.ParametroHelper.MinMarcacionEficiencia)
                    item.KpiEficiencia = 0;
                else
                    item.KpiEficiencia = -1;

                //SI ES EL DÍA DE HOY GENERA LAS REFERENCIAS DE MARCACION
                if(fecha.Date == DateTime.Now.Date)
                {
                    var marcacion = DataBase.Marcacions.GetByID(item.IdMarcacion);
                    if (marcacion != null && (marcacion.Latitud.HasValue & marcacion.Longitud.HasValue))
                        item.DireccionMarcacion = Process.RutaOptima.GetAddress(marcacion.Latitud.Value, marcacion.Longitud.Value);

                }
            }

            double promedioAtraso = 0;

            if (model.Any())
                promedioAtraso = model.Where(p => p.AplicaMarcacion).Average(p => p.MinutosAtrasoNoJustificado);

            ViewBag.Eficiencia = 0;
            ViewBag.EficienciaKpiColor = "";

            ViewBag.Eficiencia = Models.Marcacion.Marcacion.Efectividad(promedioAtraso);

            //if (promedioAtraso > DataBase.ParametroHelper.MinutoAtrasoDia)
            //{
            //    ViewBag.Eficiencia = 0;
            //}
            //else
            //{
            //    ViewBag.Eficiencia = (1 - (promedioAtraso / DataBase.ParametroHelper.MinutoAtrasoDia)) * 100;
            //}

            if (ViewBag.Eficiencia >= DataBase.ParametroHelper.MaxMarcacionEficiencia)
                ViewBag.EficienciaKpiColor = "#6FBF56";
            else if (ViewBag.Eficiencia >= DataBase.ParametroHelper.MinMarcacionEficiencia)
                ViewBag.EficienciaKpiColor = "yellow";
            else
                ViewBag.EficienciaKpiColor = "#BE2026";

            return PartialView("_Agentes", model);
        }

        [Rp3.Web.Mvc.ChildAction()]
        public ActionResult GetPermisos(int IdAgente, long FechaTicks)
        {
            DateTime fechaInicio = new DateTime(FechaTicks).Date;
            DateTime fechaFin = new DateTime(FechaTicks).Date.AddHours(23).AddMinutes(59).AddSeconds(59);

            List<PermisoCategoria> listaPermiso = new List<PermisoCategoria>();
            PermisoConsulta permisoConsulta = new PermisoConsulta();
            PermisoCategoria categoria = new PermisoCategoria();

            var permisos = DataBase.Permisos.GetPermisoListado(new List<int>() { IdAgente }, new List<int>(), fechaInicio, fechaFin, null, null, null, true);
            var justificaciones = DataBase.Permisos.GetPermisoListado(new List<int>() { IdAgente }, new List<int>(), fechaInicio, fechaFin, null, null, null, false);

            categoria = new PermisoCategoria();
            categoria.isBusqueda = false;
            categoria.Nombre = Rp3.AgendaComercial.Resources.TitleFor.PermisosPrevios;

            categoria.Permiso = permisos.OrderBy(p => p.FechaInicio).ThenBy(p=>p.HoraInicio).ToList();
            listaPermiso.Add(categoria);

            categoria = new PermisoCategoria();
            categoria.isBusqueda = false;
            categoria.Nombre = Rp3.AgendaComercial.Resources.TitleFor.Justificaciones;

            categoria.Permiso = justificaciones.OrderBy(p => p.FechaInicio).ThenBy(p=>p.HoraInicio).ToList();
            listaPermiso.Add(categoria);

            permisoConsulta.PermisoCategorias = listaPermiso;

            return PartialView("_PermisoListaDetalle", permisoConsulta);
        }

        [Rp3.Web.Mvc.ChildAction()]
        public ActionResult GetMap(int IdAgente, long FechaTicks)
        {
            DateTime fecha = new DateTime(FechaTicks).Date;

            var marcacion = DataBase.Marcacions.Get(p => p.Fecha == fecha && p.IdAgente == IdAgente).ToList();

            List<Ubicacion> model = new List<Ubicacion>();

            ViewBag.HorarioInicioJornada1 = String.Empty;
            ViewBag.HorarioFinJornada1 = String.Empty;
            ViewBag.HorarioInicioJornada2 = String.Empty;
            ViewBag.HorarioFinJornada2 = String.Empty;

            foreach (var marc in marcacion)
            {
                switch (marc.Tipo)
                {
                    case Rp3.AgendaComercial.Models.Constantes.TipoMarcacion.InicioJornada1:

                        if (marc.HoraInicioPlan != null)
                        {
                            model.Add(new Ubicacion()
                            {
                                MarkerIndex = 1,
                                Titulo = String.Format("{0} - {1} - {2}", Rp3.AgendaComercial.Resources.LabelFor.LugarPlanificado, Rp3.AgendaComercial.Resources.LabelFor.HoraInicioPrimeraJornada, marc.HoraInicioPlan.Value.ToString("HH:mm")),
                                Latitud = marc.LatitudPlan,
                                Longitud = marc.LongitudPlan,
                                Color = "blue"
                            });

                            if (marc.HoraInicioPlan != null)
                                ViewBag.HorarioInicioJornada1 = String.Format("[{0}]", marc.HoraInicioPlan.Value.ToString("HH:mm"));
                        }

                        if (marc.HoraInicio != null)
                        {
                            model.Add(new Ubicacion()
                            {
                                MarkerIndex = 1,
                                Titulo = String.Format("{0} - {1} - {2}", Rp3.AgendaComercial.Resources.LabelFor.LugarMarcacion, Rp3.AgendaComercial.Resources.LabelFor.HoraInicioPrimeraJornada, marc.HoraInicio.Value.ToString("HH:mm")),
                                Latitud = marc.Latitud,
                                Longitud = marc.Longitud,
                                Color = "orange"
                            });
                        }

                        break;

                    case Rp3.AgendaComercial.Models.Constantes.TipoMarcacion.FinJornada1:

                        if (marc.HoraFinPlan != null)
                        {
                            model.Add(new Ubicacion()
                            {
                                MarkerIndex = 2,
                                Titulo = String.Format("{0} - {1} - {2}", Rp3.AgendaComercial.Resources.LabelFor.LugarPlanificado, Rp3.AgendaComercial.Resources.LabelFor.Break, marc.HoraFinPlan.Value.ToString("HH:mm")),
                                Latitud = marc.LatitudPlan,
                                Longitud = marc.LongitudPlan,
                                Color = "blue"
                            });

                            if (marc.HoraFinPlan != null)
                                ViewBag.HorarioFinJornada1 = String.Format("[{0}]", marc.HoraFinPlan.Value.ToString("HH:mm"));
                        }

                        if (marc.HoraFin != null)
                        {
                            model.Add(new Ubicacion()
                            {
                                MarkerIndex = 2,
                                Titulo = String.Format("{0} - {1} - {2}", Rp3.AgendaComercial.Resources.LabelFor.LugarMarcacion, Rp3.AgendaComercial.Resources.LabelFor.Break, marc.HoraFin.Value.ToString("HH:mm")),
                                Latitud = marc.Latitud,
                                Longitud = marc.Longitud,
                                Color = "orange"
                            });
                        }

                        break;

                    case Rp3.AgendaComercial.Models.Constantes.TipoMarcacion.InicioJornada2:

                        if (marc.HoraInicioPlan != null)
                        {
                            model.Add(new Ubicacion()
                            {
                                MarkerIndex = 3,
                                Titulo = String.Format("{0} - {1} - {2}", Rp3.AgendaComercial.Resources.LabelFor.LugarPlanificado, Rp3.AgendaComercial.Resources.LabelFor.HoraInicioSegundaJornada, marc.HoraInicioPlan.Value.ToString("HH:mm")),
                                Latitud = marc.LatitudPlan,
                                Longitud = marc.LongitudPlan,
                                Color = "blue"
                            });

                            if (marc.HoraInicioPlan != null)
                                ViewBag.HorarioInicioJornada2 = String.Format("[{0}]", marc.HoraInicioPlan.Value.ToString("HH:mm"));
                        }

                        if (marc.HoraInicio != null)
                        {
                            model.Add(new Ubicacion()
                            {
                                MarkerIndex = 3,
                                Titulo = String.Format("{0} - {1} - {2}", Rp3.AgendaComercial.Resources.LabelFor.LugarMarcacion, Rp3.AgendaComercial.Resources.LabelFor.HoraInicioSegundaJornada, marc.HoraInicio.Value.ToString("HH:mm")),
                                Latitud = marc.Latitud,
                                Longitud = marc.Longitud,
                                Color = "orange"
                            });
                        }

                        break;

                    case Rp3.AgendaComercial.Models.Constantes.TipoMarcacion.FinJornada2:

                         if (marc.HoraFinPlan != null)
                        {
                            model.Add(new Ubicacion()
                            {
                                MarkerIndex = 4,
                                Titulo = String.Format("{0} - {1} - {2}", Rp3.AgendaComercial.Resources.LabelFor.LugarPlanificado, Rp3.AgendaComercial.Resources.LabelFor.HoraFinJornada, marc.HoraFinPlan.Value.ToString("HH:mm")),
                                Latitud = marc.LatitudPlan,
                                Longitud = marc.LongitudPlan,
                                Color = "blue"
                            });

                            if (marc.HoraFinPlan != null)
                                ViewBag.HorarioFinJornada2 = String.Format("[{0}]", marc.HoraFinPlan.Value.ToString("HH:mm"));
                        }

                        if (marc.HoraFin != null)
                        {
                            model.Add(new Ubicacion()
                            {
                                MarkerIndex = 4,
                                Titulo = String.Format("{0} - {1} - {2}", Rp3.AgendaComercial.Resources.LabelFor.LugarMarcacion, Rp3.AgendaComercial.Resources.LabelFor.HoraFinJornada, marc.HoraFin.Value.ToString("HH:mm")),
                                Latitud = marc.Latitud,
                                Longitud = marc.Longitud,
                                Color = "orange"
                            });
                        }

                        break;
                }
            }

            DataBase.ParametroHelper.Load();
            ViewBag.MapRadius = DataBase.ParametroHelper.MarcacionDistance;

            return PartialView("_UbicacionMapMarkerAgente", model);
        }
    }
}