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
    [Table("tbOportunidadBitacora", Schema = "opt")]
    public class OportunidadBitacora : Rp3.Data.Entity.EntityBase
    {
        public override string Key
        {
            get
            {
                return string.Format("{0}-{1}", this.IdOportunidad, this.IdOportunidadBitacora);
            }
        }
        [Key]
        [Column(Order = 0)]
        public int IdOportunidad { get; set; }
        [Key]
        [Column(Order = 1)]
        public int IdOportunidadBitacora { get; set; }
        public string Detalle { get; set; }
        public int IdAgente { get; set; }
        public DateTime FecIng { get; set; }

        [ForeignKey("IdAgente"), NonSerializableToXmlAttribute]
        public virtual Agente Agente { get; set; }
        [ForeignKey("IdOportunidad"), NonSerializableToXmlAttribute]
        public virtual Oportunidad Oportunidad { get; set; }
        public void AsignarId()
        {
            ContextService service = new ContextService();
            this.IdOportunidadBitacora = service.OportunidadBitacoras.GetMaxValue<int>(p => p.IdOportunidadBitacora, 0, p => p.IdOportunidad == this.IdOportunidad) + 1;
        }
    }
}
