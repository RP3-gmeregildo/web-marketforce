using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Rp3.Data.Models.Security;
using Rp3.Web.Mvc.Application.Security.Models;
using Rp3.Web.Mvc;
using Rp3.AgendaComercial.Models;
using Rp3.AgendaComercial.Web.Services.Ruta.Models;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Converters;
using Rp3.AgendaComercial.Models.Ruta.View;
using Rp3.AgendaComercial.Models.General.View;
using Rp3.AgendaComercial.Models.General;

namespace Rp3.AgendaComercial.Web.Ruta.Controllers
{
    public class AgendaController : Rp3.AgendaComercial.Web.Controllers.AgendaComercialController
    {
        #region Main

        [AgenteRequired]
        [Rp3.Web.Mvc.Authorize("AGENDA", "QUERY", "AGENDACOMERCIAL")]
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
                    return RedirectToAction("SeleccionAgente");
                }
            }

            ViewBag.EsSupervisor = (this.Agente.CargoRol == AgenteCargoRol.Supervisor || this.Agente.CargoRol == AgenteCargoRol.Gerente);
            ViewBag.TodosMisAgentes = this.TodosMisAgentes;

            return View(AgenteWork);
        }

        [Rp3.Web.Mvc.Authorize("AGENDA", "QUERY", "AGENDACOMERCIAL")]
        public ActionResult CambiarAgente()
        {
            this.UserWorkId = null;
            this.TodosMisAgentes = false;
            this.UserWorkIds = null;

            return RedirectToAction("SeleccionAgente");
        }

        [Rp3.Web.Mvc.Authorize("AGENDA", "QUERY", "AGENDACOMERCIAL")]
        public ActionResult SeleccionAgente()
        {
            //ViewBag.AgenteSelectList = DataBase.Agentes.Get(p => p.IdSupervisor == this.Agente.IdAgente && p.Estado == Models.Constantes.Estado.Activo).ToSelectList(includeNullItem: true);
            ViewBag.AgenteSelectList = DataBase.Agentes.GetAgentesPermitidos(this.Agente.IdAgente, true).ToSelectList(includeNullItem: true);
            return View();
        }

        [HttpPost]
        [Rp3.Web.Mvc.Authorize("AGENDA", "QUERY", "AGENDACOMERCIAL")]
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
        public ActionResult GetCalendario()
        {
            return PartialView("_AgendaCalendario");
        }

        [HttpGet]
        public ActionResult GetLista()
        {
            ViewBag.TodosMisAgentes = this.TodosMisAgentes;

            return PartialView("_AgendaLista");
        }

        [HttpGet]
        public ActionResult GetPhotoBook()
        {
            ViewBag.TodosMisAgentes = this.TodosMisAgentes;

            return PartialView("_AgendaPhotoBook");
        }

        [HttpGet]
        public ActionResult GetSeleccionCliente(string idruta)
        {
            var model = DataBase.Clientes.GetClienteContactoSearchText(String.Empty, idruta, 0).OrderBy(p => p.DescriptionName).ToList();

            return PartialView("_SeleccionCliente", model);
        }

        [HttpGet]
        [Rp3.Web.Mvc.Authorize("AGENDA", "QUERY", "AGENDACOMERCIAL", Order = 2)]
        public ActionResult ExpandMap(int idRuta, long idAgenda)
        {
            var data = DataBase.Agendas.GetAgendaDetalle(idRuta, idAgenda);
            var agenda = DataBase.Agendas.Get(p => p.IdRuta == idRuta && p.IdAgenda == idAgenda).FirstOrDefault();
            var cliente = data.FirstOrDefault();

            List<Ubicacion> ubicaciones = GetAgendaMap(cliente.Latitud, cliente.Longitud, agenda.Latitud, agenda.Longitud, agenda.EstadoAgenda, cliente.ClienteContacto, cliente.Agente);

            ViewBag.MapSelector = "viewMapExpand";
            
            ViewBag.ClienteContacto = cliente.ClienteContacto;
            ViewBag.Agente = cliente.Agente;
            ViewBag.Direccion = cliente.Direccion;
            ViewBag.Color = cliente.Color;
            ViewBag.ColorDetalle = cliente.ColorDetalle;
            ViewBag.FechaInicio = cliente.FechaInicio;
            ViewBag.FechaFin = cliente.FechaFin;
            ViewBag.FechaInicioGestion = cliente.FechaInicioGestion;
            ViewBag.FechaFinGestion = cliente.FechaFinGestion;
            ViewBag.EstadoAgenda = cliente.EstadoAgenda;
            ViewBag.OrigenImageUrl = cliente.OrigenImageUrl;
            ViewBag.IdAgente = cliente.IdAgente;

            return PartialView("_UbicacionMapMarkerExpand", ubicaciones);
        }

        public Rp3.AgendaComercial.Web.BinaryResult Preview(int idRuta, long idAgenda, string addressBase)
        {
            PartialViewResult data = GetDatos(idRuta, idAgenda) as PartialViewResult;

            AgendaConsulta agenda = data.Model as AgendaConsulta;
            agenda.AgendaClientes.ForEach(p => p.SetSecuencia());
            //PartialViewResult map = ExpandMap(idRuta, idAgenda) as PartialViewResult;
            data.ViewBag.MapWidth = 450;
            data.ViewBag.MapHeight = 195;
            data.ViewBag.UrlBase = string.Format("{0}://{1}{2}", this.Request.UrlReferrer.Scheme, this.Request.UrlReferrer.Authority, string.IsNullOrEmpty(addressBase) ? string.Empty : string.Format("/{0}", addressBase));
            Rp3.AgendaComercial.Web.Areas.Ruta.Reports.Agenda report = new Areas.Ruta.Reports.Agenda(data.ViewBag, data.Model as AgendaConsulta);
            System.IO.MemoryStream reportStream = new System.IO.MemoryStream();
            report.ExportToPdf(reportStream);
            reportStream.Position = 0;
            return new Rp3.AgendaComercial.Web.BinaryResult { Data = reportStream.ToArray(), ContentType = "application/pdf", FileName = string.Format("Agenda.{0:yyyy.MM.dd}.{1:HH.mm.ss}.pdf", DateTime.Now, DateTime.Now) };

        }

        [HttpGet]
        public ActionResult GetSort()
        {
            return PartialView("_SeleccionSort");
        }

        private List<Ubicacion> GetAgendaMap(double? clienteLatitud, double? clienteLongitud, double? agendaLatitud, double? agendaLongitud, string estadoAgenda, string cliente, string agente)
        {
            List<Ubicacion> ubicaciones = new List<Ubicacion>();

            var distance = Ubicacion.Distance(agendaLongitud ?? 0, agendaLatitud ?? 0, clienteLongitud ?? 0, clienteLatitud ?? 0);

            if (clienteLongitud != null && clienteLatitud != null && clienteLongitud != 0 && clienteLatitud != 0)
            {
                ubicaciones.Add(new Ubicacion() { IdUbicacion = 1, MarkerIndex = 1, Titulo = cliente, Latitud = clienteLatitud, Longitud = clienteLongitud });
                ViewBag.ShowClienteSinUbicacion = false;
            }
            else
            {
                ViewBag.ShowClienteSinUbicacion = true;
            }

            if (agendaLongitud != null && agendaLatitud != null && agendaLongitud != 0 && agendaLatitud != 0)
            {
                ubicaciones.Add(new Ubicacion() { IdUbicacion = 2, MarkerIndex = 2, Titulo = agente, Latitud = agendaLatitud, Longitud = agendaLongitud });
                ViewBag.ShowGestionSinUbicacion = false;
            }
            else
            {
                ViewBag.ShowGestionSinUbicacion = estadoAgenda == Rp3.AgendaComercial.Models.Constantes.EstadoAgenda.Visitada;
            }

            ViewBag.SuppressRouteMarkers = true;
            ViewBag.MapRadiusMode = true;

            DataBase.ParametroHelper.Load();
            ViewBag.MapRadius = DataBase.ParametroHelper.VisitDistance;

            return ubicaciones;
        }

        #endregion

        #region CALENDARIO

        [ChildAction(Order = 1)]
        [Rp3.Web.Mvc.Authorize("AGENDA", "QUERY", "AGENDACOMERCIAL", Order = 2)]
        public string GetAllData(int start = 0, int end = 0)
        {
            DateTime time1 = new DateTime((start * Convert.ToInt64(Math.Pow(10, 9))) / 100);
            time1 = time1.AddYears(1970 - 1);

            DateTime time2 = new DateTime((end * Convert.ToInt64(Math.Pow(10, 9))) / 100);
            time2 = time2.AddYears(1970 - 1);

            var data_json = DataBase.Agendas.GetAgendaUsuario(this.UserWorkId ?? 0, time1, time2);

            AgendaConsulta agendaConsulta = new AgendaConsulta();
            agendaConsulta.Appointments = data_json;

            return JsonConvert.SerializeObject(data_json, Formatting.None, new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });
        }

        [ChildAction(Order = 1)]
        [Rp3.Web.Mvc.Authorize("AGENDA", "QUERY", "AGENDACOMERCIAL", Order = 2)]
        [HttpPost]
        public ActionResult EditData(long id, DateTime fechaInicio, DateTime? fechaFin, string opt)
        {
            try
            {
                long t_id = Convert.ToInt64(id);
                int t_opt = Convert.ToInt32(opt);
                int idruta = this.AgenteWork.IdRuta.Value;
                var agenda = DataBase.Agendas.Get(p => p.IdRuta == idruta && p.IdAgenda == t_id).SingleOrDefault();

                DateTime time1 = fechaInicio;
                DateTime time2 = fechaInicio;

                if (fechaFin.HasValue)
                {
                    time2 = fechaFin.Value;
                }

                agenda.FechaInicio = time1;
                agenda.FechaFin = time2;
                agenda.FecMod = GetCurrentDateTime();
                agenda.UsrMod = this.UserLogonName;

                if (agenda.EstadoAgenda == Constantes.EstadoAgenda.Pendiente ||
                    agenda.EstadoAgenda == Constantes.EstadoAgenda.Reprogramada ||
                    agenda.EstadoAgenda == Constantes.EstadoAgenda.NoVisitado)
                {
                    if (opt == "1")
                    {
                        agenda.EstadoAgenda = Constantes.EstadoAgenda.Reprogramada;
                    }
                }

                DataBase.Agendas.Update(agenda);
                DataBase.Save();

                this.AddDefaultSuccessMessage();
            }
            catch (Exception e)
            {
                this.AddDefaultErrorMessage();
            }
            return Json();
        }

        #endregion

        #region LISTADO

        [HttpGet]
        public ActionResult GetListadoInicial(DateTime fechaInicial, DateTime fechaFinal, string sortField, int sortMode, string groupMode = Rp3.AgendaComercial.Models.Constantes.AgendaGroupMode.Calendario)
        {
            fechaInicial = fechaInicial.Date;
            fechaFinal = fechaFinal.Date.AddHours(23).AddMinutes(59).AddSeconds(59); ;
            /*
             * 1 = Next Few Days
             * 2 = Next Weeks (Until last Week of Month)
             * 3 = Next Month
             */
            //Modelo
            List<AgendaCategoria> listaCategoria = new List<AgendaCategoria>();
            AgendaConsulta agendaConsulta = new AgendaConsulta();
            AgendaCategoria categoria = new AgendaCategoria();

            //Current Day
            DateTime currentDay = DateTime.Now;
            //Find Remainder Sundays
            int remainderSundays = GetRemainderSundays(fechaInicial);// (currentDay);
            //First Sunday
            DateTime beginDate = GetNextSunday(fechaInicial);// (currentDay);

            //Range of Search
            int dayRange = fechaFinal.Date == fechaInicial.Date ? 1 : (fechaFinal - fechaInicial).Days;
            //Days Left Until End of Week
            int daysLeft = (GetNextSunday(fechaInicial) - fechaInicial).Days;

            DateTime endOfMonth = new DateTime(beginDate.Year, beginDate.Month, DateTime.DaysInMonth(beginDate.Year, beginDate.Month));
            int daysLeftEndOfMonth = (endOfMonth - beginDate.AddDays(1)).Days;

            int weeksLeftEndOfMonth = (int)Math.Ceiling((daysLeftEndOfMonth / 7.0));
            int remainderdaysMonthSearch = dayRange - daysLeft - daysLeftEndOfMonth;

            List<int> listRuta = new List<int>();

            if (!this.TodosMisAgentes)
            {
                if (this.AgenteWork.IdRuta != null)
                {
                    listRuta.Add(this.AgenteWork.IdRuta.Value);
                }
            }
            else
            {
                foreach (var agente in this.AgentesWork)
                {
                    if (agente.IdRuta != null)
                        listRuta.Add(agente.IdRuta.Value);
                }
            }

            var data = DataBase.Agendas.GetAgendaListado(listRuta, fechaInicial, fechaFinal);

            switch (groupMode)
            {
                case Rp3.AgendaComercial.Models.Constantes.AgendaGroupMode.Calendario:

                    bool exit = false;
                    if (dayRange >= 1)
                    {
                        for (int x = 0; x <= daysLeft; x++)
                        {
                            if (!exit)
                            {
                                categoria = new AgendaCategoria();
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
                                categoria.Agenda = data.Where(p => p.fechaInicio.Date == /*currentDay*/fechaInicial.AddDays(x).Date).OrderBy(p => p.fechaInicio).Distinct().ToList();

                                listaCategoria.Add(categoria);
                                dayRange--;

                                //if (fechaInicial.AddDays(x) >= fechaFinal) { exit = true; }
                            }
                        }
                    }

                    if (weeksLeftEndOfMonth > 0)
                    {

                        for (int x = 0; x < weeksLeftEndOfMonth; x++)
                        {
                            DateTime beginweek = GetNextSunday(beginDate);
                            if (!exit)
                            {
                                int daysInWeek = 7;

                                DateTime CalFechaInicial = beginweek.AddDays((daysInWeek * (x - 1) + 1));
                                DateTime CalFechaFinal = beginweek.AddDays(daysInWeek * (x));

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

                                categoria = new AgendaCategoria();
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
                                categoria.Agenda = data.Where(p => (p.fechaInicio.Date >= CalFechaInicial.Date)
                                                                        && (p.fechaInicio.Date <= CalFechaFinal.Date))
                                                                        .OrderBy(p => p.fechaInicio).Distinct().ToList();

                                listaCategoria.Add(categoria);
                            }
                        }
                    }

                    if (remainderdaysMonthSearch > 0)
                    {
                        DateTime monthbegin = endOfMonth.AddDays(1);

                        for (int x = 1; x <= 2; x++)
                        {
                            if (!exit)
                            {
                                DateTime CalFechaInicial = monthbegin.AddMonths(x - 1);
                                DateTime CalFechaFinal = new DateTime(CalFechaInicial.Year, CalFechaInicial.Month, DateTime.DaysInMonth(CalFechaInicial.Year, CalFechaInicial.Month));

                                if (CalFechaFinal.Date > fechaFinal.Date)
                                {
                                    CalFechaFinal = fechaFinal;
                                    exit = true;
                                }

                                categoria = new AgendaCategoria();
                                categoria.isBusqueda = false;

                                categoria.Nombre = CalFechaInicial.ToString("MMMM").First().ToString().ToUpper() + String.Join("", CalFechaInicial.ToString("MMMM").Skip(1));
                                if (x == 1) { categoria.FechaInicio = CalFechaInicial; }
                                else { categoria.FechaInicio = CalFechaInicial.AddDays(1); }

                                categoria.FechaFin = CalFechaFinal;
                                categoria.Agenda = data.Where(p => (p.fechaInicio.Date >= CalFechaInicial.Date)
                                                                        && (p.fechaInicio.Date <= CalFechaFinal.Date))
                                                                        .OrderBy(p => p.fechaInicio).Distinct().ToList();

                                listaCategoria.Add(categoria);
                            }
                        }
                    }

                    break;

                case Rp3.AgendaComercial.Models.Constantes.AgendaGroupMode.Cliente:

                    var clientes = data.Select(p => p.ClienteContacto).Distinct().OrderBy(p => p);

                    foreach (var cliente in clientes)
                    {
                        categoria = new AgendaCategoria();
                        categoria.isBusqueda = false;
                        categoria.Nombre = cliente;

                        categoria.Agenda = data.Where(p => p.ClienteContacto == cliente).OrderBy(p => p.fechaInicio).ToList();

                        listaCategoria.Add(categoria);
                    }

                    break;

                case Rp3.AgendaComercial.Models.Constantes.AgendaGroupMode.Agente:

                    var agentes = data.Select(p => p.Agente).Distinct().OrderBy(p => p);

                    foreach (var agente in agentes)
                    {
                        categoria = new AgendaCategoria();
                        categoria.isBusqueda = false;
                        categoria.Nombre = agente;

                        categoria.Agenda = data.Where(p => p.Agente == agente).OrderBy(p => p.fechaInicio).ToList();

                        listaCategoria.Add(categoria);
                    }

                    break;
            }

            if (fechaInicial < DateTime.Now.Date)
            {
                if (groupMode == Rp3.AgendaComercial.Models.Constantes.AgendaGroupMode.Calendario)
                    listaCategoria = listaCategoria.OrderBy(p => p.FechaInicio).ToList();
            }

            if (String.IsNullOrEmpty(sortField))
            {
                foreach (var cat in listaCategoria)
                    cat.Agenda = cat.Agenda.OrderBy(p => p.fechaInicio).ToList();
            }
            else
            {
                Sort(listaCategoria, sortField, sortMode);
            }

            agendaConsulta.AgendaCategorias = listaCategoria;

            ViewBag.TodosMisAgentes = this.TodosMisAgentes;

            return PartialView("_AgendaListaDetalle", agendaConsulta);
        }

        [HttpGet]
        public ActionResult GetListado(DateTime fechaInicial, DateTime fechaFinal, string sortField, int sortMode, string groupMode = Rp3.AgendaComercial.Models.Constantes.AgendaGroupMode.Calendario)
        {
            fechaInicial = fechaInicial.Date;
            fechaFinal = fechaFinal.Date.AddDays(1).AddSeconds(-1); // .AddHours(23).AddMinutes(59).AddSeconds(59); ;
            /*
             * 1 = Next Few Days
             * 2 = Next Weeks (Until last Week of Month)
             * 3 = Next Month
             */
            //Modelo
            List<AgendaCategoria> listaCategoria = new List<AgendaCategoria>();
            AgendaConsulta agendaConsulta = new AgendaConsulta();
            AgendaCategoria categoria = new AgendaCategoria();

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

            List<int> listRuta = new List<int>();

            if (!this.TodosMisAgentes)
            {
                if (this.AgenteWork.IdRuta != null)
                {
                    listRuta.Add(this.AgenteWork.IdRuta.Value);
                }
            }
            else
            {
                foreach (var agente in this.AgentesWork)
                {
                    listRuta.Add(agente.IdRuta.Value);
                }
            }

            var data = DataBase.Agendas.GetAgendaListado(listRuta, fechaInicial, fechaFinal);

            switch (groupMode)
            {
                case Rp3.AgendaComercial.Models.Constantes.AgendaGroupMode.Calendario:

                    bool exit = false;
                    if (dayRange <= 7)
                    {
                        for (int x = 0; x <= 7; x++)
                        {
                            if (!exit)
                            {
                                categoria = new AgendaCategoria();
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
                                categoria.Agenda = data.Where(p => p.fechaInicio.Date == /*currentDay*/fechaInicial.AddDays(x).Date).OrderBy(p => p.fechaInicio).Distinct().ToList();

                                listaCategoria.Add(categoria);

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

                                categoria = new AgendaCategoria();
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
                                categoria.Agenda = data.Where(p => (p.fechaInicio.Date >= CalFechaInicial.Date)
                                                                        && (p.fechaInicio.Date <= CalFechaFinal.Date))
                                                                        .OrderBy(p => p.fechaInicio).Distinct().ToList();

                                listaCategoria.Add(categoria);
                            }
                        }
                    }

                    if (dayRange > 29)
                    {
                        for (int x = 1; x < 20; x++)
                        {
                            if (!exit)
                            {
                                DateTime CalFechaInicial = fechaInicial.AddMonths(x - 1);
                                DateTime CalFechaFinal = new DateTime(CalFechaInicial.Year, CalFechaInicial.Month, DateTime.DaysInMonth(CalFechaInicial.Year, CalFechaInicial.Month));

                                if (CalFechaFinal.Date > fechaFinal.Date)
                                {
                                    CalFechaFinal = fechaFinal;
                                    exit = true;
                                }

                                categoria = new AgendaCategoria();
                                categoria.isBusqueda = false;

                                categoria.Nombre = CalFechaInicial.ToString("MMMM").First().ToString().ToUpper() + String.Join("", CalFechaInicial.ToString("MMMM").Skip(1));
                                if (x == 1) { categoria.FechaInicio = CalFechaInicial; }
                                else { categoria.FechaInicio = CalFechaInicial.AddDays(1); }

                                categoria.FechaFin = CalFechaFinal;
                                categoria.Agenda = data.Where(p => (p.fechaInicio.Date >= CalFechaInicial.Date)
                                                                        && (p.fechaInicio.Date <= CalFechaFinal.Date))
                                                                        .OrderBy(p => p.fechaInicio).Distinct().ToList();

                                listaCategoria.Add(categoria);
                            }
                        }
                    }

                    break;

                case Rp3.AgendaComercial.Models.Constantes.AgendaGroupMode.Cliente:

                    var clientes = data.Select(p => p.ClienteContacto).Distinct().OrderBy(p => p);

                    foreach (var cliente in clientes)
                    {
                        categoria = new AgendaCategoria();
                        categoria.isBusqueda = false;
                        categoria.Nombre = cliente;

                        categoria.Agenda = data.Where(p => p.ClienteContacto == cliente).OrderBy(p => p.fechaInicio).ToList();

                        listaCategoria.Add(categoria);
                    }

                    break;

                case Rp3.AgendaComercial.Models.Constantes.AgendaGroupMode.Agente:

                    var agentes = data.Select(p => p.Agente).Distinct().OrderBy(p => p);

                    foreach (var agente in agentes)
                    {
                        categoria = new AgendaCategoria();
                        categoria.isBusqueda = false;
                        categoria.Nombre = agente;

                        categoria.Agenda = data.Where(p => p.Agente == agente).OrderBy(p => p.fechaInicio).ToList();

                        listaCategoria.Add(categoria);
                    }

                    break;
            }

            if (fechaInicial < DateTime.Now.Date)
            {
                if (groupMode == Rp3.AgendaComercial.Models.Constantes.AgendaGroupMode.Calendario)
                    listaCategoria = listaCategoria.OrderBy(p => p.FechaInicio).ToList();
            }


            if (String.IsNullOrEmpty(sortField))
            {
                foreach (var cat in listaCategoria)
                    cat.Agenda = cat.Agenda.OrderBy(p => p.fechaInicio).ToList();
            }
            else
            {
                Sort(listaCategoria, sortField, sortMode);
            }

            agendaConsulta.AgendaCategorias = listaCategoria;

            ViewBag.TodosMisAgentes = this.TodosMisAgentes;

            return PartialView("_AgendaListaDetalle", agendaConsulta);
        }

        public void Sort(List<AgendaCategoria> categorias, string sortField, int sortMode)
        {
            switch (sortField)
            {
                case "origen":
                    foreach (var cat in categorias)
                        if (sortMode == 1)
                            cat.Agenda = cat.Agenda.OrderBy(p => p.Origen).ToList();
                        else
                            cat.Agenda = cat.Agenda.OrderByDescending(p => p.Origen).ToList();
                    break;

                case "agente":
                    foreach (var cat in categorias)
                        if (sortMode == 1)
                            cat.Agenda = cat.Agenda.OrderBy(p => p.Agente).ToList();
                        else
                            cat.Agenda = cat.Agenda.OrderByDescending(p => p.Agente).ToList();
                    break;

                case "cliente":
                    foreach (var cat in categorias)
                        if (sortMode == 1)
                            cat.Agenda = cat.Agenda.OrderBy(p => p.ClienteContacto).ToList();
                        else
                            cat.Agenda = cat.Agenda.OrderByDescending(p => p.ClienteContacto).ToList();
                    break;

                case "fecha":
                    foreach (var cat in categorias)
                        if (sortMode == 1)
                            cat.Agenda = cat.Agenda.OrderBy(p => p.fechaInicio).ToList();
                        else
                            cat.Agenda = cat.Agenda.OrderByDescending(p => p.fechaInicio).ToList();
                    break;

                case "estado":
                    foreach (var cat in categorias)
                        if (sortMode == 1)
                            cat.Agenda = cat.Agenda.OrderBy(p => p.EstadoAgenda).ToList();
                        else
                            cat.Agenda = cat.Agenda.OrderByDescending(p => p.EstadoAgenda).ToList();
                    break;

                case "direccion":
                    foreach (var cat in categorias)
                        if (sortMode == 1)
                            cat.Agenda = cat.Agenda.OrderBy(p => p.Direccion).ToList();
                        else
                            cat.Agenda = cat.Agenda.OrderByDescending(p => p.Direccion).ToList();
                    break;
            }
        }

        [ChildAction(Order = 1)]
        [Rp3.Web.Mvc.Authorize("AGENDA", "QUERY", "AGENDACOMERCIAL", Order = 2)]
        [HttpGet]
        public ActionResult GetListadoSearch(string busqueda, int pagina, int numreg)
        {
            List<AgendaCategoria> listaCategoria = new List<AgendaCategoria>();
            AgendaConsulta agendaConsulta = new AgendaConsulta();
            AgendaCategoria categoria = new AgendaCategoria();

            List<int> listRuta = new List<int>();

            if (!this.TodosMisAgentes)
            {
                if (this.AgenteWork.IdRuta != null)
                {
                    listRuta.Add(this.AgenteWork.IdRuta.Value);
                }
            }
            else
            {
                foreach (var agente in this.AgentesWork)
                {
                    listRuta.Add(agente.IdRuta.Value);
                }
            }

            var data = DataBase.Agendas.GetListadoFullTextSearch(listRuta, busqueda, pagina, numreg, this.TodosMisAgentes);

            categoria = new AgendaCategoria();

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
                categoria.Agenda = data.ToList();
            }
            listaCategoria.Add(categoria);

            agendaConsulta.AgendaCategorias = listaCategoria;

            ViewBag.TodosMisAgentes = this.TodosMisAgentes;

            return PartialView("_AgendaListaDetalle", agendaConsulta);
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


        [ChildAction]
        [Rp3.Web.Mvc.Authorize("AGENDA", "QUERY", "AGENDACOMERCIAL", Order = 2)]
        public ActionResult GetAgentes()
        {
            var idAgente = this.AgenteWork.IdAgente;

            List<Rp3.AgendaComercial.Models.General.Agente> data = null;
            data = DataBase.Agentes.Get(p => p.IdSupervisor == idAgente).ToList();

            return new JsonResult() { Data = data.Select(p => new { id = p.IdAgente, text = p.Descripcion }), JsonRequestBehavior = System.Web.Mvc.JsonRequestBehavior.AllowGet };
        }

        #endregion

        #region PHOTOBOOK

        [HttpGet]
        public ActionResult GetPhotoBookInicial(DateTime fechaInicial, DateTime fechaFinal, string sortField, int sortMode, string groupMode = Rp3.AgendaComercial.Models.Constantes.AgendaGroupMode.Calendario)
        {
            fechaInicial = fechaInicial.Date;
            fechaFinal = fechaFinal.Date.AddHours(23).AddMinutes(59).AddSeconds(59); ;
            /*
             * 1 = Next Few Days
             * 2 = Next Weeks (Until last Week of Month)
             * 3 = Next Month
             */
            //Modelo

            List<AgendaCategoria> listaCategoria = new List<AgendaCategoria>();
            AgendaConsulta agendaConsulta = new AgendaConsulta();
            AgendaCategoria categoria = new AgendaCategoria();

            //Current Day
            DateTime currentDay = DateTime.Now;
            //Find Remainder Sundays
            int remainderSundays = GetRemainderSundays(fechaInicial);// (currentDay);
            //First Sunday
            DateTime beginDate = GetNextSunday(fechaInicial);// (currentDay);

            //Range of Search
            int dayRange = fechaFinal.Date == fechaInicial.Date ? 1 : (fechaFinal - fechaInicial).Days;
            //Days Left Until End of Week
            int daysLeft = (GetNextSunday(fechaInicial) - fechaInicial).Days;

            DateTime endOfMonth = new DateTime(beginDate.Year, beginDate.Month, DateTime.DaysInMonth(beginDate.Year, beginDate.Month));
            int daysLeftEndOfMonth = (endOfMonth - beginDate.AddDays(1)).Days;

            int weeksLeftEndOfMonth = (int)Math.Ceiling((daysLeftEndOfMonth / 7.0));
            int remainderdaysMonthSearch = dayRange - daysLeft - daysLeftEndOfMonth;

            List<int> listRuta = new List<int>();

            if (!this.TodosMisAgentes)
            {
                if (this.AgenteWork.IdRuta != null)
                {
                    listRuta.Add(this.AgenteWork.IdRuta.Value);
                }
            }
            else
            {
                foreach (var agente in this.AgentesWork)
                {
                    listRuta.Add(agente.IdRuta.Value);
                }
            }

            var data = DataBase.Agendas.GetAgendaPhotoBook(listRuta, fechaInicial, fechaFinal);

            switch (groupMode)
            {
                case Rp3.AgendaComercial.Models.Constantes.AgendaGroupMode.Calendario:

                    bool exit = false;

                    if (dayRange >= 1)
                    {
                        for (int x = 0; x < daysLeft; x++)
                        {
                            if (!exit)
                            {
                                categoria = new AgendaCategoria();
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
                                categoria.Agenda = data.Where(p => p.fechaInicio.Date == /*currentDay*/fechaInicial.AddDays(x).Date).OrderBy(p => p.fechaInicio).Distinct().ToList();

                                listaCategoria.Add(categoria);
                                dayRange--;

                                //if (fechaInicial.AddDays(x) >= fechaFinal) { exit = true; }
                            }
                        }
                    }

                    if (weeksLeftEndOfMonth > 0)
                    {

                        for (int x = 0; x < weeksLeftEndOfMonth; x++)
                        {
                            DateTime beginweek = GetNextSunday(beginDate);
                            if (!exit)
                            {
                                int daysInWeek = 7;

                                DateTime CalFechaInicial = beginweek.AddDays((daysInWeek * (x - 1) + 1));
                                DateTime CalFechaFinal = beginweek.AddDays(daysInWeek * (x));

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

                                categoria = new AgendaCategoria();
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
                                categoria.Agenda = data.Where(p => (p.fechaInicio.Date >= CalFechaInicial.Date)
                                                                        && (p.fechaInicio.Date <= CalFechaFinal.Date))
                                                                        .OrderBy(p => p.fechaInicio).Distinct().ToList();

                                listaCategoria.Add(categoria);
                            }
                        }
                    }

                    if (remainderdaysMonthSearch > 0)
                    {
                        DateTime monthbegin = endOfMonth.AddDays(1);

                        for (int x = 1; x <= 2; x++)
                        {
                            if (!exit)
                            {
                                DateTime CalFechaInicial = monthbegin.AddMonths(x - 1);
                                DateTime CalFechaFinal = new DateTime(CalFechaInicial.Year, CalFechaInicial.Month, DateTime.DaysInMonth(CalFechaInicial.Year, CalFechaInicial.Month));

                                if (CalFechaFinal.Date > fechaFinal.Date)
                                {
                                    CalFechaFinal = fechaFinal;
                                    exit = true;
                                }

                                categoria = new AgendaCategoria();
                                categoria.isBusqueda = false;

                                categoria.Nombre = CalFechaInicial.ToString("MMMM").First().ToString().ToUpper() + String.Join("", CalFechaInicial.ToString("MMMM").Skip(1));
                                if (x == 1) { categoria.FechaInicio = CalFechaInicial; }
                                else { categoria.FechaInicio = CalFechaInicial.AddDays(1); }

                                categoria.FechaFin = CalFechaFinal;
                                categoria.Agenda = data.Where(p => (p.fechaInicio.Date >= CalFechaInicial.Date)
                                                                        && (p.fechaInicio.Date <= CalFechaFinal.Date))
                                                                        .OrderBy(p => p.fechaInicio).Distinct().ToList();

                                listaCategoria.Add(categoria);
                            }
                        }
                    }

                    break;

                case Rp3.AgendaComercial.Models.Constantes.AgendaGroupMode.Cliente:

                    var clientes = data.Select(p => p.ClienteContacto).Distinct().OrderBy(p => p);

                    foreach (var cliente in clientes)
                    {
                        categoria = new AgendaCategoria();
                        categoria.isBusqueda = false;
                        categoria.Nombre = cliente;

                        categoria.Agenda = data.Where(p => p.ClienteContacto == cliente).OrderBy(p => p.fechaInicio).ToList();

                        listaCategoria.Add(categoria);
                    }

                    break;

                case Rp3.AgendaComercial.Models.Constantes.AgendaGroupMode.Agente:

                    var agentes = data.Select(p => p.Agente).Distinct().OrderBy(p => p);

                    foreach (var agente in agentes)
                    {
                        categoria = new AgendaCategoria();
                        categoria.isBusqueda = false;
                        categoria.Nombre = agente;

                        categoria.Agenda = data.Where(p => p.Agente == agente).OrderBy(p => p.fechaInicio).ToList();

                        listaCategoria.Add(categoria);
                    }

                    break;
            }

            //if (fechaInicial < DateTime.Now.Date)
            //{
            //    if (groupMode == Rp3.AgendaComercial.Models.Constantes.AgendaGroupMode.Calendario)
            //        listaCategoria = listaCategoria.OrderByDescending(p => p.FechaInicio).ToList();

            //    foreach (var cat in listaCategoria)
            //        cat.Agenda = cat.Agenda.OrderByDescending(p => p.fechaInicio).ToList();
            //}

            if (String.IsNullOrEmpty(sortField))
            {
                foreach (var cat in listaCategoria)
                    cat.Agenda = cat.Agenda.OrderBy(p => p.fechaInicio).ToList();
            }
            else
            {
                Sort(listaCategoria, sortField, sortMode);
            }

            agendaConsulta.AgendaCategorias = listaCategoria;

            ViewBag.TodosMisAgentes = this.TodosMisAgentes;

            return PartialView("_AgendaPhotoBookDetalle", agendaConsulta);
        }

        [HttpGet]
        public ActionResult GetPhotoBookListado(DateTime fechaInicial, DateTime fechaFinal, string sortField, int sortMode, string groupMode = Rp3.AgendaComercial.Models.Constantes.AgendaGroupMode.Calendario)
        {
            fechaInicial = fechaInicial.Date;
            fechaFinal = fechaFinal.Date.AddHours(23).AddMinutes(59).AddSeconds(59); ;
            /*
             * 1 = Next Few Days
             * 2 = Next Weeks (Until last Week of Month)
             * 3 = Next Month
             */
            //Modelo
            List<AgendaCategoria> listaCategoria = new List<AgendaCategoria>();
            AgendaConsulta agendaConsulta = new AgendaConsulta();
            AgendaCategoria categoria = new AgendaCategoria();

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

            List<int> listRuta = new List<int>();

            if (!this.TodosMisAgentes)
            {
                if (this.AgenteWork.IdRuta != null)
                {
                    listRuta.Add(this.AgenteWork.IdRuta.Value);
                }
            }
            else
            {
                foreach (var agente in this.AgentesWork)
                {
                    listRuta.Add(agente.IdRuta.Value);
                }
            }

            var data = DataBase.Agendas.GetAgendaPhotoBook(listRuta, fechaInicial, fechaFinal);

            switch (groupMode)
            {
                case Rp3.AgendaComercial.Models.Constantes.AgendaGroupMode.Calendario:

                    bool exit = false;
                    if (dayRange <= 7)
                    {
                        for (int x = 0; x <= 7; x++)
                        {
                            if (!exit)
                            {
                                categoria = new AgendaCategoria();
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
                                categoria.Agenda = data.Where(p => p.fechaInicio.Date == /*currentDay*/fechaInicial.AddDays(x).Date).OrderBy(p => p.fechaInicio).Distinct().ToList();

                                listaCategoria.Add(categoria);

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

                                categoria = new AgendaCategoria();
                                categoria.isBusqueda = false;

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
                                categoria.Agenda = data.Where(p => (p.fechaInicio.Date >= CalFechaInicial.Date)
                                                                        && (p.fechaInicio.Date <= CalFechaFinal.Date))
                                                                        .OrderBy(p => p.fechaInicio).Distinct().ToList();

                                listaCategoria.Add(categoria);
                            }
                        }
                    }

                    if (dayRange > 29)
                    {
                        for (int x = 1; x < 20; x++)
                        {
                            if (!exit)
                            {
                                DateTime CalFechaInicial = fechaInicial.AddMonths(x - 1);
                                DateTime CalFechaFinal = new DateTime(CalFechaInicial.Year, CalFechaInicial.Month, DateTime.DaysInMonth(CalFechaInicial.Year, CalFechaInicial.Month));

                                if (CalFechaFinal.Date > fechaFinal.Date)
                                {
                                    CalFechaFinal = fechaFinal;
                                    exit = true;
                                }

                                categoria = new AgendaCategoria();
                                categoria.isBusqueda = false;

                                categoria.Nombre = CalFechaInicial.ToString("MMMM").First().ToString().ToUpper() + String.Join("", CalFechaInicial.ToString("MMMM").Skip(1));
                                if (x == 1) { categoria.FechaInicio = CalFechaInicial; }
                                else { categoria.FechaInicio = CalFechaInicial.AddDays(1); }

                                categoria.FechaFin = CalFechaFinal;
                                categoria.Agenda = data.Where(p => (p.fechaInicio.Date >= CalFechaInicial.Date)
                                                                        && (p.fechaInicio.Date <= CalFechaFinal.Date))
                                                                        .OrderBy(p => p.fechaInicio).Distinct().ToList();

                                listaCategoria.Add(categoria);
                            }
                        }
                    }

                    break;

                case Rp3.AgendaComercial.Models.Constantes.AgendaGroupMode.Cliente:

                    var clientes = data.Select(p => p.ClienteContacto).Distinct().OrderBy(p => p);

                    foreach (var cliente in clientes)
                    {
                        categoria = new AgendaCategoria();
                        categoria.isBusqueda = false;
                        categoria.Nombre = cliente;

                        categoria.Agenda = data.Where(p => p.ClienteContacto == cliente).OrderBy(p => p.fechaInicio).ToList();

                        listaCategoria.Add(categoria);
                    }

                    break;

                case Rp3.AgendaComercial.Models.Constantes.AgendaGroupMode.Agente:

                    var agentes = data.Select(p => p.Agente).Distinct().OrderBy(p => p);

                    foreach (var agente in agentes)
                    {
                        categoria = new AgendaCategoria();
                        categoria.isBusqueda = false;
                        categoria.Nombre = agente;

                        categoria.Agenda = data.Where(p => p.Agente == agente).OrderBy(p => p.fechaInicio).ToList();

                        listaCategoria.Add(categoria);
                    }

                    break;
            }

            //if (fechaInicial < DateTime.Now.Date)
            //{
            //    if (groupMode == Rp3.AgendaComercial.Models.Constantes.AgendaGroupMode.Calendario)
            //        listaCategoria = listaCategoria.OrderByDescending(p => p.FechaInicio).ToList();

            //    foreach (var cat in listaCategoria)
            //        cat.Agenda = cat.Agenda.OrderByDescending(p => p.fechaInicio).ToList();
            //}

            if (String.IsNullOrEmpty(sortField))
            {
                foreach (var cat in listaCategoria)
                    cat.Agenda = cat.Agenda.OrderBy(p => p.fechaInicio).ToList();
            }
            else
            {
                Sort(listaCategoria, sortField, sortMode);
            }

            agendaConsulta.AgendaCategorias = listaCategoria;

            ViewBag.TodosMisAgentes = this.TodosMisAgentes;

            return PartialView("_AgendaPhotoBookDetalle", agendaConsulta);
        }

        [ChildAction(Order = 1)]
        [Rp3.Web.Mvc.Authorize("AGENDA", "QUERY", "AGENDACOMERCIAL", Order = 2)]
        [HttpGet]
        public ActionResult GetPhotoBookSearch(string busqueda, int pagina, int numreg)
        {
            List<AgendaCategoria> listaCategoria = new List<AgendaCategoria>();
            AgendaConsulta agendaConsulta = new AgendaConsulta();
            AgendaCategoria categoria = new AgendaCategoria();

            List<int> listRuta = new List<int>();

            if (!this.TodosMisAgentes)
            {
                if (this.AgenteWork.IdRuta != null)
                {
                    listRuta.Add(this.AgenteWork.IdRuta.Value);
                }
            }
            else
            {
                foreach (var agente in this.AgentesWork)
                {
                    listRuta.Add(agente.IdRuta.Value);
                }
            }

            var data = DataBase.Agendas.GetPhotoBookFullTextSearch(listRuta, busqueda, pagina, numreg, this.TodosMisAgentes);

            categoria = new AgendaCategoria();

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
                categoria.Agenda = data.ToList();
            }

            listaCategoria.Add(categoria);

            agendaConsulta.AgendaCategorias = listaCategoria;

            ViewBag.TodosMisAgentes = this.TodosMisAgentes;

            return PartialView("_AgendaPhotoBookDetalle", agendaConsulta);
        }

        #endregion PHOTOBOOK

        #region MODAL_DATOS

        //Se usa para ser llamado desde otras opciones diferentes a la Agenda
        [ChildAction(Order = 1)]
        [Rp3.Web.Mvc.Authorize("AGENDA", "QUERY", "AGENDACOMERCIAL", Order = 2)]
        [HttpGet]
        public ActionResult GetView(int idRuta, long idAgenda)
        {
            var modelo = PrepararModeloDetalle(idRuta, idAgenda);
            modelo.ReadOnly = true;
            return PartialView("_AgendaDetalleGestion", modelo);
        }

        [ChildAction(Order = 1)]
        [Rp3.Web.Mvc.Authorize("AGENDA", "QUERY", "AGENDACOMERCIAL", Order = 2)]
        [HttpGet]
        public ActionResult GetDatos(int idRuta, long idAgenda)
        {
            var modelo = PrepararModeloDetalle(idRuta, idAgenda);
            modelo.ReadOnly = false;
            return PartialView("_AgendaDetalleGestion", modelo);
        }

        private AgendaConsulta PrepararModeloDetalle(int idRuta, long idAgenda)
        {
            AgendaConsulta agendaConsulta = new AgendaConsulta();
            var data = DataBase.Agendas.GetAgendaDetalle(idRuta, idAgenda);
            agendaConsulta.AgendaClientes = data;

            var agenda = DataBase.Agendas.Get(p => p.IdRuta == idRuta && p.IdAgenda == idAgenda).FirstOrDefault();
            var cliente = data.FirstOrDefault();

            agendaConsulta.Ubicaciones = GetAgendaMap(cliente.Latitud, cliente.Longitud, agenda.Latitud, agenda.Longitud, agenda.EstadoAgenda, cliente.ClienteContacto, cliente.Agente);
            
            var pedido = DataBase.Pedidos.GetSingleOrDefault(p => p.IdRuta == idRuta && p.IdAgenda == idAgenda);
            if(pedido != null)
                agendaConsulta.idPedido = pedido.IdPedido;

            ViewBag.TodosMisAgentes = this.TodosMisAgentes;

            return agendaConsulta;
        }

        #endregion

        #region MODAL_CREATE

        [ChildAction(Order = 1)]
        [Rp3.Web.Mvc.Authorize("AGENDA", "QUERY", "AGENDACOMERCIAL", Order = 2)]
        [HttpGet]
        public ActionResult Crear(int idRuta)
        {
            ViewBag.titulo = "Nueva Visita";

            AgendaEdit agendaEdit = new AgendaEdit();
            agendaEdit.IdRuta = idRuta;

            DateTime fechaActual = GetCurrentDateTime();

            //var programacionDetalle = DataBase.ProgramacionTareaDetalles.Get(p => p.ProgramacionTarea.Estado == Constantes.Estado.Activo &&
            //    fechaActual >= p.ProgramacionTarea.FechaInicio && (p.ProgramacionTarea.EsIndefinida || fechaActual <= p.ProgramacionTarea.FechaFin) &&
            //    p.ProgramacionTarea.ProgramacionTareaRutas.Any(q => q.IdRuta == idRuta), includeProperties: "Tarea");

            //agendaEdit.Tareas = DataBase.Tareas.Get(p=>p.Estado == Constantes.Estado.Activo).ToList();

            agendaEdit.Tareas = DataBase.Tareas.Get(p => p.Estado == Constantes.Estado.Activo &&
               (!p.AplicaRutasEspecificas || p.TareaRutaAplicas.Where(q => q.IdRuta == idRuta).Count() > 0) &&
               p.FechaVigenciaDesde <= fechaActual &&
               (p.EsVigenciaIndefinida || p.FechaVigenciaHasta >= fechaActual)
               ).ToList();

            //programacionDetalle.Where()

            return PartialView("_Create", agendaEdit);
        }

        [ChildAction(Order = 1)]
        [Rp3.Web.Mvc.Authorize("AGENDA", "QUERY", "AGENDACOMERCIAL", Order = 2)]
        [HttpPost]
        public ActionResult Crear(AgendaEdit agendaEdit)
        {
            try
            {
                DateTime fechaActual = GetCurrentDateTime();

                //agendaEdit.FechaFin = agendaEdit.FechaInicio.Date.AddHours(agendaEdit.FechaFin.Hour).AddMinutes(agendaEdit.FechaFin.Minute);

                var duracion = DataBase.GeneralValues.Get(p => p.Id == Rp3.AgendaComercial.Models.Constantes.Duracion.Tabla && p.Code == agendaEdit.DuracionVisita).FirstOrDefault();
                agendaEdit.FechaFin = agendaEdit.FechaInicio.AddMinutes(Convert.ToInt32(duracion.Reference01));

                //CLIENTE/CONTACTO EXISTA
                if (agendaEdit.IdCliente == 0 && agendaEdit.IdContacto == 0)
                {
                    this.AddErrorMessage(Rp3.AgendaComercial.Resources.ErrorMessageValidation.AgendaClienteOContactoRequerido);
                }
                //FECHA VALIDA
                else if (agendaEdit.FechaInicio.Date < fechaActual.AddDays(-1).Date)
                {
                    this.AddErrorMessage(Rp3.AgendaComercial.Resources.ErrorMessageValidation.AgendaFechaPasada);
                }
                //DIRECCION EXISTA
                if (agendaEdit.IdClienteDireccion == 0)
                {
                    this.AddErrorMessage(Rp3.AgendaComercial.Resources.ErrorMessageValidation.AgendaDireccionRequerida);
                }
                //TIEMPO VALIDACION
                else if (agendaEdit.FechaInicio > agendaEdit.FechaFin)
                {
                    this.AddErrorMessage(Rp3.AgendaComercial.Resources.ErrorMessageValidation.HoraInicioMayorHoraFin);
                }

                /*
                 * PROCESO GRABAR :: [SOLO SI NO HAY ERRORES]
                 */
                if (!this.MessageCollection.HasError())
                {
                    Models.Ruta.Agenda agenda = new Models.Ruta.Agenda();
                    agenda.IdRuta = agendaEdit.IdRuta;
                    agenda.EstadoAgenda = Constantes.EstadoAgenda.Pendiente;
                    agenda.EstadoAgendaTabla = Constantes.EstadoAgenda.Tabla;

                    agenda.Origen = Constantes.OrigenAgenda.Web;
                    agenda.OrigenTabla = Constantes.OrigenAgenda.Tabla;

                    agenda.FechaFin = agendaEdit.FechaFin;
                    agenda.FechaInicio = agendaEdit.FechaInicio;
                    agenda.FecIng = GetCurrentDateTime();
                    agenda.IdCliente = agendaEdit.IdCliente;
                    agenda.IdAgente = this.AgenteWork.IdAgente;
                    agenda.FechaInicioOriginal = agendaEdit.FechaInicio;
                    agenda.FechaFinOriginal = agendaEdit.FechaFin;

                    if (agendaEdit.IdContacto != 0)
                        agenda.IdClienteContacto = agendaEdit.IdContacto;

                    agenda.IdClienteDireccion = agendaEdit.IdClienteDireccion;
                    agenda.UsrIng = this.UserLogonName;
                    if (!string.IsNullOrEmpty(agendaEdit.TareasSeleccion))
                        agenda.TareaIds = agendaEdit.TareasSeleccion.Replace(',', '-');

                    agenda.AsignarId();

                    if (agenda.AgendaTareas != null)
                        foreach (var tarea in agenda.AgendaTareas)
                            tarea.IdAgenda = agenda.IdAgenda;

                    //DataBase.Agendas.Insert(agenda);

                    //if (agenda.AgendaTareas != null)
                    //{
                    //    foreach (var tarea in agenda.AgendaTareas)
                    //    {
                    //        DataBase.AgendaTareas.Insert(tarea);
                    //    }
                    //}

                    ////GRABAR
                    //DataBase.Save();

                    DataBase.Agendas.InsertXml(agenda);

                    this.AddDefaultSuccessMessage();
                }
            }
            catch
            {
                this.AddDefaultErrorMessage();
            }

            return Json();
        }

        [ChildAction]
        [Rp3.Web.Mvc.Authorize("AGENDA", "QUERY", "AGENDACOMERCIAL", Order = 2)]
        public ActionResult GetClienteDirecciones(int idCliente, int idContacto)
        {
            List<Rp3.AgendaComercial.Models.General.ClienteDireccion> data = null;
            if (idContacto == 0)
                data = DataBase.ClienteDirecciones.Get(p => p.IdCliente == idCliente).ToList();
            else
            {
                var contacto = DataBase.ClienteContactos.Get(p => p.IdCliente == idCliente && p.IdClienteContacto == idContacto, includeProperties: "ClienteDireccion").FirstOrDefault();
                if (contacto != null)
                    data = new List<Rp3.AgendaComercial.Models.General.ClienteDireccion>() { contacto.ClienteDireccion };
            }

            return new JsonResult() { Data = data.Select(p => new { id = p.IdClienteDireccion, text = (p.IdClienteDireccion + "-" + p.DescriptionName), principal = p.EsPrincipal }), JsonRequestBehavior = System.Web.Mvc.JsonRequestBehavior.AllowGet };
        }
        #endregion

        #region MODAL_UPDATE

        [ChildAction(Order = 1)]
        [Rp3.Web.Mvc.Authorize("AGENDA", "QUERY", "AGENDACOMERCIAL", Order = 2)]
        [HttpGet]
        public ActionResult Update(int idRuta, int idAgenda)
        {
            ViewBag.titulo = "Actualizar Visita";

            Models.Ruta.Agenda agenda = DataBase.Agendas.Get(p => p.IdAgenda == idAgenda && p.IdRuta == idRuta).SingleOrDefault();

            AgendaUpdates agendaEdit = new AgendaUpdates();

            DateTime dt_inicio = agenda.FechaInicio.Value;
            DateTime dt_fin = agenda.FechaFin.Value;

            agendaEdit.IdRuta = agenda.IdRuta;
            agendaEdit.IdAgenda = agenda.IdAgenda;
            agendaEdit.Cliente_Nombre1 = agenda.ClienteDireccion.Cliente.Nombre1;
            agendaEdit.Cliente_Nombre2 = agenda.ClienteDireccion.Cliente.Nombre2;
            agendaEdit.Cliente_Apellido1 = agenda.ClienteDireccion.Cliente.Apellido1;
            agendaEdit.Cliente_Apellido2 = agenda.ClienteDireccion.Cliente.Apellido2;

            agendaEdit.FechaInicio = dt_inicio;
            agendaEdit.FechaFin = dt_fin;

            string difMin = (agendaEdit.FechaFin - agendaEdit.FechaInicio).TotalMinutes.ToString();

            var duracion = DataBase.GeneralValues.Get(p => p.Id == Rp3.AgendaComercial.Models.Constantes.Duracion.Tabla && p.Reference01 == difMin).FirstOrDefault();

            if (duracion != null)
                agendaEdit.DuracionVisita = duracion.Code;

            string dd = agenda.FechaInicio.Value.ToShortDateString();

            agendaEdit.IdCliente = agenda.IdCliente;
            agendaEdit.IdContacto = agenda.IdClienteContacto ?? 0;

            if (agenda.ClienteContacto != null)
            {
                agendaEdit.Contacto_Nombre = agenda.ClienteContacto.Nombre;
                agendaEdit.Contacto_Apellido = agenda.ClienteContacto.Apellido;
            }
            else
            {
                agendaEdit.Contacto_Nombre = "";
                agendaEdit.Contacto_Apellido = "";

            }
            agendaEdit.Direccion = agenda.ClienteDireccion.Direccion;
            //agendaEdit.FechaInicio = agenda.FechaInicio.Value;

            agendaEdit.Motivo = agenda.MotivoReprogramacion;
            agendaEdit.EsReprogramada = agenda.EsReprogramada;

            DateTime fechaActual = GetCurrentDateTime();




            //var programacionDetalle = DataBase.ProgramacionTareaDetalles.Get(
            //    p =>
            //        p.ProgramacionTarea.Estado == Constantes.Estado.Activo
            //        && fechaActual >= p.ProgramacionTarea.FechaInicio
            //        && (p.ProgramacionTarea.EsIndefinida || fechaActual <= p.ProgramacionTarea.FechaFin)
            //        && p.ProgramacionTarea.ProgramacionTareaRutas.Any(q => q.IdRuta == idRuta), includeProperties: "Tarea");

            agendaEdit.Tareas = DataBase.Tareas.Get(p => p.Estado == Constantes.Estado.Activo &&
                (!p.AplicaRutasEspecificas || p.TareaRutaAplicas.Where(q => q.IdRuta == idRuta).Count() > 0) &&
                p.FechaVigenciaDesde <= fechaActual &&
                (p.EsVigenciaIndefinida || p.FechaVigenciaHasta >= fechaActual)
                ).ToList();

            ViewBag.MotivoReprogrmacionSelectList = DataBase.GeneralValues.Get(p => p.Id == Constantes.MotivosReprogramacion.Tabla).ToSelectList(includeNullItem: false);

            var idstarea = agenda.AgendaTareas.Select(p => p.IdTarea);

            var elementos = agendaEdit.Tareas.Where(p => idstarea.Contains(p.IdTarea));
            //Array ele = elementos;

            List<Models.General.Tarea> ele = elementos.ToList();

            //
            string tareasID = "";
            for (int x = 0; x < ele.Count; x++)
            {
                if (x == 0)
                {
                    tareasID = ele[x].IdTarea.ToString();
                }
                else { tareasID += "," + ele[x].IdTarea.ToString(); }
            }

            agendaEdit.TareasSeleccion = tareasID;

            return PartialView("_Update", agendaEdit);
        }

        [ChildAction(Order = 1)]
        [Rp3.Web.Mvc.Authorize("AGENDA", "QUERY", "AGENDACOMERCIAL", Order = 2)]
        [HttpPost]
        public ActionResult Update(AgendaEdit agendaEdit)
        {
            try
            {
                DateTime fechaActual = GetCurrentDateTime();

                //agendaEdit.FechaFin = agendaEdit.FechaInicio.Date.AddHours(agendaEdit.FechaFin.Hour).AddMinutes(agendaEdit.FechaFin.Minute);

                var duracion = DataBase.GeneralValues.Get(p => p.Id == Rp3.AgendaComercial.Models.Constantes.Duracion.Tabla && p.Code == agendaEdit.DuracionVisita).FirstOrDefault();
                agendaEdit.FechaFin = agendaEdit.FechaInicio.AddMinutes(Convert.ToInt32(duracion.Reference01));

                //CLIENTE/CONTACTO EXISTA
                if (agendaEdit.IdCliente == 0 && agendaEdit.IdContacto == 0)
                {
                    this.AddErrorMessage(Rp3.AgendaComercial.Resources.ErrorMessageValidation.AgendaClienteOContactoRequerido);
                }
                //FECHA VALIDA
                else if (agendaEdit.FechaInicio.Date < fechaActual.AddDays(-1).Date)
                {
                    this.AddErrorMessage(Rp3.AgendaComercial.Resources.ErrorMessageValidation.AgendaFechaPasada);
                }
                //DIRECCION EXISTA
                /*if (agendaEdit.IdClienteDireccion == 0)
                {
                    this.AddErrorMessage(Rp3.AgendaComercial.Resources.ErrorMessageValidation.AgendaDireccionRequerida);
                }*/
                //TIEMPO VALIDACION
                else if (agendaEdit.FechaInicio > agendaEdit.FechaFin)
                {
                    this.AddErrorMessage(Rp3.AgendaComercial.Resources.ErrorMessageValidation.HoraInicioMayorHoraFin);
                }

                Models.Ruta.Agenda agenda = DataBase.Agendas.Get(p => p.IdRuta == agendaEdit.IdRuta && p.IdAgenda == agendaEdit.IdAgenda).SingleOrDefault();
                if (agenda.EstadoAgenda != Constantes.EstadoAgenda.Pendiente && agenda.EstadoAgenda != Constantes.EstadoAgenda.Reprogramada)
                    this.AddErrorMessage(string.Format(Rp3.AgendaComercial.Resources.ErrorMessageValidation.AgendaModificarEstadoNoValido, agenda.EstadoAgendaGeneralValue.Content));

                if (agendaEdit.FechaInicio != agenda.FechaInicio && String.IsNullOrEmpty(agendaEdit.Motivo))
                {
                    this.AddErrorMessage(Rp3.AgendaComercial.Resources.ErrorMessageValidation.AgendaMotivoReprogramacion);
                }

                /*
                 * PROCESO ACTUALIZAR :: [SOLO SI NO HAY ERRORES]
                 */
                if (!this.MessageCollection.HasError())
                {
                    if (agendaEdit.FechaInicio != agenda.FechaInicio)
                    {
                        agenda.EstadoAgenda = Constantes.EstadoAgenda.Reprogramada;
                        agenda.EsReprogramada = true;
                    }

                    if (agenda.EsReprogramada)
                    {
                        agenda.MotivoReprogramacionTabla = Constantes.MotivosReprogramacion.Tabla;
                        agenda.MotivoReprogramacion = agendaEdit.Motivo;
                    }

                    agenda.EstadoAgendaTabla = Constantes.EstadoAgenda.Tabla;
                    agenda.FechaFin = agendaEdit.FechaFin;
                    agenda.FechaInicio = agendaEdit.FechaInicio;
                    agenda.FecMod = fechaActual;

                    if (agendaEdit.TareasSeleccion != null)
                    {
                        agenda.AgendaTareas.Clear();
                        agenda.TareaIds = agendaEdit.TareasSeleccion.Replace(',', '-');
                    }

                    //ACTUALIZAR
                    DataBase.Agendas.UpdateXml(agenda);

                    this.AddDefaultSuccessMessage();
                }
            }
            catch
            {
                this.AddDefaultErrorMessage();
            }

            return Json();
        }

        #endregion

        #region MODAL_DELETE

        [ChildAction(Order = 1)]
        [Rp3.Web.Mvc.Authorize("AGENDA", "QUERY", "AGENDACOMERCIAL", Order = 2)]
        [HttpPost]
        public ActionResult Delete(int idRuta, long idAgenda)
        {
            try
            {
                //Models.Ruta.Agenda modelDelete = GetModel(idRuta,idAgenda);
                Models.Ruta.Agenda modelDelete = DataBase.Agendas.Get(p => p.IdRuta == idRuta && p.IdAgenda == idAgenda).SingleOrDefault();
                if (modelDelete.EstadoAgenda == "P" || modelDelete.EstadoAgenda == "R")
                {
                    modelDelete.EstadoAgenda = Rp3.AgendaComercial.Models.Constantes.EstadoAgenda.Eliminado;
                    modelDelete.UsrMod = this.UserLogonName;
                    modelDelete.FecMod = this.GetCurrentDateTime();
                    DataBase.Agendas.Update(modelDelete);
                    DataBase.Save();

                    /*Models.Ruta.Agenda agenda = DataBase.Agendas.Get(p => p.IdRuta == idRuta && p.IdAgenda == idAgenda).SingleOrDefault();
                    DataBase.Agendas.Delete(agenda);
                    DataBase.Save();*/
                    this.AddDefaultSuccessMessage();
                }
                else
                {
                    this.AddDefaultErrorMessage();
                }
            }
            catch
            {
                this.AddDefaultErrorMessage();
            }

            return Json();
        }
        private Models.Ruta.Agenda GetModel(int idRuta, long idAgenda)
        {
            Models.Ruta.Agenda result = DataBase.Agendas.Get(p => p.IdRuta == idRuta && p.IdAgenda == idAgenda).SingleOrDefault();// , includeProperties: "RutaLotes, RutaDetalles, RutaIncluirs, RutaExcluirs").SingleOrDefault();

            return result;
        }
        /*[ChildAction(Order = 1)]
        [Rp3.Web.Mvc.Authorize("AGENDA", "QUERY", "AGENDACOMERCIAL", Order = 2)]
        [HttpGet]
        public ActionResult EliminarDatos (int idRuta, long idAgenda)
        {
            try
            {
                
                Models.Ruta.Agenda agenda = DataBase.Agendas.Get(p => p.IdRuta == idRuta && p.IdAgenda == idAgenda).SingleOrDefault();
                DataBase.Agendas.Delete(agenda);
                DataBase.Save();

                this.AddDefaultSuccessMessage();
            }
            catch
            {
                this.AddDefaultErrorMessage();
            }

            return Json();
        }*/

        #endregion

        #region MODAL_TAREA

        [ChildAction(Order = 1)]
        [Rp3.Web.Mvc.Authorize("AGENDA", "QUERY", "AGENDACOMERCIAL", Order = 2)]
        [HttpGet]
        public ActionResult GetTareas(int idRuta, int idAgenda, long idTarea)
        {
            Models.Ruta.View.AgendaTarea agendaConsulta = new Models.Ruta.View.AgendaTarea();
            agendaConsulta.IdTarea = idTarea;
            agendaConsulta.Nombre = DataBase.Tareas.GetByID(idTarea).Descripcion;


            var data = DataBase.Agendas.GetAgendaTareas(idRuta, idAgenda, idTarea);

            agendaConsulta.Actividades = data;

            return PartialView("_VerTareas", agendaConsulta);
        }

        #endregion
    }
}
