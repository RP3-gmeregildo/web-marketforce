using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rp3.AgendaComercial.Models.Pedido
{
    [Table("tbPedidoDetalle", Schema = "ped")]
    public class PedidoDetalle : Rp3.Data.Entity.EntityBase
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)]
        public int IdPedidoDetalle { get; set; }
        public int IdPedido { get; set; }
        public int IdProducto { get; set; }
        public string Descripcion { get; set; }
        public decimal ValorUnitario { get; set; }
        public int Cantidad { get; set; }
        public decimal ValorTotal { get; set; }

        public void AsignarId()
        {
            ContextService service = new ContextService();
            this.IdPedidoDetalle = service.PedidoDetalles.GetMaxValue<int>(p => p.IdPedidoDetalle, 0) + 1;
        }

    }
}
