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
    [Table("tbCalificacionCarteraTipoCliente", Schema = "cxc")]
    public class CalificacionCarteraTipoCliente : Rp3.Data.Entity.EntityBase
    {
        public override string Key
        {
            get
            {
                return string.Format("{0}-{1}", this.IdCalificacionCartera, this.IdTipoCliente);
            }
        }

        public string KeyCombo
        {
            get
            {
                if (this.IdTipoCliente == 0) return " ";
                return this.IdTipoCliente.ToString();
            }
        }

        [Key]
        [Column(Order = 0)]
        public int IdCalificacionCartera { get; set; }

        [Key]
        [Column(Order = 1)]
        public int IdTipoCliente { get; set; }
        public string UsrIng { get; set; }
        public DateTime FecIng { get; set; }
        public string UsrMod { get; set; }
        public DateTime? FecMod { get; set; }

        [ForeignKey("IdCalificacionCartera")]
        public virtual CalificacionCartera CalificacionCartera { get; set; }
        [ForeignKey("IdTipoCliente")]
        public virtual TipoCliente TipoCliente { get; set; }
    }
}
