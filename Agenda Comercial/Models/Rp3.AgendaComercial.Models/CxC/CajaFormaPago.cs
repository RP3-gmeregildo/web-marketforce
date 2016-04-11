using Rp3.AgendaComercial.Models.General;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rp3.AgendaComercial.Models.CxC
{
    [Table("tbCajaFormaPago", Schema = "cxc")]
    public class CajaFormaPago : Rp3.Data.Entity.EntityBase
    {
        public override string Key
        {
            get
            {
                return string.Format("{0}-{1}", this.IdCaja, this.IdFormaPago);
            }
        }

        public string KeyCombo
        {
            get
            {
                if (this.IdFormaPago == 0) return " ";
                return this.IdFormaPago.ToString();
            }
        }

        [Key]
        [Column(Order = 0)]
        public int IdCaja { get; set; }

        [Key]
        [Column(Order = 1)]
        public int IdFormaPago { get; set; }
        public string UsrIng { get; set; }
        public DateTime FecIng { get; set; }
        public string UsrMod { get; set; }
        public DateTime? FecMod { get; set; }

        [ForeignKey("IdFormaPago")]
        public virtual FormaPago FormaPago { get; set; }
        [ForeignKey("IdCaja")]
        public virtual Caja Caja { get; set; }
    }
}
