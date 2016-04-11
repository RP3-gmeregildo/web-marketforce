using Rp3.AgendaComercial.Models.General;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rp3.AgendaComercial.Models.Transaccion
{
    [Table("tbCajaControlFormaPago", Schema = "trn")]
    public class CajaControlFormaPago : Rp3.Data.Entity.EntityBase
    {
        public override string Key
        {
            get
            {
                return string.Format("{0}-{1}-{2}", this.IdCaja, this.IdControlCaja, this.IdFormaPago);
            }
        }

        public string KeyCombo
        {
            get
            {
                if (this.IdControlCaja == 0) return " ";
                return this.IdControlCaja.ToString();
            }
        }

        [Key]
        [Column(Order = 0)]
        public int IdCaja { get; set; }

        [Key]
        [Column(Order = 1)]
        public int IdControlCaja { get; set; }

        [Key]
        [Column(Order = 2)]
        public int IdFormaPago { get; set; }

        public decimal Monto { get; set; }
        public string UsrIng { get; set; }
        public DateTime FecIng { get; set; }
        public string UsrMod { get; set; }
        public DateTime? FecMod { get; set; }

        [ForeignKey("IdFormaPago")]
        public virtual FormaPago FormaPago { get; set; }
        [ForeignKey("IdCaja")]
        public virtual Caja Caja { get; set; }
        [ForeignKey("IdCaja, IdControlCaja")]
        public virtual CajaControl CajaControl { get; set; }
    }
}
