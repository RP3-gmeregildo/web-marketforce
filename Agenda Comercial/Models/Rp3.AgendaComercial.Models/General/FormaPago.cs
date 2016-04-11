using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rp3.AgendaComercial.Models.General
{
    [Table("tbFormaPago", Schema = "gen")]
    public class FormaPago : Rp3.Data.Entity.EntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdFormaPago { get; set; }

        public string Nombre { get; set; }
        public bool Activo { get; set; }
        public string UsrIng { get; set; }
        public DateTime FecIng { get; set; }
        public string UsrMod { get; set; }
        public DateTime? FecMod { get; set; }
        public void AsignarId()
        {
            ContextService service = new ContextService();
            this.IdFormaPago = service.FormasPagos.GetMaxValue<int>(p => p.IdFormaPago, 0) + 1;
        }
    }
}
