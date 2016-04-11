using Rp3.AgendaComercial.Models.General;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rp3.AgendaComercial.Models.Ruta
{
    [Table("tbProgramacionRutaTarea", Schema = "rut")]
    public class ProgramacionRutaTarea : Rp3.Data.Entity.EntityBase
    {
        public override string Key
        {
            get
            {
                return string.Format("{0}-{1}", this.IdProgramacionRuta, this.IdTarea);
            }
        }

        [Key]
        [Column(Order = 0)]
        public int IdProgramacionRuta { get; set; }

        [Key]
        [Column(Order = 1)]
        public long IdTarea { get; set; }

        [ForeignKey("IdTarea")]
        public virtual Tarea Tarea { get; set; }

        [ForeignKey("IdProgramacionRuta")]
        public virtual ProgramacionRuta ProgramacionRuta { get; set; }

        
        [NotMapped]
        public bool ReadOnly { get; set; }

    }
}
