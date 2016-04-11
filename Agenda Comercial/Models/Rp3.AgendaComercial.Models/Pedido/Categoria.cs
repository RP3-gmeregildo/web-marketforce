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
    [Table("tbCategoria", Schema = "ped")]
    public class Categoria : Rp3.Data.Entity.EntityBase
    {
        public override string Key
        {
            get
            {
                if (this.IdCategoria == 0) return " ";
                return this.IdCategoria.ToString();
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

        public virtual List<SubCategoria> SubCategorias { get; set; }

        public void AsignarId()
        {
            ContextService service = new ContextService();
            this.IdCategoria = service.Categorias.GetMaxValue<int>(p => p.IdCategoria, 0) + 1;
        }

    }
}
