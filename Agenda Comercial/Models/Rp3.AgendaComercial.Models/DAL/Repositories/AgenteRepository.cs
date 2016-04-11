using Rp3.AgendaComercial.Models.General;
using Rp3.AgendaComercial.Models.General.View;
using Rp3.AgendaComercial.Models.Ruta.View;
using Rp3.Data.Entity;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rp3.AgendaComercial.Models.Repositories
{
    public class AgenteRepository: Repository<General.Agente>
    {
        public AgenteRepository(DbContext c)
            : base(c)
        {
        }

        public bool EsDiaLaboral(int IdCalendario, DateTime Fecha)
        {
            return context.Database.SqlQuery<int>(
                "SELECT dbo.GetCountWorkday(@IdCalendario, @StartDate, @EndDate)",
                new SqlParameter("@IdCalendario", IdCalendario),
                new SqlParameter("@StartDate", Fecha),
                new SqlParameter("@EndDate", Fecha)
            ).First() == 1;
        }

        public void GrupoAgenteUpdate(int IdGrupo, string[] Agentes, string UsrMod)
        {
            Context db = context as Context;

            string idAgente = String.Empty;

            if (Agentes != null)
            {
                foreach (string id in Agentes)
                {
                    if (!String.IsNullOrEmpty(idAgente))
                        idAgente += ",";

                    idAgente += Convert.ToString(id);
                }
            }

            db.Database.ExecuteSqlCommand(String.Format("exec spGrupoAgenteUpdate {0}, '{1}', '{2}'", IdGrupo, idAgente, UsrMod));
        }

        public void EliminarDependenciaAgente(int IdAgente, string UsrMod)
        {
            Context db = context as Context;

            db.Database.ExecuteSqlCommand(String.Format("exec spEliminarDependenciaAgente {0}, '{1}'", IdAgente, UsrMod));
        }

        public List<int> GetIdsAgentesPermitidos(int IdAgente)
        {
            return context.Database.SqlQuery<int>("gen.spGetAgentesPermitidos @IdAgente = @IdAgenteParametro",
                new SqlParameter("@IdAgenteParametro", IdAgente)).ToList();
        }

        public List<Agente> GetAgentesPermitidos(int IdAgente, bool agregarTodos = false)
        {
            var list = GetIdsAgentesPermitidos(IdAgente);

            var agenteList = dbSet.Where(p => list.Contains(p.IdAgente)).OrderBy(p=>p.Descripcion).ToList();

            if (agregarTodos)
            {
                agenteList.Insert(0, new Agente()
                {
                    IdAgente = Rp3.AgendaComercial.Models.Constantes.IdTodosMisAgentes,
                    Descripcion = Rp3.AgendaComercial.Resources.LabelFor.TodosMisAgentes
                });
            }

            return agenteList;
        }

        public List<ResumenGestionAgente> GetResumenGestionAgentes(int idAgenteSupervisor,DateTime fechaInicio, DateTime fechaFin)
        {
            return context.Database.SqlQuery<ResumenGestionAgente>("rep.spResumenGestionAgentes @IdAgenteSupervisor = @IdAgenteSupervisorParametro, @FechaInicio = @FechaInicioParametro, @FechaFin = @FechaFinParametro",
               new SqlParameter("@IdAgenteSupervisorParametro", idAgenteSupervisor),
               new SqlParameter("@FechaInicioParametro", fechaInicio),
               new SqlParameter("@FechaFinParametro", fechaFin)).ToList();
        }

        public AgenteView GetAgenteView(int idUsuario)
        {
            AgenteView agenteView = dbSet.Where(p => p.IdUsuario == idUsuario && p.Estado == Models.Constantes.Estado.Activo).Select(p=>new AgenteView()
                {
                    IdAgente = p.IdAgente,
                    IdSupervisor = p.IdSupervisor,
                    Descripcion = p.Descripcion,
                    IdRuta = p.IdRuta,
                    EsSupervisor = p.Cargo.EsSupervisor,
                    EsAdministrador = p.Cargo.EsAdministrador,
                    Cargo = p.Cargo.Descripcion,
                    IdCargo = p.IdCargo,
                    IdGrupo = p.IdGrupo,
                    Foto = (p.IdUsuario != null ? p.Usuario.Contact.Photo : null),
                    GCMId = p.GCMId
                }).FirstOrDefault();

            if (agenteView != null)
                agenteView.EsAgente = true;
            else
            {
                agenteView = new AgenteView(){ EsAgente = false };
            }

            return agenteView;
        }

        public List<AgenteUbicacionView> GetResumenUbicacionAgentes(int idAgenteSupervisor, DateTime? ultActualizacion)
        {
            if(ultActualizacion.HasValue)
            {
               return context.Database.SqlQuery<AgenteUbicacionView>("rep.spResumenUbicacionAgentes @IdAgenteSupervisor = @IdAgenteSupervisorParametro, @FechaUltActualizacion = @FechaUltActualizacionParametro",
               new SqlParameter("@IdAgenteSupervisorParametro", idAgenteSupervisor),
               new SqlParameter("@FechaUltActualizacionParametro", ultActualizacion.Value)).ToList();
            }
            else
            {
               return context.Database.SqlQuery<AgenteUbicacionView>("rep.spResumenUbicacionAgentes @IdAgenteSupervisor = @IdAgenteSupervisorParametro, @FechaUltActualizacion = NULL",
               new SqlParameter("@IdAgenteSupervisorParametro", idAgenteSupervisor)).ToList();
            }
            
        }
        

     }
}
