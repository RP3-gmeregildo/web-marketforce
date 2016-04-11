using Rp3.AgendaComercial.Models.General;
using Rp3.Data.Entity;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rp3.AgendaComercial.Models.Repositories
{
    public class AgenteUbicacionRepository: Repository<Ruta.AgenteUbicacion>
    {
        public AgenteUbicacionRepository(DbContext c)
            : base(c)
        {
        }

        public List<DateTime> GetFechasRecorrido(int idAgente, DateTime inicio, DateTime fin)
        {
            return context.Database.SqlQuery<DateTime>("SELECT  DISTINCT CONVERT(DATETIME,(CONVERT(VARCHAR,Fecha,112))) FROM rut.tbAgenteUbicacion WHERE IdAgente = @IdAgente AND Fecha >= @FechaInicio AND Fecha <= @FechaFin ORDER BY CONVERT(DATETIME,(CONVERT(VARCHAR,Fecha,112))) DESC", 
                new SqlParameter("@IdAgente", idAgente),
                new SqlParameter("@FechaInicio",  inicio),
                new SqlParameter("@FechaFin",  fin)).ToList();
        }

        public void DeleteInformeTrazabilidad(DateTime FechaInicio, DateTime FechaFin)
        {
            Context db = context as Context;

            db.Database.ExecuteSqlCommand(String.Format("delete rep.tbInformeTrazabilidad where Fecha between '{0}' and '{1}'", FechaInicio.ToString("yyyyMMdd"), FechaFin.ToString("yyyyMMdd hh:mm:ss")));
        }

     }
}
