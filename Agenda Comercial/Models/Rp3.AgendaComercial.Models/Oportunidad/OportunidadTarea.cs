using Rp3.AgendaComercial.Models.General;
using Rp3.Data;
using Rp3.Data.Models.General;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rp3.AgendaComercial.Models.Oportunidad
{
    [Table("tbOportunidadTarea", Schema = "opt")]
    public class OportunidadTarea : Rp3.Data.Entity.EntityBase
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
        public short EstadoTabla { get; set; }
        public string Estado { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public int Orden { get; set; }
        public string Observacion { get; set; }

        [ForeignKey("EstadoTabla,Estado"), NonSerializableToXmlAttribute]
        public virtual GeneralValues EstadoGeneralValue { get; set; }

        [ForeignKey("IdOportunidad"), NonSerializableToXmlAttribute]
        public virtual Oportunidad Oportunidad { get; set; }

        [ForeignKey("IdOportunidad, IdEtapa"), NonSerializableToXmlAttribute]
        public virtual OportunidadEtapa OportunidadEtapa { get; set; }

        [ForeignKey("IdTarea"), NonSerializableToXmlAttribute]
        public virtual Tarea Tarea { get; set; }

        public virtual List<OportunidadTareaActividad> OportunidadTareaActividads { get; set; }
    }
}
