using Rp3.Web.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Rp3.AgendaComercial.Models.General;
using Rp3.AgendaComercial.Models;
using Rp3.Web.Mvc.Utility;
using System.IO;

namespace Rp3.AgendaComercial.Web.Services.General.Controllers
{
    public class AgenteController : AgendaComercial.Web.Services.Controllers.ApiAgendaComercialController
    {
        [ApiAuthorization]
        public IHttpActionResult GetAgente()
        {
            return Ok(Agente);
        }

        [ApiAuthorization]
        [HttpPost]
        public IHttpActionResult SetUbicaciones(List<Services.General.Models.Ubicacion> ubicaciones)
        {
            try
            {
                long newId = DataBase.AgenteUbicaciones.GetMaxValue<long>(p => p.IdAgenteUbicacion, 0) + 1;

                var agente = DataBase.Agentes.Get(p => p.Usuario.LogonName == CurrentUser.LogonName).FirstOrDefault();
                if (agente != null)
                {
                    foreach (var ubicacion in ubicaciones)
                    {
                        var model = new AgendaComercial.Models.Ruta.AgenteUbicacion();

                        model.IdAgente = agente.IdAgente;
                        model.Latitud = ubicacion.Latitud;
                        model.Longitud = ubicacion.Longitud;
                        model.Fecha = ubicacion.Fecha;

                        model.IdAgenteUbicacion = newId;

                        DataBase.AgenteUbicaciones.Insert(model);
                        newId++;
                    }
                    DataBase.Save();
                }
            }
            catch
            {
                return BadRequest();
            }

            return Ok();
        }

        [ApiAuthorization]
        [HttpPost]
        public IHttpActionResult SendLog(Models.Log logs)
        {
            try
            {
                var agente = DataBase.Agentes.GetSingleOrDefault(p => p.IdUsuario == CurrentUser.UserId);
                var newfileName = String.Format("{0}.txt", agente.Descripcion);
                string filePath =
                    Path.Combine(HttpRuntime.AppDomainAppPath, AgendaComercial.Models.Constantes.LogsMedia.NewFilePath, newfileName);

                FileInfo FileDetele = new FileInfo(filePath);
                if (FileDetele.Exists)
                    FileDetele.Delete();

                System.IO.File.AppendAllText(filePath, logs.Logs);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.StackTrace);
            }

            return Ok();
        }

        [ApiAuthorization]
        [HttpPost]
        public IHttpActionResult SetGCMId(Models.GCMId gcmid)
        {
            try
            {
                var device = DataBase.Devices.Get(p => p.IdUsuario == CurrentUser.UserId).FirstOrDefault();
                if (device != null)
                {
                    device.GCMId = gcmid.AuthId;
                    DataBase.Devices.Update(device);
                    DataBase.Save();
                }
                else
                {
                    Rp3.Core.Data.Models.Security.Device dev = new Rp3.Core.Data.Models.Security.Device();
                    dev.IdUsuario = CurrentUser.UserId;
                    dev.GCMId = gcmid.AuthId;
                    DataBase.Devices.Insert(dev);
                    DataBase.Save();
                }
            }
            catch
            {
                return BadRequest();
            }

            return Ok();
        }

        [ApiAuthorization]
        [HttpPost]
        public IHttpActionResult SetUbicacion(float latitud, float longitud) 
        {
            try
            {
                var agente = DataBase.Agentes.Get(p => p.Usuario.LogonName == CurrentUser.LogonName).FirstOrDefault();

                if (agente != null)
                {
                    var model = new AgendaComercial.Models.Ruta.AgenteUbicacion();

                    model.IdAgente = agente.IdAgente;
                    model.Latitud = latitud;
                    model.Longitud = longitud;
                    model.Fecha = Rp3.Web.Mvc.Session.CurrentDateTime;
                    model.AsignarId();

                    DataBase.AgenteUbicaciones.Insert(model);

                    DataBase.Save();
                }
            }
            catch
            {
                return BadRequest();
            }

            return Ok();
        }

        [ApiAuthorization]
        public IHttpActionResult GetAgentes()
        {
            List<Models.Agente> list = new List<Models.Agente>();
            try
            {
                var agentes = DataBase.Agentes.Get(p => p.Estado == Constantes.Estado.Activo);
                
                foreach (var agente in agentes)
                {
                    var model = new Models.Agente();

                    model.IdAgente = agente.IdAgente;
                    model.Nombre = agente.Descripcion;
                    model.Telefono = agente.Usuario.Contact.PhoneNumber;
                    model.Email = agente.Usuario.Contact.Email;
                    list.Add(model);

                }

            }
            catch
            {
                return BadRequest();
            }

            return Ok(list);
        }

        [ApiAuthorization]
        public IHttpActionResult SendNotification(Models.NotificationToUser notification)
        {
            try
            {
                var usuario = DataBase.Agentes.Get(p => p.IdUsuario == CurrentUser.UserId).FirstOrDefault();
                string footer = String.Format("Enviada por: {0} - el {1:g}", usuario.Descripcion, this.GetCurrentDateTime());
                if (notification.IdAgente != 0)
                {
                    var agente = DataBase.Agentes.Get(p => p.IdAgente == notification.IdAgente).FirstOrDefault();
                    var device = DataBase.Devices.Get(p => p.IdUsuario == agente.IdUsuario).FirstOrDefault();

                    string emails = agente.Usuario.Contact.Email;

                    if (device != null && !String.IsNullOrEmpty(device.GCMId))
                    {
                        AndroidNotificationPusher.PushNotification(device.GCMId, notification.Titulo, notification.Mensaje, null, footer);
                    }

                    if (!String.IsNullOrEmpty(emails))
                    {
                        DataBase.Notificacions.Send(this.ApplicationId, emails, notification.Titulo, String.Format("{0}<br><br>{1}", notification.Mensaje, footer), 105);
                    }
                }
                else
                {
                    string emails = String.Empty;

                    List<Rp3.AgendaComercial.Models.General.Agente> agentes = null;
                    var sender = DataBase.Agentes.GetAgenteView(this.CurrentUser.UserId);
                    if (sender.EsAdministrador)
                        agentes = DataBase.Agentes.Get(p => p.Estado == Rp3.AgendaComercial.Models.Constantes.Estado.Activo).ToList();
                    else
                        agentes = DataBase.Agentes.Get(p =>
                            p.IdSupervisor == sender.IdAgente &&
                            p.Estado == Rp3.AgendaComercial.Models.Constantes.Estado.Activo).ToList();
                    foreach (var agente in agentes)
                    {
                        try
                        {
                            if (agente != null && agente.IdUsuario != null)
                            {
                                if (!String.IsNullOrEmpty(agente.Usuario.Contact.Email))
                                {
                                    if (!String.IsNullOrEmpty(emails))
                                        emails = String.Format("{0};{1}", emails, agente.Usuario.Contact.Email);
                                    else
                                        emails = agente.Usuario.Contact.Email;
                                }

                                var device = DataBase.Devices.Get(p => p.IdUsuario == agente.IdUsuario).FirstOrDefault();

                                if (device != null && !String.IsNullOrEmpty(device.GCMId))
                                {
                                    AndroidNotificationPusher.PushNotification(device.GCMId, notification.Titulo, notification.Mensaje, null, footer);
                                }
                            }
                        }
                        catch(Exception ex)
                        {
                            return BadRequest(ex.StackTrace);
                        }
                    }

                    if (!String.IsNullOrEmpty(emails))
                    {
                        DataBase.Notificacions.Send(this.ApplicationId, emails, notification.Titulo, String.Format("{0}<br><br>{1}", notification.Mensaje, footer), 105);
                    }
                }
            }
            catch(Exception ex)
            {
                return BadRequest(ex.StackTrace);
            }

            return Ok();
        }

        [ApiAuthorization]
        public IHttpActionResult GetCalendario()
        {
            Calendario calendario = null;
            Models.Calendario cal = new Models.Calendario();
            try
            {
                var agente = DataBase.Agentes.Get(p => p.Usuario.LogonName == CurrentUser.LogonName).FirstOrDefault();
                if(agente.IdGrupo.HasValue)
                    calendario = DataBase.Grupos.Get(p => p.IdGrupo == agente.IdGrupo).FirstOrDefault().Calendario;
                if (calendario == null)
                    calendario = DataBase.Calendarios.Get(p => p.EsDefault == true).FirstOrDefault();

                cal = new Models.Calendario();
                cal.IdCalendario = calendario.IdCalendario;
                cal.DiasLaborales = new List<Models.DiaLaboral>();
                foreach(Rp3.AgendaComercial.Models.General.DiaLaboral dia in calendario.DiasLaborales)
                {
                    Models.DiaLaboral diaLaboral = new Models.DiaLaboral();
                    Rp3.Data.Service.CopyTo(dia, diaLaboral, includeProperties: new string[] {
                        "IdDia",
                        "IdDiaTabla",
                        "Orden",
                        "EsLaboral",
                        "HoraInicio1",
                        "HoraFin1",
                        "HoraInicio2",
                        "HoraFin2"
                        });
                    cal.DiasLaborales.Add(diaLaboral);
                }

                cal.DiasNoLaborables = new List<Models.DiaNoLaborable>();
                foreach (Rp3.AgendaComercial.Models.General.DiasNoLaborable dia in calendario.DiasNoLaborables)
                {
                    Models.DiaNoLaborable diaNoLaboral = new Models.DiaNoLaborable();
                    Rp3.Data.Service.CopyTo(dia, diaNoLaboral, includeProperties: new string[] {
                        "Fecha",
                        "DiaParcial",
                        "EsteAño",
                        "HoraInicio",
                        "HoraFin"
                        });
                    cal.DiasNoLaborables.Add(diaNoLaboral);
                }

            }
            catch
            {
                return BadRequest();
            }

            return Ok(cal);
        }
    }
}