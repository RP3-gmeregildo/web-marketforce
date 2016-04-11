using Rp3.AgendaComercial.Models.General.View;
using Rp3.AgendaComercial.Models.Marcacion.View;
using Rp3.Web.Mvc;
using Rp3.Web.Mvc.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Rp3.AgendaComercial.Web.Marcacion.Controllers 
{
    public class JustificacionController : AgendaComercial.Web.Controllers.AgendaComercialController
    {
        // GET: Marcacion/Justificacion
        [Rp3.Web.Mvc.Authorize("JUSTIFICACION", "QUERY", "AGENDACOMERCIAL")]
        public ActionResult Index()
        {

            if (this.Agente.CargoRol == AgenteCargoRol.Agente && this.Agente.EsAgente)
            {
                this.UserWorkId = this.UserId;
            }
            else if ((this.Agente.CargoRol == AgenteCargoRol.Supervisor || this.Agente.CargoRol == AgenteCargoRol.Gerente))
            {
                this.TodosMisAgentes = true;
                this.UserWorkId = null; 
                var agentes = DataBase.Agentes.GetAgentesPermitidos(this.Agente.IdAgente);
                this.UserWorkIds = agentes.Where(p => p.IdUsuario != null).Select(p => p.IdUsuario ?? 0).ToList<int>();
            }


            ViewBag.Agentes = GetAgentes(null).ToSelectList(includeNullItem: true);
            ViewBag.Estados = DataBase.GeneralValues.Get(p => p.Id == Models.Constantes.EstadoPermiso.Tabla).ToSelectList(includeNullItem: true);

            return View();
        }

        [HttpGet]
        public ActionResult GetLista()
        {
            return PartialView("_PermisoLista");
        }

        [HttpGet]
        public ActionResult GetSort()
        {
            return PartialView("_SeleccionSort");
        }

        [HttpGet]
        public ActionResult GetSeleccionAgente()
        {
            var model = GetAgentes(null);

            return PartialView("_SeleccionAgente", model);
        }


        [ChildAction]
        public JsonResult AgenteAutocomplete(string term)
        {
            var agentes = GetAgentes(term);

            var result = agentes.Select(p => new { label = p.Descripcion, idAgente = p.IdAgente, idGrupo = p.IdGrupo }).Take(15);

            return new JsonResult() { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public List<AgenteGrupo> GetAgentes(string term)
        {
            var agentefilter = DataBase.Agentes.GetAgentesPermitidos(this.Agente.IdAgente).Select(p => p.IdAgente).ToList();
            var grupofilter = DataBase.Agentes.GetAgentesPermitidos(this.Agente.IdAgente).Where(p => p.IdGrupo != null).Select(p => p.IdGrupo).ToList();

            var agentes = DataBase.Agentes.Get(p => agentefilter.Contains(p.IdAgente) &&
                p.IdGrupo != null && p.Estado == Rp3.AgendaComercial.Models.Constantes.Estado.Activo);

            var grupos = DataBase.Grupos.Get(p => grupofilter.Contains(p.IdGrupo) && p.Estado == Rp3.AgendaComercial.Models.Constantes.Estado.Activo);

            var list = new List<AgenteGrupo>();

            foreach (var grupo in grupos.OrderBy(p => p.Descripcion))
                list.Add(new AgenteGrupo() { IdGrupo = grupo.IdGrupo, Descripcion = String.Format("{0}*", grupo.Descripcion) });

            foreach (var agente in agentes.OrderBy(p => p.Descripcion))
                list.Add(new AgenteGrupo() { IdAgente = agente.IdAgente, IdGrupo = agente.IdGrupo, Descripcion = agente.Descripcion });

            if (!String.IsNullOrEmpty(term))
                list = list.Where(p => p.Descripcion.ToUpper().Contains(term.ToUpper())).ToList();

            return list;
        }

        #region Listado

        [HttpGet]
        public ActionResult GetListado(DateTime? fechaInicial, DateTime? fechaFinal, string sortField, int sortMode, int? agente, int? grupo, string estado)
        {
            if (fechaInicial != null)
                fechaInicial = fechaInicial.Value.Date;

            if (fechaFinal != null)
                fechaFinal = fechaFinal.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59); ;

            List<PermisoCategoria> listaPermiso = new List<PermisoCategoria>();
            PermisoConsulta permisoConsulta = new PermisoConsulta();
            PermisoCategoria categoria = new PermisoCategoria();


            List<int> listAgente = new List<int>();
            List<int> listGrupo = new List<int>();

            if (!this.TodosMisAgentes)
            {
                listAgente.Add(this.AgenteWork.IdAgente);
            }
            else
            {
                foreach (var agentework in this.AgentesWork)
                {
                    listAgente.Add(agentework.IdAgente);

                    if (agentework.IdGrupo != null)
                        listGrupo.Add(agentework.IdGrupo ?? 0);
                }

                listGrupo = listGrupo.Distinct().ToList();
            }


            var data = DataBase.Permisos.GetPermisoListado(listAgente, listGrupo, fechaInicial, fechaFinal, agente, grupo, estado, false);

            categoria = new PermisoCategoria();
            categoria.isBusqueda = false;
            categoria.Nombre = String.Empty;

            categoria.Permiso = data.OrderBy(p => p.FechaInicio).ToList();

            listaPermiso.Add(categoria);

            if (String.IsNullOrEmpty(sortField))
            {
                foreach (var cat in listaPermiso)
                    cat.Permiso = cat.Permiso.OrderByDescending(p => p.FechaInicio).ToList();
            }
            else
            {
                Sort(listaPermiso, sortField, sortMode);
            }

            permisoConsulta.PermisoCategorias = listaPermiso;

            return PartialView("_PermisoListaDetalle", permisoConsulta);
        }

        public void Sort(List<PermisoCategoria> categorias, string sortField, int sortMode)
        {
            switch (sortField)
            {

                case "agente":
                    foreach (var cat in categorias)
                        if (sortMode == 1)
                            cat.Permiso = cat.Permiso.OrderBy(p => p.Agente).ToList();
                        else
                            cat.Permiso = cat.Permiso.OrderByDescending(p => p.Agente).ToList();
                    break;


                case "tipo":
                    foreach (var cat in categorias)
                        if (sortMode == 1)
                            cat.Permiso = cat.Permiso.OrderBy(p => p.TipoDesripcion).ToList();
                        else
                            cat.Permiso = cat.Permiso.OrderByDescending(p => p.TipoDesripcion).ToList();
                    break;

                case "fechainicio":
                    foreach (var cat in categorias)
                        if (sortMode == 1)
                            cat.Permiso = cat.Permiso.OrderBy(p => p.FechaInicio).ToList();
                        else
                            cat.Permiso = cat.Permiso.OrderByDescending(p => p.FechaInicio).ToList();
                    break;

                case "fechafin":
                    foreach (var cat in categorias)
                        if (sortMode == 1)
                            cat.Permiso = cat.Permiso.OrderBy(p => p.FechaFin).ToList();
                        else
                            cat.Permiso = cat.Permiso.OrderByDescending(p => p.FechaFin).ToList();
                    break;

                case "motivo":
                    foreach (var cat in categorias)
                        if (sortMode == 1)
                            cat.Permiso = cat.Permiso.OrderBy(p => p.MotivoDescripcion).ToList();
                        else
                            cat.Permiso = cat.Permiso.OrderByDescending(p => p.MotivoDescripcion).ToList();
                    break;

                case "estado":
                    foreach (var cat in categorias)
                        if (sortMode == 1)
                            cat.Permiso = cat.Permiso.OrderBy(p => p.EstadoDescripcion).ToList();
                        else
                            cat.Permiso = cat.Permiso.OrderByDescending(p => p.EstadoDescripcion).ToList();
                    break;

                case "observacion":
                    foreach (var cat in categorias)
                        if (sortMode == 1)
                            cat.Permiso = cat.Permiso.OrderBy(p => p.Observacion).ToList();
                        else
                            cat.Permiso = cat.Permiso.OrderByDescending(p => p.Observacion).ToList();
                    break;

                case "observacionsupervisor":
                    foreach (var cat in categorias)
                        if (sortMode == 1)
                            cat.Permiso = cat.Permiso.OrderBy(p => p.ObservacionSupervisor).ToList();
                        else
                            cat.Permiso = cat.Permiso.OrderByDescending(p => p.ObservacionSupervisor).ToList();
                    break;

            }
        }


        #endregion Listado

        #region MODAL_DETAIL

        [ChildAction(Order = 1)]
        [Rp3.Web.Mvc.Authorize("JUSTIFICACION", "QUERY", "AGENDACOMERCIAL", Order = 2)]
        [HttpPost]
        public ActionResult Aprobar(int id, string observacionSupervisor)
        {
            try
            {
                if (String.IsNullOrEmpty(observacionSupervisor))
                {
                    this.AddErrorMessage(Rp3.AgendaComercial.Resources.ErrorMessageValidation.ObservacionSupervisorObligatoria);
                }

                if (!this.MessageCollection.HasError())
                {
                    var model = DataBase.Permisos.Get(p => p.IdPermiso == id).FirstOrDefault();

                    model.ObservacionSupervisor = observacionSupervisor;
                    model.Estado = Rp3.AgendaComercial.Models.Constantes.EstadoPermiso.Aprobado;
                    model.UsrMod = this.UserLogonName;
                    model.FecMod = this.GetCurrentDateTime();

                    DataBase.Permisos.Update(model);

                    DataBase.Save();

                    try
                    {
                        DataBase.Permisos.Permiso(model.IdPermiso);
                    }
                    catch
                    {
                    }

                    try
                    {
                        var agente = DataBase.Agentes.Get(p => p.IdAgente == model.IdAgente).FirstOrDefault();

                        if (agente != null && agente.IdUsuario != null)
                        {
                            var device = DataBase.Devices.Get(p => p.IdUsuario == agente.IdUsuario).FirstOrDefault();

                            if (device != null && !String.IsNullOrEmpty(device.GCMId))
                            {
                                var agenteSup = DataBase.Agentes.Get(p => p.Usuario.LogonName == this.UserLogonName).FirstOrDefault();
                                string footer = String.Format("Enviada por: {0} - el {1:g}", agenteSup.Descripcion, this.GetCurrentDateTime());

                                AndroidNotificationPusher.PushNotification(device.GCMId, "Justificación Aprobada", String.Format("Su justificación del {0:d} ha sido aprobada.", model.FechaInicio), null, footer);
                            }
                        }
                    }
                    catch
                    {
                    }

                    this.AddDefaultSuccessMessage();
                }
            }
            catch
            {
                this.AddDefaultErrorMessage();
            }

            return Json();
        }

        [ChildAction(Order = 1)]
        [Rp3.Web.Mvc.Authorize("JUSTIFICACION", "QUERY", "AGENDACOMERCIAL", Order = 2)]
        [HttpPost]
        public ActionResult Rechazar(int id, string observacionSupervisor)
        {
            try
            {
                if (String.IsNullOrEmpty(observacionSupervisor))
                {
                    this.AddErrorMessage(Rp3.AgendaComercial.Resources.ErrorMessageValidation.ObservacionSupervisorObligatoria);
                }

                if (!this.MessageCollection.HasError())
                {
                    var model = DataBase.Permisos.Get(p => p.IdPermiso == id).FirstOrDefault();

                    model.ObservacionSupervisor = observacionSupervisor;
                    model.Estado = Rp3.AgendaComercial.Models.Constantes.EstadoPermiso.Rechazado;
                    model.UsrMod = this.UserLogonName;
                    model.FecMod = this.GetCurrentDateTime();

                    DataBase.Permisos.Update(model);

                    DataBase.Save();

                    try
                    {
                        DataBase.Permisos.Permiso(model.IdPermiso);
                    }
                    catch
                    {
                    }

                    try 
                    { 
                        var agente = DataBase.Agentes.Get(p=>p.IdAgente == model.IdAgente).FirstOrDefault();

                        if(agente != null && agente.IdUsuario != null)
                        {
                            var device = DataBase.Devices.Get(p => p.IdUsuario == agente.IdUsuario).FirstOrDefault();

                            if (device != null && !String.IsNullOrEmpty(device.GCMId))
                            {
                                var agenteSup = DataBase.Agentes.Get(p => p.Usuario.LogonName == this.UserLogonName).FirstOrDefault();
                                string footer = String.Format("Enviada por: {0} - el {1:g}", agenteSup.Descripcion, this.GetCurrentDateTime());

                                AndroidNotificationPusher.PushNotification(device.GCMId, "Justificación Rechazada", String.Format("Su justificación del {0:d} ha sido rechazada.", model.FechaInicio), null, footer);
                            }
                        }
                    }
                    catch
                    {
                    }

                    this.AddDefaultSuccessMessage();
                }
            }
            catch
            {
                this.AddDefaultErrorMessage();
            }

            return Json();
        }

        [ChildAction(Order = 1)]
        [Rp3.Web.Mvc.Authorize("JUSTIFICACION", "QUERY", "AGENDACOMERCIAL", Order = 2)]
        [HttpPost]
        public ActionResult Cancelar(int id, string observacionSupervisor)
        {
            try
            {
                if (String.IsNullOrEmpty(observacionSupervisor))
                {
                    this.AddErrorMessage(Rp3.AgendaComercial.Resources.ErrorMessageValidation.ObservacionSupervisorObligatoria);
                }

                if (!this.MessageCollection.HasError())
                {
                    var model = DataBase.Permisos.Get(p => p.IdPermiso == id).FirstOrDefault();

                    //model.ObservacionSupervisor = observacionSupervisor;
                    model.Estado = Rp3.AgendaComercial.Models.Constantes.EstadoPermiso.Pendiente;
                    model.UsrMod = this.UserLogonName;
                    model.FecMod = this.GetCurrentDateTime();

                    DataBase.Permisos.Update(model);

                    DataBase.Save();

                    this.AddDefaultSuccessMessage();
                }
            }
            catch
            {
                this.AddDefaultErrorMessage();
            }

            return Json();
        }

        [ChildAction(Order = 1)]
        [Rp3.Web.Mvc.Authorize("JUSTIFICACION", "QUERY", "AGENDACOMERCIAL", Order = 2)]
        [HttpGet]
        public ActionResult Detail(int id)
        {
            ViewBag.titulo = "Detalle Justificación";

            var model = DataBase.Permisos.Get(p => p.IdPermiso == id).FirstOrDefault();

            PermisoEdit permisoEdit = new PermisoEdit();

            permisoEdit.IdPermiso = model.IdPermiso;
            permisoEdit.IdAgente = model.IdAgente;
            permisoEdit.IdGrupo = model.IdGrupo;
            permisoEdit.Agente = model.IdAgente != null ? model.Agente.Descripcion : null;
            permisoEdit.Grupo = model.IdGrupo != null ? model.Grupo.Descripcion : null;

            permisoEdit.EsPrevio = false;

            if (model.IdAgente != null && model.Agente.Usuario != null && !String.IsNullOrEmpty(model.Agente.Usuario.Contact.PhoneNumber))
            {
                permisoEdit.Agente = String.Format("{0} - {1}: {2}", permisoEdit.Agente, Rp3.AgendaComercial.Resources.LabelFor.Telefono, model.Agente.Usuario.Contact.PhoneNumber);
            }

            permisoEdit.Motivo = model.Motivo;
            permisoEdit.Tipo = model.Tipo;

            permisoEdit.FechaInicio = model.FechaInicio.Date.AddHours(model.HoraInicio.Hour).AddMinutes(model.HoraInicio.Minute);
            permisoEdit.FechaFin = model.FechaFin.Date.AddHours(model.HoraFin.Hour).AddMinutes(model.HoraFin.Minute);

            permisoEdit.Observacion = model.Observacion;
            permisoEdit.ObservacionSupervisor = model.ObservacionSupervisor;

            return PartialView("_PermisoEdit", permisoEdit);
        }

        #endregion MODAL_DETAIL
    }
}