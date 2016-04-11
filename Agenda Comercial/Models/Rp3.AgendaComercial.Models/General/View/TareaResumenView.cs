using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rp3.AgendaComercial.Models.General.View
{
    [Table("vwTareaResumen", Schema = "dbo")]
    public class TareaResumenView : Rp3.Data.Entity.EntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long IdTarea { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaVigenciaDesde { get; set; }
        public DateTime? FechaVigenciaHasta { get; set; }
        public bool Vigente { get; set; }
        public int NumeroGestiones { get; set; }
        public int NumeroClientes { get; set; }
        public int NumeroPreguntas { get; set; }
        public DateTime? PrimeraGestion { get; set; }
        public DateTime? UltimaGestion { get; set; } 
    }
}
