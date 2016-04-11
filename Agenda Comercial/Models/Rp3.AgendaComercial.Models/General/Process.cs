using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rp3.AgendaComercial.Models.General
{
    [Table("tbProcess", Schema = "gen")]
    public class Process : Rp3.Data.Entity.EntityBase
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdProcess { get; set; }
        public string Descripcion { get; set; }
        public string UsrIng { get; set; }
        public DateTime FecIng { get; set; }

        public void AsignarId()
        {
            ContextService service = new ContextService();
            this.IdProcess = service.Processes.GetMaxValue<int>(p => p.IdProcess, 0) + 1;
        }
    }
}
