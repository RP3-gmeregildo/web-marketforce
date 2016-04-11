using Rp3.AgendaComercial.Models.Marcacion;
using Rp3.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rp3.AgendaComercial.Models.Repositories
{
    public class GrupoRepository : Rp3.Data.Entity.Repository<Grupo>
    {
        public GrupoRepository(DbContext context)
            :base(context)
        {
        }

        public void EliminarDependenciaGrupo(int IdGrupo, string UsrMod)
        {
            Context db = context as Context;

            db.Database.ExecuteSqlCommand(String.Format("exec spEliminarDependenciaGrupo {0}, '{1}'", IdGrupo, UsrMod));
        }
    }
}
