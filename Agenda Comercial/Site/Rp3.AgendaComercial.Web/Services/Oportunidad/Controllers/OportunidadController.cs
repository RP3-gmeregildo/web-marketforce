using Rp3.AgendaComercial.Models;
using Rp3.AgendaComercial.Web.Services.Oportunidad.Models;
using Rp3.Web.Http;
using Rp3.Web.Mvc.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Rp3.AgendaComercial.Web.Services.Oportunidad.Controllers
{
    public class OportunidadController : AgendaComercial.Web.Services.Controllers.ApiAgendaComercialController
    {
        [ApiAuthorization]
        public IHttpActionResult GetOportunidades(long? ultimaActualizacion = null)
        {
            DateTime? fechaUltimaActualizacion = null;
            
            if(ultimaActualizacion!=null)
            {
                fechaUltimaActualizacion = new DateTime(ultimaActualizacion.Value);
            }

            List<AgendaComercial.Models.Oportunidad.Oportunidad> data = null;
            if(Agente.EsSupervisor)
                data = DataBase.Oportunidades.GetOportunidadServiceSupervisor(Agente.IdAgente, fechaUltimaActualizacion);
            else
                data = DataBase.Oportunidades.GetOportunidadService(Agente.IdAgente, fechaUltimaActualizacion);

            List<Models.Oportunidad> oportunidades = new List<Models.Oportunidad>();

            Assign(data, oportunidades);
            int cont = 0;
            foreach(Rp3.AgendaComercial.Models.Oportunidad.Oportunidad opt in data)
            {
                int contMed = 0;
                foreach (Rp3.AgendaComercial.Models.Oportunidad.OportunidadMedia media in opt.OportunidadMedias)
                {
                    string fileName = Path.GetFileName(media.Path);
                    oportunidades[cont].OportunidadMedias[contMed].Path= fileName;
                    contMed++;
                }
                int contCont = 0;
                foreach (Rp3.AgendaComercial.Models.Oportunidad.OportunidadContacto contacto in opt.OportunidadContactos)
                {
                    string fileName = Path.GetFileName(contacto.Path);
                    oportunidades[cont].OportunidadContactos[contCont].Path = fileName;
                    contCont++;
                }
                oportunidades[cont].FechaCreacion = opt.FecIng;
                cont++;
            }

            return Ok(oportunidades);
        }

        [ApiAuthorization]
        [HttpPost]
        public IHttpActionResult Create(Models.Oportunidad oportunidad)
        {
            var agente = DataBase.Agentes.Get(p => p.Usuario.LogonName == CurrentUser.LogonName).FirstOrDefault();
            var modelInsert = new Rp3.AgendaComercial.Models.Oportunidad.Oportunidad()
            {
                OportunidadContactos = new List<AgendaComercial.Models.Oportunidad.OportunidadContacto>(),
                OportunidadMedias = new List<AgendaComercial.Models.Oportunidad.OportunidadMedia>(),
                OportunidadResponsables = new List<AgendaComercial.Models.Oportunidad.OportunidadResponsable>(),
                OportunidadTareas = new List<AgendaComercial.Models.Oportunidad.OportunidadTarea>(),
                OportunidadEtapas = new List<AgendaComercial.Models.Oportunidad.OportunidadEtapa>(),
                OportunidadBitacoras = new List<AgendaComercial.Models.Oportunidad.OportunidadBitacora>()
            };
            if (agente != null)
            {
                try
                {

                    Rp3.Data.Service.CopyTo(oportunidad, modelInsert, includeProperties: new string[] {
                        "Descripcion",
                        "Probabilidad",
                        "Importe",
                        "Estado",
                        "IdAgente",
                        "FechaUltimaGestionTicks",
                        "Calificacion",
                        "Observacion",
                        "Direccion",
                        "ReferenciaDireccion",
                        "Telefono1",
                        "Telefono2",
                        "CorreoElectronico",
                        "PaginaWeb",
                        "TipoEmpresa",
                        "Referencia",
                        "Latitud",
                        "Longitud",
                        "IdEtapa",
                        "FechaCreacionTicks",
                        "IdOportunidadTipo"
                        });

                    modelInsert.UsrIng = CurrentUser.LogonName;
                    modelInsert.FecIng = oportunidad.FechaCreacion.Value;
                    modelInsert.FechaUltimaGestion = oportunidad.FechaUltimaGestion;
                    modelInsert.EstadoTabla = Rp3.AgendaComercial.Models.Constantes.EstadoOportunidad.Tabla;
                    modelInsert.IdAgente = agente.IdAgente;
                    modelInsert.AsignarId();

                    int contactCont = 1;
                    foreach (OportunidadContacto cont in oportunidad.OportunidadContactos)
                    {
                        AgendaComercial.Models.Oportunidad.OportunidadContacto opCont = new AgendaComercial.Models.Oportunidad.OportunidadContacto();
                        opCont.Cargo = cont.Cargo;
                        opCont.CorreoElectronico = cont.CorreoElectronico;
                        opCont.EsPrincipal = cont.EsPrincipal;
                        opCont.IdOportunidad = modelInsert.IdOportunidad;
                        opCont.Nombre = cont.Nombre;
                        opCont.Path = cont.Path;
                        opCont.Telefono1 = cont.Telefono1;
                        opCont.Telefono2 = cont.Telefono2;
                        opCont.UsrMod = CurrentUser.LogonName;
                        opCont.FecMod = GetCurrentDateTime();
                        opCont.IdOportunidadContacto = contactCont;
                        contactCont++;
                        modelInsert.OportunidadContactos.Add(opCont);
                    }

                    foreach (OportunidadEtapa etapa in oportunidad.OportunidadEtapas)
                    {
                        AgendaComercial.Models.Oportunidad.OportunidadEtapa opEtapa = new AgendaComercial.Models.Oportunidad.OportunidadEtapa();
                        opEtapa.IdOportunidad = modelInsert.IdOportunidad;
                        opEtapa.IdEtapa = etapa.IdEtapa;
                        opEtapa.IdEtapaPadre = etapa.IdEtapaPadre;
                        opEtapa.Estado = etapa.Estado;
                        opEtapa.FechaInicio = etapa.FechaInicio;
                        opEtapa.FechaFin = etapa.FechaFin;
                        opEtapa.Observacion = etapa.Observacion;
                        opEtapa.Orden = etapa.Orden;
                        opEtapa.EstadoTabla = Constantes.EstadoEtapaOportunidad.Tabla;
                        opEtapa.FechaFinPlan = etapa.FechaFinPlan;
                        modelInsert.OportunidadEtapas.Add(opEtapa);
                    }

                    foreach (OportunidadResponsable resp in oportunidad.OportunidadResponsables)
                    {
                        AgendaComercial.Models.Oportunidad.OportunidadResponsable opResp = new AgendaComercial.Models.Oportunidad.OportunidadResponsable();
                        opResp.IdAgente = resp.IdAgente;
                        opResp.TipoTabla = Constantes.TipoResponsable.Tabla;
                        if (resp.Tipo != null)
                            opResp.Tipo = resp.Tipo;
                        else
                            opResp.Tipo = Constantes.TipoResponsable.Gestor;
                        opResp.IdOportunidad = modelInsert.IdOportunidad;
                        modelInsert.OportunidadResponsables.Add(opResp);
                    }

                    if (oportunidad.OportunidadBitacoras != null)
                    {
                        foreach (OportunidadBitacora bitacora in oportunidad.OportunidadBitacoras)
                        {
                            AgendaComercial.Models.Oportunidad.OportunidadBitacora opBit = new AgendaComercial.Models.Oportunidad.OportunidadBitacora();
                            opBit.Detalle = bitacora.Detalle;
                            opBit.FecIng = bitacora.FecIng.Value;
                            opBit.IdAgente = bitacora.IdAgente;
                            opBit.IdOportunidad = modelInsert.IdOportunidad;
                            opBit.AsignarId();
                            modelInsert.OportunidadBitacoras.Add(opBit);
                        }
                    }

                    foreach (OportunidadTarea tarea in oportunidad.OportunidadTareas)
                    {
                        AgendaComercial.Models.Oportunidad.OportunidadTarea opTarea = new AgendaComercial.Models.Oportunidad.OportunidadTarea();
                        opTarea.Estado = tarea.Estado;
                        opTarea.FechaFin = tarea.FechaFin;
                        opTarea.FechaInicio = tarea.FechaInicio;
                        opTarea.IdEtapa = tarea.IdEtapa;
                        opTarea.IdTarea = tarea.IdTarea;
                        opTarea.Observacion = tarea.Observacion;
                        opTarea.Orden = tarea.Orden;
                        opTarea.IdOportunidad = modelInsert.IdOportunidad;
                        opTarea.EstadoTabla = Constantes.EstadoTarea.Tabla;
                        opTarea.OportunidadTareaActividads = new List<AgendaComercial.Models.Oportunidad.OportunidadTareaActividad>();

                        foreach (OportunidadTareaActividad tareaActividad in tarea.OportunidadTareaActividads)
                        {
                            AgendaComercial.Models.Oportunidad.OportunidadTareaActividad opTareaActividad = new AgendaComercial.Models.Oportunidad.OportunidadTareaActividad();
                            opTareaActividad.IdEtapa = tarea.IdEtapa;
                            opTareaActividad.IdOportunidad = modelInsert.IdOportunidad;
                            opTareaActividad.IdTarea = tarea.IdTarea;
                            opTareaActividad.Resultado = tareaActividad.Resultado;
                            opTareaActividad.ResultadoCodigo = tareaActividad.ResultadoCodigo;
                            opTareaActividad.IdTareaActividad = tareaActividad.IdTareaActividad;
                            opTarea.OportunidadTareaActividads.Add(opTareaActividad);
                        }

                        modelInsert.OportunidadTareas.Add(opTarea);
                    }

                    DataBase.Oportunidades.Insert(modelInsert);

                    DataBase.Save();

                    try
                    {
                        DataBase.Oportunidades.NotificacionNuevo(modelInsert.IdOportunidad);
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

            return Ok(modelInsert.IdOportunidad);
        }

        [ApiAuthorization]
        [HttpPost]
        public IHttpActionResult Update(List<Models.Oportunidad> oportunidades)
        {
            var agente = DataBase.Agentes.Get(p => p.Usuario.LogonName == CurrentUser.LogonName).FirstOrDefault();

            if (agente != null)
            {
                try
                {
                    foreach (var oportunidad in oportunidades)
                    {
                        var modelUpdate = DataBase.Oportunidades.Get(p => p.IdOportunidad == oportunidad.IdOportunidad, includeProperties: "OportunidadContactos, OportunidadMedias, OportunidadResponsables, OportunidadTareas, OportunidadTareas.OportunidadTareaActividads").FirstOrDefault();

                        if (modelUpdate != null)
                        {
                            Rp3.Data.Service.CopyTo(oportunidad, modelUpdate, includeProperties: new string[] {
                                "Descripcion",
                                "Probabilidad",
                                "Importe",
                                "Estado",
                                "FechaUltimaGestionTicks",
                                "Calificacion",
                                "Observacion",
                                "Direccion",
                                "ReferenciaDireccion",
                                "Telefono1",
                                "Telefono2",
                                "CorreoElectronico",
                                "PaginaWeb",
                                "TipoEmpresa",
                                "Referencia",
                                "Latitud",
                                "Longitud",
                                "IdEtapa",
                                "FechaCreacionTicks"
                            });
                            //modelUpdate.IdAgente = agente.IdAgente;

                            if (oportunidad.OportunidadContactos != null)
                            {
                                foreach(OportunidadContacto contacto in oportunidad.OportunidadContactos)
                                {
                                    contacto.FecMod = GetCurrentDateTime();
                                    contacto.UsrMod = CurrentUser.LogonName;
                                }
                                DataBase.OportunidadContactos.Update(oportunidad.OportunidadContactos, modelUpdate.OportunidadContactos);
                            }

                            if (oportunidad.OportunidadResponsables != null)
                            {
                                DataBase.OportunidadResponsables.Update(oportunidad.OportunidadResponsables, modelUpdate.OportunidadResponsables);
                            }

                            if (oportunidad.OportunidadBitacoras != null)
                            {
                                int idBit = 0;
                                foreach(OportunidadBitacora bitacora in oportunidad.OportunidadBitacoras)
                                {
                                    bool repetida = false;
                                    if (bitacora.IdOportunidadBitacora == 0)
                                    {
                                        //chequeo si esta repetida
                                        foreach (var bita in modelUpdate.OportunidadBitacoras)
                                        {
                                            if (bita.FecIng.ToString("MM/dd/yy H:mm:ss") == bitacora.FecIng.Value.ToString("MM/dd/yy H:mm:ss"))
                                            {
                                                repetida = true;
                                            }
                                        }
                                        if (!repetida)
                                        {
                                            AgendaComercial.Models.Oportunidad.OportunidadBitacora opBit = new AgendaComercial.Models.Oportunidad.OportunidadBitacora();
                                            opBit.Detalle = bitacora.Detalle;
                                            opBit.FecIng = bitacora.FecIng.Value;
                                            opBit.IdAgente = bitacora.IdAgente;
                                            opBit.IdOportunidad = bitacora.IdOportunidad;
                                            if (idBit == 0)
                                            {
                                                opBit.AsignarId();
                                                idBit = opBit.IdOportunidadBitacora;
                                            }
                                            else
                                            {
                                                idBit++;
                                                opBit.IdOportunidadBitacora = idBit;
                                            }
                                            DataBase.OportunidadBitacoras.Insert(opBit);
                                        }
                                    }
                                }
                            }

                            if (oportunidad.OportunidadEtapas != null)
                            {
                                foreach (var etapa in oportunidad.OportunidadEtapas)
                                {
                                    var etapaUpdate = modelUpdate.OportunidadEtapas.Where(p => p.IdEtapa == etapa.IdEtapa).FirstOrDefault();

                                    if (etapaUpdate != null)
                                    {
                                        Rp3.Data.Service.CopyTo(etapa, etapaUpdate);
                                    }
                                }
                            }

                            if (oportunidad.OportunidadTareas != null)
                            {
                                foreach (var tarea in oportunidad.OportunidadTareas)
                                {
                                    var tareaUpdate = modelUpdate.OportunidadTareas.Where(p => p.IdTarea == tarea.IdTarea && p.IdEtapa == tarea.IdEtapa).FirstOrDefault();

                                    if (tareaUpdate != null)
                                    {
                                        Rp3.Data.Service.CopyTo(tarea, tareaUpdate);

                                        var tareaActividades = DataBase.TareaActividades.Get(p => p.IdTarea == tareaUpdate.IdTarea);

                                        tareaUpdate.OportunidadTareaActividads.Clear();

                                        foreach (var act in tareaActividades)
                                        {
                                            var respuesta = tarea.OportunidadTareaActividads.Where(p => p.IdTareaActividad == act.IdTareaActividad).FirstOrDefault();

                                            var actividad = new Rp3.AgendaComercial.Models.Oportunidad.OportunidadTareaActividad()
                                            {
                                                IdOportunidad = tareaUpdate.IdOportunidad,
                                                IdTarea = tareaUpdate.IdTarea,
                                                IdTareaActividad = act.IdTareaActividad,
                                                IdTareaActividadPadre = act.IdTareaActividadPadre,
                                                Descripcion = act.Descripcion,
                                                IdTipoActividad = act.IdTipoActividad,
                                                IdEtapa = tareaUpdate.IdEtapa,
                                                Orden = act.Orden,
                                                Opciones = act.Opciones
                                            };

                                            if (respuesta != null)
                                            {
                                                //actividad.IdTareaOpcion = respuesta.IdTareaOpcion;
                                                actividad.Resultado = respuesta.Resultado;
                                                actividad.ResultadoCodigo = respuesta.ResultadoCodigo;
                                            }

                                            tareaUpdate.OportunidadTareaActividads.Add(actividad);
                                        }
                                    }
                                }
                            }

                            modelUpdate.UsrMod = CurrentUser.LogonName;
                            modelUpdate.FecMod = GetCurrentDateTime();
                            modelUpdate.OportunidadTipo = null;

                            DataBase.Save();

                            //var tip = modelUpdate.ToXml();
                            DataBase.Oportunidades.ExecuteSerializableUpdate(modelUpdate);
                        }
                        //else
                        //    return BadRequest(String.Format("IdRuta:{0} IdAgenda:{1}", agenda.IdRuta, agenda.IdAgenda));
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            return Ok();
        }

        [ApiAuthorization]
        [HttpPost]
        public IHttpActionResult NotificationEveryone(Rp3.AgendaComercial.Web.Services.General.Models.NotificationToUser notification)
        {
            try
            {
                var usuario = DataBase.Agentes.Get(p => p.IdUsuario == CurrentUser.UserId).FirstOrDefault();
                var responsables = DataBase.OportunidadResponsables.Get(p => p.IdOportunidad == notification.IdAgente).ToList();
                string emails = "";
                string footer = String.Format("Enviada por: {0} - el {1:g}", usuario.Descripcion, this.GetCurrentDateTime());
                foreach (var responsable in responsables)
                {
                    if (!String.IsNullOrEmpty(responsable.Agente.Usuario.Contact.Email))
                    {
                        if (!String.IsNullOrEmpty(emails))
                            emails = String.Format("{0};{1}", emails, responsable.Agente.Usuario.Contact.Email);
                        else
                            emails = responsable.Agente.Usuario.Contact.Email;

                        var device = DataBase.Devices.Get(p => p.IdUsuario == responsable.Agente.IdUsuario).FirstOrDefault();

                        if (device != null && !String.IsNullOrEmpty(device.GCMId))
                        {
                            AndroidNotificationPusher.PushNotification(device.GCMId, notification.Titulo, notification.Mensaje, null, footer);
                        }
                    }
                }

                if (!String.IsNullOrEmpty(emails))
                {
                    DataBase.Notificacions.Send(this.ApplicationId, emails, notification.Titulo, String.Format("{0}<br><br>{1}", notification.Mensaje, footer), 105);
                }
            }
            catch(Exception ex)
            {
                return BadRequest(ex.StackTrace);
            }

            return Ok();
        }

        [ApiAuthorization]
        [HttpPost]
        public IHttpActionResult SetOportunidadFoto(Rp3.AgendaComercial.Web.Services.Oportunidad.Models.OportunidadMedia media)
        {
            var mediaUpdate = DataBase.OportunidadMedias.Get(p => p.IdOportunidad == media.IdOportunidad 
                && p.IdMedia == media.IdMedia).SingleOrDefault();
            bool delete = false;
            short resultId = media.IdMedia;

            string deletePath = string.Empty;
            if (mediaUpdate == null)
            {
                mediaUpdate = new AgendaComercial.Models.Oportunidad.OportunidadMedia();
                mediaUpdate.TipoMedia = Constantes.TiposAgendaMedia.Imagenes;
                mediaUpdate.TipoMediaTabla = Constantes.TiposAgendaMedia.Tabla;
                mediaUpdate.Estado = Constantes.Estado.Activo;
                mediaUpdate.EstadoTabla = Constantes.Estado.Tabla;
                mediaUpdate.IdOportunidad = media.IdOportunidad;
                mediaUpdate.IdMedia = media.IdMedia;


                resultId = DataBase.OportunidadMedias.GetMaxValue<short>(p => p.IdMedia, 0,
                    p => p.IdOportunidad == media.IdOportunidad);

                mediaUpdate.IdMedia = ++resultId;

                DataBase.OportunidadMedias.Insert(mediaUpdate);
            }
            else
            {
                delete = true;
                deletePath = mediaUpdate.Path;
                DataBase.OportunidadMedias.Update(mediaUpdate);
            }

            mediaUpdate.FecMod = GetCurrentDateTime();
            mediaUpdate.UsrMod = CurrentUser.LogonName;


            byte[] content = Convert.FromBase64String(media.Contenido);
            var newfileName = String.Format("{0}{1}", Guid.NewGuid(), System.IO.Path.GetExtension(media.Nombre));
            string filePath =
                Path.Combine(HttpRuntime.AppDomainAppPath, AgendaComercial.Models.Constantes.OportunidadMedia.NewFilePath, newfileName);
            Image image;
            using (MemoryStream ms = new MemoryStream(content))
            {
                image = Image.FromStream(ms);
                ms.Flush();
                image.Save(filePath);
            }

            try
            {
                if (delete && !string.IsNullOrWhiteSpace(deletePath))
                {
                    string deleteImagePath = Path.Combine(HttpRuntime.AppDomainAppPath, mediaUpdate.Path);
                    FileInfo FileDetele = new FileInfo(deleteImagePath);
                    if (FileDetele.Exists)
                        FileDetele.Delete();

                }
            }
            catch
            {

            }

            mediaUpdate.Path = Path.Combine(Constantes.OportunidadMedia.FilePath, newfileName);
            DataBase.Save();

            return Ok(newfileName);
        }

        [ApiAuthorization]
        [HttpPost]
        public IHttpActionResult SetOportunidadContactoFoto(Models.OportunidadContactoMedia media)
        {
            byte[] content = Convert.FromBase64String(media.Contenido);
            var newfileName = String.Format("{0}{1}", Guid.NewGuid(), System.IO.Path.GetExtension(media.Nombre));
            string filePath =
                Path.Combine(HttpRuntime.AppDomainAppPath, AgendaComercial.Models.Constantes.OportunidadMedia.NewFilePath, newfileName);

            Image image;
            using (MemoryStream ms = new MemoryStream(content))
            {
                image = Image.FromStream(ms);
                ms.Flush();
                image.Save(filePath);
            }

            var cliente = DataBase.OportunidadContactos.Get(p => p.IdOportunidad == media.IdOportunidad && p.IdOportunidadContacto == media.IdOportunidadContacto).SingleOrDefault();

            if (cliente != null)
            {
                if (!string.IsNullOrEmpty(cliente.Path))
                {
                    string deleteImagePath = Path.Combine(HttpRuntime.AppDomainAppPath, cliente.Path);
                    FileInfo FileDetele = new FileInfo(deleteImagePath);
                    if (FileDetele.Exists)
                        FileDetele.Delete();

                }

                cliente.Path = Path.Combine(Constantes.OportunidadMedia.FilePath, newfileName);
                DataBase.OportunidadContactos.Update(cliente);
                DataBase.Save();
      
            }

            return Ok(newfileName);
        }
    }
}