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
    [Table("tbCalificacionCartera", Schema = "cxc")]
    public class CalificacionCartera : Rp3.Data.Entity.EntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdCalificacionCartera { get; set; }

        public string Nombre { get; set; }
        public decimal MontoDesde { get; set; }
        public decimal MontoHasta { get; set; }
        public short EstadoTabla { get; set; }
        public string Estado { get; set; }
        public string UsrIng { get; set; }
        public DateTime FecIng { get; set; }
        public string UsrMod { get; set; }
        public DateTime? FecMod { get; set; }
        [ForeignKey("EstadoTabla,Estado")]
        public virtual GeneralValues EstadoGeneralValue { get; set; }
        public void AsignarId()
        {
            ContextService service = new ContextService();
            this.IdCalificacionCartera = service.CalificacionesCartera.GetMaxValue<int>(p => p.IdCalificacionCartera, 0) + 1;
        }
    }
}
