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
    public class LoteRepository : Rp3.Data.Entity.Repository<Lote>
    {
        public LoteRepository(DbContext context)
            : base(context)
        {

        }

        public void EliminarDependenciaLote(int IdLote, string UsrMod)
        {
            Context db = context as Context;

            db.Database.ExecuteSqlCommand(String.Format("exec spEliminarDependenciaLote {0}, '{1}'", IdLote, UsrMod));
        }

        public void ProcesarLote(Lote entity)
        {
            this.context.Database.ExecuteSqlCommand(string.Format("dbo.spProcesarLote @IdLote = {0}", entity.IdLote));
        }

        public List<LoteView> GetListadoLoteCliente(int? idlote, string idzona, string idtipocliente, string idcanal, int pagina, int numreg, int isbegin, int calificacion)
        {
            /*string t_cal = null;
            if (calificacion > 0 && calificacion != 0) { t_cal = calificacion.ToString(); }*/

            return context.Database.SqlQuery<LoteView>(string.Format("dbo.SPSearchLoteCliente @idlote = {0}, @idzona = '{1}', @idtipocliente = '{2}', @IdCanal = '{3}', @pagina = {4}, @numreg = {5}, @isbegin = {6}, @calificacion = {7}",
                idlote.HasValue ? idlote.ToString() : "NULL",
                idzona ?? "",
                idtipocliente ?? "",
                idcanal ?? "",
                pagina,
                numreg,
                isbegin,
                calificacion)).ToList();
        }
    }
}
