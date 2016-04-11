using Rp3.AgendaComercial.Models.General.View;
using Rp3.AgendaComercial.Web.Areas.Ruta.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Rp3.Web.Mvc;
using Rp3.Web.Mvc.Utility;
using Rp3.AgendaComercial.Models.General;
using Rp3.AgendaComercial.Models.Marcacion.View;
using Rp3.AgendaComercial.Models.Ruta;
using Rp3.Web.Mvc.Html;

namespace Rp3.AgendaComercial.Web
{
    public class BinaryResult : ActionResult
    {
        public byte[] Data { get; set; }
        public bool IsAttachment { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public override void ExecuteResult(ControllerContext context)
        {
            if (this.Data != null)
            {
                context.HttpContext.Response.Clear();
                context.HttpContext.Response.ContentType = ContentType;
                if (!string.IsNullOrEmpty(FileName))
                {
                    context.HttpContext.Response.AddHeader("content-disposition",
                        ((IsAttachment) ? "attachment;filename=" : "inline;filename=") +
                        FileName);
                }
                context.HttpContext.Response.BinaryWrite(Data);
            }
        }
    }
}

namespace Rp3.AgendaComercial.Web.Ruta.Controllers
{
    public class InformeParadaController : AgendaComercial.Web.Controllers.AgendaComercialController
    {
        // GET: Ruta/InformeParada
        [Rp3.Web.Mvc.Authorize("INFORMEPARADA", "QUERY", "AGENDACOMERCIAL")]
        public ActionResult Index(int? IdAgente, long? FechaTicks)
        {
            InformeParadaView model;
            if(IdAgente == null)
            {
                if (!this.TodosMisAgentes)
                {
                    if (this.Agente.CargoRol == AgenteCargoRol.Agente && this.Agente.EsAgente)
                    {
                        this.UserWorkId = this.UserId;
                    }
                    else if ((this.Agente.CargoRol == AgenteCargoRol.Supervisor || this.Agente.CargoRol == AgenteCargoRol.Gerente) && !this.AgenteWork.EsAgente)
                    {
                        this.TodosMisAgentes = true;
                        var agentes = DataBase.Agentes.GetAgentesPermitidos(this.Agente.IdAgente);
                        this.UserWorkIds = agentes.Where(p => p.IdUsuario != null).Select(p => p.IdUsuario ?? 0).ToList<int>();
                    }
                }

                model = new InformeParadaView() { Fecha = this.GetCurrentDateTime() };

                ViewBag.Agentes = DataBase.Agentes.GetAgentesPermitidos(this.Agente.IdAgente).ToSelectList(includeNullItem: true);
            }
            else
            {

                model = new InformeParadaView() { Fecha = new DateTime(FechaTicks.Value).Date, FechaByAgente = new DateTime(FechaTicks.Value).Date.ToString("dd/MM/yyyy"), IdAgente = IdAgente.Value };

                var agentes = DataBase.Agentes.Get(p => p.IdAgente == IdAgente.Value);
                this.UserWorkIds = agentes.Where(p => p.IdUsuario != null).Select(p => p.IdUsuario ?? 0).ToList<int>();

                ViewBag.Agentes = agentes.ToSelectList(includeNullItem: false);
            }

            InicializarTab();
            return View(model);
        }

        private void InicializarTab()
        {
            TabCollection tabCollection = new TabCollection();

            tabCollection.Add("tabtabla", TabTarget.HtmlElement, "#tabtabla", "Resultados", true);
            tabCollection.Add("tabtimeline", TabTarget.HtmlElement, "#tabtimeline", "Timeline", false);

            ViewBag.TabCollection = tabCollection;
        }

        public ActionResult Preview(int? IdAgente, long FechaInicioTicks, long FechaFinTicks)
        {
            PartialViewResult data = GetData(IdAgente, FechaInicioTicks, FechaFinTicks) as PartialViewResult;

            Rp3.AgendaComercial.Web.Areas.Ruta.Reports.Trazabilidad report = new Areas.Ruta.Reports.Trazabilidad(data.ViewBag, data.Model);
            System.IO.MemoryStream reportStream = new System.IO.MemoryStream();
            report.ExportToPdf(reportStream);
            reportStream.Position = 0;
            //return Json(new { buffer = Convert.ToBase64String(reportStream.ToArray()) } , JsonRequestBehavior.AllowGet);
            return new Rp3.AgendaComercial.Web.BinaryResult { Data = reportStream.ToArray(), ContentType = "application/pdf", FileName = string.Format("InformeTrazabilidad.{0:yyyy.MM.dd}.{1:HH.mm.ss}.pdf", DateTime.Now, DateTime.Now) };

        }

        public ActionResult PreviewReporteGestion(int IdAgente, long FechaInicioTicks, long FechaFinTicks, Models.Consulta.View.AgenteReporteGestionModo Modo, bool MostrarFotos)
        {
            Models.Consulta.View.AgenteReporteGestion data = new Models.Consulta.View.AgenteReporteGestion();
            DateTime fechaInicio = new DateTime(FechaInicioTicks);
            DateTime fechaFin = new DateTime(FechaFinTicks).AddDays(1).AddSeconds(-1);
            var efectividad = DataBase.Reporte.GetDashboardResumenGestionAgente(fechaInicio, fechaFin, IdAgente, modoAgente: true);
            foreach(var item in efectividad)
            {
                data.Efectividad = item.Efectividad;
                data.Gestionados.Add(new Models.Consulta.View.AgenteReporteGestionItem<int?> { Description = "Visitados", Value = item.Gestionados });
                data.NoGestionados.Add(new Models.Consulta.View.AgenteReporteGestionItem<int?> { Description = "No Visitados", Value = item.NoGestionados });
                data.Proximos.Add(new Models.Consulta.View.AgenteReporteGestionItem<int?> { Description = "Próximos", Value = item.Proximos });
            }

            var agenteActual = DataBase.Agentes.Get(p => p.IdAgente == IdAgente).FirstOrDefault();

            int index;

            data.Clientes = DataBase.Agendas.GetAgendaListadoReporteGestion(new List<int>(new int[] { agenteActual.IdRuta.Value }), fechaInicio, fechaFin);
            data.Clientes.ForEach(p => p.Config());
            data.ClientesCreados = DataBase.Agendas.GetClientesCreadosReporteGestion(fechaInicio, fechaFin, IdAgente);
            data.Marcaciones = DataBase.Reporte.GetReporteMarcacionReporteGestion(IdAgente, fechaInicio, fechaFin).OrderBy(p => p.Fecha).ToList();
            data.PermisosJustificaciones = DataBase.Permisos.GetPermisosJustificaciones(IdAgente, fechaInicio, fechaFin);
            data.Oportunidades = DataBase.Oportunidades.GetOportunidadListadoReporteGestion(IdAgente, fechaInicio, fechaFin);

            List<InformeTrazabilidad> traza = ((PartialViewResult)GetData(IdAgente, FechaInicioTicks, FechaFinTicks)).Model as List<InformeTrazabilidad>;

            var trazaParada = traza.Where(p => !p.EsMovimiento).GroupBy(p => p.Fecha.Date).Select(s => new { Fecha = s.Key, Minutos = s.Sum(c => c.Minutos) } );
            var trazaRecorrido = traza.Where(p => p.EsMovimiento).GroupBy(p => p.Fecha.Date).Select(s => new { Fecha = s.Key, Minutos = s.Sum(c => c.Minutos) });

            foreach (var fecha in data.ClientesNoVisitados.GroupBy(p => p.fechaInicio.Date).Select(p => p.Key).OrderBy(p => p))
                data.GruposNoVisitados.Add(new Models.Consulta.View.AgenteReporteGestionGrupo(data, fecha, Models.Consulta.View.AgenteReporteGestionTipo.NoVisitado));
            foreach (var fecha in data.ClientesVisitados.GroupBy(p => p.fechaInicio.Date).Select(p => p.Key).OrderBy(p => p))
                data.GruposVisitados.Add(new Models.Consulta.View.AgenteReporteGestionGrupo(data, fecha, Models.Consulta.View.AgenteReporteGestionTipo.Visitado));
            foreach (var fecha in data.ClientesReprogramados.GroupBy(p => p.fechaInicio.Date).Select(p => p.Key).OrderBy(p => p))
                data.GruposReprogramados.Add(new Models.Consulta.View.AgenteReporteGestionGrupo(data, fecha, Models.Consulta.View.AgenteReporteGestionTipo.Reprogramado));
            foreach (var fecha in data.ClientesPendientes.GroupBy(p => p.fechaInicio.Date).Select(p => p.Key).OrderBy(p => p))
                data.GruposPendientes.Add(new Models.Consulta.View.AgenteReporteGestionGrupo(data, fecha, Models.Consulta.View.AgenteReporteGestionTipo.Pendiente));

            foreach (var fecha in data.ClientesCreados.GroupBy(p => p.FecIng.Date).Select(p => p.Key).OrderBy(p => p))
                data.GruposClientesCreados.Add(new Models.Consulta.View.AgenteReporteGestionClienteGreado(data, fecha));

            foreach(var fecha in data.Marcaciones.GroupBy(p => p.Fecha.Date).Select(p => p.Key).OrderBy(p => p))
            {
                data.GruposResumen.Add(new Models.Consulta.View.AgenteReporteGestionResumen
                {
                    Fecha = fecha,
                    NoVisitados = data.ClientesNoVisitados.Where(p => p.fechaInicio.Date == fecha).Count(),
                    Visitados = data.ClientesVisitados.Where(p => p.fechaInicio.Date == fecha).Count(),
                    Reprogramados = data.ClientesReprogramados.Where(p => p.fechaInicio.Date == fecha).Count(),
                    Pendientes = data.ClientesPendientes.Where(p => p.fechaInicio.Date == fecha).Count(),
                    Parada = trazaParada.Where(p => p.Fecha == fecha).Sum(p => p.Minutos),
                    Recorrido = trazaRecorrido.Where(p => p.Fecha == fecha).Sum(p => p.Minutos)
                });
            }

            DataBase.ParametroHelper.Load();
            data.Marcaciones.ForEach(p => p.ConfigEficiencia(DataBase.ParametroHelper.MaxMarcacionEficiencia, DataBase.ParametroHelper.MinMarcacionEficiencia));

            foreach (var fecha in data.GruposNoVisitados)
            {
                index = 1;
                fecha.Clientes.ForEach(p => p.Secuencia = index++);
            }
            foreach(var fecha in data.GruposVisitados)
            {
                index = 1;
                fecha.Clientes.ForEach(p => p.Secuencia = index++);
            }
            foreach (var fecha in data.GruposReprogramados)
            {
                index = 1;
                fecha.Clientes.ForEach(p => p.Secuencia = index++);
            }
            foreach (var fecha in data.GruposPendientes)
            {
                index = 1;
                fecha.Clientes.ForEach(p => p.Secuencia = index++);
            }
            foreach (var fecha in data.GruposClientesCreados)
            {
                index = 1;
                fecha.Clientes.ForEach(p => p.Secuencia = index++);
            }
            index = 1;
            data.PermisosJustificaciones.ForEach(p => p.Secuencia = index++);
            index = 1;
            data.Oportunidades.ForEach(p => p.Secuencia = index++);

            ViewBag.Agente = agenteActual.Descripcion;
            ViewBag.RangoFechas = string.Format("Desde el {0:g} Hasta el {1:g}", fechaInicio, fechaFin);
            
            Rp3.AgendaComercial.Web.Areas.Ruta.Reports.AgenteGestion report = new Areas.Ruta.Reports.AgenteGestion(this.ViewBag, data, Modo, MostrarFotos);
            System.IO.MemoryStream reportStream = new System.IO.MemoryStream();
            report.ExportToPdf(reportStream);
            reportStream.Position = 0;
            //return Json(new { buffer = Convert.ToBase64String(reportStream.ToArray()) } , JsonRequestBehavior.AllowGet);
            return new Rp3.AgendaComercial.Web.BinaryResult { Data = reportStream.ToArray(), ContentType = "application/pdf", FileName = string.Format("ReporteGestion.{0:yyyy.MM.dd}.{1:HH.mm.ss}.pdf", DateTime.Now, DateTime.Now) };

        }


        [Rp3.Web.Mvc.ChildAction()]
        [Rp3.Web.Mvc.Authorize("INFORMEPARADA", "QUERY", "AGENDACOMERCIAL")]
        public ActionResult GetData(int? IdAgente, long FechaInicioTicks, long FechaFinTicks)
        {
            DateTime fechaFin = new DateTime(FechaFinTicks).Date.AddDays(1).Date.AddSeconds(-1);
            DateTime fechaInicio = new DateTime(FechaInicioTicks).Date;

            double minDetenido = 0;
            double minMovimiento = 0;
            double minTotal = 0;

            var model = new List<InformeTrazabilidad>();

            if (fechaInicio.Date == this.GetCurrentDateTime().Date || fechaFin.Date == this.GetCurrentDateTime().Date)
            {
                model.AddRange(Rp3.AgendaComercial.Process.Executor.GenerateInformeTrazabilidad(this.DataBase, IdAgente: IdAgente, FechaInicio: this.GetCurrentDateTime().Date, FechaFin: this.GetCurrentDateTime().Date.AddDays(1).Date.AddSeconds(-1), totalizar: IdAgente == null));
            }

            if (fechaInicio.Date != this.GetCurrentDateTime().Date || fechaFin.Date != this.GetCurrentDateTime().Date)
            {
                var list = DataBase.InformeTrazabilidads.Get(p => (IdAgente == null || p.IdAgente == IdAgente)
                    && p.Fecha >= fechaInicio && p.Fecha <= fechaFin).ToList();

                model.AddRange(list);
            }

            minDetenido = model.Where(p => !p.EsMovimiento).Sum(p => p.Minutos);
            minMovimiento = model.Where(p => p.EsMovimiento).Sum(p => p.Minutos);
            minTotal = model.Sum(p => p.Minutos);

            ViewBag.TiempoDetenido = FormatMinutes(minDetenido);
            ViewBag.TiempoRecorrido = FormatMinutes(minMovimiento);
            ViewBag.MinutosDetenido = Convert.ToInt32(minDetenido);
            ViewBag.MinutosRecorrido = Convert.ToInt32(minMovimiento);
            ViewBag.Kms = model.Sum(p => p.Distancia).ToString("n2");
            ViewBag.RangoFechas = string.Format("Desde el {0:g} Hasta el {1:g}", fechaInicio, fechaFin);

            if (IdAgente != null)
            {
                var agenteActual = DataBase.Agentes.Get(p => p.IdAgente == IdAgente).FirstOrDefault();
                ViewBag.Agente = agenteActual.Descripcion;
            }
            else
            {
                model.Clear();
                ViewBag.Agente = Rp3.AgendaComercial.Resources.LabelFor.Todos;
            }

            return PartialView("_Agentes", model);
        }

        [Rp3.Web.Mvc.ChildAction()]
        [Rp3.Web.Mvc.Authorize("INFORMEPARADA", "QUERY", "AGENDACOMERCIAL")]
        public ActionResult GetChart(int? Detenido, int? EnMovimiento)
        {
            var model = new List<AnalisisAgenteChart>();

            model.Add(new AnalisisAgenteChart() { Descripcion = Rp3.AgendaComercial.Resources.LabelFor.Detenido, Valor = Detenido ?? 0, Color = "#6FBF56" });
            model.Add(new AnalisisAgenteChart() { Descripcion = Rp3.AgendaComercial.Resources.LabelFor.Movimiento, Valor = EnMovimiento ?? 0, Color = "#BE2026" });

            ViewBag.Titulo = Rp3.AgendaComercial.Resources.LabelFor.Tiempo;

            return PartialView("_Chart", model);
        }

        [Rp3.Web.Mvc.ChildAction()]
        [Rp3.Web.Mvc.Authorize("INFORMEPARADA", "QUERY", "AGENDACOMERCIAL")]
        public ActionResult GetMap(string Ubicacion, int? IdAgente)
        {
            ViewBag.MapRoute = true;
            ViewBag.MapStart = true;
            ViewBag.SuppressRouteMarkers = true;
            ViewBag.IdAgente = IdAgente;

            List<Ubicacion> model = new List<Ubicacion>();
            string[] ubicaciones = Ubicacion.Split('~');

            int index = 1;

            foreach (string item in ubicaciones)
            {
                string[] posicion = item.Split('|');

                model.Add(new Ubicacion()
                {
                    MarkerIndex = index,
                    Titulo = posicion[0],
                    Latitud = Convert.ToDouble(posicion[1]),
                    Longitud = Convert.ToDouble(posicion[2])
                });

                index++;
            }

            return PartialView("_UbicacionMapMarker", model);
        }

        private string FormatMinutes(double minutos)
        {
            DateTime fecha = new DateTime().AddMinutes(minutos);
            if (fecha.Hour > 0)
                return string.Format("{0:00} H. {1:00} Min.", fecha.Hour, fecha.Minute);
            else
                return string.Format("{0:00} Min.", fecha.Minute);

            string tiempo = String.Empty;

            if (minutos > 60)
            {
                double horasb = minutos / 60;
                double minutosb = minutos % 60;

                tiempo = String.Format("{0:n0} H.", horasb);
                tiempo += String.Format("{0:n0} Min.", minutosb);
            }
            else
                tiempo = String.Format("{0:n0} Min.", minutos);

            return tiempo;
        }
    }
}