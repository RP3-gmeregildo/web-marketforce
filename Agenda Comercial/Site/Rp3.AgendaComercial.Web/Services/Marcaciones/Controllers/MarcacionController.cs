using Rp3.AgendaComercial.Models.Marcacion.View;
using Rp3.Web.Http;
using Rp3.Web.Mvc.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Rp3.AgendaComercial.Web.Services.Marcaciones.Controllers
{
    public class MarcacionController : AgendaComercial.Web.Services.Controllers.ApiAgendaComercialController
    {
        [ApiAuthorization]
        [HttpPost]
        public IHttpActionResult Create(Models.Marcacion marcacion)
        {
            var agente = DataBase.Agentes.Get(p => p.Usuario.LogonName == CurrentUser.LogonName).FirstOrDefault();
            var modelInsert = new Rp3.AgendaComercial.Models.Marcacion.Marcacion();
            var existeMarca = DataBase.Marcacions.GetSingleOrDefault(p => p.Fecha == marcacion.Fecha && p.Tipo == marcacion.Tipo && p.IdAgente == agente.IdAgente);
            if (existeMarca == null)
            {
                if (agente != null)
                {
                    try
                    {

                        Rp3.Data.Service.CopyTo(marcacion, modelInsert, includeProperties: new string[] {
                        "TipoTabla",
                        "Tipo",
                        "Fecha",
                        "HoraInicio",
                        "HoraFin",
                        "HoraInicioPlan",
                        "HoraFinPlan",
                        "MinutosAtraso",
                        "EnUbicacion",
                        "LongitudPlan",
                        "LatitudPlan",
                        "Longitud",
                        "Latitud",
                        "IdPermiso"
                        });
                        if (modelInsert.Tipo == Rp3.AgendaComercial.Models.Constantes.TipoMarcacion.InicioJornada1 || modelInsert.Tipo == Rp3.AgendaComercial.Models.Constantes.TipoMarcacion.InicioJornada2)
                            modelInsert.HoraFin = null;
                        else
                            modelInsert.HoraInicio = null;
                        modelInsert.IdAgente = agente.IdAgente;
                        modelInsert.IdGrupo = agente.IdGrupo.Value;
                        modelInsert.Ausente = false;
                        modelInsert.Atraso = false;
                        modelInsert.AsignarId();
                        modelInsert.FecIng = GetCurrentDateTime();

                        if (marcacion.Permiso != null && marcacion.IdPermiso == 0)
                        {
                            var permisoInsert = new Rp3.AgendaComercial.Models.Marcacion.Permiso();
                            Rp3.Data.Service.CopyTo(marcacion.Permiso, permisoInsert, includeProperties: new string[] {
                        "TipoTabla",
                        "Tipo",
                        "MotivoTabla",
                        "Motivo",
                        "FechaInicio",
                        "FechaFin",
                        "HoraInicio",
                        "HoraFin",
                        "Observacion",
                        "EsPrevio",
                        "EstadoTabla",
                        "Estado"
                        });

                            permisoInsert.AsignarId();
                            modelInsert.IdPermiso = permisoInsert.IdPermiso;
                            permisoInsert.IdAgente = modelInsert.IdAgente;
                            permisoInsert.IdGrupo = agente.IdGrupo;
                            permisoInsert.UsrIng = CurrentUser.LogonName;
                            permisoInsert.FecIng = GetCurrentDateTime();

                            DataBase.Permisos.Insert(permisoInsert);
                        }
                        if (modelInsert.IdPermiso == 0)
                            modelInsert.IdPermiso = null;

                        DataBase.Marcacions.Insert(modelInsert);


                        DataBase.Save();

                    }
                    catch (Exception ex)
                    {
                        return BadRequest(ex.Message);
                    }
                }
            }
            else
            {
                modelInsert = existeMarca;
            }

            return Ok(modelInsert.IdMarcacion);
        }

        [ApiAuthorization]
        [HttpPost]
        public IHttpActionResult InsertPermiso(Models.Permiso permiso)
        {
            var agente = DataBase.Agentes.Get(p => p.Usuario.LogonName == CurrentUser.LogonName).FirstOrDefault();
            var permisoInsert = new Rp3.AgendaComercial.Models.Marcacion.Permiso();
            if (agente != null)
            {
                try
                {
                    Rp3.Data.Service.CopyTo(permiso, permisoInsert, includeProperties: new string[] {
                        "TipoTabla",
                        "Tipo",
                        "MotivoTabla",
                        "Motivo",
                        "FechaInicio",
                        "FechaFin",
                        "HoraInicio",
                        "HoraFin",
                        "Observacion",
                        "EsPrevio",
                        "EstadoTabla",
                        "Estado"
                        });

                    permisoInsert.AsignarId();
                    permisoInsert.IdAgente = agente.IdAgente;
                    permisoInsert.IdGrupo = agente.IdGrupo;
                    permisoInsert.UsrIng = CurrentUser.LogonName;
                    permisoInsert.FecIng = GetCurrentDateTime();

                    DataBase.Permisos.Insert(permisoInsert);

                    DataBase.Save();

                    try
                    {
                        DataBase.Permisos.Permiso(permisoInsert.IdPermiso);
                    }
                    catch
                    {
                    }

                    try
                    {
                        if (agente != null && agente.IdSupervisor != null)
                        {
                            var device = DataBase.Devices.Get(p => p.IdUsuario == agente.Supervisor.IdUsuario).FirstOrDefault();

                            if (device != null && !String.IsNullOrEmpty(device.GCMId))
                            {
                                AndroidNotificationPusher.PushNotification(device.GCMId, "Solicitud de Justificación", String.Format("{0} ha solicitado una Justificación.", agente.Descripcion),"APROBAR_JUSTIFICACION");
                            }
                        }
                    }
                    catch
                    {
                    }

                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            return Ok(permisoInsert.IdPermiso);
        }

        [ApiAuthorization]
        [HttpPost]
        public IHttpActionResult InsertPermisoPrevio(Models.Permiso permiso)
        {
            var agente = DataBase.Agentes.Get(p => p.Usuario.LogonName == CurrentUser.LogonName).FirstOrDefault();
            var permisoInsert = new Rp3.AgendaComercial.Models.Marcacion.Permiso();
            if (agente != null)
            {
                try
                {
                    Rp3.Data.Service.CopyTo(permiso, permisoInsert, includeProperties: new string[] {
                        "TipoTabla",
                        "Tipo",
                        "MotivoTabla",
                        "Motivo",
                        "FechaInicio",
                        "FechaFin",
                        "HoraInicio",
                        "HoraFin",
                        "Observacion",
                        "EsPrevio",
                        "EstadoTabla",
                        "Estado"
                        });

                    permisoInsert.AsignarId();
                    permisoInsert.IdAgente = agente.IdAgente;
                    permisoInsert.IdGrupo = agente.IdGrupo;
                    permisoInsert.UsrIng = CurrentUser.LogonName;
                    permisoInsert.FecIng = GetCurrentDateTime();
                    permisoInsert.EsPrevio = true;

                    DataBase.Permisos.Insert(permisoInsert);

                    DataBase.Save();

                    try
                    {
                        DataBase.Permisos.Permiso(permisoInsert.IdPermiso);
                    }
                    catch
                    {
                    }

                    try
                    {
                        if (agente != null && agente.IdSupervisor != null)
                        {
                            var device = DataBase.Devices.Get(p => p.IdUsuario == agente.Supervisor.IdUsuario).FirstOrDefault();

                            if (device != null && !String.IsNullOrEmpty(device.GCMId))
                            {
                                string footer = String.Format("Enviada por: {0} - el {1:g}", agente.Descripcion, this.GetCurrentDateTime());
                                AndroidNotificationPusher.PushNotification(device.GCMId, "Solicitud de Justificación", String.Format("{0} ha solicitado una Justificación.", agente.Descripcion), "APROBAR_JUSTIFICACION", footer);
                            }
                        }
                    }
                    catch
                    {
                    }

                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            return Ok(permisoInsert.IdPermiso);
        }

        [ApiAuthorization]
        public IHttpActionResult GetGrupo()
        {
            var agente = DataBase.Agentes.Get(p => p.Usuario.LogonName == CurrentUser.LogonName).FirstOrDefault();
            Models.Grupo grupoSend = new Models.Grupo();
            if (agente != null)
            {
                try
                {
                    var grupo = DataBase.Grupos.Get(p => p.IdGrupo == agente.IdGrupo).FirstOrDefault();
                    Rp3.Data.Service.CopyTo(grupo, grupoSend, includeProperties: new string[] {
                        "IdGrupo",
                        "AplicaMarcacion",
                        "AplicaBreak",
                        "LongitudPuntoPartida",
                        "LatitudPuntoPartida"
                        });
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            return Ok(grupoSend);
        }

        [ApiAuthorization]
        public IHttpActionResult GetPermisoHoy()
        {
            var agente = DataBase.Agentes.Get(p => p.Usuario.LogonName == CurrentUser.LogonName).FirstOrDefault();
            Models.Permiso permiso = null;
            if (agente != null)
            {
                try
                {
                    DateTime desde = DateTime.Now;
                    desde = desde.Subtract(new TimeSpan(0, desde.Hour, desde.Minute, desde.Second + 1, desde.Millisecond));
                    var permisos = DataBase.Permisos.Get(p => p.IdAgente == agente.IdAgente && p.EsPrevio == true && 
                        p.Tipo == Rp3.AgendaComercial.Models.Constantes.TipoPermiso.Ausencia && p.Estado == Rp3.AgendaComercial.Models.Constantes.EstadoPermiso.Aprobado &&
                        p.FechaInicio >= desde).FirstOrDefault();
                    if (permisos != null)
                    {
                        permiso = new Models.Permiso();
                        Rp3.Data.Service.CopyTo(permisos, permiso, includeProperties: new string[] {
                        "TipoTabla",
                        "Tipo",
                        "MotivoTabla",
                        "Motivo",
                        "FechaInicio",
                        "FechaFin",
                        "HoraInicio",
                        "HoraFin",
                        "Observacion",
                        "EsPrevio",
                        "EstadoTabla",
                        "Estado"
                        });
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            return Ok(permiso);
        }

        [ApiAuthorization]
        public IHttpActionResult GetMarcacionesHoy()
        {
            var agente = DataBase.Agentes.Get(p => p.Usuario.LogonName == CurrentUser.LogonName).FirstOrDefault();
            List<Models.Marcacion> marcaciones = new List<Models.Marcacion>();
            if (agente != null)
            {
                try
                {
                    DateTime now = GetCurrentDateTime();
                    now = now.Subtract(new TimeSpan(0, now.Hour, now.Minute, now.Second + 1, now.Millisecond));
                    var list_marcaciones = DataBase.Marcacions.Get(p => p.IdAgente == agente.IdAgente && p.Fecha >= now).ToList();
                    
                    if (list_marcaciones != null)
                    {
                        Rp3.Data.Service.Assign(list_marcaciones, marcaciones);
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            return Ok(marcaciones);
        }

        [ApiAuthorization]
        public IHttpActionResult GetPermisoAprobar()
        {
            var agente = DataBase.Agentes.Get(p => p.Usuario.LogonName == CurrentUser.LogonName).FirstOrDefault();
            List<Models.Permiso> permisos = new List<Models.Permiso>();
            if (agente != null)
            {
                try
                {
                    var agentes = DataBase.Agentes.GetAgentesPermitidos(this.Agente.IdAgente);
                    var listAgente = agentes.Select(p => p.IdAgente).ToList();
                    var listGrupo = agentes.Where(p => p.IdUsuario != null).Select(p => p.IdGrupo ?? 0).ToList<int>().Distinct().ToList();
                    var data = DataBase.Permisos.GetPermisoListado(listAgente, listGrupo, null, null, null, null, Rp3.AgendaComercial.Models.Constantes.EstadoPermiso.Pendiente, false);
                    foreach(PermisoListado listado in data)
                    {
                        Models.Permiso permiso = new Models.Permiso();
                        Rp3.Data.Service.CopyTo(listado, permiso, includeProperties: new string[] {
                        "IdPermiso",
                        "IdAgente",
                        "TipoTabla",
                        "Tipo",
                        "MotivoTabla",
                        "Motivo",
                        "FechaInicio",
                        "FechaFin",
                        "HoraInicio",
                        "HoraFin",
                        "Observacion",
                        "EsPrevio",
                        "EstadoTabla",
                        "Estado",
                        "TipoJornada"
                        });
                        permisos.Add(permiso);
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            return Ok(permisos);
        }

        [ApiAuthorization]
        [HttpPost]
        public IHttpActionResult AprobarPermiso(Models.PermisoPorAprobar permiso)
        {
            var agente = DataBase.Agentes.Get(p => p.Usuario.LogonName == CurrentUser.LogonName).FirstOrDefault();
            var permisoUpdate = DataBase.Permisos.Get(p => p.IdPermiso == permiso.IdPermiso).FirstOrDefault();
            if (agente != null && permisoUpdate != null && permisoUpdate.Estado == Rp3.AgendaComercial.Models.Constantes.EstadoPermiso.Pendiente)
            {
                try
                {
                    Rp3.Data.Service.CopyTo(permiso, permisoUpdate, includeProperties: new string[] {
                        "ObservacionSupervisor",
                        "Estado"
                        });

                    permisoUpdate.UsrMod = CurrentUser.LogonName;
                    permisoUpdate.FecMod = GetCurrentDateTime();

                    DataBase.Permisos.Update(permisoUpdate);

                    DataBase.Save();

                    try
                    {
                        DataBase.Permisos.Permiso(permisoUpdate.IdPermiso);
                    }
                    catch
                    {
                    }

                    try
                    {
                        if (agente != null && agente.IdSupervisor != null)
                        {
                            var device = DataBase.Devices.Get(p => p.IdUsuario == agente.Supervisor.IdUsuario).FirstOrDefault();

                            if (device != null && !String.IsNullOrEmpty(device.GCMId))
                            {
                                string footer = String.Format("Enviada por: {0} - el {1:g}", agente.Descripcion, this.GetCurrentDateTime());
                                if(permisoUpdate.Estado == Rp3.AgendaComercial.Models.Constantes.EstadoPermiso.Aprobado)
                                    AndroidNotificationPusher.PushNotification(device.GCMId, "Justificación Aprobada", String.Format("Su justificación del {0:d} ha sido aprobada.", permisoUpdate.FechaInicio), null, footer);
                                else
                                    AndroidNotificationPusher.PushNotification(device.GCMId, "Justificación Rechazada", String.Format("Su justificación del {0:d} ha sido rechazada.", permisoUpdate.FechaInicio), null, footer);
                            }
                        }
                    }
                    catch
                    {
                    }

                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            return Ok();
        }
    
    }


}