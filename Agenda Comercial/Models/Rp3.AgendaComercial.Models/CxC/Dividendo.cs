using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rp3.AgendaComercial.Models.CxC
{
    [Table("tbDividendo", Schema = "cxc")]
    public class Dividendo : Rp3.Data.Entity.EntityBase
    {
        public override string Key
        {
            get
            {
                return string.Format("{0}-{1}", this.IdDocumento, this.IdDividendo);
            }
        }

        public string KeyCombo
        {
            get
            {
                if (this.IdDividendo == 0) return " ";
                return this.IdDividendo.ToString();
            }
        }

        [Key]
        [Column(Order = 0)]
        public long IdDocumento { get; set; }

        [Key]
        [Column(Order = 1)]
        public int IdDividendo { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public decimal ValorDividendo { get; set; }
        public decimal Interes { get; set; }
        public decimal Capital { get; set; }
        public decimal SaldoCapital { get; set; }
        public decimal SaldoDividendo { get; set; }
        public string UsrIng { get; set; }
        public DateTime FecIng { get; set; }
        public string UsrMod { get; set; }
        public DateTime? FecMod { get; set; }

        [ForeignKey("IdDocumento")]
        public virtual Documento Documento { get; set; }
        public void AsignarId()
        {
            ContextService service = new ContextService();
            this.IdDocumento = service.Dividendos.GetMaxValue<int>(p => p.IdDividendo, 0, p => p.IdDocumento == this.IdDocumento) + 1;
        }
    }
}
