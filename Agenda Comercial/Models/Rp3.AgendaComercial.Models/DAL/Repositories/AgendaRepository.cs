using Rp3.AgendaComercial.Models.Ruta;
using Rp3.AgendaComercial.Models.Ruta.View;
using Rp3.Data.Entity;
using Rp3.Xml;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rp3.AgendaComercial.Models.Repositories
{
    public class AgendaRepository : Rp3.Data.Entity.Repository<Agenda>
    {
        public AgendaRepository(DbContext context)
            : base(context)
        {
            
        }

        public List<AgendaListado> GetAgendaListadoReporteGestion(List<int> idsRuta, DateTime fechaInicio, DateTime fechaFin)
        {
            Context db = (Context)context;

            var query = db.Agendas.Where(p =>
                idsRuta.Contains(p.Ruta.IdRuta) &&
                p.FechaInicio >= fechaInicio.Date &&
                p.FechaInicio <= fechaFin &&
                p.EstadoAgenda != Rp3.AgendaComercial.Models.Constantes.EstadoAgenda.Eliminado
                ).Select(p => new AgendaListado()
                {
                    idRuta = p.IdRuta,
                    idAgenda = p.IdAgenda,
                    IdProgramacionRuta = p.IdProgramacionRuta,
                    IdAgente = p.IdAgente,
                    Agente = p.Agente.Descripcion,
                    Cliente_Nombre1 = p.ClienteDireccion.Cliente.Nombre1,
                    Cliente_Nombre2 = p.ClienteDireccion.Cliente.Nombre2,
                    Cliente_Apellido1 = p.ClienteDireccion.Cliente.Apellido1,
                    Cliente_Apellido2 = p.ClienteDireccion.Cliente.Apellido2,
                    Contacto_Nombre = p.ClienteContacto.Nombre,
                    Contacto_Apellido = p.ClienteContacto.Apellido,
                    Cargo = p.ClienteContacto.Cargo,
                    fechaInicio = p.FechaInicio.Value,
                    fechaFin = p.FechaFin.Value,
                    fechaInicioGestion = p.FechaInicioGestion.Value,
                    fechaFinGestion = p.FechaFinGestion.Value,
                    Color = p.EstadoAgendaGeneralValue.Reference01,
                    Estado = p.EstadoAgenda,
                    EstadoAgenda = p.EstadoAgendaGeneralValue.Content,
                    Origen = p.Origen,
                    Direccion = p.ClienteDireccion.Direccion,
                    Path = p.ClienteDireccion.Cliente.Foto,
                    Telefono = p.ClienteDireccion.Telefono1,
                    CorreoElectronico = p.ClienteDireccion.Cliente.CorreoElectronico,
                    Latitud = p.ClienteDireccion.Latitud,
                    Longitud = p.ClienteDireccion.Longitud,
                    Motivo = p.MotivoNoGestionGeneralValue != null ? p.MotivoNoGestionGeneralValue.Content : "No visitado",
                    Observacion = string.IsNullOrEmpty(p.Observacion) ? " " : p.Observacion,
                    Fotos = p.AgendaMedias.Select(m => m.Path).ToList(),
                    UsrIng = p.UsrIng,
                    Tareas = p.AgendaTareas
                });

            return query.ToList();
        }

        public List<General.Cliente> GetClientesCreadosReporteGestion(DateTime fechaInicio, DateTime fechaFin, int idAgente)
        {
            Context db = (Context)this.context;
            Models.General.Agente agente = db.Agentes.SingleOrDefault(p => p.IdAgente == idAgente);
            if (agente == null || agente.Usuario == null) return new List<General.Cliente>();
            string logonName = agente.Usuario.LogonName;
            return db.Clientes.Where(p => p.UsrIng == logonName &&
                                          p.Estado != Rp3.AgendaComercial.Models.Constantes.Estado.Eliminado &&
                                          p.FecIng >= fechaInicio &&
                                          p.FecIng <= fechaFin).ToList();
        }


        public List<Agenda> GetAgendaService(int idUsuario, DateTime fechaInicio, DateTime fechaFin, DateTime? ultimaActualizacion)
        {
            Context db = (Context)this.context;
            int? idRuta = db.Agentes.Where(p => p.Usuario.UserId == idUsuario).Select(p => p.IdRuta).FirstOrDefault();
            if (idRuta == null) return new List<Agenda>();

            var query = (from r in db.Rutas
                          join d in db.Agendas.Include("AgendaTareas,ClienteDireccion.Ciudad,ClienteContacto")
                          on r.IdRuta equals d.IdRuta
                          where r.IdRuta == idRuta.Value &&
                          d.FechaInicio >= fechaInicio && d.FechaFin <= fechaFin && d.EstadoAgenda != Rp3.AgendaComercial.Models.Constantes.EstadoAgenda.SinProgramar
                          select d);

            if(ultimaActualizacion.HasValue)
                query = query.Where(p=> (p.FecIng >= ultimaActualizacion.Value || p.FecMod >= ultimaActualizacion.Value) );
            
            return query.ToList();
        }

        public List<Appointment> GetAgendaUsuario(int idUsuario, DateTime fechaInicio, DateTime fechaFin)
        {
            Context db = (Context)context;

            var query = (from a in db.Agendas
                         join r in db.Rutas on a.IdRuta equals r.IdRuta
                         join ag in db.Agentes on r.IdRuta equals ag.IdRuta
                         join g in db.GeneralValues on new { Id = a.EstadoAgendaTabla, Code =  a.EstadoAgenda } equals new { g.Id, g.Code }
                         where ag.IdUsuario == idUsuario
                         && a.FechaInicio >= fechaInicio
                         && a.FechaInicio <= fechaFin
                         && a.EstadoAgenda != Rp3.AgendaComercial.Models.Constantes.EstadoAgenda.Eliminado
                         && a.EstadoAgenda != Rp3.AgendaComercial.Models.Constantes.EstadoAgenda.SinProgramar
                         select new Appointment()
                         {
                             id = a.IdAgenda,
                             Cliente_Nombre1 = a.ClienteDireccion.Cliente.Nombre1,
                             Cliente_Nombre2 = a.ClienteDireccion.Cliente.Nombre2,
                             Cliente_Apellido1 = a.ClienteDireccion.Cliente.Apellido1,
                             Cliente_Apellido2 = a.ClienteDireccion.Cliente.Apellido2,
                             Contacto_Nombre = a.ClienteContacto.Nombre,
                             Contacto_Apellido = a.ClienteContacto.Apellido,
                             Cargo = a.ClienteContacto.Cargo,
                             start = a.FechaInicio.Value,
                             end = a.FechaFin.Value,
                             //ClienteNombre = a.ClienteDireccion.Cliente.Nombre1 + " " + a.ClienteDireccion.Cliente.Apellido1,
                             //ClienteContactoNombre = a.ClienteContacto.Nombre + " " + a.ClienteContacto.Apellido,
                             allDay = false,
                             editable = true,
                             color = g.Reference01,
                             ruta = a.IdRuta
                         });


            return query.GroupBy(i => new { id = i.id, idruta = i.ruta }).Select(g => g.FirstOrDefault()).ToList();            
        }

        public List<AgendaListado> GetAgendaListado(List<int> idsRuta, DateTime fechaInicio, DateTime fechaFin)
        {
            Context db = (Context)context;

            var query = db.Agendas.Where(p=>
                idsRuta.Contains(p.Ruta.IdRuta) &&
                p.FechaInicio >= fechaInicio.Date &&
                p.FechaInicio <= fechaFin &&
                p.EstadoAgenda != Rp3.AgendaComercial.Models.Constantes.EstadoAgenda.Eliminado
                && p.EstadoAgenda != Rp3.AgendaComercial.Models.Constantes.EstadoAgenda.SinProgramar
                ).Select(p => new AgendaListado()
            {
                idRuta = p.IdRuta,
                idAgenda = p.IdAgenda,
                IdProgramacionRuta = p.IdProgramacionRuta,
                IdAgente = p.IdAgente,
                Agente = p.Agente.Descripcion,
                Cliente_Nombre1 = p.ClienteDireccion.Cliente.Nombre1,
                Cliente_Nombre2 = p.ClienteDireccion.Cliente.Nombre2,
                Cliente_Apellido1 = p.ClienteDireccion.Cliente.Apellido1,
                Cliente_Apellido2 = p.ClienteDireccion.Cliente.Apellido2,
                Contacto_Nombre = p.ClienteContacto.Nombre,
                Contacto_Apellido = p.ClienteContacto.Apellido,
                Cargo = p.ClienteContacto.Cargo,
                fechaInicio = p.FechaInicio.Value,
                fechaFin = p.FechaFin.Value,
                fechaInicioGestion = p.FechaInicioGestion.Value,
                fechaFinGestion = p.FechaFinGestion.Value,
                Color = p.EstadoAgendaGeneralValue.Reference01,
                EstadoAgenda = p.EstadoAgendaGeneralValue.Content,
                Origen = p.Origen,
                Direccion = p.ClienteDireccion.Direccion,
                Path = p.ClienteDireccion.Cliente.Foto,
                Telefono = p.ClienteDireccion.Telefono1,
                CorreoElectronico = p.ClienteDireccion.Cliente.CorreoElectronico,
                Latitud = p.ClienteDireccion.Latitud,
                Longitud = p.ClienteDireccion.Longitud
            });           

            return query.ToList();
        }

        public List<AgendaListado> GetAgendaPhotoBook(List<int> idsRuta, DateTime fechaInicio, DateTime fechaFin)
        {
            Context db = (Context)context;

            var query = db.AgendaMedias.Where(p =>
                idsRuta.Contains(p.Agenda.Ruta.IdRuta) &&
                p.Agenda.FechaInicio >= fechaInicio.Date &&
                p.Agenda.FechaInicio <= fechaFin &&
                p.Agenda.EstadoAgenda != Rp3.AgendaComercial.Models.Constantes.EstadoAgenda.Eliminado &&
                p.TipoMedia == Rp3.AgendaComercial.Models.Constantes.TiposAgendaMedia.Imagenes
                ).Select(p => new AgendaListado()
                {
                    idRuta = p.IdRuta,
                    idAgenda = p.IdAgenda,
                    IdAgente = p.Agenda.IdAgente,
                    Agente = p.Agenda.Agente.Descripcion,
                    IdProgramacionRuta = p.Agenda.IdProgramacionRuta,
                    Cliente_Nombre1 = p.Agenda.ClienteDireccion.Cliente.Nombre1,
                    Cliente_Nombre2 = p.Agenda.ClienteDireccion.Cliente.Nombre2,
                    Cliente_Apellido1 = p.Agenda.ClienteDireccion.Cliente.Apellido1,
                    Cliente_Apellido2 = p.Agenda.ClienteDireccion.Cliente.Apellido2,
                    Contacto_Nombre = p.Agenda.ClienteContacto.Nombre,
                    Contacto_Apellido = p.Agenda.ClienteContacto.Apellido,
                    Cargo = p.Agenda.ClienteContacto.Cargo,
                    fechaInicio = p.Agenda.FechaInicio.Value,
                    fechaFin = p.Agenda.FechaFin.Value,
                    fechaInicioGestion = p.Agenda.FechaInicioGestion.Value,
                    fechaFinGestion = p.Agenda.FechaFinGestion.Value,
                    Color = p.Agenda.EstadoAgendaGeneralValue.Reference01,
                    EstadoAgenda = p.Agenda.EstadoAgendaGeneralValue.Content,
                    Origen = p.Agenda.Origen,
                    Direccion = p.Agenda.ClienteDireccion.Direccion,
                    Path = p.Path
                });

            return query.ToList();
        }

        public List<AgendaListado> GetListadoFullTextSearch(List<int> idsRuta, string busqueda, int pagina, int numreg, bool porAgente = false)
        {
            string idRuta = String.Empty;

            foreach (int id in idsRuta)
            {
                if(!String.IsNullOrEmpty(idRuta))
                    idRuta += ",";

                idRuta += Convert.ToString(id);
            }

            return context.Database.SqlQuery<AgendaListado>("dbo.FullTextSearchAgenda @idruta = {0}, @busqueda = {1}, @pagina = {2}, @numreg = {3}, @porAgente = {4}", idRuta, busqueda, pagina, numreg, porAgente).ToList();
        }

        public List<AgendaListado> GetPhotoBookFullTextSearch(List<int> idsRuta, string busqueda, int pagina, int numreg, bool porAgente = false)
        {
            string idRuta = String.Empty;

            foreach (int id in idsRuta)
            {
                if (!String.IsNullOrEmpty(idRuta))
                    idRuta += ",";

                idRuta += Convert.ToString(id);
            }

            return context.Database.SqlQuery<AgendaListado>("dbo.FullTextSearchAgendaMedia @idruta = {0}, @busqueda = {1}, @pagina = {2}, @numreg = {3}, @porAgente = {4}", idRuta, busqueda, pagina, numreg, porAgente).ToList();
        }

        public List<AgendaClientes> GetAgendaDetalle(int idruta, long idagenda)
        {
            Context db = (Context)context;

            var query = (from a in db.Agendas
                         join c in db.Clientes on a.IdCliente equals c.IdCliente
                         join d in db.ClienteDirecciones on new { a.IdCliente, a.IdClienteDireccion } equals new { d.IdCliente, d.IdClienteDireccion }
                         //join e in db.Canales on c.IdCanal equals e.IdCanal
                         where a.IdRuta == idruta
                         && a.IdAgenda == idagenda
                         && a.EstadoAgenda != Rp3.AgendaComercial.Models.Constantes.EstadoAgenda.Eliminado
                         && a.EstadoAgenda != Rp3.AgendaComercial.Models.Constantes.EstadoAgenda.SinProgramar
                         select new AgendaClientes()
                         {
                             idRuta = a.IdRuta,
                             idAgenda = a.IdAgenda,
                             IdAgente = a.IdAgente,
                             Agente = a.Agente.Descripcion,
                             IdProgramacionRuta = a.IdProgramacionRuta,
                             Foto = c.Foto,
                             FotoContacto = a.ClienteContacto.Foto,
                             Cliente_Nombre1 = a.ClienteDireccion.Cliente.Nombre1,
                             Cliente_Nombre2 = a.ClienteDireccion.Cliente.Nombre2,
                             Cliente_Apellido1 = a.ClienteDireccion.Cliente.Apellido1,
                             Cliente_Apellido2 = a.ClienteDireccion.Cliente.Apellido2,
                             Contacto_Nombre = a.ClienteContacto.Nombre,
                             Contacto_Apellido = a.ClienteContacto.Apellido,
                             Cargo = a.ClienteContacto.Cargo,
                             Canal = c.Canal.Descripcion,
                             Telefono = d.Telefono1,
                             Correo = c.CorreoElectronico,
                             Direccion = d.Direccion,
                             FechaInicio = a.FechaInicio,
                             FechaFin = a.FechaFin,

                             FechaInicioGestion = a.FechaInicioGestion,
                             FechaFinGestion = a.FechaFinGestion,

                             Latitud = d.Latitud,
                             Longitud = d.Longitud,
                             Tarea = a.AgendaTareas.Select(p => new ListaTarea { Nombre = p.Tarea.Descripcion, idTarea = p.Tarea.IdTarea, Estado = p.EstadoTarea, EstadoDescripcion = p.EstadoTareaGeneralValue.Content, Actividades = p.AgendaTareaActividades  }).ToList(),
                             Imagen = a.AgendaMedias.Select(q => new ListaIMG { URL = q.Path }).ToList(),
                             Observacion = a.Observacion,
                             Color = a.EstadoAgendaGeneralValue.Reference01,
                             ColorDetalle = a.EstadoAgendaGeneralValue.Content,

                             EstadoAgenda = a.EstadoAgenda,
                             Origen = a.Origen,
                             FechaInicioOriginal = a.FechaInicioOriginal,
                             MotivoNoGestion = a.MotivoNoGestionGeneralValue != null ? a.MotivoNoGestionGeneralValue.Content : null,
                             MotivoReprogramacion = a.MotivoReprogramacionGeneralValue != null ? a.MotivoReprogramacionGeneralValue.Content : null,
                             EsReprogramada = a.EsReprogramada
                         });


            var data = query.ToList();
            return data;
        }

        /*public List<AgendaUpdates> GetVisitaDetalle(int idruta, long idagenda)
        {
            Context db = (Context)context;

            var query = (from a in db.Agendas
                         join c in db.Clientes on a.IdCliente equals c.IdCliente
                         join d in db.ClienteDirecciones on new { a.IdCliente, a.IdClienteDireccion } equals new { d.IdCliente, d.IdClienteDireccion }
                         //join e in db.Canales on c.IdCanal equals e.IdCanal
                         where a.IdRuta == idruta
                         && a.IdAgenda == idagenda
                         select new AgendaUpdates()
                         {
                             IdRuta = a.IdRuta,
                             IdAgenda = a.IdAgenda,
                             Cliente_Nombre1 = a.ClienteDireccion.Cliente.Nombre1,
                             Cliente_Nombre2 = a.ClienteDireccion.Cliente.Nombre2,
                             Cliente_Apellido1 = a.ClienteDireccion.Cliente.Apellido1,
                             Cliente_Apellido2 = a.ClienteDireccion.Cliente.Apellido2,
                             Contacto_Nombre = a.ClienteContacto.Nombre,
                             Contacto_Apellido = a.ClienteContacto.Apellido,                             
                             Direccion = d.Direccion,
                             FechaInicio = a.FechaInicio,
                             Tarea = a.AgendaTareas.Select(p => new ListaTarea { Nombre = p.Tarea.Descripcion, idTarea = p.Tarea.IdTarea }).ToList(),
                             
                         });


            var data = query.ToList();
            return data;
        }*/
        public List<Models.Ruta.View.AgendaTareaActividad> GetAgendaTareas(int idRuta, long idAgenda, long idTarea)
        {
            Context db = (Context)context;

            var query = (from a in db.Agendas
                         join b in db.AgendaTareas on new { a.IdRuta, a.IdAgenda } equals new { b.IdRuta, b.IdAgenda }
                         join e in db.AgendaTareaActividades on new { a.IdRuta, a.IdAgenda, b.IdTarea } equals new { e.IdRuta, e.IdAgenda, e.IdTarea }
                         where a.IdRuta == idRuta
                         && a.IdAgenda == idAgenda
                         && b.IdTarea == idTarea
                         && a.EstadoAgenda != Rp3.AgendaComercial.Models.Constantes.EstadoAgenda.Eliminado
                         && a.EstadoAgenda != Rp3.AgendaComercial.Models.Constantes.EstadoAgenda.SinProgramar
                         select new Models.Ruta.View.AgendaTareaActividad()
                         {
                             IdRuta = a.IdRuta,
                             IdAgenda = a.IdAgenda,
                             IdTarea = b.IdTarea,
                             Pregunta = e.Descripcion,
                             Respuesta = e.Resultado,
                             IdTareaActividadPadre = e.IdTareaActividadPadre,
                             IdTareaActividad = e.IdTareaActividad,
                             IdTipoTarea = b.Tarea.TipoTarea
                         });

            var data = query.ToList();
            return data;
        }

        public void UpdateGeolocation(int idRuta, long idAgenda, double latitud, double longitud, int distancia,
            string usrMod, DateTime fecMod)
        {
            Context db = context as Context;
            string lat = latitud.ToString().Replace(",", ".");
            string lng = longitud.ToString().Replace(",", ".");
            db.Database.ExecuteSqlCommand(String.Format("exec dbo.[spAgendaUpdateGeolocation] {0}, {1}, {2}, {3}, {4}, '{5}', '{6}'",
                idRuta, idAgenda, lat, lng, distancia, usrMod, fecMod.ToString("yyyyMMdd hh:mm:ss")));
        }

       


        #region StorageProperties

        public string[] StorageProperties
        {
            get
            {
                return new String[] { "IdRuta","IdAgenda","IdCliente","IdClienteDireccion","IdProgramacionRuta","FechaInicio","FechaFin","EstadoAgendaTabla","EstadoAgenda","OrigenTabla","Origen","UsrIng","FecIng","UsrMod","FecMod","FechaInicioGestion","FechaFinGestion","Observacion","Latitud","Longitud","IdClienteContacto","MotivoNoGestion","MotivoNoGestionTabla","IdAgente","MotivoReprogramacion", "MotivoReprogramacionTabla", "Duracion", "TiempoViaje","EsReprogramada","FechaInicioOriginal", "FechaFinOriginal",
                "AgendaTareas" };
            }
        }

        public string[] StoragePropertiesDetalle
        {
            get
            {
                return new String[] { "IdRuta","IdAgenda","IdTarea","IdProgramacionTarea","EstadoTareaTabla","EstadoTarea",
                "AgendaTareaActividades" };
            }
        }

        public string[] StoragePropertiesDetalleActividad
        {
            get
            {
                return new String[] { "IdRuta", "IdAgenda", "IdTarea", "IdTareaActividad", "Descripcion", "IdTipoActividad", "Opciones", "Orden", "IdTareaActividadPadre", "IdTareaOpcion", "Resultado" };
            }
        }

        #endregion StorageProperties

        public void InsertXml(Agenda entityToUpdate)
        {
            //base.Update(entityToUpdate);

            Context db = context as Context;

            string infoXml = entityToUpdate.ToXml(new XmlPropertiesInfo("Agenda", typeof(Agenda), StorageProperties),
                       new XmlPropertiesInfo(typeof(Models.Ruta.AgendaTarea), StoragePropertiesDetalle),
                       new XmlPropertiesInfo(typeof(Models.Ruta.AgendaTareaActividad), StoragePropertiesDetalleActividad)
                       );

            db.Database.ExecuteSqlCommand(String.Format("exec spAgendaInsert '{0}'", infoXml));
        }

        public void UpdateXml(Agenda entityToUpdate)
        {
            //base.Update(entityToUpdate);

            Context db = context as Context;

            string infoXml = entityToUpdate.ToXml(new XmlPropertiesInfo("Agenda", typeof(Agenda), StorageProperties),
                       new XmlPropertiesInfo(typeof(Models.Ruta.AgendaTarea), StoragePropertiesDetalle),
                       new XmlPropertiesInfo(typeof(Models.Ruta.AgendaTareaActividad), StoragePropertiesDetalleActividad)
                       );

            db.Database.ExecuteSqlCommand(String.Format("exec spAgendaUpdate '{0}'", infoXml));
        }

        public void ExecuteAutomaticAgendas(int? IdRuta = null)
        {
            Context db = context as Context;
            db.Database.ExecuteSqlCommand("exec dbo.[spEjecutarProgramacionRutaRecurrente] @IdRuta = {0}", IdRuta);
        }
    }
}
