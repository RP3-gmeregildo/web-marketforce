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
    public class RegionController : Rp3.Web.Mvc.Controllers.BaseController<Models.ContextService>
    {
        //
        // GET: /General/Region/

        [Rp3.Web.Mvc.Authorize("REGION", "QUERY", "AGENDACOMERCIAL")]
        public ActionResult Index()
        {
            return View(GetListIndex());
        }

        public ActionResult GridViewIndex()
        {
            return PartialView("_GridViewIndex", GetListIndex());
        }

        public List<Region> GetListIndex()
        {
            return DataBase.Regiones.Get(p=>p.Estado != Rp3.AgendaComercial.Models.Constantes.Estado.Eliminado, includeProperties: "EstadoGeneralValue").ToList();
        }

        [Rp3.Web.Mvc.Authorize("REGION", "NEW", "AGENDACOMERCIAL")]
        public ActionResult Create()
        {
            Region model = new Region();
            model.Estado = Models.Constantes.Estado.Activo;
            return View(model);
        }

        private Region GetModel(int id)
        {
            Region result = DataBase.Regiones.Get(p => p.IdRegion == id).SingleOrDefault();

            return result;
        }

        [Rp3.Web.Mvc.Authorize("REGION", "EDIT", "AGENDACOMERCIAL")]
        public ActionResult Edit(int id)
        {
            return View(GetModel(id));
        }

        [Rp3.Web.Mvc.Authorize("REGION", "DETAIL", "AGENDACOMERCIAL")]
        public ActionResult Detail(int id)
        {
            return View(GetModel(id));
        }

        [Rp3.Web.Mvc.Authorize("REGION", "DELETE", "AGENDACOMERCIAL")]
        public ActionResult Delete(int id)
        {
            return View(GetModel(id));
        }

        [HttpPost]
        [PreventSpam(Order = 0)]
        [Rp3.Web.Mvc.Authorize("REGION", "NEW", "AGENDACOMERCIAL", Order = 1)]
        public ActionResult Create(Region model)
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

                    DataBase.Regiones.Insert(model);
                    DataBase.Save();

                    return RedirectToAction("Index");
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
        [Rp3.Web.Mvc.Authorize("REGION", "EDIT", "AGENDACOMERCIAL", Order = 1)]
        public ActionResult Edit(Region model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Region modelUpdate = GetModel(model.IdRegion);

                    modelUpdate.Descripcion = model.Descripcion;
                    modelUpdate.Estado = model.Estado;

                    modelUpdate.UsrMod = this.UserLogonName;
                    modelUpdate.FecMod = this.GetCurrentDateTime();

                    DataBase.Regiones.Update(modelUpdate);
                    DataBase.Save();

                    return RedirectToAction("Index");
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
        [Rp3.Web.Mvc.Authorize("REGION", "DELETE", "AGENDACOMERCIAL")]
        public ActionResult Delete(Region model)
        {
            try
            {
                //Cargo cargoDelete = GetCargo(cargo.IdCargo);

                DataBase.Regiones.Delete(model);
                DataBase.Save();

                return RedirectToAction("Index");
            }
            catch
            {
                this.AddDefaultErrorMessage();
            }
            return RedirectToAction("Index", null, null, true);
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

            settings.Name = "Regiones";
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
