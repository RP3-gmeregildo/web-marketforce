using Rp3.AgendaComercial.Models.Consulta.View;
using Rp3.AgendaComercial.Models.Marcacion.View;
using Rp3.AgendaComercial.Models.Ruta;
using Rp3.AgendaComercial.Models.Ruta.View;
using Rp3.Data.Entity;
using Rp3.Xml;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rp3.AgendaComercial.Models.Repositories
{
    public class ReporteRepository : Rp3.Data.Entity.Repository
    {
        public ReporteRepository(DbContext context)
            : base(context)
        {

        }

        public List<AnalisisAgente> GetAnalisisAgente(int IdAgente, DateTime FechaInicio, DateTime FechaFin)
        {
            return Db.SqlQuery<AnalisisAgente>("rep.spAnalisisAgente  @IdAgente = @IdAgenteParam, @FechaInicio = @FechaInicioParam, @FechaFin = @FechaFinParam",
                new SqlParameter("@IdAgenteParam", IdAgente),
                new SqlParameter("@FechaInicioParam", FechaInicio),
                new SqlParameter("@FechaFinParam", FechaFin)
                ).ToList();
        }

        public List<ReporteMarcacion> GetReporteMarcacionRango(int IdAgente, DateTime FechaInicio, DateTime FechaFin, bool Todos)
        {
            return Db.SqlQuery<ReporteMarcacion>("rep.spReporteMarcacionRango  @IdAgente = @IdAgenteParam, @FechaInicio = @FechaInicioParam, @FechaFin = @FechaFinParam, @Todos = @TodosParam",
                new SqlParameter("@IdAgenteParam", IdAgente),
                 new SqlParameter("@FechaInicioParam", FechaInicio),
                new SqlParameter("@FechaFinParam", FechaFin),
                new SqlParameter("@TodosParam", Todos)
                ).ToList();
        }
        public List<ReporteMarcacion> GetReporteMarcacion(int IdAgente, DateTime Fecha)
        {
            return GetReporteMarcacion(IdAgente, Fecha, true);
        }
        public List<ReporteMarcacion> GetReporteMarcacion(int IdAgente, DateTime Fecha, bool todos)
        {
            return Db.SqlQuery<ReporteMarcacion>("rep.spReporteMarcacion  @IdAgente = @IdAgenteParam, @Fecha = @FechaParam, @Todos = @TodosParam",
                new SqlParameter("@IdAgenteParam", IdAgente),
                new SqlParameter("@FechaParam", Fecha),
                new SqlParameter("@TodosParam", todos)
                ).ToList();
        }

        public List<ReporteMarcacion> GetReporteMarcacionReporteGestion(int idAgente, DateTime fechaIni, DateTime fechaFin)
        {
            return this.GetReporteMarcacionRango(idAgente, fechaIni, fechaFin, false).Where(p => p.DiaLaboral).ToList();
        }


        public DashboardResumenGestion GetDashboardResumenGestion(DateTime fechaInicio, DateTime fechaFin, int idAgenteConsulta, bool modoAgente = false)
        {
            return Db.SqlQuery<DashboardResumenGestion>("rep.spDashboardResumenGestion @FechaInicio = @FechaInicioParam, @FechaFin = @FechaFinParam, @IdAgenteConsulta = @IdAgenteConsultaParam, @ModoAgente = @ModoAgenteParam",
                new SqlParameter("@FechaInicioParam", fechaInicio),
                new SqlParameter("@FechaFinParam", fechaFin),
                new SqlParameter("@IdAgenteConsultaParam", idAgenteConsulta),
                new SqlParameter("@ModoAgenteParam", modoAgente)
                ).FirstOrDefault();
        }

        public List<DashboardResumenGestionAgente> GetDashboardResumenGestionAgente(DateTime fechaInicio, DateTime fechaFin, int idAgenteConsulta, bool modoAgente = false)
        {
            return Db.SqlQuery<DashboardResumenGestionAgente>("rep.spDashboardResumenGestionAgente @FechaInicio = @FechaInicioParam, @FechaFin = @FechaFinParam, @IdAgenteConsulta = @IdAgenteConsultaParam, @ModoAgente = @ModoAgenteParam",
                new SqlParameter("@FechaInicioParam", fechaInicio),
                new SqlParameter("@FechaFinParam", fechaFin),
                new SqlParameter("@IdAgenteConsultaParam", idAgenteConsulta),
                new SqlParameter("@ModoAgenteParam", modoAgente)
                ).ToList();
        }

        public List<DashboardAgenteCategoriaCliente> GetDashboardAgenteAgenda(DateTime fechaInicio, DateTime fechaFin, int idAgenteConsulta, int? idAgenteFiltro = null)
        {
            SqlParameter idagentefiltroparam = null;
            if(idAgenteFiltro.HasValue)
                idagentefiltroparam = new SqlParameter("@IdAgenteFiltroParam", idAgenteFiltro.Value);
            else
                idagentefiltroparam = new SqlParameter("@IdAgenteFiltroParam", DBNull.Value);

            return Db.SqlQuery<DashboardAgenteCategoriaCliente>("rep.spDashboardAgenteAgenda @FechaInicio = @FechaInicioParam, @FechaFin = @FechaFinParam, @IdAgenteConsulta = @IdAgenteConsultaParam, @IdAgenteFiltro = @IdAgenteFiltroParam",
                new SqlParameter("@FechaInicioParam", fechaInicio),
                new SqlParameter("@FechaFinParam", fechaFin),
                new SqlParameter("@IdAgenteConsultaParam", idAgenteConsulta),
                idagentefiltroparam).ToList();
        }
        
    }
}
