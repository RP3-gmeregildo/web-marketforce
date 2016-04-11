using Rp3.Data;
using Rp3.Data.Models.General;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rp3.AgendaComercial.Models.Oportunidad
{
    [Table("tbOportunidadTipo", Schema = "opt")]
    public class OportunidadTipo : Rp3.Data.Entity.EntityBase
    {
        public override string Key
        {
            get
            {
                if (this.IdOportunidadTipo == 0) return " ";
                return this.IdOportunidadTipo.ToString();
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
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdOportunidadTipo { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Tipo")]
        public string Descripcion { get; set; }

        public short EstadoTabla { get; set; }
        public string Estado { get; set; }
        public string UsrIng { get; set; }
        public DateTime FecIng { get; set; }
        public string UsrMod { get; set; }
        public DateTime? FecMod { get; set; }

        [ForeignKey("EstadoTabla,Estado")]
        public virtual GeneralValues EstadoGeneralValue { get; set; }

        [NonSerializableToXmlAttribute]
        public virtual List<Etapa> Etapas { get; set; }

        [NotMapped]
        public bool ReadOnly { get; set; }

        public void AsignarId()
        {
            ContextService service = new ContextService();
            this.IdOportunidadTipo = service.OportunidadTipos.GetMaxValue<int>(p => p.IdOportunidadTipo, 0) + 1;
        }
    }
}
