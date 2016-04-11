using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rp3.AgendaComercial.Models.CxC
{
    [Table("tbTipoDocumento", Schema = "cxc")]
    public class TipoDocumento : Rp3.Data.Entity.EntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdTipoDocumento { get; set; }

        public string Nombre { get; set; }

        public bool Activo { get; set; }
        public string UsrIng { get; set; }
        public DateTime FecIng { get; set; }
        public string UsrMod { get; set; }
        public DateTime? FecMod { get; set; }
        public void AsignarId()
        {
            ContextService service = new ContextService();
            this.IdTipoDocumento = service.TipoDocumentos.GetMaxValue<int>(p => p.IdTipoDocumento, 0) + 1;
        }
    }
}
