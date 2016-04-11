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
    [Table("tbMemoDestinatario", Schema = "rut")]
    public class MemoDestinatario : Rp3.Data.Entity.EntityBase
    {
        public override string Key
        {
            get
            {
                return string.Format("{0}-{1}", this.IdMemo, this.IdAgente);
            }
        }

        [Key]
        [Column(Order = 0)]
        public long IdMemo { get; set; }

        [Key]
        [Column(Order = 1)]
        public int IdAgente { get; set; }

        [ForeignKey("IdMemo")]
        public virtual Memo Memo { get; set; }

        [ForeignKey("IdAgente")]
        public virtual Agente Agente { get; set; }
    }
}
