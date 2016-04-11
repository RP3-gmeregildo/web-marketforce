using Rp3.AgendaComercial.Models.General;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rp3.AgendaComercial.Models.Oportunidad
{
    [Table("tbEtapaTarea", Schema = "opt")]
    public class EtapaTarea : Rp3.Data.Entity.EntityBase
    {
        [Key]
        [Column(Order = 0)]
        public int IdEtapa { get; set; }

        [Key]
        [Column(Order = 1)]
        public long IdTarea { get; set; }
        public int Orden { get; set; }

        [ForeignKey("IdEtapa")]
        public virtual Etapa Etapa { get; set; }

        [ForeignKey("IdTarea")]
        public virtual Tarea Tarea { get; set; }
    }
}
