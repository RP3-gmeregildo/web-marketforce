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
    public class TipoCarteraController : Rp3.Web.Mvc.Controllers.BaseController<Models.ContextService>
    {
        //
        // GET: /General/TipoCartera/

        [Rp3.Web.Mvc.Authorize("TIPOCARTERA", "QUERY", "AGENDACOMERCIAL")]
        public ActionResult Index()
        {
            return View(GetListIndex());
        }

        public ActionResult GridViewIndex()
        {
            return PartialView("_GridViewIndex", GetListIndex());
        }

        public List<TipoCartera> GetListIndex()
        {
            return DataBase.TipoCarteras.Get(p=>p.Estado != Rp3.AgendaComercial.Models.Constantes.Estado.Eliminado, includeProperties: "EstadoGeneralValue").ToList();
        }

        [Rp3.Web.Mvc.Authorize("TIPOCARTERA", "NEW", "AGENDACOMERCIAL")]
        public ActionResult Create()
        {
            TipoCartera model = new TipoCartera();
            model.Estado = Models.Constantes.Estado.Activo;
            return View(model);
        }

        private TipoCartera GetModel(int id)
        {
            TipoCartera result = DataBase.TipoCarteras.Get(p => p.IdTipoCartera == id).SingleOrDefault();

            return result;
        }

        [Rp3.Web.Mvc.Authorize("TIPOCARTERA", "EDIT", "AGENDACOMERCIAL")]
        public ActionResult Edit(int id)
        {
            return View(GetModel(id));
        }

        [Rp3.Web.Mvc.Authorize("TIPOCARTERA", "DETAIL", "AGENDACOMERCIAL")]
        public ActionResult Detail(int id)
        {
            return View(GetModel(id));
        }

        [Rp3.Web.Mvc.Authorize("TIPOCARTERA", "DELETE", "AGENDACOMERCIAL")]
        public ActionResult Delete(int id)
        {
            return View(GetModel(id));
        }

        [HttpPost]
        [PreventSpam(Order = 0)]
        [Rp3.Web.Mvc.Authorize("TIPOCARTERA", "NEW", "AGENDACOMERCIAL", Order = 1)]
        public ActionResult Create(TipoCartera model)
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

                    DataBase.TipoCarteras.Insert(model);
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
            //return RedirectToAction("Index", "Cargo", null, true);
        }

        [HttpPost]
        [PreventSpam(Order = 0)]
        [Rp3.Web.Mvc.Authorize("TIPOCARTERA", "EDIT", "AGENDACOMERCIAL", Order = 1)]
        public ActionResult Edit(TipoCartera model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    TipoCartera modelUpdate = GetModel(model.IdTipoCartera);

                    modelUpdate.Descripcion = model.Descripcion;
                    modelUpdate.Estado = model.Estado;

                    modelUpdate.UsrMod = this.UserLogonName;
                    modelUpdate.FecMod = this.GetCurrentDateTime();

                    DataBase.TipoCarteras.Update(modelUpdate);
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
            //return RedirectToAction("Index", "Cargo", null, true);
        }

        [HttpPost]
        [PreventSpam(Order = 0)]
        [Rp3.Web.Mvc.Authorize("TIPOCARTERA", "DELETE", "AGENDACOMERCIAL")]
        public ActionResult Delete(TipoCartera model)
        {
            try
            {
                TipoCartera modelUpdate = GetModel(model.IdTipoCartera);
                modelUpdate.Estado = Rp3.AgendaComercial.Models.Constantes.Estado.Eliminado;
                modelUpdate.UsrMod = this.UserLogonName;
                modelUpdate.FecMod = this.GetCurrentDateTime();

                DataBase.TipoCarteras.Update(modelUpdate);
                DataBase.Save();

                //DataBase.TipoCarteras.Delete(model);
                //DataBase.Save();

                this.AddDefaultSuccessMessage();
                return RedirectToAction("Index", model);
            }
            catch
            {
                this.AddDefaultErrorMessage();
            }
            return RedirectToAction("Index",model);
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

            settings.Name = "TiposCartera";
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
