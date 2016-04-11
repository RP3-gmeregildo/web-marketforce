using Rp3.AgendaComercial.Models.General;
using Rp3.AgendaComercial.Models.General.View;
using Rp3.AgendaComercial.Models.Oportunidad.View;
using Rp3.AgendaComercial.Web.Ruta;
using Rp3.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Rp3.AgendaComercial.Web.Oportunidad.Controllers
{
    public class OportunidadController : AgendaComercial.Web.Controllers.AgendaComercialController
    {

        public Rp3.AgendaComercial.Web.BinaryResult Preview(int idOportunidad, string addressBase)
        {
            PartialViewResult data = GetDatos(idOportunidad) as PartialViewResult;

            OportunidadListado oportunidad = data.Model as OportunidadListado;
            //agenda.AgendaClientes.ForEach(p => p.SetSecuencia());
            //PartialViewResult map = ExpandMap(idOportunidad) as PartialViewResult;
            data.ViewBag.MapWidth = 450;
            data.ViewBag.MapHeight = 195;
            data.ViewBag.UrlBase = string.Format("{0}://{1}{2}", this.Request.UrlReferrer.Scheme, this.Request.UrlReferrer.Authority, string.IsNullOrEmpty(addressBase) ? string.Empty : string.Format("/{0}", addressBase));
            Rp3.AgendaComercial.Web.Areas.Oportunidad.Reports.Oportunidad report = new Areas.Oportunidad.Reports.Oportunidad(data.ViewBag, oportunidad);
            System.IO.MemoryStream reportStream = new System.IO.MemoryStream();
            report.ExportToPdf(reportStream);
            reportStream.Position = 0;
            return new Rp3.AgendaComercial.Web.BinaryResult { Data = reportStream.ToArray(), ContentType = "application/pdf", FileName = string.Format("Oportunidad.{0:yyyy.MM.dd}.{1:HH.mm.ss}.pdf", DateTime.Now, DateTime.Now) };

        }
        // GET: Oportunidad/Oportunidad
        [Rp3.Web.Mvc.Authorize("OPORTUNIDAD", "QUERY", "AGENDACOMERCIAL")]
        public ActionResult Index()
        {
            if (!this.TodosMisAgentes)
            {
                if (this.Agente.CargoRol == AgenteCargoRol.Agente && this.Agente.EsAgente)
                {
                    this.UserWorkId = this.UserId;
                }
                else if ((this.Agente.CargoRol == AgenteCargoRol.Supervisor || this.Agente.CargoRol == AgenteCargoRol.Gerente) && !this.AgenteWork.EsAgente)
                {
                    //return RedirectToAction("SeleccionAgente");

                    this.TodosMisAgentes = true;
                    var agentes = DataBase.Agentes.GetAgentesPermitidos(this.Agente.IdAgente);
                    this.UserWorkIds = agentes.Where(p => p.IdUsuario != null).Select(p => p.IdUsuario ?? 0).ToList<int>();
                    this.UserWorkIds.Add(this.UserId);
                }
            }

            ViewBag.Estados = DataBase.GeneralValues.Get(p => p.Id == Models.Constantes.EstadoOportunidad.Tabla).ToSelectList(includeNullItem: true);
            ViewBag.DiasInactividad = DataBase.GeneralValues.Get(p => p.Id == Models.Constantes.DiasInactividad.Tabla).OrderBy(p => p.Code).ToSelectList(includeNullItem: true);
            ViewBag.Tipos = DataBase.OportunidadTipos.Get(p => p.Estado == Models.Constantes.Estado.Activo).ToSelectList(includeNullItem: true);

            ViewBag.EsSupervisor = (this.Agente.CargoRol == AgenteCargoRol.Supervisor || this.Agente.CargoRol == AgenteCargoRol.Gerente);
            ViewBag.EsGerente = this.Agente.CargoRol == AgenteCargoRol.Gerente;
            ViewBag.TodosMisAgentes = this.TodosMisAgentes;

            return View(AgenteWork);
        }

        [Rp3.Web.Mvc.Authorize("OPORTUNIDAD", "QUERY", "AGENDACOMERCIAL")]
        public ActionResult CambiarAgente()
        {
            this.UserWorkId = null;
            this.TodosMisAgentes = false;
            this.UserWorkIds = null;

            return RedirectToAction("SeleccionAgente");
        }

        [Rp3.Web.Mvc.Authorize("OPORTUNIDAD", "QUERY", "AGENDACOMERCIAL")]
        public ActionResult SeleccionAgente()
        {
            ViewBag.AgenteSelectList = DataBase.Agentes.GetAgentesPermitidos(this.Agente.IdAgente, true).ToSelectList(includeNullItem: true);
            return View();
        }

        [HttpPost]
        [Rp3.Web.Mvc.Authorize("OPORTUNIDAD", "QUERY", "AGENDACOMERCIAL")]
        public ActionResult SeleccionAgente(SeleccionAgente model)
        {
            this.TodosMisAgentes = false;

            if (model.IdAgente != Rp3.AgendaComercial.Models.Constantes.IdTodosMisAgentes)
            {
                var agente = DataBase.Agentes.Get(p => p.IdAgente == model.IdAgente).FirstOrDefault();
                this.UserWorkId = agente.IdUsuario;
            }
            else
            {
                this.TodosMisAgentes = true;

                var agentes = DataBase.Agentes.GetAgentesPermitidos(this.Agente.IdAgente);

                this.UserWorkIds = agentes.Where(p => p.IdUsuario != null).Select(p => p.IdUsuario ?? 0).ToList<int>();
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult GetLista()
        {
            return PartialView("_OportunidadLista");
        }

        [ChildAction(Order = 1)]
        [Rp3.Web.Mvc.Authorize("OPORTUNIDAD", "QUERY", "AGENDACOMERCIAL", Order = 2)]
        [HttpGet]
        public ActionResult GetDatos(int idOportunidad)
        {
            var modelo = PrepararModeloDetalle(idOportunidad);
            
            return PartialView("_OportunidadDetalleGestion", modelo);
        }

        [ChildAction(Order = 1)]
        [Rp3.Web.Mvc.Authorize("OPORTUNIDAD", "QUERY", "AGENDACOMERCIAL", Order = 2)]
        [HttpGet]
        public ActionResult GetMoreInfo(int idOportunidad)
        {
            var modelo = PrepararModeloDetalle(idOportunidad);

            return PartialView("_OportunidadDetalleInfo", modelo);
        }

        [ChildAction(Order = 1)]
        [Rp3.Web.Mvc.Authorize("OPORTUNIDAD", "QUERY", "AGENDACOMERCIAL", Order = 2)]
        [HttpGet]
        public ActionResult GetBitacora(int idOportunidad)
        {
            var modelo = DataBase.OportunidadBitacoras.Get(p => p.IdOportunidad == idOportunidad, includeProperties: "Agente").ToList();

            return PartialView("_OportunidadDetalleBitacora", modelo);
        }

        [ChildAction(Order = 1)]
        [Rp3.Web.Mvc.Authorize("OPORTUNIDAD", "QUERY", "AGENDACOMERCIAL", Order = 2)]
        [HttpGet]
        public ActionResult GetEtapas(int idOportunidad, int idEtapa)
        {
            var modelo = DataBase.OportunidadEtapas.Get(p => p.IdOportunidad == idOportunidad && p.IdEtapa == idEtapa).FirstOrDefault();

            modelo.OportunidadEtapas = DataBase.OportunidadEtapas.Get(p => p.IdOportunidad == idOportunidad && p.IdEtapaPadre == idEtapa).ToList();

            return PartialView("_OportunidadDetalleEtapa", modelo);
        }

        [ChildAction(Order = 1)]
        [Rp3.Web.Mvc.Authorize("OPORTUNIDAD", "QUERY", "AGENDACOMERCIAL", Order = 2)]
        [HttpGet]
        public ActionResult GetTareas(int idOportunidad, int idEtapa, int idTarea)
        {
            var modelo = DataBase.OportunidadTareas.Get(p => p.IdOportunidad == idOportunidad && p.IdEtapa == idEtapa && p.IdTarea == idTarea, includeProperties: "EstadoGeneralValue, OportunidadTareaActividads").FirstOrDefault();

            return PartialView("_OportunidadDetalleTarea", modelo);
        }

        private OportunidadListado PrepararModeloDetalle(int idOportunidad)
        {
            var data = DataBase.Oportunidades.GetOportunidadDetalle(idOportunidad);

            data.Contactos = DataBase.OportunidadContactos.Get(p => p.IdOportunidad == idOportunidad).ToList();
            data.Responsables = DataBase.OportunidadResponsables.Get(p => p.IdOportunidad == idOportunidad).ToList();

            data.Ubicaciones = new List<Ubicacion>();

            if (data.Latitud != null && data.Longitud != null && data.Latitud != 0 && data.Longitud != 0)
            {
                data.Ubicaciones.Add(new Ubicacion() { IdUbicacion = 1, MarkerIndex = 1, Titulo = Rp3.AgendaComercial.Resources.LabelFor.Prospecto, Latitud = data.Latitud, Longitud = data.Longitud });
                ViewBag.ShowProspectoSinUbicacion = false;
            }
            else 
            {
                ViewBag.ShowProspectoSinUbicacion = true;
            }

            ViewBag.SuppressRouteMarkers = true;
            ViewBag.MapRadiusMode = true;

            ViewBag.TodosMisAgentes = this.TodosMisAgentes;

            return data;
        }

        [HttpGet]
        public ActionResult ExpandMap(int idOportunidad)
        {
            var modelo = PrepararModeloDetalle(idOportunidad);

            ViewBag.MapSelector = "viewMapExpand";

            ViewBag.Descripcion = modelo.Descripcion;
            ViewBag.Agente = modelo.Agente;
            ViewBag.Estado = modelo.Estado;
            ViewBag.EtapaOrden = modelo.EtapaOrden;
            ViewBag.EstadoImageUrl = modelo.EstadoImageUrl;
            ViewBag.EstadoDescripcion = modelo.EstadoDescripcion;
            ViewBag.Probabilidad = modelo.Probabilidad;
            ViewBag.Importe = modelo.Importe;

            return PartialView("_UbicacionMapMarkerExpand", modelo.Ubicaciones);
        }

        [HttpGet]
        public ActionResult GetSort()
        {
            return PartialView("_SeleccionSort");
        }

        [Rp3.Web.Mvc.Authorize("OPORTUNIDAD", "QUERY", "AGENDACOMERCIAL")]
        public ActionResult SetUbicacion(int markerIndex, double? latitud, double? longitud)
        {
            if (longitud == null || longitud == -1)
                longitud = Ubicacion.DefaultLongitud;

            if (latitud == null || latitud == -1)
                latitud = Ubicacion.DefaultLatitud;

            Ubicacion model = new Ubicacion() { MarkerIndex = markerIndex, Longitud = longitud, Latitud = latitud };

            return PartialView("_SetUbicacion", model);
        }

        [Rp3.Web.Mvc.Authorize("OPORTUNIDAD", "QUERY", "AGENDACOMERCIAL")]
        public ActionResult SaveUbicacion(int idOportunidad, double? latitud, double? longitud)
        {
            try
            {
                var model = DataBase.Oportunidades.Get(p => p.IdOportunidad == idOportunidad).FirstOrDefault();

                model.Latitud = latitud;
                model.Longitud = longitud;

                DataBase.Oportunidades.Update(model);

                DataBase.Save();

                this.AddDefaultSuccessMessage();
                return Json();
            }
            catch (Exception e)
            {
                this.AddDefaultErrorMessage();
                return Json();
            }
        }

        #region Listado

        [HttpGet]
        public ActionResult GetListado(DateTime fechaInicial, DateTime fechaFinal, string estado, string sortField, int sortMode, int? diasInactividad, int? tipo, string groupMode = Rp3.AgendaComercial.Models.Constantes.OportunidadGroupMode.Calendario)
        {
            fechaInicial = fechaInicial.Date;
            fechaFinal = fechaFinal.Date.AddHours(23).AddMinutes(59).AddSeconds(59); ;
            /*
             * 1 = Next Few Days
             * 2 = Next Weeks (Until last Week of Month)
             * 3 = Next Month
             */
            //Modelo
            List<OportunidadCategoria> listaOportunidad = new List<OportunidadCategoria>();
            OportunidadConsulta oportunidadConsulta = new OportunidadConsulta();
            OportunidadCategoria categoria = new OportunidadCategoria();

            //Current Day
            DateTime currentDay = DateTime.Now;
            //Find Remainder Sundays
            int remainderSundays = GetRemainderSundays(fechaInicial);// (currentDay);
            //First Sunday
            DateTime beginDate = GetNextSunday(fechaInicial);// (currentDay);

            //Range of Search
            int dayRange = (fechaFinal - fechaInicial).Days;
            //Days Left Until End of Week
            int daysLeft = (GetNextSunday(fechaInicial) - fechaInicial).Days;

            List<int> listAgente = new List<int>();

            if (!this.TodosMisAgentes)
            {
                listAgente.Add(this.AgenteWork.IdAgente);
            }
            else
            {
                foreach (var agente in this.AgentesWork)
                    listAgente.Add(agente.IdAgente);
            }

            var data = DataBase.Oportunidades.GetOportunidadListado(listAgente, fechaInicial, fechaFinal, estado);

            if (diasInactividad != null)
            {
                data = data.Where(p => p.DiasInactividad > diasInactividad.Value).ToList();
            }

            if(tipo.HasValue)
            {
                data = data.Where(p => p.IdOportunidadTipo == tipo.Value).ToList();
            }

            switch (groupMode)
            {
                case Rp3.AgendaComercial.Models.Constantes.OportunidadGroupMode.Calendario:

                    bool exit = false;

                    if (dayRange <= 7)
                    {
                        for (int x = 0; x <= 7; x++)
                        {
                            if (!exit)
                            {
                                categoria = new OportunidadCategoria();
                                categoria.isBusqueda = false;

                                if (fechaInicial.Date == currentDay.Date)
                                {
                                    if (x == 0)
                                    {
                                        categoria.Nombre = Rp3.AgendaComercial.Resources.LabelFor.Hoy;
                                    }
                                    else if (x == 1) { categoria.Nombre = Rp3.AgendaComercial.Resources.LabelFor.Maniana; }
                                    else
                                    {
                                        categoria.Nombre = fechaInicial.AddDays(x).ToString("dddd").First().ToString().ToUpper() +
                                                      String.Join("", fechaInicial.AddDays(x).ToString("dddd").Skip(1))
                                                      + " " + fechaInicial.AddDays(x).Day.ToString() + ", " +
                                                      fechaInicial.ToString("MMMM").First().ToString().ToUpper() +
                                                       String.Join("", fechaInicial.ToString("MMMM").Skip(1));
                                    }
                                }
                                else
                                {
                                    categoria.Nombre = /*currentDay*/fechaInicial.AddDays(x).ToString("dddd").First().ToString().ToUpper() +
                                                      String.Join("", /*currentDay*/fechaInicial.AddDays(x).ToString("dddd").Skip(1))
                                                      + " " + /*currentDay*/fechaInicial.AddDays(x).Day.ToString() + ", " +
                                                      fechaInicial.ToString("MMMM").First().ToString().ToUpper() +
                                                       String.Join("", fechaInicial.ToString("MMMM").Skip(1));
                                }

                                categoria.FechaInicio = /*currentDay*/fechaInicial.AddDays(x);
                                categoria.FechaFin = /*currentDay*/fechaInicial.AddDays(x);
                                categoria.Oportunidad = data.Where(p => p.FechaInicio.Date == /*currentDay*/fechaInicial.AddDays(x).Date).OrderBy(p => p.FechaInicio).Distinct().ToList();

                                listaOportunidad.Add(categoria);

                                if (fechaInicial.AddDays(x) >= fechaFinal) { exit = true; }
                            }
                        }
                    }

                    if (dayRange > 7 && dayRange <= 29)
                    {

                        for (int x = 0; x < 5; x++)
                        {
                            if (!exit)
                            {
                                int daysInWeek = 7;

                                DateTime CalFechaInicial = beginDate.AddDays((daysInWeek * (x - 1) + 1));
                                DateTime CalFechaFinal = beginDate.AddDays(daysInWeek * (x));

                                if (x == 0)
                                {
                                    CalFechaInicial = fechaInicial;
                                    CalFechaFinal = beginDate;
                                }
                                if (CalFechaFinal.Date > fechaFinal.Date)
                                {
                                    CalFechaFinal = fechaFinal;
                                    exit = true;

                                }

                                string beginWeekDay = CalFechaInicial.ToString("dddd").First().ToString().ToUpper() +
                                                      String.Join("", CalFechaInicial.ToString("dddd").Skip(1));

                                string beginMontName = String.Empty;
                                if (CalFechaInicial.Year * 100 + CalFechaInicial.Month != CalFechaFinal.Year * 100 + CalFechaFinal.Month)
                                    beginMontName = CalFechaInicial.ToString("MMMM").First().ToString().ToUpper() + String.Join("", CalFechaInicial.ToString("MMMM").Skip(1));

                                string endWeekDay = CalFechaFinal.ToString("dddd").First().ToString().ToUpper() +
                                                      String.Join("", CalFechaFinal.ToString("dddd").Skip(1));

                                string textoEndMonth = CalFechaFinal.ToString("MMMM").First().ToString().ToUpper() +
                                                       String.Join("", CalFechaFinal.ToString("MMMM").Skip(1));

                                categoria = new OportunidadCategoria();
                                categoria.isBusqueda = false;

                                //categoria.Nombre = beginWeekDay + " " + CalFechaInicial.Day.ToString() + " - " + endWeekDay + " " + CalFechaFinal.Day.ToString() + ", " + textoEndMonth;

                                if (String.IsNullOrEmpty(beginMontName))
                                {
                                    if (beginWeekDay + " " + CalFechaInicial.Day.ToString() != endWeekDay + " " + CalFechaFinal.Day.ToString())
                                        categoria.Nombre = beginWeekDay + " " + CalFechaInicial.Day.ToString() + " - " + endWeekDay + " " + CalFechaFinal.Day.ToString() + ", " + textoEndMonth;
                                    else
                                        categoria.Nombre = beginWeekDay + " " + CalFechaInicial.Day.ToString() + ", " + textoEndMonth;
                                }
                                else
                                    categoria.Nombre = beginWeekDay + " " + CalFechaInicial.Day.ToString() + ", " + beginMontName + " - " + endWeekDay + " " + CalFechaFinal.Day.ToString() + ", " + textoEndMonth;

                                categoria.FechaInicio = CalFechaInicial.AddDays(1);
                                categoria.FechaFin = CalFechaFinal;
                                categoria.Oportunidad = data.Where(p => (p.FechaInicio.Date >= CalFechaInicial.Date)
                                                                        && (p.FechaInicio.Date <= CalFechaFinal.Date))
                                                                        .OrderBy(p => p.FechaInicio).Distinct().ToList();

                                listaOportunidad.Add(categoria);
                            }
                        }
                    }

                    if (dayRange > 29)
                    {
                        for (int x = 1; x < 20; x++)
                        {
                            if (!exit)
                            {
                                DateTime CalFechaInicial = fechaInicial.AddDays(-(fechaInicial.Day-1)).AddMonths(x - 1);
                                DateTime CalFechaFinal = new DateTime(CalFechaInicial.Year, CalFechaInicial.Month, DateTime.DaysInMonth(CalFechaInicial.Year, CalFechaInicial.Month));

                                if (CalFechaFinal.Date > fechaFinal.Date)
                                {
                                    CalFechaFinal = fechaFinal;
                                    exit = true;
                                }

                                categoria = new OportunidadCategoria();
                                categoria.isBusqueda = false;

                                categoria.Nombre = CalFechaInicial.ToString("MMMM").First().ToString().ToUpper() + String.Join("", CalFechaInicial.ToString("MMMM").Skip(1));
                                if (x == 1) { categoria.FechaInicio = CalFechaInicial; }
                                else { categoria.FechaInicio = CalFechaInicial.AddDays(1); }

                                categoria.FechaFin = CalFechaFinal;
                                categoria.Oportunidad = data.Where(p => (p.FechaInicio.Date >= CalFechaInicial.Date)
                                                                        && (p.FechaInicio.Date <= CalFechaFinal.Date))
                                                                        .OrderBy(p => p.FechaInicio).Distinct().ToList();

                                listaOportunidad.Add(categoria);
                            }
                        }
                    }

                    break;

                case Rp3.AgendaComercial.Models.Constantes.OportunidadGroupMode.Estado:

                    var estados = data.Select(p => p.EstadoDescripcion).Distinct().OrderBy(p => p);

                    foreach (var estadolabel in estados)
                    {
                        categoria = new OportunidadCategoria();
                        categoria.isBusqueda = false;
                        categoria.Nombre = estadolabel;

                        categoria.Oportunidad = data.Where(p => p.EstadoDescripcion == estadolabel).OrderBy(p => p.FechaInicio).ToList();

                        listaOportunidad.Add(categoria);
                    }

                    break;

                case Rp3.AgendaComercial.Models.Constantes.OportunidadGroupMode.Agente:

                    var agentes = data.Select(p => p.Agente).Distinct().OrderBy(p => p);

                    foreach (var agente in agentes)
                    {
                        categoria = new OportunidadCategoria();
                        categoria.isBusqueda = false;
                        categoria.Nombre = agente;

                        categoria.Oportunidad = data.Where(p => p.Agente == agente).OrderBy(p => p.FechaInicio).ToList();

                        listaOportunidad.Add(categoria);
                    }

                    break;

                case Rp3.AgendaComercial.Models.Constantes.OportunidadGroupMode.Listado:

                        categoria = new OportunidadCategoria();
                        categoria.isBusqueda = false;
                        categoria.Nombre = String.Empty;

                        categoria.Oportunidad = data.OrderBy(p => p.FechaInicio).ToList();

                        listaOportunidad.Add(categoria);

                    break;
            }

            //if (fechaInicial < DateTime.Now.Date)
            //{
            //    if (groupMode == Rp3.AgendaComercial.Models.Constantes.OportunidadGroupMode.Calendario)
            //        listaOportunidad = listaOportunidad.OrderByDescending(p => p.FechaInicio).ToList();
            //}

            if (String.IsNullOrEmpty(sortField))
            {
                foreach (var cat in listaOportunidad)
                    cat.Oportunidad = cat.Oportunidad.OrderByDescending(p => p.Calificacion).ThenByDescending(p => p.Importe).ToList();
            }
            else
            {
                Sort(listaOportunidad, sortField, sortMode);
            }

            oportunidadConsulta.OportunidadCategorias = listaOportunidad;

            return PartialView("_OportunidadListaDetalle", oportunidadConsulta);
        }

        public void Sort(List<OportunidadCategoria> categorias, string sortField, int sortMode)
        {
            switch (sortField)
            {
                case "fecha": 
                     foreach (var cat in categorias)
                         if (sortMode == 1) 
                             cat.Oportunidad = cat.Oportunidad.OrderBy(p => p.FechaInicio).ToList();
                         else
                             cat.Oportunidad = cat.Oportunidad.OrderByDescending(p => p.FechaInicio).ToList();
                     break;

                case "oportunidad":
                     foreach (var cat in categorias)
                         if (sortMode == 1)
                             cat.Oportunidad = cat.Oportunidad.OrderBy(p => p.Descripcion).ToList();
                         else
                             cat.Oportunidad = cat.Oportunidad.OrderByDescending(p => p.Descripcion).ToList();
                     break;

                case "probabilidad":
                     foreach (var cat in categorias)
                         if (sortMode == 1)
                             cat.Oportunidad = cat.Oportunidad.OrderBy(p => p.Probabilidad).ToList();
                         else
                             cat.Oportunidad = cat.Oportunidad.OrderByDescending(p => p.Probabilidad).ToList();
                     break;

                case "importe":
                     foreach (var cat in categorias)
                         if (sortMode == 1)
                             cat.Oportunidad = cat.Oportunidad.OrderBy(p => p.Importe).ToList();
                         else
                             cat.Oportunidad = cat.Oportunidad.OrderByDescending(p => p.Importe).ToList();
                     break;

                case "estado":
                     foreach (var cat in categorias)
                         if (sortMode == 1)
                             cat.Oportunidad = cat.Oportunidad.OrderBy(p => p.EstadoDescripcion).ToList();
                         else
                             cat.Oportunidad = cat.Oportunidad.OrderByDescending(p => p.EstadoDescripcion).ToList();
                     break;

                case "agente":
                     foreach (var cat in categorias)
                         if (sortMode == 1)
                             cat.Oportunidad = cat.Oportunidad.OrderBy(p => p.Agente).ToList();
                         else
                             cat.Oportunidad = cat.Oportunidad.OrderByDescending(p => p.Agente).ToList();
                     break;

                case "diastranscurridos":
                     foreach (var cat in categorias)
                         if (sortMode == 1)
                             cat.Oportunidad = cat.Oportunidad.OrderBy(p => p.DiasTranscurridos).ToList();
                         else
                             cat.Oportunidad = cat.Oportunidad.OrderByDescending(p => p.DiasTranscurridos).ToList();
                     break;

                case "diasinactividad":
                     foreach (var cat in categorias)
                         if (sortMode == 1)
                             cat.Oportunidad = cat.Oportunidad.OrderBy(p => p.DiasInactividad).ToList();
                         else
                             cat.Oportunidad = cat.Oportunidad.OrderByDescending(p => p.DiasInactividad).ToList();
                     break;

                case "etapa":
                     foreach (var cat in categorias)
                         if (sortMode == 1)
                             cat.Oportunidad = cat.Oportunidad.OrderBy(p => p.EtapaOrden).ToList();
                         else
                             cat.Oportunidad = cat.Oportunidad.OrderByDescending(p => p.EtapaOrden).ToList();
                     break;

                case "calificacion":
                     foreach (var cat in categorias)
                         if (sortMode == 1)
                             cat.Oportunidad = cat.Oportunidad.OrderBy(p => p.Calificacion).ToList();
                         else
                             cat.Oportunidad = cat.Oportunidad.OrderByDescending(p => p.Calificacion).ToList();
                     break;
            }
        }

        [ChildAction(Order = 1)]
        [Rp3.Web.Mvc.Authorize("OPORTUNIDAD", "QUERY", "AGENDACOMERCIAL", Order = 2)]
        [HttpGet]
        public ActionResult GetListadoSearch(string busqueda, int pagina, int numreg, string estado)
        {
            List<OportunidadCategoria> listaCategoria = new List<OportunidadCategoria>();
            OportunidadConsulta oportunidadConsulta = new OportunidadConsulta();
            OportunidadCategoria categoria = new OportunidadCategoria();

            List<int> listAgente = new List<int>();

            if (!this.TodosMisAgentes)
            {
                listAgente.Add(this.AgenteWork.IdAgente);
            }
            else
            {
                foreach (var agente in this.AgentesWork)
                    listAgente.Add(agente.IdAgente);
            }
            var data = DataBase.Oportunidades.GetListadoFullTextSearch(listAgente, busqueda, pagina, numreg, estado);

            categoria = new OportunidadCategoria();

            if (data.Count > 0)
            {
                if (data[0].TotalPages > 1)
                    categoria.Nombre = data[0].CurrentPage.ToString() + "/" + data[0].TotalPages.ToString() + " " + Rp3.AgendaComercial.Resources.LabelFor.Paginas + " [" + data[0].TotalRows.ToString() + " " + Rp3.AgendaComercial.Resources.LabelFor.Registros + "]";
                else
                    categoria.Nombre = "[" + data[0].TotalRows.ToString() + " " + Rp3.AgendaComercial.Resources.LabelFor.Registros + "]";
            }
            else
            {
                categoria.Nombre = data.Count.ToString() + " " + Rp3.AgendaComercial.Resources.LabelFor.Registros;
            }

            categoria.isBusqueda = true;

            if (data.Count > 0)
            {
                categoria.FechaInicio = DateTime.Today;
                categoria.FechaFin = DateTime.Today;
                categoria.Oportunidad = data.ToList();
            }
            listaCategoria.Add(categoria);

            oportunidadConsulta.OportunidadCategorias = listaCategoria;

            return PartialView("_OportunidadListaDetalle", oportunidadConsulta);
        }

        protected static DateTime GetNextSunday(DateTime date)
        {
            if (date.DayOfWeek == DayOfWeek.Sunday)
                date = date.AddDays(1);
            while (date.DayOfWeek != DayOfWeek.Sunday)
                date = date.AddDays(1);

            return date;
        }
        protected static int GetRemainderSundays(DateTime currentDay)
        {
            int numSundaysLeft = 0;
            for (int i = currentDay.Day; i <= DateTime.DaysInMonth(currentDay.Year, currentDay.Month); i++)
            {
                if (new DateTime(currentDay.Year, currentDay.Month, i).DayOfWeek == DayOfWeek.Sunday)
                    numSundaysLeft++;
            }

            return numSundaysLeft;
        }

        #endregion Listado

        #region MODAL_DELETE

        [ChildAction(Order = 1)]
        [Rp3.Web.Mvc.Authorize("OPORTUNIDAD", "QUERY", "AGENDACOMERCIAL", Order = 2)]
        [HttpPost]
        public ActionResult Delete(int idOportunidad)
        {
            try
            {
                Models.Oportunidad.Oportunidad modelDelete = DataBase.Oportunidades.Get(p => p.IdOportunidad == idOportunidad).SingleOrDefault();
                
                modelDelete.Estado = Rp3.AgendaComercial.Models.Constantes.EstadoOportunidad.Eliminado;
                modelDelete.UsrMod = this.UserLogonName;
                modelDelete.FecMod = this.GetCurrentDateTime();
                DataBase.Oportunidades.Update(modelDelete);
                DataBase.Save();

                this.AddDefaultSuccessMessage();
            }
            catch
            {
                this.AddDefaultErrorMessage();
            }

            return Json();
        }

        #endregion MODAL_DELETE
    }
}