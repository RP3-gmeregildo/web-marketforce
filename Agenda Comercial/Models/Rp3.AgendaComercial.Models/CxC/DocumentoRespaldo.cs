using Rp3.AgendaComercial.Models.Financiero;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rp3.AgendaComercial.Models.CxC
{
    [Table("tbDocumentoRespaldo", Schema = "cxc")]
    public class DocumentoRespaldo : Rp3.Data.Entity.EntityBase
    {
        public override string Key
        {
            get
            {
                return string.Format("{0}-{1}", this.IdDocumento, this.IdDocumentoRespaldo);
            }
        }

        public string KeyCombo
        {
            get
            {
                if (this.IdDocumentoRespaldo == 0) return " ";
                return this.IdDocumentoRespaldo.ToString();
            }
        }

        [Key]
        [Column(Order = 0)]
        public long IdDocumento { get; set; }

        [Key]
        [Column(Order = 1)]
        public int IdDocumentoRespaldo { get; set; }
        public int IdBanco { get; set; }
        public string NumeroCuenta { get; set; }
        public string NumeroDocumento { get; set; }
        public DateTime FechaEmision { get; set; }
        public string Observacion { get; set; }
        public decimal Valor { get; set; }
        public string UsrIng { get; set; }
        public DateTime FecIng { get; set; }
        public string UsrMod { get; set; }
        public DateTime? FecMod { get; set; }

        [ForeignKey("IdDocumento")]
        public virtual Documento Documento { get; set; }
        [ForeignKey("IdBanco")]
        public virtual Banco Banco { get; set; }
        public void AsignarId()
        {
            ContextService service = new ContextService();
            this.IdDocumentoRespaldo = service.DocumentoRespaldos.GetMaxValue<int>(p => p.IdDocumentoRespaldo, 0, p => p.IdDocumento == this.IdDocumento) + 1;
        }
    }
}
