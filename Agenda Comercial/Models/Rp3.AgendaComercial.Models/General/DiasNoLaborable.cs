using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rp3.AgendaComercial.Models.General
{
    [Table("tbDiasNoLaborables", Schema = "gen")]
    public class DiasNoLaborable : Rp3.Data.Entity.EntityBase
    {
        public override string Key
        {
            get
            {
                return string.Format("{0}-{1}", this.IdCalendario, this.IdDiaNoLaborable);
            }
        }

        public string KeyCombo
        {
            get
            {
                if (this.IdDiaNoLaborable == 0) return " ";
                return this.IdDiaNoLaborable.ToString();
            }
        }

        public override string DescriptionName
        {
            get
            {
                return this.Fecha.ToString();
            }
        }

        [Key]
        [Column(Order = 0)]
        public int IdCalendario { get; set; }
        [Key]
        [Column(Order = 1)]
        public int IdDiaNoLaborable { get; set; }

        public DateTime Fecha { get; set; }
        public bool DiaParcial { get; set; }
        public bool EsteAño { get; set; }
        public string HoraInicio { get; set; }
        public string HoraFin { get; set; }
        public string UsrIng { get; set; }
        public DateTime FecIng { get; set; }
        public string UsrMod { get; set; }
        public DateTime? FecMod { get; set; }
        [ForeignKey("IdCalendario")]
        public virtual Calendario Calendario { get; set; }

        public void AsignarId()
        {
            ContextService service = new ContextService();
            this.IdDiaNoLaborable = service.DiasNoLaborables.GetMaxValue<int>(p => p.IdDiaNoLaborable, 0, p => p.IdCalendario == this.IdCalendario) + 1;
        }
    }
}
