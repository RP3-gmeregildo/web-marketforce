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
    public class SubCategoriaController : Rp3.AgendaComercial.Web.Controllers.AgendaComercialController
    {
        // GET: Pedido/SubCategoria
        [Rp3.Web.Mvc.Authorize("SUBCATEGORIA", "QUERY", "AGENDACOMERCIAL")]
        public ActionResult Index()
        {
            return View(GetListIndex());
        }

        public ActionResult GridViewIndex()
        {
            return PartialView("_GridViewIndex", GetListIndex());
        }

        public List<SubCategoria> GetListIndex()
        {
            return DataBase.SubCategorias.Get(p => p.Estado != Rp3.AgendaComercial.Models.Constantes.Estado.Eliminado, includeProperties: "EstadoGeneralValue").ToList();
        }

        [Rp3.Web.Mvc.Authorize("SUBCATEGORIA", "NEW", "AGENDACOMERCIAL")]
        public ActionResult Create()
        {
            SubCategoria model = new SubCategoria();
            ViewBag.Categorias = DataBase.Categorias.Get(p => p.Estado == Models.Constantes.Estado.Activo).ToSelectList(includeNullItem: true);
            return View(model);
        }

        private SubCategoria GetModel(int id)
        {
            SubCategoria result = DataBase.SubCategorias.Get(p => p.IdSubCategoria == id).SingleOrDefault();

            return result;
        }

        [Rp3.Web.Mvc.Authorize("SUBCATEGORIA", "EDIT", "AGENDACOMERCIAL")]
        public ActionResult Edit(int id)
        {
            ViewBag.Categorias = DataBase.Categorias.Get(p => p.Estado == Models.Constantes.Estado.Activo).ToSelectList(includeNullItem: true);
            return View(GetModel(id));
        }

        [Rp3.Web.Mvc.Authorize("SUBCATEGORIA", "DETAIL", "AGENDACOMERCIAL")]
        public ActionResult Detail(int id)
        {
            return View(GetModel(id));
        }

        [Rp3.Web.Mvc.Authorize("SUBCATEGORIA", "DELETE", "AGENDACOMERCIAL")]
        public ActionResult Delete(int id)
        {
            return View(GetModel(id));
        }

        [HttpPost]
        [PreventSpam(Order = 0)]
        [Rp3.Web.Mvc.Authorize("SUBCATEGORIA", "NEW", "AGENDACOMERCIAL", Order = 1)]
        public ActionResult Create(SubCategoria model)
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

                    DataBase.SubCategorias.Insert(model);
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
        [Rp3.Web.Mvc.Authorize("SUBCATEGORIA", "EDIT", "AGENDACOMERCIAL", Order = 1)]
        public ActionResult Edit(SubCategoria model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    SubCategoria modelUpdate = GetModel(model.IdSubCategoria);

                    modelUpdate.IdCategoria = model.IdCategoria;
                    modelUpdate.Descripcion = model.Descripcion;
                    modelUpdate.Estado = model.Estado;

                    modelUpdate.UsrMod = this.UserLogonName;
                    modelUpdate.FecMod = this.GetCurrentDateTime();

                    DataBase.SubCategorias.Update(modelUpdate);
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
        [Rp3.Web.Mvc.Authorize("SUBCATEGORIA", "DELETE", "AGENDACOMERCIAL")]
        public ActionResult Delete(SubCategoria model)
        {
            try
            {
                SubCategoria modelUpdate = GetModel(model.IdSubCategoria);
                modelUpdate.Estado = Rp3.AgendaComercial.Models.Constantes.Estado.Eliminado;
                modelUpdate.UsrMod = this.UserLogonName;
                modelUpdate.FecMod = this.GetCurrentDateTime();

                DataBase.SubCategorias.Update(modelUpdate);
                DataBase.Save();

                //DataBase.SubCategorias.Delete(modelUpdate);
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

            settings.Name = "SubCategoria";
            settings.Width = Unit.Percentage(100);

            settings.Columns.Add("Descripcion");
            settings.Columns.Add("Categoria.Descripcion", Rp3.AgendaComercial.Resources.LabelFor.Categoria);
            settings.Columns.Add("EstadoGeneralValue.Content", Rp3.AgendaComercial.Resources.LabelFor.Estado);

            settings.SettingsExport.Landscape = true;
            settings.SettingsExport.ReportHeader = settings.Name;

            return settings;
        }

        #endregion Export

    }
}