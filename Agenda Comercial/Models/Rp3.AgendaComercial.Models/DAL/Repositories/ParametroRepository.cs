using Rp3.AgendaComercial.Models.General;
using Rp3.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rp3.AgendaComercial.Models.Repositories
{
    public class ParametroRepository : Repository<Parametro>
    {
        public ParametroRepository(DbContext context)
            : base(context)
        {

        }         
    }
}
