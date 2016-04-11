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
    [Table("tbMarcacion", Schema = "mar")]
    public class Marcacion : Rp3.Data.Entity.EntityBase
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)]
        public long IdMarcacion { get; set; }
        public short TipoTabla { get; set; }
        public string Tipo { get; set; }
        public int IdAgente { get; set; }
        public DateTime Fecha { get; set; }
        public int IdGrupo { get; set; }
        public DateTime? HoraInicioPlan { get; set; }
        public DateTime? HoraFinPlan { get; set; }
        public DateTime? HoraInicio { get; set; }
        public DateTime? HoraFin { get; set; }
        public bool Ausente { get; set; }
        public bool Atraso { get; set; }
        public int MinutosAtraso { get; set; }
        public bool EnUbicacion { get; set; }
        public double? LongitudPlan { get; set; }
        public double? LatitudPlan { get; set; }
        public double? Longitud { get; set; }
        public double? Latitud { get; set; }
        public long? IdPermiso { get; set; }
        public DateTime? FecIng { get; set; }

        [ForeignKey("IdPermiso")]
        public virtual Permiso Permiso { get; set; }

        [ForeignKey("IdAgente")]
        public virtual Agente Agente { get; set; }

        [ForeignKey("IdGrupo")]
        public virtual Grupo Grupo { get; set; }

        [NotMapped, Rp3.Data.NonSerializableToXml]
        public string Ubicacion { get; set; }

        [NotMapped, Rp3.Data.NonSerializableToXml]
        public int Reportes { get; set; }

        public void AsignarId()
        {
            ContextService service = new ContextService();
            this.IdMarcacion = service.Marcacions.GetMaxValue<long>(p => p.IdMarcacion, 0) + 1;
        }

        //[ForeignKey("TipoTabla,Tipo")]
        //public virtual GeneralValues TipoGeneralValue { get; set; }
        [ForeignKey("TipoTabla,Tipo")]
        public virtual GeneralValues TipoGeneralValue { get; set; }

        public static int Efectividad(double minutosAtraso)
        {
            if (minutosAtraso <= 2)
                return 100;
            else if (minutosAtraso > 2 && minutosAtraso <= 10)
                return 98;
            else if (minutosAtraso > 10 && minutosAtraso <= 15)
                return 95;
            else if (minutosAtraso > 15 && minutosAtraso <= 30)
                return 80;
            else if (minutosAtraso > 30 && minutosAtraso <= 40)
                return 70;
            else if (minutosAtraso > 40 && minutosAtraso <= 50)
                return 60;
            else if (minutosAtraso > 50 )
                return 50;

            return 0;
        }
    }
}
