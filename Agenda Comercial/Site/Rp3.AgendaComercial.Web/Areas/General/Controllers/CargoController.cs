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
    public class CargoController : Rp3.Web.Mvc.Controllers.BaseController<Models.ContextService>
    {
        //
        // GET: /General/Cargo/

        [Rp3.Web.Mvc.Authorize("CARGO", "QUERY", "AGENDACOMERCIAL")]
        public ActionResult Index()
        {
            return View(GetListIndex());
        }

        public ActionResult GridViewIndex()
        {
            return PartialView("_GridViewIndex", GetListIndex());
        }

        private List<Cargo> GetListIndex()
        {
            return DataBase.Cargos.Get(p=>p.Estado != Rp3.AgendaComercial.Models.Constantes.Estado.Eliminado, includeProperties: "EstadoGeneralValue").ToList();
        }

        [Rp3.Web.Mvc.Authorize("CARGO", "NEW", "AGENDACOMERCIAL")]
        public ActionResult Create()
        {
            Cargo model = new Cargo();
            model.Estado = Models.Constantes.Estado.Activo;
            return View(model);
        }

        private Cargo GetModel(int id)
        {
            Cargo result = DataBase.Cargos.Get(p => p.IdCargo == id).SingleOrDefault();

            return result;
        }

        [Rp3.Web.Mvc.Authorize("CARGO", "EDIT", "AGENDACOMERCIAL")]
        public ActionResult Edit(int id)
        {
            return View(GetModel(id));
        }

        [Rp3.Web.Mvc.Authorize("CARGO", "DETAIL", "AGENDACOMERCIAL")]
        public ActionResult Detail(int id)
        {
            return View(GetModel(id));
        }

        [Rp3.Web.Mvc.Authorize("CARGO", "DELETE", "AGENDACOMERCIAL")]
        public ActionResult Delete(int id)
        {
            ViewBag.PermitirEliminar = DataBase.Agentes.Get(p => p.IdCargo == id && p.Estado == Rp3.AgendaComercial.Models.Constantes.Estado.Activo).Count() == 0;

            return View(GetModel(id));
        }

        [HttpPost]
        [PreventSpam(Order = 0)]
        [Rp3.Web.Mvc.Authorize("CARGO", "NEW", "AGENDACOMERCIAL", Order = 1)]
        public ActionResult Create(Cargo model)
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

                    DataBase.Cargos.Insert(model);
                    DataBase.Save();

                    this.AddDefaultSuccessMessage();
                    return RedirectToAction("Index");
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
        [Rp3.Web.Mvc.Authorize("CARGO", "EDIT", "AGENDACOMERCIAL", Order = 1)]
        public ActionResult Edit(Cargo model)
        {
            try
            {              
                if (ModelState.IsValid)
                {
                    Cargo modelUpdate = GetModel(model.IdCargo);

                    modelUpdate.Descripcion = model.Descripcion;
                    modelUpdate.EsSupervisor = model.EsSupervisor;
                    modelUpdate.Estado = model.Estado;

                    modelUpdate.UsrMod = this.UserLogonName;
                    modelUpdate.FecMod = this.GetCurrentDateTime();

                    DataBase.Cargos.Update(modelUpdate);
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
        [Rp3.Web.Mvc.Authorize("CARGO", "DELETE", "AGENDACOMERCIAL")]
        public ActionResult Delete(Cargo model)
        {
            try
            {
                Cargo modelUpdate = GetModel(model.IdCargo);
                modelUpdate.Estado = Rp3.AgendaComercial.Models.Constantes.Estado.Eliminado;
                
                modelUpdate.UsrMod = this.UserLogonName;
                modelUpdate.FecMod = this.GetCurrentDateTime();

                DataBase.Cargos.Update(modelUpdate);
                DataBase.Save();

                
                this.AddDefaultSuccessMessage();
                return RedirectToAction("Index", model);
            }
            catch
            {
                this.AddDefaultErrorMessage();
            }

            return RedirectToAction("Index", model);
        }

        #region Export

        public ActionResult ExportToXls()
        {
            return GridViewExtension.ExportToXls(CreateExportGridViewSettings(), GetListIndex());

        }
        public ActionResult ExportToPdf() 
        {
            return GridViewExtension.ExportToPdf(CreateExportGridViewSettings(), GetListIndex());
        }

        static GridViewSettings CreateExportGridViewSettings()
        {
            GridViewSettings settings = new GridViewSettings();

            settings.Name = "Cargos";
            settings.Width = Unit.Percentage(100);

            settings.Columns.Add("Descripcion");
            settings.Columns.Add("EsSupervisor", MVCxGridViewColumnType.CheckBox);
            settings.Columns.Add("EsAdministrador", MVCxGridViewColumnType.CheckBox);
            settings.Columns.Add("EstadoGeneralValue.Content", Rp3.AgendaComercial.Resources.LabelFor.Estado);

            settings.SettingsExport.Landscape = true;
            settings.SettingsExport.ReportHeader = settings.Name;       

            return settings;
        }

        #endregion Export
    }
}
