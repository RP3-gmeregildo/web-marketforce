using Rp3.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rp3.AgendaComercial.Models.Repositories
{
    public class PedidoRepository : Rp3.Data.Entity.Repository<Pedido.Pedido>
    {
        public PedidoRepository(DbContext context)
            : base(context)
        {
            
        }

        public void NotificacionPedido(int IdPedido)
        {
            Context db = context as Context;

            db.Database.ExecuteSqlCommand(String.Format("exec [not].[spSendMailEnvioPedido] {0}", IdPedido));
        }
    }
}
