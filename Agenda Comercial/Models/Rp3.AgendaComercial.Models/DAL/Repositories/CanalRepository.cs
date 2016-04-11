using Rp3.AgendaComercial.Models.General;
using Rp3.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rp3.AgendaComercial.Models.Repositories
{
    public class CanalRepository : Rp3.Data.Entity.Repository<Canal>
    {
        public CanalRepository(DbContext context)
            :base(context)
        {
        }

        public void EliminarDependenciaCanal(int IdCanal, string UsrMod)
        {
            Context db = context as Context;

            db.Database.ExecuteSqlCommand(String.Format("exec spEliminarDependenciaCanal {0}, '{1}'", IdCanal, UsrMod));
        }

    }
}
