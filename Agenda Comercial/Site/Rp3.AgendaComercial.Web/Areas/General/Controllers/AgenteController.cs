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
using DevExpress.Web.Mvc;
using System.Web.UI.WebControls;

namespace Rp3.AgendaComercial.Web.General.Controllers
{
    public class AgenteController : AgendaComercial.Web.Controllers.AgendaComercialController
    {
        //
        // GET: /General/Agente/

        [Rp3.Web.Mvc.Authorize("AGENTE", "QUERY", "AGENDACOMERCIAL")]
        public ActionResult Index()
        {
            return View(GetListIndex());
        }

        public ActionResult GridViewIndex()
        {
            return PartialView("_GridViewIndex", GetListIndex());
        }

        public List<Agente> GetListIndex()
        {
            var list = DataBase.Agentes.GetQueryable(p => p.Estado != Models.Constantes.Estado.Eliminado, includeProperties: "EstadoGeneralValue,Cargo,Grupo,Supervisor,Usuario.Contact");

            AgenteView agente = (AgenteView)Rp3.Web.Mvc.Session.GetValue("AgenteView");

            if (agente != null && agente.EsAgente)
            {
                switch (agente.CargoRol)
                {
                    case AgenteCargoRol.Agente:
                        list = list.Where(p => p.IdRuta == agente.IdRuta);
                        break;

                    case AgenteCargoRol.Supervisor:
                        var idAgentes = DataBase.Agentes.GetAgentesPermitidos(agente.IdAgente).Select(p => p.IdAgente).ToList<int>();
                        list = list.Where(p => idAgentes.Contains(p.IdAgente));
                        break;
                }
            }

            return list.ToList();
        }
        private void InicializarEdit(int idRuta, bool isNew = false)
        {
            ViewBag.GruposSelectList = DataBase.Grupos.Get(p => p.Estado == Models.Constantes.Estado.Activo).ToSelectList(includeNullItem: true);
            ViewBag.CargosSelectList = DataBase.Cargos.Get(p => p.Estado == Models.Constantes.Estado.Activo).ToSelectList(includeNullItem: true);
            ViewBag.SupervisoresSelectList = DataBase.Agentes.Get(p => p.Cargo.EsSupervisor && p.Estado == Models.Constantes.Estado.Activo).ToSelectList(includeNullItem: true);
            ViewBag.RutasSelectList = DataBase.Rutas.Get(p =>
                (!p.Agentes.Any() || p.IdRuta == idRuta) &&
                p.Estado == Models.Constantes.Estado.Activo).ToSelectList(includeNullItem: true);

            if (!isNew)
                ViewBag.UsuariosSelectList = DataBase.Users.Get(p => p.Active).ToSelectList(includeNullItem: true);
            else
            {
                var ids = DataBase.Agentes.Get(p => p.Estado == Models.Constantes.Estado.Activo && p.IdUsuario != null).Select(p => p.IdUsuario).Distinct().ToList();

                ViewBag.UsuariosSelectList = DataBase.Users.Get(p => p.Active && !ids.Contains(p.UserId)).ToSelectList(includeNullItem: true);
            }
        }

        public string GetUserName(int userId)
        {
            var user = DataBase.Contacts.Get(p => p.ContactId == userId).SingleOrDefault();
            return user.DefaultFullName;
        }

        [Rp3.Web.Mvc.Authorize("AGENTE", "NEW", "AGENDACOMERCIAL")]
        public ActionResult Create()
        {
            InicializarEdit(0, true);
           
            Agente model = new Agente();
            model.Estado = Models.Constantes.Estado.Activo;

            ViewBag.Ubicaciones = GetRuta(model.IdRuta);
            return View(model);
        }

        private Agente GetModel(int id)
        {
            Agente result = DataBase.Agentes.Get(p => p.IdAgente == id).SingleOrDefault();

            return result;
        }

        [Rp3.Web.Mvc.Authorize("AGENTE", "EDIT", "AGENDACOMERCIAL")]
        public ActionResult Edit(int id)
        {
            var model = GetModel(id);
            InicializarEdit(model.IdRuta??0);
            ViewBag.Ubicaciones = GetRuta(model.IdRuta);
            return View(model);
        }

        [Rp3.Web.Mvc.Authorize("AGENTE", "DETAIL", "AGENDACOMERCIAL")]
        public ActionResult Detail(int id)
        {
            var model = GetModel(id);
            ViewBag.Ubicaciones = GetRuta(model.IdRuta);
            return View(model);
        }

        [Rp3.Web.Mvc.Authorize("AGENTE", "DELETE", "AGENDACOMERCIAL")]
        public ActionResult Delete(int id)
        {
            var model = GetModel(id);
            ViewBag.Ubicaciones = GetRuta(model.IdRuta);
            return View(model);
        }

        [HttpPost]
        [PreventSpam(Order = 0)]
        [Rp3.Web.Mvc.Authorize("AGENTE", "NEW", "AGENDACOMERCIAL", Order = 1)]
        public ActionResult Create(Agente model)
        {
            try
            {
                if (model.IdUsuario == null)
                {
                    this.ModelState.AddModelError("IdUsuario", Rp3.AgendaComercial.Resources.ErrorMessageValidation.Required);
                }
                else if (DataBase.Agentes.Exists(p => p.IdUsuario == model.IdUsuario))
                {
                    this.ModelState.AddModelError("IdUsuario", Rp3.AgendaComercial.Resources.ErrorMessageValidation.AgenteUsuarioRepetido);
                }
                /*else if (DataBase.Agentes.Exists(p => p.IdAgente == model.IdAgente))
                {
                    this.ModelState.AddModelError("IdAgente", Rp3.AgendaComercial.Resources.ErrorMessageValidation.AgenteRepetido);
                }*/
                else if (model.IdRuta != null && DataBase.Agentes.Exists(p => p.IdRuta == model.IdRuta && p.IdAgente != model.IdAgente))
                {
                    this.ModelState.AddModelError("IdRuta", Rp3.AgendaComercial.Resources.ErrorMessageValidation.AgenteRutaRepetido);
                }
                else
                {
                    model.AsignarId();

                    model.Descripcion = GetUserName(model.IdUsuario.Value);

                    model.Estado = Rp3.AgendaComercial.Models.Constantes.Estado.Activo;
                    model.EstadoTabla = Rp3.AgendaComercial.Models.Constantes.Estado.Tabla;
                    model.UsrIng = this.UserLogonName;
                    model.FecIng = this.GetCurrentDateTime();

                    DataBase.Agentes.Insert(model);
                    DataBase.Save();
                    
                    this.AddDefaultSuccessMessage();

                    return RedirectToAction("Index");
                }
            }
            catch
            {
                this.AddDefaultErrorMessage();
            }

            InicializarEdit(0, true);
            ViewBag.Ubicaciones = GetRuta(model.IdRuta);
            return View(model);            
        }

        [HttpPost]
        [Rp3.Web.Mvc.Authorize("AGENTE", "EDIT", "AGENDACOMERCIAL", Order = 1)]
        public ActionResult Edit(Agente model)
        {
            int idRuta = 0;
            try
            {
                if (DataBase.Agentes.Exists(p => p.IdRuta != null && p.IdRuta == model.IdRuta && p.IdAgente != model.IdAgente))
                {
                    this.ModelState.AddModelError("IdRuta", Rp3.AgendaComercial.Resources.ErrorMessageValidation.AgenteRutaRepetido);
                }
                else
                {
                    Agente modelUpdate = GetModel(model.IdAgente);

                    idRuta = modelUpdate.IdRuta ?? 0;

                    modelUpdate.IdRuta = model.IdRuta;
                    modelUpdate.IdCargo = model.IdCargo;
                    modelUpdate.IdGrupo = model.IdGrupo;

                    if (model.IdSupervisor != modelUpdate.IdAgente)
                        modelUpdate.IdSupervisor = model.IdSupervisor;

                    modelUpdate.Estado = model.Estado;

                    modelUpdate.UsrMod = this.UserLogonName;
                    modelUpdate.FecMod = this.GetCurrentDateTime();

                    DataBase.Agentes.Update(modelUpdate);
                    DataBase.Save();


                    if ((idRuta != 0 && idRuta != modelUpdate.IdRuta) || modelUpdate.IdRuta == null)
                        DataBase.Agentes.EliminarDependenciaAgente(modelUpdate.IdAgente, this.UserLogonName);
                    else 
                        VerificarDependencia(modelUpdate);

                    return RedirectToAction("Index");
                }
            }
            catch
            {
                this.AddDefaultErrorMessage();
            }

            InicializarEdit(idRuta);
            ViewBag.Ubicaciones = GetRuta(model.IdRuta);
            return View(model);            
        }

        [HttpPost]
        [PreventSpam(Order = 0)]
        [Rp3.Web.Mvc.Authorize("AGENTE", "DELETE", "AGENDACOMERCIAL")]
        public ActionResult Delete(Agente model)
        {
            try
            {
                model.Estado = Models.Constantes.Estado.Eliminado;
                model.UsrMod = this.UserLogonName;
                model.FecMod = this.GetCurrentDateTime();

                DataBase.Agentes.Update(model);
                DataBase.Save();

                VerificarDependencia(model);

                return RedirectToAction("Index");
            }
            catch
            {
                this.AddDefaultErrorMessage();
            }

            return RedirectToAction("Index", null, null, true);
        }

        private void VerificarDependencia(Agente agente)
        {
            if (agente.Estado == Rp3.AgendaComercial.Models.Constantes.Estado.Inactivo ||
                agente.Estado == Rp3.AgendaComercial.Models.Constantes.Estado.Eliminado)
            {
                DataBase.Agentes.EliminarDependenciaAgente(agente.IdAgente, this.UserLogonName);
            }
        }

        List<Ubicacion> GetRuta(int? idRuta)
        {
            List<Ubicacion> ubicaciones = new List<Ubicacion>();

            if (idRuta != null)
            {
               var direcciones = DataBase.RutaDetalles.Get(p => p.IdRuta == idRuta);

                foreach (var item in direcciones)
                    ubicaciones.Add(new Ubicacion() { Titulo = item.ClienteDireccion.Cliente.NombresCompletos, Latitud = item.ClienteDireccion.Latitud, Longitud = item.ClienteDireccion.Longitud });
            }

            return ubicaciones;
        }     

        public ActionResult UbicacionMapMarker(int? idRuta)
        {
            return PartialView("_UbicacionMapMarker", GetRuta(idRuta));
        }

        [ChildAction]
        public JsonResult AgenteGrupoAutocomplete(string term)
        {
            var agentes = DataBase.Agentes.Get(p => p.Descripcion.Contains(term) && p.IdGrupo == null &&
                p.Estado == Rp3.AgendaComercial.Models.Constantes.Estado.Activo);

            var result = agentes.Select(p => new { label = p.Descripcion, id = p.IdAgente }).Take(15);

            return new JsonResult() { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
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

            settings.Name = "Agentes";
            settings.Width = Unit.Percentage(100);

            settings.Columns.Add("Usuario.Contact.DefaultFullName", Rp3.AgendaComercial.Resources.LabelFor.Usuario);
            settings.Columns.Add("Usuario.DescriptionName", Rp3.AgendaComercial.Resources.LabelFor.Usuario);
            settings.Columns.Add("Ruta.Descripcion", Rp3.AgendaComercial.Resources.LabelFor.Ruta);
            settings.Columns.Add("Cargo.Descripcion", Rp3.AgendaComercial.Resources.LabelFor.Cargo);
            settings.Columns.Add("Supervisor.Descripcion", Rp3.AgendaComercial.Resources.LabelFor.Supervisor);
            settings.Columns.Add("Grupo.Descripcion", Rp3.AgendaComercial.Resources.LabelFor.Grupo);
            settings.Columns.Add("EstadoGeneralValue.Content", Rp3.AgendaComercial.Resources.LabelFor.Estado);

            settings.SettingsExport.Landscape = true;
            settings.SettingsExport.ReportHeader = settings.Name;        

            return settings;
        }

        #endregion Export
    }
}
