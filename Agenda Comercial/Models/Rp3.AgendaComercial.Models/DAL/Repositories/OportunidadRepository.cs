using Rp3.AgendaComercial.Models.Oportunidad.View;
using Rp3.AgendaComercial.Models.Ruta.View;
using Rp3.Data.Entity;
using Rp3.Xml;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rp3.AgendaComercial.Models.Repositories
{
    public class OportunidadRepository : Rp3.Data.Entity.Repository<Oportunidad.Oportunidad>
    {
        public OportunidadRepository(DbContext context)
            : base(context)
        {
            
        }

        public void NotificaciónAtrasada(int IdOportunidad)
        {
            Context db = context as Context;

            db.Database.ExecuteSqlCommand(String.Format("exec [not].[spSendMailOportunidadAtrasada] {0}", IdOportunidad));
        }

        public void NotificacionNuevo(int IdOportunidad)
        {
            Context db = context as Context;

            db.Database.ExecuteSqlCommand(String.Format ("exec [not].[spSendMailNuevaOportunidad] {0}", IdOportunidad));
        }

        public List<OportunidadListado> GetOportunidadListadoReporteGestion(int idAgente, DateTime fechaInicio, DateTime fechaFin)
        {
            Context db = (Context)context;

            var query = db.Oportunidades.Where(p =>
                p.IdAgente == idAgente &&
                p.FecIng >= fechaInicio &&
                p.FecIng <= fechaFin &&
                p.Estado != Models.Constantes.EstadoOportunidad.Eliminado
                ).Select(p => new OportunidadListado()
                {
                    IdOportunidad = p.IdOportunidad,
                    Descripcion = p.Descripcion,
                    Probabilidad = p.Probabilidad,
                    Importe = p.Importe,
                    IdAgente = p.IdAgente,
                    Agente = p.Agente.Descripcion,
                    FechaUltimaGestion = p.FechaUltimaGestion,
                    Calificacion = p.Calificacion,
                    Observacion = p.Observacion,
                    Direccion = p.Direccion,
                    Referencia = p.Referencia,
                    Latitud = p.Latitud,
                    Longitud = p.Longitud,
                    IdEtapa = p.IdEtapa,
                    Etapa = p.Etapa.Descripcion,
                    EtapaOrden = p.Etapa.Orden,
                    Estado = p.Estado,
                    EstadoDescripcion = p.EstadoGeneralValue.Content,
                    FechaInicio = p.FecIng,
                    IdProspecto = p.OportunidadContactos.Where(q => q.EsPrincipal).FirstOrDefault().IdOportunidadContacto,
                    Prospecto = p.OportunidadContactos.Where(q => q.EsPrincipal).FirstOrDefault().Nombre,
                    PaginaWeb = p.PaginaWeb,
                    TipoEmpresa = p.TipoEmpresa,
                    IdOportunidadTipo = p.IdOportunidadTipo
                });

            return query.OrderByDescending(p => p.Importe).ToList();
        }


        public List<OportunidadListado> GetOportunidadListado(List<int> idsAgentes, DateTime fechaInicio, DateTime fechaFin, string estado)
        {
            Context db = (Context)context;

            var query = db.Oportunidades.Where(p =>
                p.OportunidadResponsables.Where(q => idsAgentes.Contains(q.IdAgente)).Count() > 0 &&
                p.FecIng >= fechaInicio.Date &&
                p.FecIng <= fechaFin &&
                (String.IsNullOrEmpty(estado) || p.Estado == estado) &&
                p.Estado != Models.Constantes.EstadoOportunidad.Eliminado
                ).Select(p => new OportunidadListado()
                {
                    IdOportunidad = p.IdOportunidad,
                    Descripcion = p.Descripcion,
                    Probabilidad = p.Probabilidad,
                    Importe = p.Importe,
                    IdAgente = p.IdAgente,
                    Agente = p.Agente.Descripcion,
                    FechaUltimaGestion = p.FechaUltimaGestion,
                    Calificacion = p.Calificacion,
                    Observacion = p.Observacion,
                    Direccion = p.Direccion,
                    Referencia = p.Referencia,
                    Latitud = p.Latitud,
                    Longitud = p.Longitud,
                    IdEtapa = p.IdEtapa,
                    Etapa = p.Etapa.Descripcion,
                    EtapaOrden = p.Etapa.Orden,
                    Estado = p.Estado,
                    EstadoDescripcion = p.EstadoGeneralValue.Content,
                    FechaInicio = p.FecIng,
                    IdProspecto = p.OportunidadContactos.Where(q=> q.EsPrincipal).FirstOrDefault().IdOportunidadContacto,
                    Prospecto = p.OportunidadContactos.Where(q=> q.EsPrincipal).FirstOrDefault().Nombre,
                    PaginaWeb = p.PaginaWeb,
                    TipoEmpresa = p.TipoEmpresa,
                    IdOportunidadTipo = p.IdOportunidadTipo
                });

            return query.ToList();
        }

        public List<OportunidadListado> GetListadoFullTextSearch(List<int> idsAgentes, string busqueda, int pagina, int numreg, string estado)
        {
            string idAgente = String.Empty;

            foreach (int id in idsAgentes)
            {
                if (!String.IsNullOrEmpty(idAgente))
                    idAgente += ",";

                idAgente += Convert.ToString(id);
            }

            return context.Database.SqlQuery<OportunidadListado>("dbo.FullTextSearchOportunidad @idagente = {0}, @busqueda = {1}, @pagina = {2}, @numreg = {3}, @estado = {4}", idAgente, busqueda, pagina, numreg, estado).ToList();
        }

        public List<Oportunidad.Oportunidad> GetOportunidadService(int idUsuario, DateTime? ultimaActualizacion)
        {
            Context db = (Context)this.context;

            var query = (from r in db.Oportunidades.Include("OportunidadContactos,OportunidadMedias,OportunidadTareas,OportunidadResponsables,OportunidadBitacoras")
                         join d in db.OportunidadResponsables
                         on r.IdOportunidad equals d.IdOportunidad
                         where d.IdAgente == idUsuario
                         select r);

            if (ultimaActualizacion.HasValue)
                query = query.Where(p => (p.FecIng >= ultimaActualizacion.Value || p.FecMod >= ultimaActualizacion.Value));

            return query.ToList();
        }

        public List<Oportunidad.Oportunidad> GetOportunidadServiceSupervisor(int idUsuario, DateTime? ultimaActualizacion)
        {
            Context db = (Context)this.context;

            List<int> ids = context.Database.SqlQuery<int>("opt.spOportunidadesSupervisores @IdAgenteSupervisor = @IdAgenteSupervisorParametro",
               new SqlParameter("@IdAgenteSupervisorParametro", idUsuario)).ToList();

            var query = db.Oportunidades.Where(p => ids.Contains(p.IdOportunidad));

            if (ultimaActualizacion.HasValue)
                query = query.Where(p => (p.FecIng >= ultimaActualizacion.Value || p.FecMod >= ultimaActualizacion.Value));

            return query.ToList();
        }

        public OportunidadListado GetOportunidadDetalle(int idOportunidad)
        {
            Context db = (Context)context;

            var query = db.Oportunidades.Where(p =>p.IdOportunidad == idOportunidad).Select(p => new OportunidadListado()
                {
                    IdOportunidad = p.IdOportunidad,
                    IdOportunidadTipo = p.IdOportunidadTipo,
                    Descripcion = p.Descripcion,
                    Probabilidad = p.Probabilidad,
                    Importe = p.Importe,
                    IdAgente = p.IdAgente,
                    Agente = p.Agente.Descripcion,
                    FechaUltimaGestion = p.FechaUltimaGestion,
                    Calificacion = p.Calificacion,
                    Observacion = p.Observacion,
                    Direccion = p.Direccion,
                    ReferenciaDireccion = p.ReferenciaDireccion,
                    Referencia = p.Referencia,
                    Latitud = p.Latitud,
                    Longitud = p.Longitud,
                    IdEtapa = p.IdEtapa,
                    Etapa = p.Etapa.Descripcion,
                    EtapaOrden = p.Etapa.Orden,
                    Estado = p.Estado,
                    EstadoDescripcion = p.EstadoGeneralValue.Content,
                    FechaInicio = p.FecIng,
                    
                    Telefono1 = p.Telefono1,
                    Telefono2 = p.Telefono2,
                    PaginaWeb = p.PaginaWeb,
                    TipoEmpresa = p.TipoEmpresa,
                    CorreoElectronico = p.CorreoElectronico,

                    IdProspecto = p.OportunidadContactos.Where(q => q.EsPrincipal).FirstOrDefault().IdOportunidadContacto,
                    Prospecto = p.OportunidadContactos.Where(q => q.EsPrincipal).FirstOrDefault().Nombre,
                    ProspectoCargo = p.OportunidadContactos.Where(q => q.EsPrincipal).FirstOrDefault().Cargo,
                    Etapas = p.OportunidadEtapas,
                    OportunidadTipo = p.OportunidadTipo,

                    Imagen = p.OportunidadMedias.Select(q => new ListaIMG { URL = q.Path }).ToList(),
                });

            return query.FirstOrDefault();
        }

        #region StorageProperties

        public string[] StorageProperties
        {
            get
            {
                return new String[] { "IdOportunidad","Descripcion","Probabilidad","Importe","IdAgente","FechaUltimaGestion","Calificacion","Observacion","Direccion","Referencia","Latitud","Longitud","IdEtapa","EstadoTabla","Estado","UsrIng","FecIng","UsrMod","FecMod",
                "OportunidadTareas", "OportunidadEtapas" };
            }
        }

        public string[] StoragePropertiesContacto
        {
            get
            {
                return new String[] { "IdOportunidad", "IdOportunidadContacto", "Nombre", "EsPrincipal", "Telefono1", "Telefono2", "Cargo", "CorreoElectronico", "TipoMedia", "TipoMediaTabla", "Path", "FecMod", "UsrMod" };
            }
        }

        public string[] StoragePropertiesMedia
        {
            get
            {
                return new String[] { "IdOportunidad", "IdMedia", "TipoMedia", "TipoMediaTabla", "Path", "Estado", "EstadoTabla", "FecMod", "UsrMod" };
            }
        }

        public string[] StoragePropertiesResponsable
        {
            get
            {
                return new String[] { "IdOportunidad", "IdAgente", "TipoTabla", "Tipo" };
            }
        }

        public string[] StoragePropertiesEtapa
        {
            get
            {
                return new String[] { "IdOportunidad", "IdEtapa", "EstadoTabla", "Estado", "FechaInicio", "FechaFin", "Orden", "Observacion", };
            }
        }

        public string[] StoragePropertiesTarea
        {
            get
            {
                return new String[] { "IdOportunidad","IdEtapa","IdTarea","EstadoTabla","Estado","FechaInicio","FechaFin","Orden","Observacion",
                "OportunidadTareaActividads" };
            }
        }

        public string[] StoragePropertiesDeTareaActividad
        {
            get
            {
                return new String[] { "IdOportunidad", "IdEtapa", "IdTarea", "IdTareaActividad", "Descripcion", "IdTipoActividad", "Opciones", "Orden", "IdTareaActividadPadre", "IdTareaOpcion", "Resultado", "ResultadoCodigo" };
            }
        }

        #endregion StorageProperties

        public void UpdateXml(Oportunidad.Oportunidad entityToUpdate)
        {

            //base.Update(entityToUpdate);

            Context db = context as Context;

            string infoXml = entityToUpdate.ToXml(new XmlPropertiesInfo("Oportunidad", typeof(Oportunidad.Oportunidad), StorageProperties),
                       new XmlPropertiesInfo(typeof(Models.Oportunidad.OportunidadContacto), StoragePropertiesContacto),
                       new XmlPropertiesInfo(typeof(Models.Oportunidad.OportunidadMedia), StoragePropertiesMedia),
                       new XmlPropertiesInfo(typeof(Models.Oportunidad.OportunidadResponsable), StoragePropertiesResponsable),
                       new XmlPropertiesInfo(typeof(Models.Oportunidad.OportunidadEtapa), StoragePropertiesEtapa),
                       new XmlPropertiesInfo(typeof(Models.Oportunidad.OportunidadTarea), StoragePropertiesTarea),
                       new XmlPropertiesInfo(typeof(Models.Oportunidad.OportunidadTareaActividad), StoragePropertiesDeTareaActividad)
                       );

            db.Database.ExecuteSqlCommand(String.Format("exec spOportunidadUpdate '{0}'", infoXml));
        }
    }
}
