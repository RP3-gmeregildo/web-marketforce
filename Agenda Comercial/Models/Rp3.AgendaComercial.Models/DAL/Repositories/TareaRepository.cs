using Rp3.AgendaComercial.Models.Consulta.View;
using Rp3.AgendaComercial.Models.General;
using Rp3.AgendaComercial.Models.General.View;
using Rp3.Data.Entity;
using Rp3.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rp3.AgendaComercial.Models.Repositories
{
    public class TareaRepository : Repository<General.Tarea>
    {
        public TareaRepository(DbContext c)
            : base(c)
        {
            
        }

        public List<Models.General.Tarea> GetSync(int idRuta,int? idTarea = null, DateTime? fecMod = null)
        {
            var query = dbSet.Include("TareaActividades.TipoActividad.TipoActividadOpciones").Include("TareaRutaAplicas").AsQueryable();
            if(idTarea.HasValue)
                query = query.Where(p => p.IdTarea == idTarea.Value);

            if (fecMod != null)            
                query = query.Where(p => p.FecMod >= fecMod.Value);
            else
            {
                //DateTime fechactual  = Rp3.Runtime.Current.GetCurrentDateTime();
                query = query.Where(p => (!p.AplicaRutasEspecificas || p.TareaRutaAplicas.Any(q=>q.IdRuta == idRuta) || p.TipoTarea == Models.Constantes.TipoTarea.CheckListOportunidad) );
            }

            var list = query.ToList();

            foreach (var i in list.Where(p => p.AplicaRutasEspecificas && p.TareaRutaAplicas.Where(q => q.IdRuta == idRuta).Count() == 0))
                if (i.TipoTarea != Models.Constantes.TipoTarea.CheckListOportunidad)
                    i.Estado = Constantes.Estado.Eliminado;

            return list;
        }

        public List<EstadisticaEncuesta> GetEstadisticaEncuesta(int IdTarea, int IdTareaActividad, DateTime FechaInicio, DateTime FechaFin,
            string IdZona, string IdTipoCliente, string IdCanal, string IdAgente, string IdCliente, string IdClienteContacto, string RazonSocial)
        {
            return context.Database.SqlQuery<EstadisticaEncuesta>(@"dbo.spEstadisticaEncuesta 
                @IdTarea = {0},  
                @IdTareaActividad = {1},  
                @FechaInicio = {2}, 
                @FechaFin = {3},
                @IdZona = {4},  
                @IdTipoCliente = {5},  
                @IdCanal = {6},
                @IdAgente = {7},
                @IdCliente = {8},
                @IdClienteContacto = {9},
                @RazonSocial = {10}
                ",
                IdTarea, IdTareaActividad, FechaInicio.Date, FechaFin.Date, IdZona, IdTipoCliente, IdCanal, IdAgente, IdCliente, IdClienteContacto, RazonSocial).ToList();
        }

        public void ProcesarTarea(long IdTarea, string UsrMod)
        {
            Context db = context as Context;

            db.Database.ExecuteSqlCommand(String.Format("exec spProcesarTarea {0}, '{1}'", IdTarea, UsrMod));
        }

        public void EliminarDependenciaTarea(long IdTarea, string UsrMod)
        {
            Context db = context as Context;

            db.Database.ExecuteSqlCommand(String.Format("exec spEliminarDependenciaTarea {0}, '{1}'", IdTarea, UsrMod));
        }

        public dynamic GrabarListaTareaSP(int idtarea, string tarea, string tipo, string estado, string actividad)
        {
            return context.Database.SqlQuery<dynamic>(string.Format("dbo.SPGrabarListaTarea @idtarea = {0}, @tarea = '{1}',  @tipo = '{2}', @estado = '{3}', @actividad = '{4}'", idtarea, tarea, tipo, estado, actividad)).ToList();
        }

        #region StorageProperties

        public string[] StorageProperties
        {
            get
            {
                return new String[] { "IdTarea","Descripcion","TipoTareaTabla","TipoTarea","EstadoTabla","Estado","UsrIng","FecIng","UsrMod","FecMod","FechaVigenciaDesde","FechaVigenciaHasta","EsVigenciaIndefinida","AplicaRutasEspecificas",
                    "TareaRutaAplicas","TareaActividades", "TareaClienteActualizacion", "TareaClienteActualizacionCampos" };
            }
        }

        public string[] StoragePropertiesClienteActualizacion
        {
            get
            {
                return new String[] { "IdTarea", "PermitirCreacion", "PermitirModificacion", "SiempreEditarEnGestion", "SoloFaltantesEditarEnGestion" };
            }
        }

        public string[] StoragePropertiesClienteActualizacionCampo
        {
            get
            {
                return new String[] { "IdTarea", "IdCampo", "Creacion", "Modificacion", "Gestion" };
            }
        }

        public string[] StoragePropertiesDetalleRuta
        {
            get
            {
                return new String[] { "IdTarea", "IdRuta" };
            }
        }

        public string[] StoragePropertiesDetalleActividad
        {
            get
            {
                return new String[] { "IdTarea", "IdTareaActividad", "Descripcion", "IdTipoActividad", "Opciones", "Orden", "IdTareaActividadPadre", "Valor", "Limite" };
            }
        }

        #endregion StorageProperties

        public void InsertXml(Tarea entityToInsert)
        {
            //base.Update(entityToUpdate);

            Context db = context as Context;

            string infoXml = entityToInsert.ToXml(new XmlPropertiesInfo("Tarea", typeof(Tarea), StorageProperties),
                       new XmlPropertiesInfo(typeof(Models.General.TareaRutaAplica), StoragePropertiesDetalleRuta),
                       new XmlPropertiesInfo(typeof(Models.General.TareaActividad), StoragePropertiesDetalleActividad),
                       new XmlPropertiesInfo(typeof(Models.General.TareaClienteActualizacion), StoragePropertiesClienteActualizacion),
                       new XmlPropertiesInfo(typeof(Models.General.TareaClienteActualizacionCampo), StoragePropertiesClienteActualizacionCampo)
                       );

            db.Database.ExecuteSqlCommand(String.Format("exec spTareaInsert '{0}'", infoXml));
        }

        public void UpdateXml(Tarea entityToUpdate)
        {
            //base.Update(entityToUpdate);

            Context db = context as Context;

            string infoXml = entityToUpdate.ToXml(new XmlPropertiesInfo("Tarea", typeof(Tarea), StorageProperties),
                       new XmlPropertiesInfo(typeof(Models.General.TareaRutaAplica), StoragePropertiesDetalleRuta),
                       new XmlPropertiesInfo(typeof(Models.General.TareaActividad), StoragePropertiesDetalleActividad)
                       );

            db.Database.ExecuteSqlCommand(String.Format("exec spTareaUpdate '{0}'", infoXml));
        }
    }
}