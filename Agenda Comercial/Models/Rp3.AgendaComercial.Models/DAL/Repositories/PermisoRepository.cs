using Rp3.AgendaComercial.Models.Marcacion;
using Rp3.AgendaComercial.Models.Marcacion.View;
using Rp3.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rp3.AgendaComercial.Models.Repositories
{
    public class PermisoRepository : Rp3.Data.Entity.Repository<Permiso>
    {
        public PermisoRepository(DbContext context)
            : base(context)
        {

        }

        public void Permiso(long IdPermiso)
        {
            Context db = context as Context;

            db.Database.ExecuteSqlCommand(String.Format("exec [not].[spSendMailPermiso] {0}", IdPermiso));
        }

        public List<PermisoListado> GetPermisosJustificaciones(int idAgente, DateTime fechaInicio, DateTime fechaFin)
        {
            List<PermisoListado> data = new List<PermisoListado>();
            List<int> idsAgentes = new List<int>();
            idsAgentes.Add(idAgente);
            List<int> idsGrupos = new List<int>();

            data.AddRange(GetPermisoListado(idsAgentes, idsGrupos, fechaInicio, fechaFin, null, null, string.Empty, true));
            data.AddRange(GetPermisoListado(idsAgentes, idsGrupos, fechaInicio, fechaFin, null, null, string.Empty, false));
            return data.OrderBy(p => p.FechaInicio).ToList();
        }

        public List<PermisoListado> GetPermisoListado(List<int> idsAgentes, List<int> idsGrupos, DateTime? fechaInicio, DateTime? fechaFin, int? agente, int? grupo, string estado, bool esPrevio)
        {
            Context db = (Context)context;

            var query = db.Permisos.Where(p =>
                (idsAgentes.Contains(p.IdAgente ?? 0) || (p.IdAgente == null && idsGrupos.Contains(p.IdGrupo ?? 0))) &&
                (fechaInicio == null || (p.FechaInicio >= fechaInicio && p.FechaInicio <= fechaFin)) &&
                (agente == null || p.IdAgente == agente) &&
                (grupo == null || p.IdGrupo == grupo) &&
                (String.IsNullOrEmpty(estado) || p.Estado == estado) &&
                p.EsPrevio == esPrevio
                )
                .Select(p => new PermisoListado()
                {
                    IdPermiso = p.IdPermiso,
                    IdAgente = p.IdAgente,
                    Agente = p.Agente.Descripcion,
                    IdGrupo = p.IdGrupo,
                    Grupo = p.Grupo.Descripcion,
                    Tipo = p.Tipo,
                    TipoDesripcion = p.TipoGeneralValue.Content,
                    Motivo = p.Motivo,
                    MotivoDescripcion = p.MotivoGeneralValue.Content,
                    FechaInicio = p.FechaInicio,
                    FechaFin = p.FechaFin,
                    HoraInicio = p.HoraInicio,
                    HoraFin = p.HoraFin,
                    Observacion = p.Observacion,
                    ObservacionSupervisor = p.ObservacionSupervisor,
                    EsPrevio = p.EsPrevio,
                    Estado = p.Estado,
                    EstadoDescripcion = p.EstadoGeneralValue.Content                    
                });

            var list = query.ToList();

            if (!esPrevio)
            {
                var idsPermisos = list.Select(p=>p.IdPermiso).Distinct().ToList();

                var marcaciones = db.Marcaciones.Where(p => idsPermisos.Contains(p.IdPermiso ?? 0)).ToList();

                foreach (var permiso in list)
                {
                    var marcacion = marcaciones.Where(p => p.IdPermiso == permiso.IdPermiso).FirstOrDefault();

                    if (marcacion != null)
                    {
                        permiso.TipoJornada = marcacion.Tipo;
                        permiso.TipoJornadaDesripcion = marcacion.TipoGeneralValue.Content;
                    }
                }
            }

            return list;
        }
    }
}
