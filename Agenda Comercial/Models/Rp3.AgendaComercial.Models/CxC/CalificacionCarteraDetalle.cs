using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rp3.AgendaComercial.Models.CxC
{
    [Table("tbCalificacionCarteraDetalle", Schema = "cxc")]
    public class CalificacionCarteraDetalle : Rp3.Data.Entity.EntityBase
    {
        public override string Key
        {
            get
            {
                return string.Format("{0}-{1}", this.IdCalificacionCartera, this.IdDetalle);
            }
        }

        public string KeyCombo
        {
            get
            {
                if (this.IdDetalle == 0) return " ";
                return this.IdDetalle.ToString();
            }
        }

        [Key]
        [Column(Order = 0)]
        public int IdCalificacionCartera { get; set; }

        [Key]
        [Column(Order = 1)]
        public int IdDetalle { get; set; }
        public string Nombre { get; set; }
        public int DiasDesde { get; set; }
        public int DiasHasta { get; set; }

        public string AccionComentario { get; set; }
        public string UsrIng { get; set; }
        public DateTime FecIng { get; set; }
        public string UsrMod { get; set; }
        public DateTime? FecMod { get; set; }

        [ForeignKey("IdCalificacionCartera")]
        public virtual CalificacionCartera CalificacionCartera { get; set; }
        public void AsignarId()
        {
            ContextService service = new ContextService();
            this.IdDetalle = service.CalificacionCarteraDetalles.GetMaxValue<int>(p => p.IdDetalle, 0, p => p.IdCalificacionCartera == this.IdCalificacionCartera) + 1;
        }
    }
}
