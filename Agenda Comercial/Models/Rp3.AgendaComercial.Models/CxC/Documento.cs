using Rp3.AgendaComercial.Models.General;
using Rp3.Data.Models.General;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rp3.AgendaComercial.Models.CxC
{
    [Table("tbDocumento", Schema = "cxc")]
    public class Documento : Rp3.Data.Entity.EntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long IdDocumento { get; set; }

        public int IdCliente { get; set; }

        public DateTime FechaEmision { get; set; }

        public string NumeroDocumento { get; set; }

        public short EstadoTabla { get; set; }
        public string Estado { get; set; }

        public decimal Total { get; set; }
        public decimal Cobrado { get; set; }
        public decimal Vencido { get; set; }
        public decimal Saldo { get; set; }
        public string ResumenRespaldo { get; set; }
        public int AgenteVenta { get; set; }
        public string AgenteVentaNombre { get; set; }

        public string UsrIng { get; set; }
        public DateTime FecIng { get; set; }
        public string UsrMod { get; set; }
        public DateTime? FecMod { get; set; }

        [ForeignKey("EstadoTabla,Estado")]
        public virtual GeneralValues EstadoGeneralValue { get; set; }

        [ForeignKey("IdCliente")]
        public virtual Cliente Cliente { get; set; }

        public virtual List<Dividendo> Dividendos { get; set; }
        public virtual List<DocumentoRespaldo> DocumentoRespaldos { get; set; }

        public void AsignarId()
        {
            ContextService service = new ContextService();
            this.IdDocumento = service.Documentos.GetMaxValue<long>(p => p.IdDocumento, 0) + 1;
        }
    }
}
