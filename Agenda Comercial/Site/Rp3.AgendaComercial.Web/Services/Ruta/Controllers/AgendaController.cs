using Rp3.Web.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Rp3.Xml;
using System.IO;
using System.Drawing;
using Rp3.Web.Mvc.Utility;
using Rp3.AgendaComercial.Models;
using System.Drawing.Imaging;

namespace Rp3.AgendaComercial.Web.Services.Ruta.Controllers
{
    public class AgendaController : Rp3.Web.Http.BaseApiController<AgendaComercial.Models.ContextService>
    {
        [ApiAuthorization]
        public IHttpActionResult Get(long fechaInicio, long fechaFin, long? ultimaActualizacion = null)
        {
            List<Ruta.Models.Agenda> data = new List<Ruta.Models.Agenda>();
            DateTime fechaInicioDate = new DateTime(fechaInicio);
            DateTime fechaFinDate = new DateTime(fechaFin);           
            DateTime? fechaUltimaActualizacion = null;
            
            if(ultimaActualizacion!=null)
            {
                fechaUltimaActualizacion = new DateTime(ultimaActualizacion.Value);
            }

            var values = DataBase.Agendas.GetAgendaService(CurrentUser.UserId, fechaInicioDate, fechaFinDate, fechaUltimaActualizacion);            

            Assign(values, data);

            foreach (var ag in values)
            {
                Models.Agenda agenda = data.SingleOrDefault(p => p.IdRuta == ag.IdRuta && p.IdAgenda == ag.IdAgenda);

                if (ag.ClienteDireccion != null)
                {
                    agenda.NombresCompletos = ag.ClienteDireccion.Cliente.NombresCompletos;
                    agenda.Direccion = ag.ClienteDireccion.Direccion;
                    if (ag.ClienteDireccion.Ciudad != null)
                    {
                        agenda.Ciudad = ag.ClienteDireccion.Ciudad.Name;
                    }
                }

                if (ag.ClienteContacto != null)
                    agenda.ContactoNombresCompletos = ag.ClienteContacto.NombresCompletos;

                foreach (var atarea in ag.AgendaTareas)
                {
                    Models.AgendaTarea tarea = agenda.AgendaTareas.SingleOrDefault(p => p.IdTarea == atarea.IdTarea);
                    tarea.Nombre = atarea.Tarea.Descripcion;
                    tarea.TipoTarea = atarea.Tarea.TipoTarea;
                }
            }            

            return Ok(data);
        }

        [ApiAuthorization]
        [HttpPost]
        public IHttpActionResult SetMotivoNoGestion(List<Models.AgendaNoGestion> agendasNoGestion)
        {
            foreach(var agenda in agendasNoGestion)
            {
                var agendaModel = DataBase.Agendas.Get(p =>
                p.IdRuta == agenda.IdRuta &&
                p.IdAgenda == agenda.IdAgenda).FirstOrDefault();

                agendaModel.MotivoNoGestion = agenda.MotivoNoGestion;
                agendaModel.MotivoNoGestionTabla = Constantes.MotivosNoGestion.Tabla;

                agendaModel.Observacion = agenda.Observacion;
                agendaModel.EstadoAgenda = Constantes.EstadoAgenda.NoVisitado;

                agendaModel.FecMod = this.GetCurrentDateTime();
                agendaModel.UsrMod = CurrentUser.LogonName;

                DataBase.Agendas.Update(agendaModel);

                DataBase.Save();
            }

            return Ok();
        }

        [ApiAuthorization]
        [HttpPost]
        public IHttpActionResult SetAgendaMedia(Models.AgendaMedia media)
        {
            var mediaUpdate = DataBase.AgendaMedias.Get(p => p.IdAgenda == media.IdAgenda && p.IdRuta == media.IdRuta
                && p.IdMedia == media.IdMedia).SingleOrDefault();
            bool delete = false;
            short resultId = media.IdMedia;


            string deletePath = string.Empty;
            if (mediaUpdate == null)
            {
                mediaUpdate = new AgendaComercial.Models.Ruta.AgendaMedia();
                mediaUpdate.TipoMedia = Constantes.TiposAgendaMedia.Imagenes;
                mediaUpdate.TipoMediaTabla = Constantes.TiposAgendaMedia.Tabla;
                mediaUpdate.Estado = Constantes.Estado.Activo;
                mediaUpdate.EstadoTabla = Constantes.Estado.Tabla;
                mediaUpdate.IdAgenda = media.IdAgenda;
                mediaUpdate.IdRuta = media.IdRuta;
                mediaUpdate.IdMedia = media.IdMedia;


                resultId = DataBase.AgendaMedias.GetMaxValue<short>(p => p.IdMedia, 0,
                    p => p.IdRuta == media.IdRuta && p.IdAgenda == media.IdAgenda);

                if (resultId == 3)
                    return Ok(mediaUpdate);

                mediaUpdate.IdMedia = ++resultId;

                DataBase.AgendaMedias.Insert(mediaUpdate);
            }
            else
            {
                delete = true;
                deletePath = mediaUpdate.Path;
                DataBase.AgendaMedias.Update(mediaUpdate);
            }

            mediaUpdate.FecMod = GetCurrentDateTime();
            mediaUpdate.UsrMod = CurrentUser.LogonName;


            byte[] content = Convert.FromBase64String(media.Contenido);
            var newfileName = String.Format("{0}{1}", Guid.NewGuid(), System.IO.Path.GetExtension(media.Nombre));
            string filePath =
                Path.Combine(System.Web.HttpContext.Current.Server.MapPath(AgendaComercial.Models.Constantes.AgendaMedia.FilePath), newfileName);

            Image image;
            using (MemoryStream ms = new MemoryStream(content))
            {
                image = Image.FromStream(ms);
                ms.Flush();
                image.Save(filePath);
            }

            PropertyItem property = null;
            double proportion = 1;
            string bigger = "h";
            try
            {
                FileInfo fileDelete = new FileInfo(filePath);

                Bitmap src = Image.FromFile(filePath) as Bitmap;
                if (src.Height > src.Width)
                {
                    proportion = (double)src.Height / (double)src.Width;
                }
                else
                {
                    proportion = (double)src.Width / (double)src.Height;
                    bigger = "w";
                }

                property = src.GetPropertyItem(0x112);

                if (property.Value[0] == 6) //90
                    src.RotateFlip(RotateFlipType.Rotate90FlipNone);
                else if (property.Value[0] == 8)//-90
                    src.RotateFlip(RotateFlipType.Rotate90FlipX);
                else if (property.Value[0] == 3)//180
                    src.RotateFlip(RotateFlipType.Rotate180FlipNone);

                newfileName = String.Format("{0}{1}", Guid.NewGuid(), System.IO.Path.GetExtension(media.Nombre));
                filePath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(Url.Content(AgendaComercial.Models.Constantes.AgendaMedia.FilePath)), newfileName);

                src.Save(filePath);

                fileDelete.Delete();
            }
            catch
            {
            }


            if (bigger == "h")
            {
                Thumbnail.SaveThumbnail(filePath, Constantes.ProfileFotosSize.ProfilePictuteMinWidth, (int)(Math.Round(Constantes.ProfileFotosSize.ProfilePictuteMinHeight * proportion)), "min");
                Thumbnail.SaveThumbnail(filePath, Constantes.ProfileFotosSize.ProfilePictuteMedWidth, (int)(Math.Round(Constantes.ProfileFotosSize.ProfilePictuteMedHeight * proportion)), "med");
                Thumbnail.SaveThumbnail(filePath, Constantes.ProfileFotosSize.ProfilePictuteSmaWidth, (int)(Math.Round(Constantes.ProfileFotosSize.ProfilePictuteSmaHeight * proportion)), "sma");
            }
            else
            {
                Thumbnail.SaveThumbnail(filePath, (int)(Math.Round(Constantes.ProfileFotosSize.ProfilePictuteMinWidth * proportion)), Constantes.ProfileFotosSize.ProfilePictuteMinHeight, "min");
                Thumbnail.SaveThumbnail(filePath, (int)(Math.Round(Constantes.ProfileFotosSize.ProfilePictuteMedWidth * proportion)), Constantes.ProfileFotosSize.ProfilePictuteMedHeight, "med");
                Thumbnail.SaveThumbnail(filePath, (int)(Math.Round(Constantes.ProfileFotosSize.ProfilePictuteSmaWidth * proportion)), Constantes.ProfileFotosSize.ProfilePictuteSmaHeight, "sma");
            }




            try
            {
                if (delete && !string.IsNullOrWhiteSpace(deletePath))
                {
                    string deleteImagePath = System.Web.HttpContext.Current.Server.MapPath(mediaUpdate.Path);
                    FileInfo FileDetele = new FileInfo(deleteImagePath);
                    if (FileDetele.Exists)
                        FileDetele.Delete();

                    deleteImagePath = System.Web.HttpContext.Current.Server.MapPath(mediaUpdate.PathMedium);
                    FileDetele = new FileInfo(deleteImagePath);
                    if (FileDetele.Exists)
                        FileDetele.Delete();

                    deleteImagePath = System.Web.HttpContext.Current.Server.MapPath(mediaUpdate.PathSmall);
                    FileDetele = new FileInfo(deleteImagePath);
                    if (FileDetele.Exists)
                        FileDetele.Delete();

                    deleteImagePath = System.Web.HttpContext.Current.Server.MapPath(mediaUpdate.PathMin);
                    FileDetele = new FileInfo(deleteImagePath);
                    if (FileDetele.Exists)
                        FileDetele.Delete();
                }
            }
            catch
            {

            }

            mediaUpdate.Path = Path.Combine(Constantes.AgendaMedia.FilePath, newfileName);
            DataBase.Save();


            return Ok(mediaUpdate);
        }

        [ApiAuthorization]
        [HttpPost]
        public IHttpActionResult DeleteAgendaMedia(Models.AgendaMedia media)
        {
            var mediaDelete = DataBase.AgendaMedias.Get(p => p.IdAgenda == media.IdAgenda && p.IdRuta == media.IdRuta
                   && p.IdMedia == media.IdMedia).SingleOrDefault();
            
            if(mediaDelete!=null && !string.IsNullOrWhiteSpace(mediaDelete.Path))
            {
                string deleteImagePath = System.Web.HttpContext.Current.Server.MapPath(mediaDelete.Path);
                FileInfo FileDetele = new FileInfo(deleteImagePath);
                if (FileDetele.Exists)
                    FileDetele.Delete();

                deleteImagePath = System.Web.HttpContext.Current.Server.MapPath(mediaDelete.PathMedium);
                FileDetele = new FileInfo(deleteImagePath);
                if (FileDetele.Exists)
                    FileDetele.Delete();

                deleteImagePath = System.Web.HttpContext.Current.Server.MapPath(mediaDelete.PathSmall);
                FileDetele = new FileInfo(deleteImagePath);
                if (FileDetele.Exists)
                    FileDetele.Delete();

                deleteImagePath = System.Web.HttpContext.Current.Server.MapPath(mediaDelete.PathMin);
                FileDetele = new FileInfo(deleteImagePath);
                if (FileDetele.Exists)
                    FileDetele.Delete();
            }
            mediaDelete.Estado = Constantes.Estado.Eliminado;
            DataBase.AgendaMedias.Update(mediaDelete);

            return Ok();
        }

        [ApiAuthorization]
        [HttpPost]
        public IHttpActionResult Update(List<Models.AgendaUpdate> agendas)
        {
            var agente = DataBase.Agentes.Get(p => p.Usuario.LogonName == CurrentUser.LogonName).FirstOrDefault();
            
            if (agente != null)
            {
                try
                {
                    foreach (var agenda in agendas)
                    {
                        var modelUpdate = DataBase.Agendas.Get(p => p.IdRuta == agenda.IdRuta && p.IdAgenda == agenda.IdAgenda, includeProperties: "AgendaTareas, AgendaTareas.AgendaTareaActividades").FirstOrDefault();

                        modelUpdate.IdAgente = agente.IdAgente;

                        if (modelUpdate != null)
                        {
                            Rp3.Data.Service.CopyTo(agenda, modelUpdate, includeProperties: new string[] {
                                "IdClienteContacto",
                                "EstadoAgenda",
                                "FechaInicioGestionTicks",
                                "FechaFinGestionTicks",
                                "Observacion",
                                "Latitud",
                                "Longitud",
                                "MotivoReprogramacion",
                                "MotivoReprogramacionTabla",
                                "Duracion",
                                "TiempoViaje",
                                "DistanciaUbicacion"
                                });
                            

                            if (agenda.AgendaTareas != null)
                            {
                                foreach (var tarea in agenda.AgendaTareas)
                                {
                                    var tareaUpdate = modelUpdate.AgendaTareas.Where(p => p.IdTarea == tarea.IdTarea).FirstOrDefault();

                                    if (tareaUpdate != null)
                                    {
                                        Rp3.Data.Service.CopyTo(tarea, tareaUpdate, includeProperties: new string[] { "EstadoTarea" });

                                        var tareaActividades = DataBase.TareaActividades.Get(p => p.IdTarea == tareaUpdate.IdTarea);

                                        tareaUpdate.AgendaTareaActividades.Clear();

                                        foreach (var act in tareaActividades)
                                        {
                                            var respuesta = tarea.AgendaTareaActividades.Where(p => p.IdTareaActividad == act.IdTareaActividad).FirstOrDefault();

                                            var actividad = new Rp3.AgendaComercial.Models.Ruta.AgendaTareaActividad()
                                            {
                                                IdRuta = modelUpdate.IdRuta,
                                                IdAgenda = modelUpdate.IdAgenda,
                                                IdTarea = tareaUpdate.IdTarea,
                                                IdTareaActividad = act.IdTareaActividad,
                                                IdTareaActividadPadre = act.IdTareaActividadPadre,
                                                Descripcion = act.Descripcion,
                                                IdTipoActividad = act.IdTipoActividad,
                                                Orden = act.Orden,
                                                Opciones = act.Opciones
                                            };

                                            if (respuesta != null)
                                            {
                                                actividad.IdTareaOpcion = respuesta.IdTareaOpcion;
                                                actividad.Resultado = respuesta.Resultado;
                                                actividad.ResultadoCodigo = respuesta.ResultadoCodigo;
                                            }

                                            tareaUpdate.AgendaTareaActividades.Add(actividad);
                                        }
                                        

                                        //if (tarea.AgendaTareaActividades != null)
                                        //{
                                        //    foreach (var actividad in tarea.AgendaTareaActividades)
                                        //    {
                                        //        var actividadUpdate = tareaUpdate.AgendaTareaActividades.Where(p => p.IdTarea == tarea.IdTarea && p.IdTareaActividad == actividad.IdTareaActividad).FirstOrDefault();

                                        //        if (actividadUpdate != null)
                                        //        {
                                        //            Rp3.Data.Service.CopyTo(actividad, actividadUpdate, includeProperties: new string[] { "IdTareaOpcion", "Resultado" });
                                        //        }
                                        //    }
                                        //}
                                    }
                                }
                            }

                            modelUpdate.UsrMod = CurrentUser.LogonName;
                            modelUpdate.FecMod = GetCurrentDateTime();

                            DataBase.Agendas.ExecuteSerializableUpdate(modelUpdate);
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
        public IHttpActionResult UpdateGeolocation(List<Models.AgendaUpdate> agendas)
        {
            var agente = DataBase.Agentes.Get(p => p.Usuario.LogonName == CurrentUser.LogonName).FirstOrDefault();

            if (agente != null)
            {
                try
                {
                    foreach (var agenda in agendas)
                    {
                        var modelUpdate = DataBase.Agendas.Get(p => p.IdRuta == agenda.IdRuta && p.IdAgenda == agenda.IdAgenda).FirstOrDefault();

                        if (modelUpdate != null)
                        {
                            modelUpdate.IdAgente = agente.IdAgente;
                            modelUpdate.Latitud = agenda.Latitud;
                            modelUpdate.Longitud = agenda.Longitud;
                            modelUpdate.DistanciaUbicacion = agenda.DistanciaUbicacion;
                            modelUpdate.UsrMod = CurrentUser.LogonName;
                            modelUpdate.FecMod = GetCurrentDateTime();

                            DataBase.Agendas.UpdateGeolocation(modelUpdate.IdRuta, modelUpdate.IdAgenda, modelUpdate.Latitud.Value, modelUpdate.Longitud.Value,
                                modelUpdate.DistanciaUbicacion.Value, modelUpdate.UsrMod, modelUpdate.FecMod.Value);
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
        public IHttpActionResult Reprogramar(List<Models.AgendaReprogramar> agendas)
        {
            var agente = DataBase.Agentes.Get(p => p.Usuario.LogonName == CurrentUser.LogonName).FirstOrDefault();

            string infoXml = String.Empty;

            if (agente != null)
            {
                try
                {
                    foreach (var agenda in agendas)
                    {
                        var modelUpdate = DataBase.Agendas.Get(p => p.IdRuta == agenda.IdRuta && p.IdAgenda == agenda.IdAgenda, includeProperties: "AgendaTareas, AgendaTareas.AgendaTareaActividades").FirstOrDefault();

                        if (modelUpdate != null)
                        {
                            Rp3.Data.Service.CopyTo(agenda, modelUpdate, includeProperties: new string[] {
                            "IdClienteContacto",
                            "EstadoAgenda",
                            "FechaInicioTicks",
                            "FechaFinTicks",
                            "MotivoReprogramacion",
                            "MotivoReprogramacionTabla",
                            "Duracion",
                            "TiempoViaje"
                            //"Observacion"
                            });

                            modelUpdate.UsrMod = CurrentUser.LogonName;
                            modelUpdate.FecMod = GetCurrentDateTime();
                            modelUpdate.EsReprogramada = true;

                            DataBase.Agendas.ExecuteSerializableUpdate(modelUpdate);
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
        public IHttpActionResult Insert(Models.Agenda agenda)
        {                        
            var agente = DataBase.Agentes.Get(p => p.Usuario.LogonName == CurrentUser.LogonName).FirstOrDefault();
            string infoXml = String.Empty;
                                    
            if (agente != null)
            {
                try
                {
                    AgendaComercial.Models.Ruta.Ruta ruta = DataBase.Rutas.GetRutaOAsignar(agente.IdAgente);
                    agente.IdRuta = ruta.IdRuta;
                    
                    //foreach (var agenda in agendas)
                    //{
                    var modelInsert = new Rp3.AgendaComercial.Models.Ruta.Agenda() {  AgendaTareas = new List<AgendaComercial.Models.Ruta.AgendaTarea>() };

                        Rp3.Data.Service.CopyTo(agenda, modelInsert, includeProperties: new string[] {
                        "IdCliente",
                        "IdClienteDireccion",
                        "IdClienteContacto",
                        "FechaInicioTicks",
                        "FechaFinTicks",
                        "Observacion",
                        "MotivoReprogramacion",
                        "MotivoReprogramacionTabla",
                        "Duracion",
                        "TiempoViaje",
                        "DistanciaUbicacion",
                        "FechaInicioOriginalTicks",
                        "FechaFinOriginalTicks"
                        });

                        modelInsert.IdRuta = agente.IdRuta ?? 0;
                        modelInsert.IdAgente = agente.IdAgente;

                        modelInsert.EstadoAgendaTabla = AgendaComercial.Models.Constantes.EstadoAgenda.Tabla;
                        modelInsert.EstadoAgenda = AgendaComercial.Models.Constantes.EstadoAgenda.Pendiente;
                        modelInsert.OrigenTabla = AgendaComercial.Models.Constantes.OrigenAgenda.Tabla;
                        modelInsert.Origen = AgendaComercial.Models.Constantes.OrigenAgenda.Movil;
                        modelInsert.UsrIng = CurrentUser.LogonName;
                        modelInsert.FecIng = GetCurrentDateTime();

                        modelInsert.AsignarId();

                        if (agenda.AgendaTareas != null)
                        {
                            foreach (var tarea in agenda.AgendaTareas)
                            {
                                modelInsert.AgendaTareas.Add(new AgendaComercial.Models.Ruta.AgendaTarea()
                                {
                                    IdRuta = modelInsert.IdRuta,
                                    IdAgenda = modelInsert.IdAgenda,
                                    IdTarea = tarea.IdTarea,
                                    EstadoTareaTabla = AgendaComercial.Models.Constantes.EstadoTarea.Tabla,
                                    EstadoTarea = AgendaComercial.Models.Constantes.EstadoTarea.Pendiente,
                                });                                
                            }
                        }

                        DataBase.Agendas.Insert(modelInsert);
                    //}

                    DataBase.Save();

                    return Ok(modelInsert.IdAgenda);
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
        public IHttpActionResult UpdateFull(List<Models.AgendaUpdate> agendas)
        {
            var agente = DataBase.Agentes.Get(p => p.Usuario.LogonName == CurrentUser.LogonName).FirstOrDefault();
            Ruta.Models.AgendaUpdateResponse response = new Models.AgendaUpdateResponse() { Result = new List<Models.AgendaResult>() };

            if (agente != null)
            {                                                          
                foreach (var agenda in agendas)                
                {
                    Rp3.AgendaComercial.Models.Ruta.Agenda modelInsert = null;
                    
                    try
                    {
                        bool insert = agenda.IdAgenda == 0;

                        AgendaComercial.Models.Ruta.Ruta ruta = DataBase.Rutas.GetRutaOAsignar(agente.IdAgente);
                        agente.IdRuta = ruta.IdRuta;

                        if (insert)
                        {
                            modelInsert = new Rp3.AgendaComercial.Models.Ruta.Agenda() { AgendaTareas = new List<AgendaComercial.Models.Ruta.AgendaTarea>() };
                            modelInsert.IdRuta = ruta.IdRuta;
                            modelInsert.OrigenTabla = AgendaComercial.Models.Constantes.OrigenAgenda.Tabla;
                            modelInsert.Origen = AgendaComercial.Models.Constantes.OrigenAgenda.Movil;
                            modelInsert.IdCliente = agenda.IdCliente;
                            modelInsert.IdClienteDireccion = agenda.IdClienteDireccion;
                        }
                        else
                            modelInsert = DataBase.Agendas.Get(p => p.IdRuta == agente.IdRuta && p.IdAgenda == agenda.IdAgenda).SingleOrDefault();

                        modelInsert.EstadoAgenda = agenda.EstadoAgenda;
                        modelInsert.FechaFinGestionTicks = agenda.FechaFinGestionTicks;
                        modelInsert.FechaInicioGestionTicks = agenda.FechaInicioGestionTicks;

                        modelInsert.FechaFinTicks = agenda.FechaFinTicks;
                        modelInsert.FechaInicioTicks = agenda.FechaInicioTicks;

                        modelInsert.IdAgenda = agenda.IdAgenda;

                        if (modelInsert.IdClienteContacto.HasValue && modelInsert.IdClienteContacto != 0)
                            modelInsert.IdClienteContacto = agenda.IdClienteContacto;
                        else
                            modelInsert.IdClienteContacto = null;
                        
                        modelInsert.Latitud = agenda.Latitud;
                        modelInsert.Longitud = agenda.Longitud;
                        modelInsert.MotivoNoGestion = agenda.MotivoNoGestion;
                        modelInsert.Observacion = agenda.Observacion;
                        modelInsert.MotivoReprogramacion = agenda.MotivoReprogramacion;
                        if (agenda.MotivoReprogramacion != null && agenda.MotivoReprogramacion != "0")
                            modelInsert.EsReprogramada = true;
                        modelInsert.MotivoReprogramacionTabla = agenda.MotivoReprogramacionTabla;
                        modelInsert.Duracion = agenda.Duracion;
                        modelInsert.DistanciaUbicacion = agenda.DistanciaUbicacion;
                        modelInsert.TiempoViaje = agenda.TiempoViaje;
                        modelInsert.FechaFinOriginal = agenda.FechaFinOriginal;
                        modelInsert.FechaInicioOriginal = agenda.FechaInicioOriginal;

                        modelInsert.UsrIng = CurrentUser.LogonName;
                        modelInsert.UsrMod = CurrentUser.LogonName;
                        modelInsert.FecMod = GetCurrentDateTime();

                        modelInsert.IdAgente = agente.IdAgente;

                        modelInsert.MotivoNoGestionTabla = AgendaComercial.Models.Constantes.MotivosNoGestion.Tabla;
                        modelInsert.EstadoAgendaTabla = AgendaComercial.Models.Constantes.EstadoAgenda.Tabla;

                        modelInsert.AgendaTareas.Clear();

                        if (insert)
                        {
                            modelInsert.AsignarId();
                            modelInsert.FecIng = GetCurrentDateTime();
                            modelInsert.FecMod = modelInsert.FecIng;
                        }

                        if (agenda.AgendaTareas != null)
                        {
                            foreach (var tarea in agenda.AgendaTareas)
                            {
                                AgendaComercial.Models.Ruta.AgendaTarea agendaTarea = new AgendaComercial.Models.Ruta.AgendaTarea()
                                {
                                    IdRuta = modelInsert.IdRuta,
                                    IdAgenda = modelInsert.IdAgenda,
                                    IdTarea = tarea.IdTarea,
                                    EstadoTareaTabla = AgendaComercial.Models.Constantes.EstadoTarea.Tabla,
                                    EstadoTarea = tarea.EstadoTarea,
                                    AgendaTareaActividades = new List<AgendaComercial.Models.Ruta.AgendaTareaActividad>()
                                };
                                agendaTarea.Tarea = DataBase.Tareas.GetSingleOrDefault(p => p.IdTarea == tarea.IdTarea);
                                modelInsert.AgendaTareas.Add(agendaTarea);

                                //Si se envian las actividades entonces se crean a partir de la definición de la base de datos
                                if (tarea.AgendaTareaActividades != null && tarea.AgendaTareaActividades.Any())
                                {
                                    var tareaActividades = DataBase.TareaActividades.Get(p => p.IdTarea == tarea.IdTarea);

                                    foreach (var act in tareaActividades)
                                    {
                                        var respuesta = tarea.AgendaTareaActividades.Where(p => p.IdTareaActividad == act.IdTareaActividad).FirstOrDefault();

                                        var actividad = new Rp3.AgendaComercial.Models.Ruta.AgendaTareaActividad()
                                        {
                                            IdAgenda = modelInsert.IdAgenda,
                                            IdRuta = modelInsert.IdRuta,
                                            IdTarea = act.IdTarea,
                                            IdTareaActividad = act.IdTareaActividad,
                                            IdTareaActividadPadre = act.IdTareaActividadPadre,
                                            Descripcion = act.Descripcion,
                                            IdTipoActividad = act.IdTipoActividad,
                                            Orden = act.Orden,
                                            Opciones = act.Opciones                                            
                                        };

                                        if (respuesta != null)
                                        {
                                            actividad.IdTareaOpcion = respuesta.IdTareaOpcion;
                                            actividad.Resultado = respuesta.Resultado;
                                            actividad.ResultadoCodigo = respuesta.ResultadoCodigo;
                                        }

                                        agendaTarea.AgendaTareaActividades.Add(actividad);
                                    }
                                }

                                
                            }
                        }

                        if (insert)
                            DataBase.Agendas.ExecuteSerializableInsert(modelInsert);
                        else
                            DataBase.Agendas.ExecuteSerializableUpdate(modelInsert);

                        response.Result.Add(new Models.AgendaResult()
                        {
                            IdAgendaServer = modelInsert.IdAgenda,
                            IdRutaServer = modelInsert.IdRuta,
                            IdInterno = agenda.IdInterno,
                            Ok = true
                        });

                    }
                    catch(Exception e)
                    {
                        response.Result.Add(new Models.AgendaResult()
                        {                            
                            IdInterno = agenda.IdInterno,
                            Ok = false,
                            Error = e.Message
                        });
                    }                    
                }                        
            }
            return Ok(response);       
        }
    }
}