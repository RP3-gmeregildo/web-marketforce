using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rp3.AgendaComercial.Models.General
{
    [Table("tbTareaClienteActualizacion", Schema = "gen")]
    public class TareaClienteActualizacion : Rp3.Data.Entity.EntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long IdTarea { get; set; }

        public bool PermitirCreacion { get; set; }

        public bool PermitirModificacion { get; set; }

        public bool SiempreEditarEnGestion { get; set; }

        public bool SoloFaltantesEditarEnGestion { get; set; }

    }
}
