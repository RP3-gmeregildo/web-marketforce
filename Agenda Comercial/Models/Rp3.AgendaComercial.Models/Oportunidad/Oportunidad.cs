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
    [Table("tbOportunidad", Schema = "opt")]
    public class Oportunidad : Rp3.Data.Entity.EntityBase
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)]
        public int IdOportunidad { get; set; }
        public string Descripcion { get; set; }
        public decimal Probabilidad { get; set; }
        public decimal Importe { get; set; }
        public int IdAgente { get; set; }
        public DateTime? FechaUltimaGestion { get; set; }
        public int Calificacion { get; set; }
        public string Observacion { get; set; }
        public string Direccion { get; set; }
        public string Referencia { get; set; }
        public double? Latitud { get; set; }
        public double? Longitud { get; set; }
        public int IdEtapa { get; set; }
        public int IdOportunidadTipo { get; set; }
        public short EstadoTabla { get; set; }
        public string Estado { get; set; }
        public string UsrIng { get; set; }
        public DateTime FecIng { get; set; }
        public string UsrMod { get; set; }
        public DateTime? FecMod { get; set; }

        public string ReferenciaDireccion { get; set; }
        public string Telefono1 { get; set; }
        public string Telefono2 { get; set; }
        public string CorreoElectronico { get; set; }
        public string PaginaWeb { get; set; }
        public string TipoEmpresa { get; set; }

        [ForeignKey("IdEtapa"), NonSerializableToXmlAttribute]
        public virtual Etapa Etapa { get; set; }

        [ForeignKey("IdOportunidadTipo"), NonSerializableToXmlAttribute]
        public OportunidadTipo OportunidadTipo { get; set; }

        [ForeignKey("IdAgente"), NonSerializableToXmlAttribute]
        public virtual Agente Agente { get; set; }

        [ForeignKey("EstadoTabla,Estado"), NonSerializableToXmlAttribute]
        public virtual GeneralValues EstadoGeneralValue { get; set; }

        public virtual List<OportunidadContacto> OportunidadContactos { get; set; }
        public virtual List<OportunidadMedia> OportunidadMedias { get; set; }

        public virtual List<OportunidadTarea> OportunidadTareas { get; set; }

        public virtual List<OportunidadEtapa> OportunidadEtapas { get; set; }
        public virtual List<OportunidadResponsable> OportunidadResponsables { get; set; }
        public virtual List<OportunidadBitacora> OportunidadBitacoras { get; set; }
        public void AsignarId()
        {
            ContextService service = new ContextService();
            this.IdOportunidad = service.Oportunidades.GetMaxValue<int>(p => p.IdOportunidad, 0) + 1;
        }

    }
}
