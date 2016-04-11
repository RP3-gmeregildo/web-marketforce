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
using Rp3.Data;

namespace Rp3.AgendaComercial.Web.General.Controllers
{
    public class TipoActividadController : Rp3.Web.Mvc.Controllers.BaseController<Models.ContextService>
    {
        //
        // GET: /General/TipoActividad/

        [Rp3.Web.Mvc.Authorize("TIPOACTIVIDAD", "QUERY", "AGENDACOMERCIAL")]
        public ActionResult Index()
        {
            return View(GetListIndex());
        }

        public ActionResult GridViewIndex()
        {
            return PartialView("_GridViewIndex", GetListIndex());
        }

        public List<TipoActividad> GetListIndex()
        {
            return DataBase.TipoActividades.Get(p => p.IdTipoActividad > 5, includeProperties: "EstadoGeneralValue, TipoGeneralValue").ToList();
        }

        private TipoActividad GetModel(int id)
        {
            TipoActividad result = DataBase.TipoActividades.Get(p => p.IdTipoActividad == id, includeProperties: "TipoActividadOpciones").SingleOrDefault();

            return result;
        }

        [Rp3.Web.Mvc.Authorize("TIPOACTIVIDAD", "NEW", "AGENDACOMERCIAL")]
        public ActionResult Create()
        {
            TipoActividad model = new TipoActividad();
            model.Estado = Models.Constantes.Estado.Activo;
            model.Tipo = Models.Constantes.TipoActividad.Seleccion;
            model.TipoActividadOpciones = new List<TipoActividadOpcion>();

            return View(model);
        }

        [Rp3.Web.Mvc.Authorize("TIPOACTIVIDAD", "EDIT", "AGENDACOMERCIAL")]
        public ActionResult Edit(int id)
        {
            var model = GetModel(id);

            return View(model);
        }

        [Rp3.Web.Mvc.Authorize("TIPOACTIVIDAD", "DETAIL", "AGENDACOMERCIAL")]
        public ActionResult Detail(int id)
        {
            var model = GetModel(id);
            model.ReadOnly = true;

            return View(model);
        }

        [Rp3.Web.Mvc.Authorize("TIPOACTIVIDAD", "DELETE", "AGENDACOMERCIAL")]
        public ActionResult Delete(int id)
        {
            var model = GetModel(id);
            model.ReadOnly = true;

            return View(model);
        }

        [HttpPost]
        [PreventSpam(Order = 0)]
        [Rp3.Web.Mvc.Authorize("TIPOACTIVIDAD", "NEW", "AGENDACOMERCIAL", Order = 1)]
        public ActionResult Create(TipoActividad model, string[] opciones)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.AsignarId();
                    model.Estado = Rp3.AgendaComercial.Models.Constantes.Estado.Activo;
                    model.EstadoTabla = Models.Constantes.Estado.Tabla;
                    model.TipoTabla = Models.Constantes.TipoActividad.Tabla;
                    model.UsrIng = this.UserLogonName;
                    model.FecIng = this.GetCurrentDateTime();

                    var listOpcion = new List<TipoActividadOpcion>();

                    model.TipoActividadOpciones = GetListOpcion(model.IdTipoActividad, model.Tipo, opciones);

                    DataBase.TipoActividades.Insert(model);
                    DataBase.Save();

                    return RedirectToAction("Index");
                }
            }
            catch
            {
                this.AddDefaultErrorMessage();
            }

            model.TipoActividadOpciones = GetListOpcion(model.IdTipoActividad, model.Tipo, opciones);
            return View(model);
        }

        [HttpPost]
        [PreventSpam(Order = 0)]
        [Rp3.Web.Mvc.Authorize("TIPOACTIVIDAD", "EDIT", "AGENDACOMERCIAL", Order = 1)]
        public ActionResult Edit(TipoActividad model, string[] opciones)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    TipoActividad modelUpdate = GetModel(model.IdTipoActividad);

                    modelUpdate.Descripcion = model.Descripcion;
                    modelUpdate.Tipo = model.Tipo;
                    modelUpdate.Estado = model.Estado;

                    modelUpdate.UsrMod = this.UserLogonName;
                    modelUpdate.FecMod = this.GetCurrentDateTime();

                   var listOpcion = GetListOpcion(modelUpdate.IdTipoActividad, modelUpdate.Tipo, opciones);

                    DataBase.TipoActividadOpciones.Update(listOpcion, modelUpdate.TipoActividadOpciones);

                    DataBase.TipoActividades.Update(modelUpdate);
                    DataBase.Save();

                    return RedirectToAction("Index");
                }
            }
            catch
            {
                this.AddDefaultErrorMessage();
            }

            model.TipoActividadOpciones = GetListOpcion(model.IdTipoActividad, model.Tipo, opciones);
            return View(model);
        }

        private List<TipoActividadOpcion> GetListOpcion(int IdTipoActividad, string Tipo, string[] opciones)
        {
            var listOpcion = new List<TipoActividadOpcion>();

            if (Tipo == Models.Constantes.TipoActividad.Seleccion ||
                   Tipo == Models.Constantes.TipoActividad.MultipleSeleccion)
            {
                int index = 1;

                if (opciones != null)
                {
                    foreach (var opcion in opciones)
                    {
                        listOpcion.Add(new TipoActividadOpcion()
                        {
                            IdTipoActividad = IdTipoActividad,
                            IdTipoActividadOpcion = index,
                            Descripcion = opcion,
                            Orden = index
                        });

                        index++;
                    }
                }
            }

            return listOpcion;
        }

        [HttpPost]
        [PreventSpam(Order = 0)]
        [Rp3.Web.Mvc.Authorize("TIPOACTIVIDAD", "DELETE", "AGENDACOMERCIAL")]
        public ActionResult Delete(TipoActividad model)
        {
            try
            {
                TipoActividad modelDelete = GetModel(model.IdTipoActividad);

                DataBase.TipoActividades.Delete(modelDelete);
                DataBase.Save();

                return RedirectToAction("Index");
            }
            catch
            {
                this.AddDefaultErrorMessage();
            }
            return RedirectToAction("Index", null, null, true);
        }

    }
}
