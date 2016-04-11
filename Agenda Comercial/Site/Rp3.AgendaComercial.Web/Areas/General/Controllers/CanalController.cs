using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Rp3.Data.Models.Security;
using Rp3.Web.Mvc.Application.Security.Models;
using Rp3.Web.Mvc;
using Rp3.AgendaComercial.Models.General;
using DevExpress.Web.Mvc;
using System.Web.UI.WebControls;

namespace Rp3.AgendaComercial.Web.General.Controllers
{
    public class CanalController : Rp3.Web.Mvc.Controllers.BaseController<Models.ContextService>
    {
        //
        // GET: /General/Canal/

        [Rp3.Web.Mvc.Authorize("CANAL", "QUERY", "AGENDACOMERCIAL")]
        public ActionResult Index()
        {
            return View(GetListIndex());
        }

        public ActionResult GridViewIndex()
        {
            return PartialView("_GridViewIndex", GetListIndex());
        }

        public List<Canal> GetListIndex()
        {
            return DataBase.Canales.Get(p=>p.Estado != Rp3.AgendaComercial.Models.Constantes.Estado.Eliminado, includeProperties: "EstadoGeneralValue").ToList();
        }

        [Rp3.Web.Mvc.Authorize("CANAL", "NEW", "AGENDACOMERCIAL")]
        public ActionResult Create()
        {
            Canal model = new Canal();
            model.Estado = Models.Constantes.Estado.Activo;
            return View(model);
        }

        private Canal GetModel(int id)
        {
            Canal result = DataBase.Canales.Get(p => p.IdCanal == id).SingleOrDefault();

            return result;
        }

        [Rp3.Web.Mvc.Authorize("CANAL", "EDIT", "AGENDACOMERCIAL")]
        public ActionResult Edit(int id)
        {
            return View(GetModel(id));
        }

        [Rp3.Web.Mvc.Authorize("CANAL", "DETAIL", "AGENDACOMERCIAL")]
        public ActionResult Detail(int id)
        {
            return View(GetModel(id));
        }

        [Rp3.Web.Mvc.Authorize("CANAL", "DELETE", "AGENDACOMERCIAL")]
        public ActionResult Delete(int id)
        {
            return View(GetModel(id));
        }

        [HttpPost]
        [PreventSpam(Order = 0)]
        [Rp3.Web.Mvc.Authorize("CANAL", "NEW", "AGENDACOMERCIAL", Order = 1)]
        public ActionResult Create(Canal model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.AsignarId();
                    model.EstadoTabla = Rp3.AgendaComercial.Models.Constantes.Estado.Tabla;
                    model.Estado = Rp3.AgendaComercial.Models.Constantes.Estado.Activo;
                    model.UsrIng = this.UserLogonName;
                    model.FecIng = this.GetCurrentDateTime();

                    DataBase.Canales.Insert(model);
                    DataBase.Save();

                    this.AddDefaultSuccessMessage();
                    return RedirectToAction("Index", model);
                }
            }
            catch
            {
                this.AddDefaultErrorMessage();
            }

            return View(model);
        }

        [HttpPost]
        [PreventSpam(Order = 0)]
        [Rp3.Web.Mvc.Authorize("CANAL", "EDIT", "AGENDACOMERCIAL", Order = 1)]
        public ActionResult Edit(Canal model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Canal modelUpdate = GetModel(model.IdCanal);

                    modelUpdate.Descripcion = model.Descripcion;
                    modelUpdate.Estado = model.Estado;

                    modelUpdate.UsrMod = this.UserLogonName;
                    modelUpdate.FecMod = this.GetCurrentDateTime();

                    DataBase.Canales.Update(modelUpdate);
                    DataBase.Save();

                    VerificarDependencia(model);

                    this.AddDefaultSuccessMessage();
                    return RedirectToAction("Index", model);
                }
            }
            catch
            {
                this.AddDefaultErrorMessage();
            }

            return View(model);
        }

        [HttpPost]
        [PreventSpam(Order = 0)]
        [Rp3.Web.Mvc.Authorize("CANAL", "DELETE", "AGENDACOMERCIAL")]
        public ActionResult Delete(Canal model)
        {
            try
            {
                Canal modelUpdate = GetModel(model.IdCanal);
                modelUpdate.Estado = Rp3.AgendaComercial.Models.Constantes.Estado.Eliminado;
                modelUpdate.UsrMod = this.UserLogonName;
                modelUpdate.FecMod = this.GetCurrentDateTime();

                DataBase.Canales.Update(modelUpdate);
                DataBase.Save();

                VerificarDependencia(model);

                //DataBase.Canales.Delete(model);
                //DataBase.Save();

                this.AddDefaultSuccessMessage();
                return RedirectToAction("Index", model);
            }
            catch
            {
                this.AddDefaultErrorMessage();
            }
            return RedirectToAction("Index", model);
        }

        private void VerificarDependencia(Canal canal)
        {
            if (canal.Estado == Rp3.AgendaComercial.Models.Constantes.Estado.Inactivo ||
                canal.Estado == Rp3.AgendaComercial.Models.Constantes.Estado.Eliminado)
            {
                DataBase.Canales.EliminarDependenciaCanal(canal.IdCanal, this.UserLogonName);
            }
        }


        #region Export

        public ActionResult ExportToXls()
        {
            var data = GetListIndex();

            return GridViewExtension.ExportToXlsx(CreateExportGridViewSettings(), data);
        }
        public ActionResult ExportToPdf()
        {
            var data = GetListIndex();

            return GridViewExtension.ExportToPdf(CreateExportGridViewSettings(), data);
        }

        static GridViewSettings CreateExportGridViewSettings()
        {
            GridViewSettings settings = new GridViewSettings();

            settings.Name = "Canales";
            settings.Width = Unit.Percentage(100);

            settings.Columns.Add("Descripcion");
            settings.Columns.Add("EstadoGeneralValue.Content", Rp3.AgendaComercial.Resources.LabelFor.Estado);

            settings.SettingsExport.Landscape = true;
            settings.SettingsExport.ReportHeader = settings.Name;       

            return settings;
        }

        #endregion Export
    }
}
