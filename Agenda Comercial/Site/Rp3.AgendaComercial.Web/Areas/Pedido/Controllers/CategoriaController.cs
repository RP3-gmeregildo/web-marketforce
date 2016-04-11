using DevExpress.Web.Mvc;
using Rp3.AgendaComercial.Models.Pedido;
using Rp3.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace Rp3.AgendaComercial.Web.Pedido.Controllers
{
    public class CategoriaController : Rp3.AgendaComercial.Web.Controllers.AgendaComercialController
    {
        // GET: Pedido/Categoria
        [Rp3.Web.Mvc.Authorize("CATEGORIA", "QUERY", "AGENDACOMERCIAL")]
        public ActionResult Index()
        {
            return View(GetListIndex());
        }

        public ActionResult GridViewIndex()
        {
            return PartialView("_GridViewIndex", GetListIndex());
        }

        public List<Categoria> GetListIndex()
        {
            return DataBase.Categorias.Get(p => p.Estado != Rp3.AgendaComercial.Models.Constantes.Estado.Eliminado, includeProperties: "EstadoGeneralValue").ToList();
        }

        [Rp3.Web.Mvc.Authorize("CATEGORIA", "NEW", "AGENDACOMERCIAL")]
        public ActionResult Create()
        {
            Categoria model = new Categoria();
            return View(model);
        }

        private Categoria GetModel(int id)
        {
            Categoria result = DataBase.Categorias.Get(p => p.IdCategoria == id).SingleOrDefault();

            return result;
        }

        [Rp3.Web.Mvc.Authorize("CATEGORIA", "EDIT", "AGENDACOMERCIAL")]
        public ActionResult Edit(int id)
        {
            return View(GetModel(id));
        }

        [Rp3.Web.Mvc.Authorize("CATEGORIA", "DETAIL", "AGENDACOMERCIAL")]
        public ActionResult Detail(int id)
        {
            return View(GetModel(id));
        }

        [Rp3.Web.Mvc.Authorize("CATEGORIA", "DELETE", "AGENDACOMERCIAL")]
        public ActionResult Delete(int id)
        {
            return View(GetModel(id));
        }

        [HttpPost]
        [PreventSpam(Order = 0)]
        [Rp3.Web.Mvc.Authorize("CATEGORIA", "NEW", "AGENDACOMERCIAL", Order = 1)]
        public ActionResult Create(Categoria model)
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

                    DataBase.Categorias.Insert(model);
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
        [Rp3.Web.Mvc.Authorize("CATEGORIA", "EDIT", "AGENDACOMERCIAL", Order = 1)]
        public ActionResult Edit(Categoria model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Categoria modelUpdate = GetModel(model.IdCategoria);

                    modelUpdate.Descripcion = model.Descripcion;
                    modelUpdate.Estado = model.Estado;

                    modelUpdate.UsrMod = this.UserLogonName;
                    modelUpdate.FecMod = this.GetCurrentDateTime();

                    DataBase.Categorias.Update(modelUpdate);
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
        [Rp3.Web.Mvc.Authorize("CATEGORIA", "DELETE", "AGENDACOMERCIAL")]
        public ActionResult Delete(Categoria model)
        {
            try
            {
                Categoria modelUpdate = GetModel(model.IdCategoria);
                modelUpdate.Estado = Rp3.AgendaComercial.Models.Constantes.Estado.Eliminado;
                modelUpdate.UsrMod = this.UserLogonName;
                modelUpdate.FecMod = this.GetCurrentDateTime();

                DataBase.Categorias.Update(modelUpdate);
                DataBase.Save();

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

            settings.Name = "Categoria";
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