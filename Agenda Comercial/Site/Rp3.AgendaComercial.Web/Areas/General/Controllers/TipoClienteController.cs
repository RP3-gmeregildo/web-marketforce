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
    public class TipoClienteController : Rp3.Web.Mvc.Controllers.BaseController<Models.ContextService>
    {
        //
        // GET: /General/TipoCliente/

        [Rp3.Web.Mvc.Authorize("TIPOCLIENTE", "QUERY", "AGENDACOMERCIAL")]
        public ActionResult Index()
        {
            return View(GetListIndex());
        }

        public ActionResult GridViewIndex()
        {
            return PartialView("_GridViewIndex", GetListIndex());
        }

        public List<TipoCliente> GetListIndex()
        {
            return DataBase.TipoClientes.Get(p=>p.Estado != Rp3.AgendaComercial.Models.Constantes.Estado.Eliminado, includeProperties: "EstadoGeneralValue").ToList();
        }

        [Rp3.Web.Mvc.Authorize("TIPOCLIENTE", "NEW", "AGENDACOMERCIAL")]
        public ActionResult Create()
        {
            TipoCliente model = new TipoCliente();
            model.Estado = Models.Constantes.Estado.Activo;
            return View(model);
        }

        private TipoCliente GetModel(int id)
        {
            TipoCliente result = DataBase.TipoClientes.Get(p => p.IdTipoCliente == id).SingleOrDefault();

            return result;
        }

        [Rp3.Web.Mvc.Authorize("TIPOCLIENTE", "EDIT", "AGENDACOMERCIAL")]
        public ActionResult Edit(int id)
        {
            return View(GetModel(id));
        }

        [Rp3.Web.Mvc.Authorize("TIPOCLIENTE", "DETAIL", "AGENDACOMERCIAL")]
        public ActionResult Detail(int id)
        {
            return View(GetModel(id));
        }

        [Rp3.Web.Mvc.Authorize("TIPOCLIENTE", "DELETE", "AGENDACOMERCIAL")]
        public ActionResult Delete(int id)
        {
            return View(GetModel(id));
        }

        [HttpPost]
        [PreventSpam(Order = 0)]
        [Rp3.Web.Mvc.Authorize("TIPOCLIENTE", "NEW", "AGENDACOMERCIAL", Order = 1)]
        public ActionResult Create(TipoCliente model)
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

                    DataBase.TipoClientes.Insert(model);
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
        [Rp3.Web.Mvc.Authorize("TIPOCLIENTE", "EDIT", "AGENDACOMERCIAL", Order = 1)]
        public ActionResult Edit(TipoCliente model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    TipoCliente modelUpdate = GetModel(model.IdTipoCliente);

                    modelUpdate.Descripcion = model.Descripcion;
                    modelUpdate.Estado = model.Estado;

                    modelUpdate.UsrMod = this.UserLogonName;
                    modelUpdate.FecMod = this.GetCurrentDateTime();

                    DataBase.TipoClientes.Update(modelUpdate);
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
            //return RedirectToAction("Index", "Cargo", null, true);
        }

        [HttpPost]
        [PreventSpam(Order = 0)]
        [Rp3.Web.Mvc.Authorize("TIPOCLIENTE", "DELETE", "AGENDACOMERCIAL")]
        public ActionResult Delete(TipoCliente model)
        {
            try
            {
                TipoCliente modelUpdate = GetModel(model.IdTipoCliente);
                modelUpdate.Estado = Rp3.AgendaComercial.Models.Constantes.Estado.Eliminado;
                modelUpdate.UsrMod = this.UserLogonName;
                modelUpdate.FecMod = this.GetCurrentDateTime();

                DataBase.TipoClientes.Update(modelUpdate);
                DataBase.Save();

                VerificarDependencia(modelUpdate);

                //DataBase.TipoClientes.Delete(model);
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


        private void VerificarDependencia(TipoCliente tipoCliente)
        {
            if (tipoCliente.Estado == Rp3.AgendaComercial.Models.Constantes.Estado.Inactivo ||
                tipoCliente.Estado == Rp3.AgendaComercial.Models.Constantes.Estado.Eliminado)
            {
                DataBase.TipoClientes.EliminarDependenciaTipoCliente(tipoCliente.IdTipoCliente, this.UserLogonName);
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

            settings.Name = "TiposCliente";
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
