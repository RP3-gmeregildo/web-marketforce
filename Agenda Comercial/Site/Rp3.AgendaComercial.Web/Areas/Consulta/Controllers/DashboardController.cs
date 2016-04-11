using Rp3.AgendaComercial.Web.Areas.Consulta.Models;
using Rp3.Web.Mvc;
using Rp3.Web.Mvc.Html;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Rp3.AgendaComercial.Web.Areas.Consulta.Controllers
{
    public class DashboardController : Rp3.AgendaComercial.Web.Controllers.AgendaComercialController
    {
        // GET: Consulta/Dashboard
        public ActionResult Index()
        {
            string token = Rp3.Web.Http.AutoLogin.GenerateToken(this.ApplicationId, Url.AbsoluteUrl(), "smartview", "smartview-app-marketforce", "rp3-smartview");
            return Redirect(String.Format("{0}/Security/Account/AutoLogin?token={1}", ConfigurationManager.AppSettings["SmartViewUrl"], Url.Encode(token)));    
        }

        [Rp3.Web.Mvc.ChildAction()]
        [Rp3.Web.Mvc.Authorize("DASHBOARDGEN","QUERY")]
        public ActionResult ResumenGestionChart(long inicio, long fin)
        {
            DateTime fechaFin = new DateTime(fin).Date.AddDays(1).AddSeconds(-1);
            DateTime fechaInicio = new DateTime(inicio).Date;
            
            var model = DataBase.Reporte.GetDashboardResumenGestion(fechaInicio, fechaFin, Agente.IdAgente);

            return PartialView("_ResumenGestionChart", model);
        }

        [Rp3.Web.Mvc.ChildAction()]
        [Rp3.Web.Mvc.Authorize("DASHBOARDGEN", "QUERY")]
        public ActionResult ResumenGestionAgente(long inicio, long fin)
        {
            DateTime fechaFin = new DateTime(fin).Date.AddDays(1).AddSeconds(-1);
            DateTime fechaInicio = new DateTime(inicio).Date;

            var model = DataBase.Reporte.GetDashboardResumenGestionAgente(fechaInicio, fechaFin, Agente.IdAgente);

            return PartialView("_ResumenGestionAgente", model);
        }

        [Rp3.Web.Mvc.ChildAction()]
        [Rp3.Web.Mvc.Authorize("DASHBOARDGEN", "QUERY")]
        public ActionResult GestionAgenteCategoriaCliente(long inicio, long fin, int? idagente = null)
        {
            DateTime fechaInicio = new DateTime(inicio);
            DateTime fechaFin = new DateTime(fin).AddDays(1).AddSeconds(-1);

            TabCollection tabs = new TabCollection();
            tabs.Add("tabGestionados", TabTarget.HtmlElement, "#tabGestionados", Rp3.AgendaComercial.Resources.LabelFor.IndicadorGestionados, true);
            tabs.Add("tabNoGestionados", TabTarget.HtmlElement, "#tabNoGestionados", Rp3.AgendaComercial.Resources.LabelFor.IndicadorNoGestionados, false);
            tabs.Add("tabProximos", TabTarget.HtmlElement, "#tabProximos", Rp3.AgendaComercial.Resources.LabelFor.IndicadorProximos, false);

            var data = DataBase.Reporte.GetDashboardAgenteAgenda(fechaInicio, fechaFin, Agente.IdAgente, idagente);
            DashboardAgenteCategoriaClienteModel model = new DashboardAgenteCategoriaClienteModel();
            model.Gestionados = data.Where(p => p.IdCategoriaGestion == "G").OrderByDescending(p=>p.FechaInicio).ToList();
            model.NoGestionados = data.Where(p => p.IdCategoriaGestion == "NG").OrderByDescending(p=>p.FechaInicio).ToList();
            model.Proximos = data.Where(p => p.IdCategoriaGestion == "P").OrderByDescending(p => p.FechaInicio).ToList();
            model.Tabs = tabs;

            if (idagente.HasValue)
            {
                var agente = DataBase.Agentes.GetSingleOrDefault(p=>
                    p.Estado != Rp3.AgendaComercial.Models.Constantes.Estado.Eliminado &&
                    p.IdAgente == idagente.Value, includeProperties: "Usuario.Contact");

                if (agente != null)
                    ViewBag.AgenteSelectNombre = agente.Usuario.Contact.DefaultFullName;
            }

            return PartialView("_AgenteDetalleCategoriaClientes", model);
        }

        [Rp3.Web.Mvc.ChildAction()]
        [Rp3.Web.Mvc.Authorize("DASHBOARDGEN", "QUERY")]
        public ActionResult AgenteDetalle()
        {
            DashboardAgenteDetalleParametro model = new DashboardAgenteDetalleParametro();            
            DateTime fechaFin = GetCurrentDateTime().Date.AddDays(31).AddSeconds(-1);
            DateTime fechaInicio = fechaFin.AddDays(-60);

            model.FechaInicio = fechaInicio;
            model.FechaFin = fechaFin;

            return PartialView("_AgenteDetallePrincipal", model);
        }
    }
}