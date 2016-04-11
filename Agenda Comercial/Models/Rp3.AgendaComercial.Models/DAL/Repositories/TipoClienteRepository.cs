using Rp3.AgendaComercial.Models.General;
using Rp3.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rp3.AgendaComercial.Models.Repositories
{
    public class TipoClienteRepository : Rp3.Data.Entity.Repository<TipoCliente>
    {
        public TipoClienteRepository(DbContext context)
            :base(context)
        {
        }

        public void EliminarDependenciaTipoCliente(int IdTipoCliente, string UsrMod)
        {
            Context db = context as Context;

            db.Database.ExecuteSqlCommand(String.Format("exec spEliminarDependenciaTipoCliente {0}, '{1}'", IdTipoCliente, UsrMod));
        }

    }
}
