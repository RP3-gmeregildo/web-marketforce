using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Rp3.Data.Models.Security;
using Rp3.Web.Mvc.Application.Security.Models;
using Rp3.Web.Mvc;
using Rp3.AgendaComercial.Models.Ruta;
using Rp3.AgendaComercial.Web.Services.Ruta.Models;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Converters;
using Rp3.AgendaComercial.Models.Ruta.View;
using Rp3.AgendaComercial.Models;
using DevExpress.Web.Mvc;
using System.Web.UI.WebControls;

namespace Rp3.AgendaComercial.Web.Ruta.Controllers
{
    //public class LoteController : Rp3.Web.Mvc.Controllers.BaseController<Models.ContextService>
    public class LoteController : Rp3.AgendaComercial.Web.Controllers.AgendaComercialController
    {
        //
        // GET: /Ruta/Lote/

        [Rp3.Web.Mvc.Authorize("LOTE", "QUERY", "AGENDACOMERCIAL")]
        public ActionResult Index()
        {
            return View(GetListIndex());
        }

        public ActionResult GridViewIndex()
        {
            return PartialView("_GridViewIndex", GetListIndex());
        }

        public List<Lote> GetListIndex()
        {
            return DataBase.Lotes.Get(p => p.Estado != Rp3.AgendaComercial.Models.Constantes.Estado.Eliminado, includeProperties: "EstadoGeneralValue").ToList();
        }

        private void InicializarEdit()
        {
            ViewBag.TipoClientes = DataBase.TipoClientes.Get(p => p.Estado == Models.Constantes.Estado.Activo).ToList();
            ViewBag.Canales = DataBase.Canales.Get(p => p.Estado == Models.Constantes.Estado.Activo).ToList();
            ViewBag.Zonas = DataBase.Zonas.Get(p => p.Estado == Models.Constantes.Estado.Activo).ToList();
        }


        [Rp3.Web.Mvc.Authorize("LOTE", "NEW", "AGENDACOMERCIAL")]
        public ActionResult Create()
        {
            InicializarEdit();

            Lote model = new Lote();
            model.Estado = Models.Constantes.Estado.Activo;

            model.LoteTipoClientes = new List<LoteTipoCliente>();
            model.LoteCanales = new List<LoteCanal>();
            model.LoteZonas = new List<LoteZona>();
            model.LoteDetalles = new List<LoteDetalle>();

            return View(model);
        }

        private Lote GetModel(int id)
        {
            Lote result = DataBase.Lotes.Get(p => p.IdLote == id, includeProperties: "LoteTipoClientes, LoteCanales, LoteZonas, LoteDetalles").SingleOrDefault();

            return result;
        }

        [Rp3.Web.Mvc.Authorize("LOTE", "EDIT", "AGENDACOMERCIAL")]
        public ActionResult Edit(int id)
        {
            InicializarEdit();

            var model = GetModel(id);

            SetUbicacion(model.LoteDetalles);

            return View(model);
        }

        [Rp3.Web.Mvc.Authorize("LOTE", "DETAIL", "AGENDACOMERCIAL")]
        public ActionResult Detail(int id)
        {
            var model = GetModel(id);
            model.ReadOnly = true;

            ViewBag.TipoClientes = model.LoteTipoClientes.Select(p => p.TipoCliente).Distinct().ToList();
            ViewBag.Canales = model.LoteCanales.Select(p => p.Canal).Distinct().ToList();
            ViewBag.Zonas = model.LoteZonas.Select(p => p.Zona).Distinct().ToList();

            SetUbicacion(model.LoteDetalles);

            return View(model);
        }

        [Rp3.Web.Mvc.Authorize("LOTE", "DELETE", "AGENDACOMERCIAL")]
        public ActionResult Delete(int id)
        {
            var model = GetModel(id);
            model.ReadOnly = true;

            ViewBag.TipoClientes = model.LoteTipoClientes.Select(p => p.TipoCliente).Distinct().ToList();
            ViewBag.Canales = model.LoteCanales.Select(p => p.Canal).Distinct().ToList();
            ViewBag.Zonas = model.LoteZonas.Select(p => p.Zona).Distinct().ToList();

            SetUbicacion(model.LoteDetalles);

            return View(model);
        }

        //[Rp3.Web.Mvc.Authorize("LOTE", "PROCESS", "AGENDACOMERCIAL")]
        //public ActionResult Process(int id)
        //{
        //    Lote modelUpdate = GetModel(id);

        //    try
        //    {
        //        ProcessClientes(modelUpdate);

        //        DataBase.Lotes.Update(modelUpdate);
        //        DataBase.Save();

        //        SetUbicacion(modelUpdate.LoteDetalles);

        //        this.AddSuccessMessage(@Rp3.AgendaComercial.Resources.LegendFor.LoteProcesado);
        //    }
        //    catch
        //    {
        //        this.AddDefaultErrorMessage();
        //    }

        //    return View(modelUpdate);
        //}

        [HttpPost]
        [PreventSpam(Order = 0)]
        [Rp3.Web.Mvc.Authorize("LOTE", "NEW", "AGENDACOMERCIAL", Order = 1)]
        public ActionResult Create(Lote model)
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

                    model.LoteDetalles = new List<LoteDetalle>();

                    SetResumen(model);

                    DataBase.Lotes.Insert(model);
                    DataBase.Save();

                    DataBase.Lote.ProcesarLote(model);
                    DataBase.Save();

                    this.AddDefaultSuccessMessage();
                    return RedirectToAction("Index", model);
                }
            }
            catch
            {
                this.AddDefaultErrorMessage();
            }
            InicializarEdit();
            model.LoteDetalles = new List<LoteDetalle>();
            return View(model);
        }


        [HttpPost]
        [PreventSpam(Order = 0)]
        [Rp3.Web.Mvc.Authorize("LOTE", "EDIT", "AGENDACOMERCIAL", Order = 1)]
        public ActionResult Edit(Lote model)
        {
            Lote modelUpdate = GetModel(model.IdLote);

            try
            {
                if (ModelState.IsValid)
                {
                    modelUpdate.Descripcion = model.Descripcion;
                    modelUpdate.Calificacion = model.Calificacion;

                    modelUpdate.Estado = model.Estado;
                    modelUpdate.UsrMod = this.UserLogonName;
                    modelUpdate.FecMod = this.GetCurrentDateTime();

                    DataBase.LoteTipoClientes.Update(model.LoteTipoClientes, modelUpdate.LoteTipoClientes);
                    DataBase.LoteCanales.Update(model.LoteCanales, modelUpdate.LoteCanales);
                    DataBase.LoteZonas.Update(model.LoteZonas, modelUpdate.LoteZonas);

                    SetResumen(modelUpdate);

                    DataBase.Lotes.Update(modelUpdate);
                    DataBase.Save();

                    DataBase.Lote.ProcesarLote(modelUpdate);

                    VerificarDependencia(modelUpdate);

                    this.AddDefaultSuccessMessage();
                    return RedirectToAction("Index", model);
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
        [Rp3.Web.Mvc.Authorize("LOTE", "DELETE", "AGENDACOMERCIAL")]
        public ActionResult Delete(Lote model)
        {
            try
            {
                Lote modelDelete = GetModel(model.IdLote);
                modelDelete.Estado = Rp3.AgendaComercial.Models.Constantes.Estado.Eliminado;
                modelDelete.UsrMod = this.UserLogonName;
                modelDelete.FecMod = this.GetCurrentDateTime();
                DataBase.Lotes.Update(modelDelete);
                DataBase.Save();

                VerificarDependencia(modelDelete);
                //DataBase.Lotes.Delete(modelDelete);
                //DataBase.Save();

                this.AddDefaultSuccessMessage();
            }
            catch (Exception e)
            {
                this.AddDefaultErrorMessage();
            }
            return RedirectToAction("Index", model);
        }

        public void SetResumen(Lote model)
        {
            string zonas = String.Empty;
            string canales = String.Empty;
            string tipos = String.Empty;

            foreach (var zona in model.LoteZonas)
            {
                var zonaItem = DataBase.Zonas.Get(p => p.IdZona == zona.IdZona).FirstOrDefault();

                if (zonaItem != null)
                {
                    if (!String.IsNullOrEmpty(zonas))
                        zonas += ", ";

                    zonas += zonaItem.Descripcion;
                }
            }

            model.ZonaResumen = zonas;

            foreach (var canal in model.LoteCanales)
            {
                var canalItem = DataBase.Canales.Get(p => p.IdCanal == canal.IdCanal).FirstOrDefault();

                if (canalItem != null)
                {
                    if (!String.IsNullOrEmpty(canales))
                        canales += ", ";

                    canales += canalItem.Descripcion;
                }
            }

            model.CanalResumen = canales;

            foreach (var tipo in model.LoteTipoClientes)
            {
                var tipoItem = DataBase.TipoClientes.Get(p => p.IdTipoCliente == tipo.IdTipoCliente).FirstOrDefault();

                if (tipoItem != null)
                {
                    if (!String.IsNullOrEmpty(tipos))
                        tipos += ", ";

                    tipos += tipoItem.Descripcion;
                }
            }

            model.TipoClienteResumen = tipos;
        }

        public void SetUbicacion(List<LoteDetalle> ubicaciones)
        {
            var clientes = ubicaciones.Select(p => p.IdCliente).ToList<int>();

            var direcciones = DataBase.ClienteDirecciones.Get(p => clientes.Contains(p.IdCliente)).ToList();

            foreach (var item in direcciones)
            {
                var ubicacion = ubicaciones.Where(p => p.IdCliente == item.IdCliente && p.IdClienteDireccion == item.IdClienteDireccion).FirstOrDefault();

                if (ubicacion != null)
                {
                    ubicacion.Latitud = item.Latitud;
                    ubicacion.Longitud = item.Longitud;
                }

            }

            SetMarkers(ubicaciones);
        }
        private void SetMarkers(List<LoteDetalle> ubicaciones)
        {
            int markerIndex = 1;

            foreach (var item in ubicaciones.OrderBy(p => p.ClienteDireccion.Cliente.Apellido1))
            {
                item.MarkerIndex = markerIndex;
                markerIndex++;

                if (markerIndex >= 100)
                    markerIndex = 100;
            }
        }

        [ChildAction(Order = 1)]
        [Rp3.Web.Mvc.Authorize("LOTE", "QUERY", "AGENDACOMERCIAL", Order = 2)]
        [HttpGet]
        public ActionResult GetLoteCliente(LoteParam param)
        {
            string idlote = param.IdLote.ToString();
            LoteConsulta lote = new LoteConsulta();
            var data = DataBase.Lote.GetListadoLoteCliente(param.IdLote, param.Zona, param.TipoCliente, param.Canal, param.Pagina, param.NumReg, param.isbegin, param.Calificacion);
            lote.LoteViews = data.ToList();
            return PartialView("_LoteDetalle", lote);
        }

        [ChildAction(Order = 1)]
        [Rp3.Web.Mvc.Authorize("LOTE", "EDIT", "AGENDACOMERCIAL", Order = 1)]
        [HttpPost]
        public ActionResult UpdateLote(LoteParam param)
        {
            try
            {
                var lote = DataBase.Lotes.Get(p => p.IdLote == param.IdLote).SingleOrDefault();

                lote.Calificacion = param.Calificacion;
                lote.Estado = param.Estado;

                lote.UsrMod = this.UserLogonName;
                lote.FecMod = this.GetCurrentDateTime();


                if (param.Zona != null)
                {
                    lote.LoteZonas.Clear();
                    lote.Zonas = param.Zona;
                }
                if (param.TipoCliente != null)
                {
                    lote.LoteTipoClientes.Clear();
                    lote.TipoClientes = param.TipoCliente;
                }
                if (param.Canal != null)
                {
                    lote.LoteCanales.Clear();
                    lote.Canales = param.Canal;
                }

                DataBase.Lotes.Update(lote);
                DataBase.Save();

                VerificarDependencia(lote);

                this.AddDefaultSuccessMessage();
            }
            catch (Exception e)
            {
                this.AddDefaultErrorMessage();
            }
            return Json();
        }

        private void VerificarDependencia(Lote lote)
        {
            if (lote.Estado == Rp3.AgendaComercial.Models.Constantes.Estado.Inactivo ||
                lote.Estado == Rp3.AgendaComercial.Models.Constantes.Estado.Eliminado)
            {
                DataBase.Lotes.EliminarDependenciaLote(lote.IdLote, this.UserLogonName);
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

            settings.Name = "Export";
            settings.Width = Unit.Percentage(100);

            settings.Columns.Add("Descripcion", Rp3.AgendaComercial.Resources.LabelFor.Descripcion);
            settings.Columns.Add("CantidadClientes", Rp3.AgendaComercial.Resources.LabelFor.Clientes);

            settings.Columns.Add("CanalResumen", Rp3.AgendaComercial.Resources.LabelFor.Canales);
            settings.Columns.Add("TipoClienteResumen", Rp3.AgendaComercial.Resources.LabelFor.TipoClientes);
            settings.Columns.Add("ZonaResumen", Rp3.AgendaComercial.Resources.LabelFor.Zonas);
            settings.Columns.Add("EstadoGeneralValue.Content", Rp3.AgendaComercial.Resources.LabelFor.Estado);

            settings.SettingsExport.Landscape = true;
            settings.SettingsExport.ReportHeader = settings.Name;          

            return settings;
        }

        #endregion Export

    }
}

