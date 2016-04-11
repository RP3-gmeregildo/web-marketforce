using Rp3.AgendaComercial.Models.Oportunidad;
using Rp3.AgendaComercial.Models.Oportunidad.View;
using Rp3.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Rp3.AgendaComercial.Web.Oportunidad.Controllers
{
    public class EtapaController : AgendaComercial.Web.Controllers.AgendaComercialController
    {
        // GET: Oportunidad/Etapa
        [Rp3.Web.Mvc.Authorize("ETAPA", "QUERY", "AGENDACOMERCIAL")]
        public ActionResult Index()
        {
            return View(GetListIndex());
        }

        [Rp3.Web.Mvc.Authorize("ETAPA", "EDIT", "AGENDACOMERCIAL")]
        public ActionResult Edit(int id)
        {
            ViewBag.ReadOnly = false;
            ViewBag.IdOportunidadTipo = id;
            return View(GetListIndexEtapa(id));
        }

        [Rp3.Web.Mvc.Authorize("ETAPA", "DETAIL", "AGENDACOMERCIAL")]
        public ActionResult Detail(int id)
        {
            ViewBag.ReadOnly = true;
            return View(GetListIndexEtapa(id));
        }

        [Rp3.Web.Mvc.Authorize("ETAPA", "NEW", "AGENDACOMERCIAL")]
        public ActionResult Create()
        {
            ViewBag.ReadOnly = false;
            var opTipo = new OportunidadTipo();
            opTipo.AsignarId();
            ViewBag.IdOportunidadTipo = opTipo.IdOportunidadTipo;
            return View(new EtapaSave() { Etapas = new List<EtapaView>() , OportunidadTipo = opTipo});
        }

        private EtapaSave GetListIndexEtapa(int id)
        {
            var list = DataBase.Etapas.Get(p => p.Estado != Rp3.AgendaComercial.Models.Constantes.Estado.Eliminado && p.IdOportunidadTipo == id).ToList();
            var tareas = DataBase.EtapaTareas.Get(p => p.Etapa.Estado != Rp3.AgendaComercial.Models.Constantes.Estado.Eliminado).ToList();

            var result = new List<EtapaView>();

            foreach (var etapa in list.Where(p => p.IdEtapaPadre == null))
            {
                var item = new EtapaView()
                {
                    IdEtapa = etapa.IdEtapa,
                    Descripcion = etapa.Descripcion,
                    Orden = etapa.Orden,
                    IdEtapaPadre = etapa.IdEtapaPadre,
                    Dias = etapa.Dias,
                    IdOportunidadTipo = etapa.IdOportunidadTipo,
                    EsVariable = etapa.EsVariable
                };

                item.Tareas = new List<EtapaTareaView>();

                var tareasItem = tareas.Where(p => p.IdEtapa == item.IdEtapa).ToList();

                foreach (var tarea in tareasItem)
                    item.Tareas.Add(new EtapaTareaView() { IdTarea = tarea.IdTarea, Descripcion = tarea.Tarea.Descripcion, Orden = tarea.Orden });

                var subetapas = list.Where(p => p.IdEtapaPadre == etapa.IdEtapa).ToList();

                var resultSubEtapa = new List<EtapaView>();

                foreach (var subetapa in subetapas)
                {
                    var sub = new EtapaView()
                    {
                        IdEtapa = subetapa.IdEtapa,
                        Descripcion = subetapa.Descripcion,
                        Orden = subetapa.Orden,
                        IdEtapaPadre = subetapa.IdEtapaPadre,
                        Dias = etapa.Dias,
                        IdOportunidadTipo = etapa.IdOportunidadTipo,
                        EsVariable = etapa.EsVariable
                    };

                    sub.Tareas = new List<EtapaTareaView>();
                    var subtareasItem = tareas.Where(p => p.IdEtapa == sub.IdEtapa).ToList();

                    foreach (var tarea in subtareasItem)
                        sub.Tareas.Add(new EtapaTareaView() { IdTarea = tarea.IdTarea, Descripcion = tarea.Tarea.Descripcion, Orden = tarea.Orden });

                    resultSubEtapa.Add(sub);
                }

                item.SubEtapas = resultSubEtapa;

                result.Add(item);
            }
            var tipo = DataBase.OportunidadTipos.GetSingleOrDefault(p => p.IdOportunidadTipo == id);

            return new EtapaSave() { Etapas = result, OportunidadTipo = tipo };
        }

        private List<OportunidadTipo> GetListIndex()
        {
            return DataBase.OportunidadTipos.Get(p => p.Estado != Rp3.AgendaComercial.Models.Constantes.Estado.Eliminado).ToList();
        }

        [HttpGet]
        public ActionResult GetSeleccionTarea()
        {
            var model = DataBase.Tareas.Get(p => p.Estado == Rp3.AgendaComercial.Models.Constantes.Estado.Activo && p.TipoTarea == Rp3.AgendaComercial.Models.Constantes.TipoTarea.CheckListOportunidad).ToList();

            return PartialView("_SeleccionTarea", model);
        }

        [ChildAction(Order = 1)]
        [Rp3.Web.Mvc.Authorize("ETAPA", "EDIT", "AGENDACOMERCIAL", Order = 1)]
        [HttpPost]
        public ActionResult UpdateEtapa(List<EtapaView> etapaView, OportunidadTipo optTipo)
        {
            try
            {
                var etapa_last = new Etapa();
                etapa_last.AsignarId();
                int id_etapa = etapa_last.IdEtapa;
                int id_padre = 0;
                foreach(EtapaView view in etapaView)
                {
                    if (view.Nuevo)
                    {
                        view.IdEtapa = id_etapa;
                        id_etapa++;
                    }
                    if (view.Tareas != null)
                    {
                        foreach (EtapaTareaView tareaView in view.Tareas)
                        {
                            tareaView.IdEtapa = view.IdEtapa;
                        }
                    }
                    if (view.IdEtapaPadre != null)
                    {
                        view.IdEtapaPadre = id_padre;
                    }
                    else
                    {
                        id_padre = view.IdEtapa;
                    }
                }
                if (DataBase.OportunidadTipos.Exists(p => p.IdOportunidadTipo == optTipo.IdOportunidadTipo))
                {
                    var optTipoUpdate = DataBase.OportunidadTipos.GetSingleOrDefault(p => p.IdOportunidadTipo == optTipo.IdOportunidadTipo);
                    optTipoUpdate.Descripcion = optTipo.Descripcion;
                    optTipoUpdate.Estado = optTipo.Estado;
                    optTipoUpdate.UsrMod = this.UserLogonName;
                    optTipoUpdate.FecMod = GetCurrentDateTime();
                    DataBase.OportunidadTipos.Update(optTipoUpdate);
                }
                else
                {
                    optTipo.EstadoTabla = Rp3.AgendaComercial.Models.Constantes.Estado.Tabla;
                    optTipo.UsrIng = this.UserLogonName;
                    optTipo.FecIng = GetCurrentDateTime();
                    optTipo.UsrMod = this.UserLogonName;
                    optTipo.FecMod = GetCurrentDateTime();
                    DataBase.OportunidadTipos.Insert(optTipo);
                }
                DataBase.Save();
                DataBase.Etapas.UpdateXml(new EtapaSave() { Etapas = etapaView }, this.UserLogonName);

                this.AddDefaultSuccessMessage();
            }
            catch
            {
                this.AddDefaultErrorMessage();
            }

            return Json();
        }
    }
}