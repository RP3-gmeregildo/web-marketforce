using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Rp3.Data.Models.Security;
using Rp3.Web.Mvc.Application.Security.Models;
using Rp3.Web.Mvc;
using Rp3.AgendaComercial.Models.General;
using Rp3.AgendaComercial.Models.General.View;
using Rp3.AgendaComercial.Web.Areas.General.Models;
using DevExpress.Web.Mvc;
using System.Web.UI.WebControls;

namespace Rp3.AgendaComercial.Web.General.Controllers
{
    public class ZonaController : Rp3.Web.Mvc.Controllers.BaseController<Models.ContextService>
    {
        //
        // GET: /General/Zona/
        ZonaModel zonaModel;

        [Rp3.Web.Mvc.Authorize("ZONA", "QUERY", "AGENDACOMERCIAL")]
        public ActionResult Index()
        {
            return View(GetListIndex());
        }

        public ActionResult GridViewIndex()
        {
            return PartialView("_GridViewIndex", GetListIndex());
        }

        public List<Zona> GetListIndex()
        {
            return DataBase.Zonas.Get(p=>p.Estado != Rp3.AgendaComercial.Models.Constantes.Estado.Eliminado, includeProperties: "EstadoGeneralValue,Region").ToList();
        }

        private void InicializarEdit()
        {
            ViewBag.RegionesSelectList = DataBase.Regiones.Get(p => p.Estado == Models.Constantes.Estado.Activo).ToSelectList(includeNullItem: true);
            //ViewBag.Ciudades = DataBase.GeopoliticalStructures.Get(p => p.GeopoliticalStructureTypeId == Models.Constantes.GeopoliticalStructureType.Ciudad).ToList();
            ViewBag.Paises = DataBase.GeopoliticalStructures.Get(p => p.GeopoliticalStructureTypeId == Models.Constantes.GeopoliticalStructureType.Pais).ToSelectList(includeNullItem:true);
            //InicializarGeopoliticalStructure();
        }

        private void InicializarGeopoliticalStructure()
        {
            ViewBag.ProvinciaSelectList = DataBase.GeopoliticalStructures.Get(p => p.GeopoliticalStructureTypeId == Models.Constantes.GeopoliticalStructureType.Provincia).ToSelectList();
        }

        [Rp3.Web.Mvc.Authorize("ZONA", "NEW", "AGENDACOMERCIAL")]
        public ActionResult Create()
        {
            var model = new ZonaNew();

            ViewBag.TiposSelectList = DataBase.GeneralValues.Get(p => p.Id == Rp3.AgendaComercial.Models.Constantes.TipoZona.Tabla).ToSelectList();
            ViewBag.RegionesSelectList = DataBase.Regiones.Get(p => p.Estado != Rp3.AgendaComercial.Models.Constantes.Estado.Eliminado).ToSelectList();

            return PartialView("_Create", model);

            //zonaModel = new ZonaModel();
            //zonaModel.Children = new List<ZonaGroup>();
            //InicializarEdit();
            //zonaModel.Estado = Models.Constantes.Estado.Activo;
            //zonaModel.ubicacion = new Ubicacion();
            //zonaModel.ubicacion.Latitud = 0;
            //zonaModel.ubicacion.Longitud = 0;

            //ViewBag.ReadOnly = false;
            //return View(zonaModel);
        }

        private Zona GetModel(int id)
        {
            Zona result = DataBase.Zonas.Get(p => p.IdZona == id, includeProperties: "ZonaDetalles.GeopoliticalStructure, EstadoGeneralValue, Region, ZonaGeocercas, ZonaClienteGeocercas").SingleOrDefault();

            return result;
        }

        [Rp3.Web.Mvc.Authorize("ZONA", "EDIT", "AGENDACOMERCIAL")]
        public ActionResult Edit(int id)
        {
            InicializarEdit();
            zonaModel = new ZonaModel();
            zonaModel.Children = new List<ZonaGroup>();
            var zona = GetModel(id);
            zonaModel.Estado = zona.Estado;
            zonaModel.Name = zona.Descripcion;
            zonaModel.Id = zona.IdZona;
            zonaModel.IdRegion = zona.IdRegion;
            zonaModel.Movilizacion = zona.TiempoMovilizacion + "";

            zonaModel.Tipo = zona.Tipo;
            zonaModel.TipoGeneralValue = zona.TipoGeneralValue;

            List<ZonaView> listZonaView = new List<ZonaView>();
            zonaModel.ubicacion = new Ubicacion();
            zonaModel.ubicacion.Latitud = zona.LatitudPuntoPartida == null ? 0 : zona.LatitudPuntoPartida;
            zonaModel.ubicacion.Longitud = zona.LongitudPuntoPartida == null ? 0 : zona.LongitudPuntoPartida;
            zonaModel.ubicacion.Titulo = "Punto de Partida";

            zonaModel.ZonaGeocercas = zona.ZonaGeocercas;
            zonaModel.ZonaClienteGeocercas = zona.ZonaClienteGeocercas;
            zonaModel.ZonaOther = DataBase.Zonas.Get(p => p.IdZona != id && p.Estado != Models.Constantes.Estado.Eliminado && p.Tipo == Models.Constantes.TipoZona.Geocerca, includeProperties: "ZonaClienteGeocercas").ToList();

            var parents = zona.ZonaDetalles.Where(p => p.GeopoliticalStructure.ParentGeopoliticalStructureId.HasValue)
                .Select(p => p.GeopoliticalStructure.ParentGeopoliticalStructureId.Value).Distinct().ToArray();

            var detalles = zona.ZonaDetalles.Select(p=>p.IdGeopoliticalStructure).ToArray();

            string idsParents = "";
            foreach(var parentId in parents)
            {
                ZonaView setter = new ZonaView();
                var parent = DataBase.GeopoliticalStructures.Get(p => p.GeopoliticalStructureId == parentId).FirstOrDefault();
                setter.Id = parentId;
                idsParents = idsParents + parentId + ",";
                setter.label = parent.DescriptionName;
                setter.ParentName = parent.DescriptionName;
                setter.lista = DataBase.GeopoliticalStructures.Get(p => p.ParentGeopoliticalStructureId == parentId).
                    ToMultiSelectList(detalles, "GeopoliticalStructureId", "Name");
                listZonaView.Add(setter);
            }
            ViewBag.ChosenZones = listZonaView;
            ViewBag.Parents = idsParents;
            ViewBag.ReadOnly = false;

            return View(zonaModel);
        }

        [Rp3.Web.Mvc.Authorize("ZONA", "DETAIL", "AGENDACOMERCIAL")]
        public ActionResult Detail(int id)
        {
            InicializarEdit();
            zonaModel = new ZonaModel();
            zonaModel.Children = new List<ZonaGroup>();
            var zona = GetModel(id);
            zonaModel.Estado = zona.Estado;
            zonaModel.Name = zona.Descripcion;
            zonaModel.Id = zona.IdZona;
            zonaModel.IdRegion = zona.IdRegion;
            zonaModel.Movilizacion = zona.TiempoMovilizacion + "";

            zonaModel.Tipo = zona.Tipo;
            zonaModel.TipoGeneralValue = zona.TipoGeneralValue;

            zonaModel.ZonaGeocercas = zona.ZonaGeocercas;
            zonaModel.ZonaClienteGeocercas = zona.ZonaClienteGeocercas;
            zonaModel.ZonaOther = DataBase.Zonas.Get(p => p.IdZona != id && p.Estado != Models.Constantes.Estado.Eliminado && p.Tipo == Models.Constantes.TipoZona.Geocerca, includeProperties: "ZonaClienteGeocercas").ToList();

            List<ZonaView> listZonaView = new List<ZonaView>();
            zonaModel.ubicacion = new Ubicacion();
            zonaModel.ubicacion.Latitud = zona.LatitudPuntoPartida == null ? 0 : zona.LatitudPuntoPartida;
            zonaModel.ubicacion.Longitud = zona.LongitudPuntoPartida == null ? 0 : zona.LongitudPuntoPartida;
            zonaModel.ubicacion.Titulo = "Punto de Partida";
            zonaModel.EstadoGeneralValue = zona.EstadoGeneralValue;
            zonaModel.Region = zona.Region;
            zonaModel.MovilizacionGeneralValue = DataBase.GeneralValues.Get(p => p.Id == Rp3.AgendaComercial.Models.Constantes.Duracion.Tabla && p.Code == zona.TiempoMovilizacion + "").FirstOrDefault();

            var parents = zona.ZonaDetalles.Where(p => p.GeopoliticalStructure.ParentGeopoliticalStructureId.HasValue)
                .Select(p => p.GeopoliticalStructure.ParentGeopoliticalStructureId.Value).Distinct().ToArray();

            var detalles = zona.ZonaDetalles.Select(p => p.IdGeopoliticalStructure).ToArray();

            string idsParents = "";
            foreach (var parentId in parents)
            {
                ZonaView setter = new ZonaView();
                var parent = DataBase.GeopoliticalStructures.Get(p => p.GeopoliticalStructureId == parentId).FirstOrDefault();
                setter.Id = parentId;
                idsParents = idsParents + parentId + ",";
                setter.label = parent.DescriptionName;
                setter.ParentName = parent.DescriptionName;
                setter.lista = DataBase.GeopoliticalStructures.Get(p => p.ParentGeopoliticalStructureId == parentId).
                    ToMultiSelectList(detalles, "GeopoliticalStructureId", "Name");
                listZonaView.Add(setter);
            }
            ViewBag.ChosenZones = listZonaView;
            ViewBag.Parents = idsParents;
            ViewBag.ReadOnly = true;

            return View(zonaModel);
        }

        [Rp3.Web.Mvc.Authorize("ZONA", "DELETE", "AGENDACOMERCIAL")]
        public ActionResult Delete(int id)
        {
            //var model = GetModel(id);
            //model.ReadOnly = true;
            
            //ViewBag.Ciudades = model.ZonaDetalles.Select(p => p.GeopoliticalStructure).Distinct().ToList();
            //InicializarGeopoliticalStructure();

            InicializarEdit();
            zonaModel = new ZonaModel();
            zonaModel.Children = new List<ZonaGroup>();
            var zona = GetModel(id);
            zonaModel.Estado = zona.Estado;
            zonaModel.Name = zona.Descripcion;
            zonaModel.Id = zona.IdZona;
            zonaModel.IdRegion = zona.IdRegion;
            zonaModel.Movilizacion = zona.TiempoMovilizacion + "";

            zonaModel.Tipo = zona.Tipo;
            zonaModel.TipoGeneralValue = zona.TipoGeneralValue;

            zonaModel.ZonaGeocercas = zona.ZonaGeocercas;
            zonaModel.ZonaClienteGeocercas = zona.ZonaClienteGeocercas;
            zonaModel.ZonaOther = DataBase.Zonas.Get(p => p.IdZona != id && p.Estado != Models.Constantes.Estado.Eliminado && p.Tipo == Models.Constantes.TipoZona.Geocerca, includeProperties: "ZonaClienteGeocercas").ToList();

            List<ZonaView> listZonaView = new List<ZonaView>();
            zonaModel.ubicacion = new Ubicacion();
            zonaModel.ubicacion.Latitud = zona.LatitudPuntoPartida == null ? 0 : zona.LatitudPuntoPartida;
            zonaModel.ubicacion.Longitud = zona.LongitudPuntoPartida == null ? 0 : zona.LongitudPuntoPartida;
            zonaModel.ubicacion.Titulo = "Punto de Partida";
            zonaModel.EstadoGeneralValue = zona.EstadoGeneralValue;
            zonaModel.Region = zona.Region;
            zonaModel.MovilizacionGeneralValue = DataBase.GeneralValues.Get(p => p.Id == Rp3.AgendaComercial.Models.Constantes.Duracion.Tabla && p.Code == zona.TiempoMovilizacion + "").FirstOrDefault();

            var parents = zona.ZonaDetalles.Where(p => p.GeopoliticalStructure.ParentGeopoliticalStructureId.HasValue)
                .Select(p => p.GeopoliticalStructure.ParentGeopoliticalStructureId.Value).Distinct().ToArray();

            var detalles = zona.ZonaDetalles.Select(p => p.IdGeopoliticalStructure).ToArray();

            string idsParents = "";
            foreach (var parentId in parents)
            {
                ZonaView setter = new ZonaView();
                var parent = DataBase.GeopoliticalStructures.Get(p => p.GeopoliticalStructureId == parentId).FirstOrDefault();
                setter.Id = parentId;
                idsParents = idsParents + parentId + ",";
                setter.label = parent.DescriptionName;
                setter.ParentName = parent.DescriptionName;
                setter.lista = DataBase.GeopoliticalStructures.Get(p => p.ParentGeopoliticalStructureId == parentId).
                    ToMultiSelectList(detalles, "GeopoliticalStructureId", "Name");
                listZonaView.Add(setter);
            }
            ViewBag.ChosenZones = listZonaView;
            ViewBag.Parents = idsParents;
            ViewBag.ReadOnly = true;

            return View(zonaModel);
        }

        [HttpPost]
        [PreventSpam(Order = 0)]
        [Rp3.Web.Mvc.Authorize("ZONA", "NEW", "AGENDACOMERCIAL", Order = 1)]
        public ActionResult Create(ZonaNew model)
        {
            try
            {
                Zona zona = new Zona();

                zona.AsignarId();

                zona.Descripcion = model.Descripcion;
                zona.IdRegion = model.IdRegion;

                zona.TipoTabla = Rp3.AgendaComercial.Models.Constantes.TipoZona.Tabla;
                zona.Tipo = model.Tipo;

                zona.EstadoTabla = Rp3.AgendaComercial.Models.Constantes.Estado.Tabla;
                zona.Estado = Rp3.AgendaComercial.Models.Constantes.Estado.Activo;
                zona.UsrIng = this.UserLogonName;
                zona.FecIng = this.GetCurrentDateTime();
                zona.FecMod = this.GetCurrentDateTime();

                DataBase.Zonas.Insert(zona);
                DataBase.Save();

                this.AddDefaultSuccessMessage();
                //return Json();

                return new JsonResult() { Data = new { IdZona = zona.IdZona }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            catch
            {
                this.AddDefaultErrorMessage();
                return Json();
            }

            //Zona modelToSave = new Zona();
            //List<string> ciudades = new List<string>();
            //try
            //{
            //    if (ModelState.IsValid)
            //    {
            //        modelToSave.AsignarId();
            //        modelToSave.Estado = Rp3.AgendaComercial.Models.Constantes.Estado.Activo;
            //        modelToSave.EstadoTabla = Rp3.AgendaComercial.Models.Constantes.Estado.Tabla;
            //        modelToSave.UsrIng = this.UserLogonName;
            //        modelToSave.FecIng = this.GetCurrentDateTime();
            //        modelToSave.Estado = model.Estado;
            //        modelToSave.Descripcion = model.Name;
            //        modelToSave.IdRegion = model.IdRegion;
            //        if (Request["latitudZona"] != null)
            //            modelToSave.LatitudPuntoPartida = double.Parse(Request["latitudZona"].ToString());
            //        if (Request["longitudZona"] != null)
            //            modelToSave.LongitudPuntoPartida = double.Parse(Request["longitudZona"].ToString());
            //        switch(model.Movilizacion)
            //        {
            //            case Rp3.AgendaComercial.Models.Constantes.Duracion._15min: modelToSave.TiempoMovilizacion = 15; break;
            //            case Rp3.AgendaComercial.Models.Constantes.Duracion._30min: modelToSave.TiempoMovilizacion = 30; break;
            //            default: modelToSave.TiempoMovilizacion = 0; break;
            //        }
            //        var idPadres = Request["groupsIds"].ToString().Split(',');
            //        foreach(string idPadre in idPadres)
            //        {
            //            var control = Request[idPadre + "Select"];
            //            if(control != null)
            //            {
            //                var ids = control.ToString().Split(',');
            //                ciudades.AddRange(ids);
            //            }
            //        }

            //        ciudades = ciudades.Distinct().ToList();
            //        modelToSave.ZonaDetalles = GetListDetail(modelToSave.IdZona, ciudades.ToArray()); 

            //        DataBase.Zonas.Insert(modelToSave);
            //        DataBase.Save();

            //        this.AddDefaultSuccessMessage();
            //        return RedirectToAction("Index", model);
            //    }
            //}
            //catch
            //{
            //    this.AddDefaultErrorMessage();
            //}

            //InicializarEdit();
            //modelToSave.ZonaDetalles = GetListDetail(modelToSave.IdZona, ciudades.ToArray()); 
            //return View(model);            
        }

        [HttpPost]
        [PreventSpam(Order = 0)]
        [Rp3.Web.Mvc.Authorize("ZONA", "EDIT", "AGENDACOMERCIAL", Order = 1)]
        public ActionResult Edit(ZonaModel model, string[] geogroup, bool process)
        {
            Zona modelToSave = DataBase.Zonas.Get(p => p.IdZona == model.Id, includeProperties: "ZonaDetalles").FirstOrDefault();
            List<string> ciudades = new List<string>();
            try
            {
                if (ModelState.IsValid)
                {
                    modelToSave.UsrMod = this.UserLogonName;
                    modelToSave.FecMod = this.GetCurrentDateTime();
                    modelToSave.Estado = model.Estado;
                    modelToSave.Descripcion = model.Name;
                    modelToSave.IdRegion = model.IdRegion;

                    if (Request["latitudZona"] != null)
                        modelToSave.LatitudPuntoPartida = double.Parse(Request["latitudZona"].ToString());
                    if (Request["longitudZona"] != null)
                        modelToSave.LongitudPuntoPartida = double.Parse(Request["longitudZona"].ToString());

                    switch (model.Movilizacion)
                    {
                        case Rp3.AgendaComercial.Models.Constantes.Duracion._15min: modelToSave.TiempoMovilizacion = 15; break;
                        case Rp3.AgendaComercial.Models.Constantes.Duracion._30min: modelToSave.TiempoMovilizacion = 30; break;
                        default: modelToSave.TiempoMovilizacion = 0; break;
                    }

                    if (modelToSave.Tipo == Rp3.AgendaComercial.Models.Constantes.TipoZona.EstructuraGeopolitica)
                    {
                        if (Request["groupsIds"] != null)
                        {
                            var idPadres = Request["groupsIds"].ToString().Split(',');
                           
                            foreach (string idPadre in idPadres)
                            {
                                var control = Request[idPadre + "Select"];
                                if (control != null)
                                {
                                    var ids = control.ToString().Split(',');
                                    ciudades.AddRange(ids);
                                }
                            }

                            ciudades = ciudades.Distinct().ToList();
                            var listEdit = GetListDetail(modelToSave.IdZona, ciudades.ToArray());
                            DataBase.ZonaDetalles.Update(listEdit, modelToSave.ZonaDetalles);

                            modelToSave.ZonaDetalles = GetListDetail(modelToSave.IdZona, ciudades.ToArray());
                        }
                    }
                    else 
                    {
                        var listGeo = new List<ZonaGeocerca>();

                        if (geogroup != null)
                        {
                            int index = 1;
                            int iGroup = 1;

                            foreach(var group in geogroup)
                            {
                                var puntos = group.Split('|');
                                int posicion = 1;

                                foreach (string punto in puntos)
                                {
                                    var pto = punto.Replace("(", "").Replace(")", "");

                                    var p = pto.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);

                                    listGeo.Add(new ZonaGeocerca()
                                    {
                                        IdZona = modelToSave.IdZona,
                                        IdZonaGeocerca = index,
                                        IdZonaGrupoGeocerca = iGroup,
                                        Posicion = posicion,
                                        Latitud = Convert.ToDouble(p[0]),
                                        Longitud = Convert.ToDouble(p[1])
                                    });

                                    index++;
                                    posicion++;
                                }

                                iGroup++;
                            }
                            
                        }

                        DataBase.ZonaGeocercas.Update(listGeo, modelToSave.ZonaGeocercas);
                    }

                    DataBase.Zonas.Update(modelToSave);
                    DataBase.Save();

                    VerificarDependencia(modelToSave);

                    if (process)
                    {
                        try
                        {
                            DataBase.Zonas.Procesar(modelToSave.IdZona);
                        }
                        catch
                        {
                        }

                        this.AddDefaultSuccessMessage();
                        return RedirectToAction("Edit", new { id = modelToSave.IdZona });
                    }

                    this.AddDefaultSuccessMessage();
                    return RedirectToAction("Index", model);
                }
            }
            catch
            {
                this.AddDefaultErrorMessage();
            }

            InicializarEdit();
            //modelToSave.ZonaDetalles = GetListDetail(modelToSave.IdZona, ciudades.ToArray());

            model.ubicacion = new Ubicacion();
            model.ubicacion.Latitud = modelToSave.LatitudPuntoPartida == null ? 0 : modelToSave.LatitudPuntoPartida;
            model.ubicacion.Longitud = modelToSave.LongitudPuntoPartida == null ? 0 : modelToSave.LongitudPuntoPartida;
            model.ubicacion.Titulo = "Punto de Partida";

            return View(model);
        }

        public List<ZonaDetalle> GetListDetail(int IdZona, string[] ciudades)
        {
            List<ZonaDetalle> listEdit = new List<ZonaDetalle>();

            if (ciudades != null)
            {
                foreach (var insert in ciudades.Where(p => p != "false"))
                {
                    string[] keyParts = insert.Split('-');

                    ZonaDetalle zonaDetalle = new ZonaDetalle()
                    {
                        IdZona = IdZona,
                        IdGeopoliticalStructure = Convert.ToInt32(keyParts[0])
                    };

                    listEdit.Add(zonaDetalle);
                }
            }

            return listEdit;
        }

        [HttpPost]
        public ActionResult GetNextLevel(int id)
        {
            var thisLevel = DataBase.GeopoliticalStructures.Get(p => p.GeopoliticalStructureId == id).FirstOrDefault();
            var nextLevel = DataBase.GeopoliticalStructures.Get(p => p.ParentGeopoliticalStructureId == id).ToSelectList(null, "GeopoliticalStructureId", "Name", false);
            var nextLevelType = DataBase.GeopoliticalStructures.Get(p => p.ParentGeopoliticalStructureId == id, includeProperties: "GeopoliticalStructureType").FirstOrDefault();
            ZonaView zonaView = new ZonaView();
            zonaView.TypeId = nextLevelType.GeopoliticalStructureType.LevelStructure;
            zonaView.lista = nextLevel;
            zonaView.ParentName = thisLevel.DescriptionName;
            zonaView.Id = thisLevel.GeopoliticalStructureId;
            int firstId = int.Parse(nextLevel.FirstOrDefault().Value);
            if (DataBase.GeopoliticalStructures.Get(p => p.ParentGeopoliticalStructureId == firstId).ToList().Count == 0)
                zonaView.EsUltimo = true;
            else
                zonaView.EsUltimo = false;
            zonaView.label = nextLevelType.GeopoliticalStructureType.Name;
            var jsonLevels = Json(zonaView);
            return jsonLevels;
        }

        public ActionResult SetUbicacion(int markerIndex, double? latitud, double? longitud)
        {
            if (longitud == null || longitud == -1)
                longitud = Ubicacion.DefaultLongitud;

            if (latitud == null || latitud == -1)
                latitud = Ubicacion.DefaultLatitud;

            Ubicacion model = new Ubicacion() { MarkerIndex = markerIndex, Longitud = longitud, Latitud = latitud };

            return PartialView("_SetUbicacion", model);
        }
        [HttpPost]
        public void AgregarZona(ZonaGroup zona)
        {
            int idChild = zona.Lista[0];
            var child = DataBase.GeopoliticalStructures.Get(p => p.GeopoliticalStructureId == idChild).FirstOrDefault();
            zona.IdParent = child.ParentGeopoliticalStructureId;
            zona.ParentsName = child.ParentGeopoliticalStructure.DescriptionName;
            zonaModel.Children.Add(zona);
        }

        [HttpPost]
        [PreventSpam(Order = 0)]
        [Rp3.Web.Mvc.Authorize("ZONA", "DELETE", "AGENDACOMERCIAL")]
        public ActionResult Delete(ZonaModel model)
        {
            try
            {
                //Zona modelUpdate = GetModel(model.Id);
                Zona modelUpdate = DataBase.Zonas.Get(p => p.IdZona == model.Id).SingleOrDefault();
                modelUpdate.Estado = Rp3.AgendaComercial.Models.Constantes.Estado.Eliminado;
                modelUpdate.UsrMod = this.UserLogonName;
                modelUpdate.FecMod = this.GetCurrentDateTime();

                DataBase.Zonas.Update(modelUpdate);
                DataBase.Save();
                //Zona modelDelete = GetModel(model.IdZona);
                //DataBase.Zonas.Delete(modelDelete);
                //DataBase.Save();

                VerificarDependencia(modelUpdate);

                this.AddDefaultSuccessMessage();
                return RedirectToAction("Index", model);
            }
            catch
            {
                this.AddDefaultErrorMessage();
            }
            return RedirectToAction("Index", model);
            //return RedirectToAction("Index", null, null, true);
        }

        private void VerificarDependencia(Zona zona)
        {
            if (zona.Estado == Rp3.AgendaComercial.Models.Constantes.Estado.Inactivo ||
                zona.Estado == Rp3.AgendaComercial.Models.Constantes.Estado.Eliminado)
            {
                DataBase.Zonas.EliminarDependenciaZona(zona.IdZona, this.UserLogonName);
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

            settings.Columns.Add("Descripcion");
            settings.Columns.Add("Region.Descripcion", Rp3.AgendaComercial.Resources.LabelFor.Region);
            settings.Columns.Add("EstadoGeneralValue.Content", Rp3.AgendaComercial.Resources.LabelFor.Estado);

            settings.SettingsExport.Landscape = true;
            settings.SettingsExport.ReportHeader = settings.Name;          

            return settings;
        }

        #endregion Export
    }
}
