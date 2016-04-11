using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rp3.Data.Models.General;
using System.ComponentModel.DataAnnotations;
using Rp3.AgendaComercial.Models.General;
using System.ComponentModel.DataAnnotations.Schema;
using Rp3.Data;

namespace Rp3.AgendaComercial.Models.Ruta
{
    [Table("tbAgendaTareaActividad", Schema = "rut")]
    public class AgendaTareaActividad : Rp3.Data.Entity.EntityBase
    {
        public override string Key
        {
            get
            {
                return string.Format("{0}-{1}-{2}-{3}", this.IdRuta, this.IdAgenda, this.IdTarea, this.IdTareaActividad);
            }
        }

        [Key]
        [Column(Order = 0)]
        public int IdRuta { get; set; }

        [Key]
        [Column(Order = 1)]
        public long IdAgenda { get; set; }

        [Key]
        [Column(Order = 2)]
        public long IdTarea { get; set; }

        [Key]
        [Column(Order = 3)]
        public int IdTareaActividad { get; set; }

        public string Descripcion { get; set; }

        public int IdTipoActividad { get; set; }

        public int Orden { get; set; }

        public string Opciones { get; set; }

        public int? IdTareaActividadPadre { get; set; }

        public int? IdTareaOpcion { get; set; }

        public string Resultado { get; set; }
        public string ResultadoCodigo { get; set; }

        [ForeignKey("IdRuta, IdAgenda, IdTarea"), NonSerializableToXmlAttribute]
        public virtual AgendaTarea AgendaTarea { get; set; }

        [ForeignKey("IdTipoActividad"), NonSerializableToXmlAttribute]
        public virtual TipoActividad TipoActividad { get; set; }
    }
}
