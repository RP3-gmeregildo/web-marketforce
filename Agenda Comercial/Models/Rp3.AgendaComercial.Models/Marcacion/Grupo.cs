using Rp3.AgendaComercial.Models.General;
using Rp3.Data;
using Rp3.Data.Models.General;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rp3.AgendaComercial.Models.Marcacion
{
    [Table("tbGrupo", Schema = "mar")]
    public class Grupo : Rp3.Data.Entity.EntityBase
    {
        public override string Key
        {
            get
            {
                if (this.IdGrupo == 0) return " ";
                return this.IdGrupo.ToString();
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
        public int IdGrupo { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Grupo")]
        public string Descripcion { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Calendario")]
        public int IdCalendario { get; set; }
        public short EstadoTabla { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Estado")]
        public string Estado { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "AplicaMarcacion")]
        public bool AplicaMarcacion { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "AplicaBreak")]
        public bool AplicaBreak { get; set; }
        public string UsrIng { get; set; }
        public DateTime FecIng { get; set; }
        public string UsrMod { get; set; }
        public DateTime? FecMod { get; set; }
        public double? LongitudPuntoPartida { get; set; }
        public double? LatitudPuntoPartida { get; set; }

        [ForeignKey("EstadoTabla,Estado")]
        public virtual GeneralValues EstadoGeneralValue { get; set; }

        [ForeignKey("IdCalendario")]
        public virtual Calendario Calendario { get; set; }

        [NotMapped]
        public bool ReadOnly { get; set; }

        [NotMapped, Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "TieneUbicacion")]
        public bool TieneUbicacion { get; set; }

        public void AsignarId()
        {
            ContextService service = new ContextService();
            this.IdGrupo = service.Grupos.GetMaxValue<int>(p => p.IdGrupo, 0) + 1;
        }
    }
}
