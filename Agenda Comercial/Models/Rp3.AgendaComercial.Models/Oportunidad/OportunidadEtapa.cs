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
    [Table("tbOportunidadEtapa", Schema = "opt")]
    public class OportunidadEtapa : Rp3.Data.Entity.EntityBase
    {
        [Key]
        [Column(Order = 0)]
        public int IdOportunidad { get; set; }

        [Key]
        [Column(Order = 1)]
        public int IdEtapa { get; set; }

        public short EstadoTabla { get; set; }
        public string Estado { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public int Orden { get; set; }
        public string Observacion { get; set; }

        public int? IdEtapaPadre { get; set; }
        public DateTime? FechaFinPlan { get; set; }

        [ForeignKey("IdOportunidad"), NonSerializableToXmlAttribute]
        public virtual Oportunidad Oportunidad { get; set; }

        [ForeignKey("IdEtapa"), NonSerializableToXmlAttribute]
        public virtual Etapa Etapa { get; set; }

        [NonSerializableToXmlAttribute]
        public virtual List<OportunidadTarea> OportunidadTareas { get; set; }

        [NotMapped]
        public List<OportunidadEtapa> OportunidadEtapas { get; set; }

        public bool Etapa01Show { get { return this.Etapa.Orden == 1 && this.Estado != Rp3.AgendaComercial.Models.Constantes.EstadoEtapaOportunidad.Pendiente; } }
        public bool Etapa02Show { get { return this.Etapa.Orden == 2 && this.Estado != Rp3.AgendaComercial.Models.Constantes.EstadoEtapaOportunidad.Pendiente; } }
        public bool Etapa03Show { get { return this.Etapa.Orden == 3 && this.Estado != Rp3.AgendaComercial.Models.Constantes.EstadoEtapaOportunidad.Pendiente; } }
        public bool Etapa04Show { get { return this.Etapa.Orden == 4 && this.Estado != Rp3.AgendaComercial.Models.Constantes.EstadoEtapaOportunidad.Pendiente; } }
        public bool Etapa05Show { get { return this.Etapa.Orden == 5 && this.Estado != Rp3.AgendaComercial.Models.Constantes.EstadoEtapaOportunidad.Pendiente; } }
        public bool NoMostrar { get { return !this.Etapa01Show & !this.Etapa02Show & !this.Etapa03Show & !this.Etapa04Show & !this.Etapa05Show; } }

        public int DiasTranscurridos
        {
            get
            {
                int value = 0;
                if (this.FechaInicio.HasValue)
                    if (this.FechaFin.HasValue)
                        value = Convert.ToInt32((this.FechaFin.Value - this.FechaInicio.Value).TotalDays);
                    else
                        value = Convert.ToInt32((DateTime.Now - this.FechaInicio.Value).TotalDays);

                return value;
            }
        }

        public string FechaInicioDisplay
        {
            get
            {
                if (this.FechaInicio.HasValue)
                    return this.FechaInicio.Value.ToString("dd/MM/yyyy");
                else return "Sin iniciar";

            }
        }
    }
}
