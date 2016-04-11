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
    [Table("tbCajaControl", Schema = "trn")]
    public class CajaControl : Rp3.Data.Entity.EntityBase
    {
        public override string Key
        {
            get
            {
                return string.Format("{0}-{1}", this.IdCaja, this.IdControlCaja);
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
        public DateTime FechaApertura { get; set; }
        public DateTime FechaCierre { get; set; }
        public decimal MontoApertura { get; set; }
        public decimal MontoCierre { get; set; }
        public bool Activo { get; set; }
        public string UsrIng { get; set; }
        public DateTime FecIng { get; set; }
        public string UsrMod { get; set; }
        public DateTime? FecMod { get; set; }

        [ForeignKey("IdCaja")]
        public virtual Caja Caja { get; set; }
        public virtual List<CajaControlFormaPago> CajaControlFormasPago { get; set; }
        public void AsignarId()
        {
            ContextService service = new ContextService();
            this.IdControlCaja = service.CajaControles.GetMaxValue<int>(p => p.IdControlCaja, 0, p => p.IdCaja == this.IdCaja) + 1;
        }
    }
}
