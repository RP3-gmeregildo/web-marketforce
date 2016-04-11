using Rp3.AgendaComercial.Models.General;
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
    [Table("tbOportunidadResponsable", Schema = "opt")]
    public class OportunidadResponsable : Rp3.Data.Entity.EntityBase
    {
        public override string Key
        {
            get
            {
                return string.Format("{0}-{1}", this.IdOportunidad, this.IdAgente);
            }
        }
        [Key]
        [Column(Order = 0)]
        public int IdOportunidad { get; set; }

        [Key]
        [Column(Order = 1)]
        public int IdAgente { get; set; }

        [ForeignKey("IdOportunidad"), NonSerializableToXmlAttribute]
        public virtual Oportunidad Oportunidad { get; set; }

        [ForeignKey("IdAgente"), NonSerializableToXmlAttribute]
        public virtual Agente Agente { get; set; }

        public short TipoTabla { get; set; }
        public string Tipo { get; set; }
    }
}
