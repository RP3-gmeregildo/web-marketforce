using DevExpress.Web.Mvc;
using Rp3.AgendaComercial.Models.Marcacion;
using Rp3.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace Rp3.AgendaComercial.Web.Marcacion.Controllers
{
    public class GrupoController : Rp3.Web.Mvc.Controllers.BaseController<Models.ContextService>
    {
        [Rp3.Web.Mvc.Authorize("GRUPO", "QUERY", "AGENDACOMERCIAL")]
        public ActionResult Index()
        {
            return View(GetListIndex());
        }

        public ActionResult GridViewIndex()
        {
            return PartialView("_GridViewIndex", GetListIndex());
        }

        private List<Grupo> GetListIndex()
        {
            return DataBase.Grupos.Get(p=>p.Estado != Rp3.AgendaComercial.Models.Constantes.Estado.Eliminado, includeProperties: "EstadoGeneralValue").ToList();
        }

        private void InicializarEdit()
        {
            ViewBag.CalendariosSelectList = DataBase.Calendarios.Get(p =>p.Estado == Models.Constantes.Estado.Activo).ToSelectList(includeNullItem: true);
        }

        private void InicializarDetail(int IdGrupo)
        {
            ViewBag.Agentes = DataBase.Agentes.Get(p => p.IdGrupo == IdGrupo && p.Estado == Models.Constantes.Estado.Activo).ToList();
        }

        [Rp3.Web.Mvc.Authorize("GRUPO", "NEW", "AGENDACOMERCIAL")]
        public ActionResult Create()
        {
            Grupo model = new Grupo();
            model.AplicaMarcacion = true;
            model.Estado = Models.Constantes.Estado.Activo;
            
            InicializarEdit();
            InicializarDetail(model.IdGrupo);

            return View(model);
        }

        private Grupo GetModel(int id)
        {
            Grupo result = DataBase.Grupos.Get(p => p.IdGrupo == id).SingleOrDefault();
            if (!result.LongitudPuntoPartida.HasValue && !result.LatitudPuntoPartida.HasValue)
                result.TieneUbicacion = false;
            else
                result.TieneUbicacion = true;

            return result;
        }

        [Rp3.Web.Mvc.Authorize("GRUPO", "EDIT", "AGENDACOMERCIAL")]
        public ActionResult Edit(int id)
        {
            InicializarEdit();
            InicializarDetail(id);

            return View(GetModel(id));
        }

        [Rp3.Web.Mvc.Authorize("GRUPO", "DETAIL", "AGENDACOMERCIAL")]
        public ActionResult Detail(int id)
        {
            InicializarDetail(id);

            var model = GetModel(id);
            model.ReadOnly = true;

            return View(model);
        }

        [Rp3.Web.Mvc.Authorize("GRUPO", "DELETE", "AGENDACOMERCIAL")]
        public ActionResult Delete(int id)
        {
            InicializarDetail(id);

            ViewBag.PermitirEliminar = DataBase.Agentes.Get(p => p.IdGrupo == id && p.Estado == Rp3.AgendaComercial.Models.Constantes.Estado.Activo).Count() == 0;

            var model = GetModel(id);
            model.ReadOnly = true;

            return View(model);
        }

        [HttpPost]
        [PreventSpam(Order = 0)]
        [Rp3.Web.Mvc.Authorize("GRUPO", "NEW", "AGENDACOMERCIAL", Order = 1)]
        public ActionResult Create(Grupo model, string[] agentes)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.AsignarId();
                    model.Estado = Rp3.AgendaComercial.Models.Constantes.Estado.Activo;
                    model.EstadoTabla = Rp3.AgendaComercial.Models.Constantes.Estado.Tabla;
                    model.UsrIng = this.UserLogonName;
                    model.FecIng = this.GetCurrentDateTime();
                    if(!model.TieneUbicacion)
                    {
                        model.LatitudPuntoPartida = null;
                        model.LongitudPuntoPartida = null;
                    }

                    DataBase.Grupos.Insert(model);

                    DataBase.Save();

                    DataBase.Agentes.GrupoAgenteUpdate(model.IdGrupo, agentes, this.UserLogonName);

                    this.AddDefaultSuccessMessage();
                    return RedirectToAction("Index");
                }
            }
            catch
            {
                this.AddDefaultErrorMessage();
            }


            InicializarEdit();
            InicializarDetail(model.IdGrupo);

            return View(model);
        }

        [HttpPost]
        [PreventSpam(Order = 0)]
        [Rp3.Web.Mvc.Authorize("GRUPO", "EDIT", "AGENDACOMERCIAL", Order = 1)]
        public ActionResult Edit(Grupo model, string[] agentes)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Grupo modelUpdate = GetModel(model.IdGrupo);

                    modelUpdate.Descripcion = model.Descripcion;                    
                    modelUpdate.Estado = model.Estado;
                    modelUpdate.IdCalendario = model.IdCalendario;
                    modelUpdate.LatitudPuntoPartida = model.LatitudPuntoPartida;
                    modelUpdate.LongitudPuntoPartida = model.LongitudPuntoPartida;
                    if (!model.TieneUbicacion)
                    {
                        modelUpdate.LatitudPuntoPartida = null;
                        modelUpdate.LongitudPuntoPartida = null;
                    }
                    modelUpdate.AplicaMarcacion = model.AplicaMarcacion;
                    modelUpdate.AplicaBreak = model.AplicaBreak;

                    modelUpdate.UsrMod = this.UserLogonName;
                    modelUpdate.FecMod = this.GetCurrentDateTime();

                    DataBase.Grupos.Update(modelUpdate);

                    DataBase.Save();

                    DataBase.Agentes.GrupoAgenteUpdate(model.IdGrupo, agentes, this.UserLogonName);

                    VerificarDependencia(model);

                    this.AddDefaultSuccessMessage();
                    return RedirectToAction("Index", model);
                }
            }
            catch
            {
                this.AddDefaultErrorMessage();
            }

            InicializarEdit();
            InicializarDetail(model.IdGrupo);

            return View(model);
        }

        [HttpPost]
        [PreventSpam(Order = 0)]
        [Rp3.Web.Mvc.Authorize("GRUPO", "DELETE", "AGENDACOMERCIAL")]
        public ActionResult Delete(Grupo model)
        {
            try
            {
                Grupo modelUpdate = GetModel(model.IdGrupo);
                modelUpdate.Estado = Rp3.AgendaComercial.Models.Constantes.Estado.Eliminado;

                modelUpdate.UsrMod = this.UserLogonName;
                modelUpdate.FecMod = this.GetCurrentDateTime();

                DataBase.Grupos.Update(modelUpdate);
                DataBase.Save();

                VerificarDependencia(model);

                this.AddDefaultSuccessMessage();
                return RedirectToAction("Index", model);
            }
            catch
            {
                this.AddDefaultErrorMessage();
            }

            return RedirectToAction("Index", model);
        }

        [HttpGet]
        public ActionResult GetSeleccionAgente()
        {
            var model = DataBase.Agentes.Get(p => p.Estado == Rp3.AgendaComercial.Models.Constantes.Estado.Activo && p.IdGrupo == null).ToList();

            return PartialView("_SeleccionAgente", model);
        }


        private void VerificarDependencia(Grupo grupo)
        {
            if (grupo.Estado == Rp3.AgendaComercial.Models.Constantes.Estado.Inactivo ||
                grupo.Estado == Rp3.AgendaComercial.Models.Constantes.Estado.Eliminado)
            {
                DataBase.Grupos.EliminarDependenciaGrupo(grupo.IdGrupo, this.UserLogonName);
            }
        }


        #region Export

        public ActionResult ExportToXls()
        {
            var data = GetListIndex();
            GridViewExtension.WriteXlsToResponse(CreateExportGridViewSettings(), data);

            return View(data);
        }
        public ActionResult ExportToPdf()
        {
            var data = GetListIndex();
            GridViewExtension.WritePdfToResponse(CreateExportGridViewSettings(), data);

            return View(data);
        }


        static GridViewSettings CreateExportGridViewSettings()
        {
            GridViewSettings settings = new GridViewSettings();

            settings.Name = "Export";
            settings.Width = Unit.Percentage(100);

            settings.Columns.Add("Descripcion", Rp3.AgendaComercial.Resources.LabelFor.Descripcion);
            settings.Columns.Add("Calendario.Descripcion", Rp3.AgendaComercial.Resources.LabelFor.Calendario);
            settings.Columns.Add("AplicaMarcacion", Rp3.AgendaComercial.Resources.LabelFor.AplicaMarcacion, MVCxGridViewColumnType.CheckBox);
            settings.Columns.Add("EstadoGeneralValue.Content", Rp3.AgendaComercial.Resources.LabelFor.Estado);

            return settings;
        }

        #endregion Export
    }
}