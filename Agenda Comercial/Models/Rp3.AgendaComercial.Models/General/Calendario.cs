using Rp3.Data.Models.General;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rp3.AgendaComercial.Models.General
{
    [Table("tbCalendario", Schema = "gen")]
    public class Calendario : Rp3.Data.Entity.EntityBase
    {
        public override string Key
        {
            get
            {
                if (this.IdCalendario == 0) return " ";
                return this.IdCalendario.ToString();
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
        public int IdCalendario { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Calendario")]
        public string Descripcion { get; set; }

        public double HorasJornadaLaboral { get; set; }
        public short EstadoTabla { get; set; }
        public string Estado { get; set; }

        [ForeignKey("EstadoTabla,Estado")]
        public virtual GeneralValues EstadoGeneralValue { get; set; }

        public string UsrIng { get; set; }
        public DateTime FecIng { get; set; }
        public string UsrMod { get; set; }
        public DateTime? FecMod { get; set; }
        public bool EsDefault { get; set; }
        public virtual List<DiaLaboral> DiasLaborales { get; set; }
        public virtual List<DiasNoLaborable> DiasNoLaborables { get; set; }
        public void AsignarId()
        {
            ContextService service = new ContextService();
            this.IdCalendario = service.Calendarios.GetMaxValue<int>(p => p.IdCalendario, 0) + 1;
        }
    }
}
