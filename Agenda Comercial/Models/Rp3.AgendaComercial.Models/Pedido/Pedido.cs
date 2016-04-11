using Rp3.AgendaComercial.Models.General;
using Rp3.AgendaComercial.Models.Ruta;
using Rp3.Data;
using Rp3.Data.Models.General;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rp3.AgendaComercial.Models.Pedido
{
    [Table("tbPedido", Schema = "ped")]
    public class Pedido : Rp3.Data.Entity.EntityBase
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)]
        public int IdPedido { get; set; }
        public int IdCliente { get; set; }
        public int IdAgente { get; set; }
        public int? IdRuta { get; set; }
        public long? IdAgenda { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public decimal ValorTotal { get; set; }
        public string Email { get; set; }
        public short EstadoTabla { get; set; }
        public string Estado { get; set; }
        public string UsrIng { get; set; }
        public DateTime FecIng { get; set; }
        public string UsrMod { get; set; }
        public DateTime? FecMod { get; set; }

        [ForeignKey("EstadoTabla,Estado"), NonSerializableToXmlAttribute]
        public virtual GeneralValues EstadoGeneralValue { get; set; }
        [ForeignKey("IdCliente"), NonSerializableToXmlAttribute]
        public virtual Cliente Cliente { get; set; }
        [ForeignKey("IdRuta, IdAgenda"), NonSerializableToXmlAttribute]
        public virtual Agenda Agenda { get; set; }
        [ForeignKey("IdAgente"), NonSerializableToXmlAttribute]
        public virtual Agente Agente { get; set; }

        [NotMapped, NonSerializableToXmlAttribute]
        public string ValorTotalCurrency
        {
            get
            {
                return "$" + ValorTotal;
            }
        }

        [NotMapped, NonSerializableToXmlAttribute]
        public int TotalItems
        {
            get
            {
                if (PedidoDetalles == null)
                    return 0;
                else
                    return PedidoDetalles.Count;
            }
        }

        public virtual List<PedidoDetalle> PedidoDetalles { get; set; }

        public void AsignarId()
        {
            ContextService service = new ContextService();
            this.IdPedido = service.Pedidos.GetMaxValue<int>(p => p.IdPedido, 0) + 1;
        }

    }
}
