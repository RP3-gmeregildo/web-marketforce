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

namespace Rp3.AgendaComercial.Models.Marcacion
{
    [Table("tbPermiso", Schema = "mar")]
    public class Permiso : Rp3.Data.Entity.EntityBase
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)]
        public long IdPermiso { get; set; }
        public int? IdAgente { get; set; }
        public int? IdGrupo { get; set; }
        public short TipoTabla { get; set; }
        public string Tipo { get; set; }
        public short MotivoTabla { get; set; }
        public string Motivo { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public DateTime HoraInicio { get; set; }
        public DateTime HoraFin { get; set; }
        public string Observacion { get; set; }
        public string ObservacionSupervisor { get; set; }
        public bool EsPrevio { get; set; }
        public short EstadoTabla { get; set; }
        public string Estado { get; set; }
        public string UsrIng { get; set; }
        public DateTime FecIng { get; set; }
        public string UsrMod { get; set; }
        public DateTime? FecMod { get; set; }

        [ForeignKey("IdAgente")]
        public virtual Agente Agente { get; set; }

        [ForeignKey("IdGrupo")]
        public virtual Grupo Grupo { get; set; }

        public void AsignarId()
        {
            ContextService service = new ContextService();
            this.IdPermiso = service.Permisos.GetMaxValue<long>(p => p.IdPermiso, 0) + 1;
        }

        [ForeignKey("TipoTabla,Tipo")]
        public virtual GeneralValues TipoGeneralValue { get; set; }

        [ForeignKey("MotivoTabla,Motivo")]
        public virtual GeneralValues MotivoGeneralValue { get; set; }

        [ForeignKey("EstadoTabla,Estado")]
        public virtual GeneralValues EstadoGeneralValue { get; set; }
    }
}
