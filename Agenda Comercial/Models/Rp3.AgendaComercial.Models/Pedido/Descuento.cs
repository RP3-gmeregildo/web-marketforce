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
    [Table("tbDescuento", Schema = "ped")]
    public class Descuento : Rp3.Data.Entity.EntityBase
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)]
        public int IdDescuento { get; set; }
        public int IdProducto { get; set; }
        public int LimiteInferior { get; set; }
        public int LimiteSuperior { get; set; }
        public decimal Valor { get; set; }
        public short TipoTabla { get; set; }
        public string Tipo { get; set; }
        public short EstadoTabla { get; set; }
        public string Estado { get; set; }
        public string UsrIng { get; set; }
        public DateTime FecIng { get; set; }
        public string UsrMod { get; set; }
        public DateTime? FecMod { get; set; }

        [ForeignKey("EstadoTabla,Estado"), NonSerializableToXmlAttribute]
        public virtual GeneralValues EstadoGeneralValue { get; set; }
        [ForeignKey("TipoTabla,Tipo"), NonSerializableToXmlAttribute]
        public virtual GeneralValues TipoGeneralValue { get; set; }
        [ForeignKey("IdProducto"), NonSerializableToXmlAttribute]
        public virtual Producto Producto { get; set; }

        public void AsignarId()
        {
            ContextService service = new ContextService();
            this.IdDescuento = service.Descuentos.GetMaxValue<int>(p => p.IdDescuento, 0) + 1;
        }

    }
}
