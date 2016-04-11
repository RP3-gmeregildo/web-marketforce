using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rp3.AgendaComercial.Models.General
{
    [Table("tbDiaLaboral", Schema = "gen")]
    public class DiaLaboral : Rp3.Data.Entity.EntityBase
    {
        public override string Key
        {
            get
            {
                return string.Format("{0}-{1}", this.IdCalendario, this.IdDia);
            }
        }

        public string KeyCombo
        {
            get
            {
                if (this.IdCalendario == 0) return " ";
                return this.IdCalendario.ToString();
            }
        }

        [Key]
        [Column(Order = 0)]
        public int IdCalendario { get; set; }

        [Key]
        [Column(Order = 1)]
        public string IdDia { get; set; }
        public short IdDiaTabla { get; set; }
        [NotMapped]
        public string DiaString { get; set; }
        [NotMapped]
        public bool SegundaJornada { get; set; }
        public int Orden { get; set; }
        public bool EsLaboral { get; set; }
        public string HoraInicio1 { get; set; }
        public string HoraFin1 { get; set; }
        public string HoraInicio2 { get; set; }
        public string HoraFin2 { get; set; }
        public string UsrIng { get; set; }
        public DateTime FecIng { get; set; }
        public string UsrMod { get; set; }
        public DateTime? FecMod { get; set; }
        [ForeignKey("IdCalendario")]
        public virtual Calendario Calendario { get; set; }
       
    }
}