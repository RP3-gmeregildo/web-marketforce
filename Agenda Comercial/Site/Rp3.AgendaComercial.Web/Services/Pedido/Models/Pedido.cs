using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rp3.AgendaComercial.Web.Services.Pedido.Models
{
    public class Pedido
    {
        public int IdPedido { get; set; }
        public int IdCliente { get; set; }
        public int? IdRuta { get; set; }
        public long? IdAgenda { get; set; }
        public decimal ValorTotal { get; set; }
        public string Email { get; set; }
        public string Estado { get; set; }
        public DateTime? FechaCreacion { get; set; }

        public long FechaCreacionTicks
        {
            get
            {
                if (FechaCreacion.HasValue)
                    return FechaCreacion.Value.Ticks;
                return 0;
            }
            set
            {
                if (value == 0)
                    FechaCreacion = null;
                else
                    FechaCreacion = new DateTime(value);
            }
        }

        public List<PedidoDetalle> PedidoDetalles { get; set; }
        
    }

    public class PedidoDetalle
    {
        public int IdPedidoDetalle { get; set; }
        public int IdPedido { get; set; }
        public int IdProducto { get; set; }
        public string Descripcion { get; set; }
        public decimal ValorUnitario { get; set; }
        public int Cantidad { get; set; }
        public decimal ValorTotal { get; set; }
    }
}