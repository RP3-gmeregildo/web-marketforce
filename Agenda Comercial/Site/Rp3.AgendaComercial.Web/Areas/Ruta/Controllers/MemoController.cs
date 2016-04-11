using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Rp3.Data.Models.Security;
using Rp3.Web.Mvc.Application.Security.Models;
using Rp3.Web.Mvc;
using Rp3.AgendaComercial.Models.Ruta;
using Rp3.Web.Mvc.Html;

namespace Rp3.AgendaComercial.Web.Ruta.Controllers
{
    public class MemoController : Rp3.Web.Mvc.Controllers.BaseController<Models.ContextService>
    {
        //
        // GET: /Ruta/Memo/

        [Rp3.Web.Mvc.Authorize("MEMO", "QUERY", "AGENDACOMERCIAL")]
        public ActionResult Index()
        {
            return View(GetListIndex());
        }

        public ActionResult GridViewIndex()
        {
            return PartialView("_GridViewIndex", GetListIndex());
        }

        public List<Memo> GetListIndex()
        {
            return DataBase.Memos.Get(includeProperties: "Remitente, EstadoMemoGeneralValue, TipoMemoGeneralValue, ImportanciaGeneralValue").ToList();
        }

        private void InicializarEdit()
        {
            var agentes = DataBase.Agentes.Get(p => p.Estado == Models.Constantes.Estado.Activo);

            ViewBag.Agentes = agentes.ToList();
            ViewBag.AgentesSelectList = agentes.ToSelectList(includeNullItem: true);
        }

        [Rp3.Web.Mvc.Authorize("MEMO", "NEW", "AGENDACOMERCIAL")]
        public ActionResult Create()
        {
            InicializarEdit();

            Memo model = new Memo();

            model.Fecha = GetCurrentDateTime();

            model.EstadoMemo = Models.Constantes.EstadoMemo.Activo;
            model.Importancia = Models.Constantes.Importancia.Media;
            model.TipoMemo = Models.Constantes.TipoMemo.Normal;

            model.MemoDestinatarios = new List<MemoDestinatario>();

            return View(model);
        }

        private Memo GetModel(long id)
        {
            Memo result = DataBase.Memos.Get(p => p.IdMemo == id, includeProperties: "MemoDestinatarios").SingleOrDefault();

            return result;
        }

        [Rp3.Web.Mvc.Authorize("MEMO", "EDIT", "AGENDACOMERCIAL")]
        public ActionResult Edit(long id)
        {
            InicializarEdit();

            return View(GetModel(id));
        }

        [Rp3.Web.Mvc.Authorize("MEMO", "DETAIL", "AGENDACOMERCIAL")]
        public ActionResult Detail(long id)
        {
            var model = GetModel(id);
            model.ReadOnly = true;

            ViewBag.Agentes = model.MemoDestinatarios.Select(p => p.Agente).Distinct().ToList();

            return View(model);
        }

        [Rp3.Web.Mvc.Authorize("MEMO", "DELETE", "AGENDACOMERCIAL")]
        public ActionResult Delete(long id)
        {
            var model = GetModel(id);
            model.ReadOnly = true;

            ViewBag.Agentes = model.MemoDestinatarios.Select(p => p.Agente).Distinct().ToList();

            return View(model);
        }

        [HttpPost]
        [PreventSpam(Order = 0)]
        [Rp3.Web.Mvc.Authorize("MEMO", "NEW", "AGENDACOMERCIAL", Order = 1)]
        public ActionResult Create(Memo model, string[] agentes)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.AsignarId();

                    model.EstadoMemo = Rp3.AgendaComercial.Models.Constantes.EstadoMemo.Activo;
                    model.EstadoMemoTabla = Rp3.AgendaComercial.Models.Constantes.EstadoMemo.Tabla;
                    model.TipoMemoTabla = Rp3.AgendaComercial.Models.Constantes.TipoMemo.Tabla;
                    model.ImportanciaTabla = Rp3.AgendaComercial.Models.Constantes.Importancia.Tabla;

                    model.UsrIng = this.UserLogonName;
                    model.FecIng = this.GetCurrentDateTime();

                    DataBase.Memos.Insert(model);
                    DataBase.Save();

                    return RedirectToAction("Index");
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
        [Rp3.Web.Mvc.Authorize("MEMO", "EDIT", "AGENDACOMERCIAL", Order = 1)]
        public ActionResult Edit(Memo model, string[] agentes)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Memo modelUpdate = GetModel(model.IdMemo);

                    CopyTo(model, modelUpdate, includeProperties: new string[] {
                       "Fecha",
                       "Titulo",
                       "Detalle",
                       "IdAgenteRemitente",
                       "TipoMemo",
                       "Importancia",
                       "EstadoMemo"
                        });

                    modelUpdate.UsrMod = this.UserLogonName;
                    modelUpdate.FecMod = this.GetCurrentDateTime();

                    DataBase.MemoDestinatarios.Update(model.MemoDestinatarios, modelUpdate.MemoDestinatarios);

                    DataBase.Memos.Update(modelUpdate);
                    DataBase.Save();

                    return RedirectToAction("Index");
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
        [Rp3.Web.Mvc.Authorize("MEMO", "DELETE", "AGENDACOMERCIAL")]
        public ActionResult Delete(Memo model)
        {
            try
            {
                Memo modelDelete = GetModel(model.IdMemo);

                DataBase.Memos.Delete(modelDelete);
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
