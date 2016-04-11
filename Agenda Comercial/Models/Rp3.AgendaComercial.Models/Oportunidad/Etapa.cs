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
    [Table("tbEtapa", Schema = "opt")]
    public class Etapa : Rp3.Data.Entity.EntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdEtapa { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Etapa")]
        public string Descripcion { get; set; }

        public int Orden { get; set; }
        public int IdOportunidadTipo { get; set; }

        public short EstadoTabla { get; set; }
        public string Estado { get; set; }
        public string UsrIng { get; set; }
        public DateTime FecIng { get; set; }
        public string UsrMod { get; set; }
        public DateTime? FecMod { get; set; }

        public int? IdEtapaPadre { get; set; }
        public int? Dias { get; set; }
        public bool EsVariable { get; set; }

        [ForeignKey("EstadoTabla,Estado")]
        public virtual GeneralValues EstadoGeneralValue { get; set; }

        [ForeignKey("IdOportunidadTipo"), NonSerializableToXmlAttribute]
        public virtual OportunidadTipo OportunidadTipo { get; set; }

        public virtual List<EtapaTarea> EtapaTareas { get; set; }

        [NotMapped]
        public bool ReadOnly { get; set; }

        public void AsignarId()
        {
            ContextService service = new ContextService();
            this.IdEtapa = service.Etapas.GetMaxValue<int>(p => p.IdEtapa, 0) + 1;
        }

        public void AsignarOrden()
        {
            ContextService service = new ContextService();
            this.Orden = service.Etapas.GetMaxValue<int>(p => p.Orden, 0, p=>p.Estado != Constantes.Estado.Eliminado) + 1;
        }
    }
}
