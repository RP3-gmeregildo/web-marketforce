using Rp3.AgendaComercial.Models.General;
using Rp3.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rp3.AgendaComercial.Models.Repositories
{
    public class ZonaRepository : Rp3.Data.Entity.Repository<Zona>
    {
        public ZonaRepository(DbContext context)
            :base(context)
        {
        }

        public void EliminarDependenciaZona(int IdZona, string UsrMod)
        {
            Context db = context as Context;

            db.Database.ExecuteSqlCommand(String.Format("exec spEliminarDependenciaZona {0}, '{1}'", IdZona, UsrMod));
        }


        public void Procesar(int? IdZona = null)
        {
            Context db = context as Context;

            db.Database.ExecuteSqlCommand(String.Format("exec spProcesarZona {0}", IdZona));
        }
    }
}
