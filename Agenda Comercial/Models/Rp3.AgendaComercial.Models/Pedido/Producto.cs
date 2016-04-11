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
    [Table("tbProducto", Schema = "ped")]
    public class Producto : Rp3.Data.Entity.EntityBase
    {
        public override string Key
        {
            get
            {
                if (this.IdProducto == 0) return " ";
                return this.IdProducto.ToString();
            }
        }

        public override string DescriptionName
        {
            get
            {
                return this.Descripcion;
            }
        }
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)]
        public int IdProducto { get; set; }
        public string Descripcion { get; set; }
        public string URLFoto { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public decimal Precio { get; set; }
        public short EstadoTabla { get; set; }
        public string Estado { get; set; }
        public string UsrIng { get; set; }
        public DateTime FecIng { get; set; }
        public string UsrMod { get; set; }
        public DateTime? FecMod { get; set; }
        public string IdExterno { get; set; }
        public int? IdSubCategoria { get; set; }

        [ForeignKey("IdSubCategoria"), NonSerializableToXmlAttribute]
        public virtual SubCategoria SubCategoria { get; set; }
        [ForeignKey("EstadoTabla,Estado"), NonSerializableToXmlAttribute]
        public virtual GeneralValues EstadoGeneralValue { get; set; }
        [NonSerializableToXmlAttribute, NotMapped]
        public string TipoDescuento { get; set; }
        [NonSerializableToXmlAttribute, NotMapped]
        public byte[] QRCode { get; set; }
        public virtual List<Descuento> Descuentos { get; set; }

        public void AsignarId()
        {
            ContextService service = new ContextService();
            this.IdProducto = service.Productos.GetMaxValue<int>(p => p.IdProducto, 0) + 1;
        }

    }
}
