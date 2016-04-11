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
    [Table("tbSubCategoria", Schema = "ped")]
    public class SubCategoria : Rp3.Data.Entity.EntityBase
    {
        public override string Key
        {
            get
            {
                if (this.IdSubCategoria == 0) return " ";
                return this.IdSubCategoria.ToString();
            }
        }

        public override string DescriptionName
        {
            get
            {
                if (this.Descripcion != null)
                    return this.Categoria.Descripcion + " - " + this.Descripcion;
                else
                    return "";
            }
        }
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)]
        public int IdSubCategoria { get; set; }
        public int IdCategoria { get; set; }
        public string Descripcion { get; set; }
        public short EstadoTabla { get; set; }
        public string Estado { get; set; }
        public string UsrIng { get; set; }
        public DateTime FecIng { get; set; }
        public string UsrMod { get; set; }
        public DateTime? FecMod { get; set; }

        [ForeignKey("EstadoTabla,Estado"), NonSerializableToXmlAttribute]
        public virtual GeneralValues EstadoGeneralValue { get; set; }

        [ForeignKey("IdCategoria"), NonSerializableToXmlAttribute]
        public virtual Categoria Categoria { get; set; }

        public void AsignarId()
        {
            ContextService service = new ContextService();
            this.IdSubCategoria = service.SubCategorias.GetMaxValue<int>(p => p.IdSubCategoria, 0) + 1;
        }

    }
}
