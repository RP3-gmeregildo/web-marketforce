using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rp3.Data.Models.General;
using Rp3.AgendaComercial.Models.General;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rp3.AgendaComercial.Models.Ruta
{
    [Table("tbLoteTipoCliente", Schema = "rut")]
    public class LoteTipoCliente : Rp3.Data.Entity.EntityBase
    {
        public override string Key
        {
            get
            {
                return string.Format("{0}-{1}", this.IdLote, this.IdTipoCliente);
            }
        }

        [Key]
        [Column(Order = 0)]
        public int IdLote { get; set; }

        [Key]
        [Column(Order = 1)]
        public int IdTipoCliente { get; set; }

        [ForeignKey("IdLote")]
        public virtual Lote Lote { get; set; }

        [ForeignKey("IdTipoCliente")]
        public virtual TipoCliente TipoCliente { get; set; }
    }
}
