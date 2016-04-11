using Rp3.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rp3.AgendaComercial.Models.Oportunidad
{
    [Table("tbOportunidadTareaActividad", Schema = "opt")]
    public class OportunidadTareaActividad : Rp3.Data.Entity.EntityBase
    {
        [Key]
        [Column(Order = 0)]
        public int IdOportunidad { get; set; }
        [Key]
        [Column(Order = 1)]
        public int IdEtapa { get; set; }
        [Key]
        [Column(Order = 2)]
        public long IdTarea { get; set; }
        [Key]
        [Column(Order = 3)]
        public int IdTareaActividad { get; set; }
        public string Descripcion { get; set; }
        public int IdTipoActividad { get; set; }
        public string Opciones { get; set; }
        public int Orden { get; set; }
        public int? IdTareaActividadPadre { get; set; }
        public int? IdTareaOpcion { get; set; }
        public string Resultado { get; set; }
        public string ResultadoCodigo { get; set; }

        [ForeignKey("IdOportunidad,IdEtapa,IdTarea"), NonSerializableToXmlAttribute]
        public virtual OportunidadTarea OportunidadTarea { get; set; }
    }
}
