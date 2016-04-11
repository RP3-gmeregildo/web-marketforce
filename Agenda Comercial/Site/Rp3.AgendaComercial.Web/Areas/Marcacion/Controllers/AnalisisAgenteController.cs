using Rp3.AgendaComercial.Models.Marcacion.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Rp3.AgendaComercial.Web.Marcacion.Controllers
{
    public class AnalisisAgenteController : Rp3.AgendaComercial.Web.Controllers.AgendaComercialController
    {
        // GET: Marcacion/AnalisisAgente
        public ActionResult Index()
        {
            return View();
        }

        [Rp3.Web.Mvc.ChildAction()]
        public ActionResult GetDetalle(int? IdAgente, long FechaInicioTicks, long FechaFinTicks)
        {
            DateTime fechaInicio = new DateTime(FechaInicioTicks).Date;
            DateTime fechaFin = new DateTime(FechaFinTicks).Date.AddHours(23).AddMinutes(59).AddSeconds(59);

            List<ReporteMarcacion> model;

            if (IdAgente == null)
                model = DataBase.Reporte.GetReporteMarcacionRango(this.Agente.IdAgente, fechaInicio, fechaFin, true);
            else
                model = DataBase.Reporte.GetReporteMarcacionRango(IdAgente ?? 0, fechaInicio, fechaFin, true);

            DataBase.ParametroHelper.Load();

            foreach (var item in model)
            {
                if (item.MinutosAtrasoNoJustificado > DataBase.ParametroHelper.MinutoAtrasoDia || item.Ausente)
                {
                    item.Eficiencia = 0;
                }
                else
                {
                    item.Eficiencia = (1 - (item.MinutosAtrasoNoJustificado / DataBase.ParametroHelper.MinutoAtrasoDia)) * 100;
                }

                if (item.Eficiencia >= DataBase.ParametroHelper.MaxMarcacionEficiencia)
                    item.KpiEficiencia = 1;
                else if (item.Eficiencia >= DataBase.ParametroHelper.MinMarcacionEficiencia)
                    item.KpiEficiencia = 0;
                else
                    item.KpiEficiencia = -1;
            }

            var promedioAtraso = model.Where(p => p.AplicaMarcacion).Average(p => p.MinutosAtrasoNoJustificado);

            ViewBag.Eficiencia = 0;
            ViewBag.EficienciaKpiColor = "";

            if (promedioAtraso > DataBase.ParametroHelper.MinutoAtrasoDia)
            {
                ViewBag.Eficiencia = 0;
            }
            else
            {
                ViewBag.Eficiencia = (1 - (promedioAtraso / DataBase.ParametroHelper.MinutoAtrasoDia)) * 100;
            }

            if (ViewBag.Eficiencia >= DataBase.ParametroHelper.MaxMarcacionEficiencia)
                ViewBag.EficienciaKpiColor = "#6FBF56";
            else if (ViewBag.Eficiencia >= DataBase.ParametroHelper.MinMarcacionEficiencia)
                ViewBag.EficienciaKpiColor = "yellow";
            else
                ViewBag.EficienciaKpiColor = "#BE2026";

            return PartialView("_DetalleAgentes", model);
        }

        [Rp3.Web.Mvc.ChildAction()]
        public ActionResult GetData(long FechaInicioTicks, long FechaFinTicks)
        {
            DateTime fechaInicio = new DateTime(FechaInicioTicks).Date;
            DateTime fechaFin = new DateTime(FechaFinTicks).Date.AddHours(23).AddMinutes(59).AddSeconds(59);

            var model = DataBase.Reporte.GetAnalisisAgente(this.Agente.IdAgente, fechaInicio, fechaFin);

            DataBase.ParametroHelper.Load();

            foreach (var item in model)
            {
                //if (item.MinutosAtrasoNoJustificado > (DataBase.ParametroHelper.MinutoAtrasoDia * item.DiasLaborales) || item.Ausencias == item.DiasLaborales)
                //{
                //    item.Eficiencia = 0;
                //}
                //else
                //{
                //    item.Eficiencia = (1 - (item.MinutosAtrasoNoJustificado / (DataBase.ParametroHelper.MinutoAtrasoDia * item.DiasLaborales))) * 100;
                //}

                item.Eficiencia = Models.Marcacion.Marcacion.Efectividad(item.MinutosAtrasoNoJustificado);

                if (item.Eficiencia >= DataBase.ParametroHelper.MaxMarcacionEficiencia)
                    item.KpiEficiencia = 1;
                else if (item.Eficiencia >= DataBase.ParametroHelper.MinMarcacionEficiencia)
                    item.KpiEficiencia = 0;
                else
                    item.KpiEficiencia = -1;
            }

            var promedioAtraso = model.Where(p => p.AplicaMarcacion).Average(p => p.MinutosAtrasoNoJustificado);
            var promedioDiasLaborales = model.Where(p => p.AplicaMarcacion).Average(p => p.DiasLaborales);

            ViewBag.Eficiencia = 0;
            ViewBag.EficienciaKpiColor = "";

            ViewBag.Eficiencia = Models.Marcacion.Marcacion.Efectividad(promedioAtraso);

            //if (promedioAtraso > (DataBase.ParametroHelper.MinutoAtrasoDia * promedioDiasLaborales))
            //{
            //    ViewBag.Eficiencia = 0;
            //}
            //else
            //{
            //    ViewBag.Eficiencia = (1 - (promedioAtraso / (DataBase.ParametroHelper.MinutoAtrasoDia * promedioDiasLaborales))) * 100;
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
        public ActionResult GetChart(int? IdAgente, string Agente, int? Asistencias, int? Atrasos, int? Ausencias)
        {
            var model = new List<AnalisisAgenteChart>();

            model.Add(new AnalisisAgenteChart() { Descripcion = Rp3.AgendaComercial.Resources.LabelFor.Asistencias, Valor = Asistencias ?? 0, Color = "#6FBF56" });
            model.Add(new AnalisisAgenteChart() { Descripcion = Rp3.AgendaComercial.Resources.LabelFor.Atrasos, Valor = Atrasos ?? 0, Color = "yellow" });
            model.Add(new AnalisisAgenteChart() { Descripcion = Rp3.AgendaComercial.Resources.LabelFor.Ausencias, Valor = Ausencias ?? 0, Color = "#BE2026" });

            ViewBag.Titulo = String.Empty;

            if (IdAgente != null)
                ViewBag.Titulo = Agente;
            else
                ViewBag.Titulo = Rp3.AgendaComercial.Resources.LabelFor.Todos;

            return PartialView("_Chart", model);
        }
    }
}