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

namespace Rp3.AgendaComercial.Web.Ruta.Controllers
{
    public class ReunionController : Rp3.Web.Mvc.Controllers.BaseController<Models.ContextService>
    {
        //
        // GET: /Ruta/Reunion/

        [Rp3.Web.Mvc.Authorize("REUNION", "QUERY", "AGENDACOMERCIAL")]
        public ActionResult Index()
        {
            return View(GetListIndex());
        }

        public ActionResult GridViewIndex()
        {
            return PartialView("_GridViewIndex", GetListIndex());
        }

        public List<Reunion> GetListIndex()
        {
            return DataBase.Reuniones.Get(includeProperties: "Solicitante, EstadoReunionGeneralValue, TipoReunionGeneralValue, ImportanciaGeneralValue").ToList();
        }

        private void InicializarEdit()
        {
            var agentes = DataBase.Agentes.Get(p => p.Estado == Models.Constantes.Estado.Activo);

            ViewBag.Agentes = agentes.ToList();
            ViewBag.AgentesSelectList = agentes.ToSelectList(includeNullItem: true);
        }

        [Rp3.Web.Mvc.Authorize("REUNION", "NEW", "AGENDACOMERCIAL")]
        public ActionResult Create()
        {
            InicializarEdit();

            Reunion model = new Reunion();

            model.FechaInicio = GetCurrentDateTime();
            model.FechaFin = model.FechaInicio;

            model.EstadoReunion = Models.Constantes.EstadoReunion.Activa;
            model.Importancia = Models.Constantes.Importancia.Media;
            model.TipoReunion = Models.Constantes.TipoReunion.Normal;

            model.ReunionAsistentes = new List<ReunionAsistente>();

            return View(model);
        }

        private Reunion GetModel(long id)
        {
            Reunion result = DataBase.Reuniones.Get(p => p.IdReunion == id, includeProperties: "ReunionAsistentes").SingleOrDefault();

            return result;
        }

        [Rp3.Web.Mvc.Authorize("REUNION", "EDIT", "AGENDACOMERCIAL")]
        public ActionResult Edit(long id)
        {
            InicializarEdit();

            var model = GetModel(id);

            model.FechaInicioHora = model.FechaInicio;
            model.FechaFinHora = model.FechaFin;

            return View(model);
        }

        [Rp3.Web.Mvc.Authorize("REUNION", "DETAIL", "AGENDACOMERCIAL")]
        public ActionResult Detail(long id)
        {
            var model = GetModel(id);
            model.ReadOnly = true;

            ViewBag.Agentes = model.ReunionAsistentes.Select(p => p.Agente).Distinct().ToList();

            return View(model);
        }

        [Rp3.Web.Mvc.Authorize("REUNION", "DELETE", "AGENDACOMERCIAL")]
        public ActionResult Delete(long id)
        {
            var model = GetModel(id);
            model.ReadOnly = true;

            ViewBag.Agentes = model.ReunionAsistentes.Select(p => p.Agente).Distinct().ToList();

            return View(model);
        }

        [HttpPost]
        [PreventSpam(Order = 0)]
        [Rp3.Web.Mvc.Authorize("REUNION", "NEW", "AGENDACOMERCIAL", Order = 1)]
        public ActionResult Create(Reunion model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var FechaInicio = model.FechaInicio.Value.Date.AddHours(model.FechaInicioHora.Value.Hour).AddMinutes(model.FechaInicioHora.Value.Minute).AddSeconds(model.FechaInicioHora.Value.Second);
                    var FechaFin = model.FechaFin.Value.Date.AddHours(model.FechaFinHora.Value.Hour).AddMinutes(model.FechaFinHora.Value.Minute).AddSeconds(model.FechaFinHora.Value.Second);

                    if (FechaInicio < FechaFin)
                    {
                        model.AsignarId();

                        model.EstadoReunionTabla = Rp3.AgendaComercial.Models.Constantes.EstadoReunion.Tabla;
                        model.TipoReunionTabla = Rp3.AgendaComercial.Models.Constantes.TipoReunion.Tabla;
                        model.ImportanciaTabla = Rp3.AgendaComercial.Models.Constantes.Importancia.Tabla;

                        model.UsrIng = this.UserLogonName;
                        model.FecIng = this.GetCurrentDateTime();

                        DataBase.Reuniones.Insert(model);
                        DataBase.Save();

                        return RedirectToAction("Index");
                    }
                    else
                    {
                        this.AddErrorMessage(Rp3.AgendaComercial.Resources.ErrorMessageValidation.FechaInicioMayorFechaFin);
                    }
                }
            }
            catch
            {
                this.AddDefaultErrorMessage();
            }

            InicializarEdit();
            return View(model);
        }

        [HttpPost]
        [PreventSpam(Order = 0)]
        [Rp3.Web.Mvc.Authorize("REUNION", "EDIT", "AGENDACOMERCIAL", Order = 1)]
        public ActionResult Edit(Reunion model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var FechaInicio = model.FechaInicio.Value.Date.AddHours(model.FechaInicioHora.Value.Hour).AddMinutes(model.FechaInicioHora.Value.Minute).AddSeconds(model.FechaInicioHora.Value.Second);
                    var FechaFin = model.FechaFin.Value.Date.AddHours(model.FechaFinHora.Value.Hour).AddMinutes(model.FechaFinHora.Value.Minute).AddSeconds(model.FechaFinHora.Value.Second);

                    if (FechaInicio < FechaFin)
                    {
                        Reunion modelUpdate = GetModel(model.IdReunion);

                        CopyTo(model, modelUpdate, includeProperties: new string[] {
                        "Ubicacion",
                        "Asunto",
                        "Detalle",
                        "IdAgenteSolicitante",
                        "TipoReunion",
                        "Importancia",
                        "EstadoReunion"
                        });

                        modelUpdate.FechaInicio = FechaInicio;
                        modelUpdate.FechaFin = FechaFin;

                        modelUpdate.UsrMod = this.UserLogonName;
                        modelUpdate.FecMod = this.GetCurrentDateTime();

                        DataBase.ReunionAsistentes.Update(model.ReunionAsistentes, modelUpdate.ReunionAsistentes);

                        DataBase.Reuniones.Update(modelUpdate);
                        DataBase.Save();

                        return RedirectToAction("Index");
                    }
                    else
                    {
                        this.AddErrorMessage(Rp3.AgendaComercial.Resources.ErrorMessageValidation.FechaInicioMayorFechaFin);
                    }
                }
            }
            catch
            {
                this.AddDefaultErrorMessage();
            }

            InicializarEdit();
            return View(model);
        }

        [HttpPost]
        [PreventSpam(Order = 0)]
        [Rp3.Web.Mvc.Authorize("REUNION", "DELETE", "AGENDACOMERCIAL")]
        public ActionResult Delete(Reunion model)
        {
            try
            {
                Reunion modelDelete = GetModel(model.IdReunion);

                DataBase.Reuniones.Delete(modelDelete);
                DataBase.Save();

                return RedirectToAction("Index");
            }
            catch
            {
                this.AddDefaultErrorMessage();
            }

            return RedirectToAction("Index", null, null, true);
        }
    }
}