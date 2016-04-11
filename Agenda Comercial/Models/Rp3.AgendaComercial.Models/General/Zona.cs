using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rp3.Data.Models.General;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rp3.AgendaComercial.Models.General
{
    [Table("tbZona", Schema = "gen")]
    public class Zona : Rp3.Data.Entity.EntityBase
    {
        public override string Key
        {
            get
            {
                if (this.IdZona == 0) return " ";
                return this.IdZona.ToString();
            }
        }

        public override string DescriptionName
        {
            get
            {
                return this.Descripcion;
            }
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdZona { get; set; }

        [Required(ErrorMessageResourceType = typeof(Rp3.AgendaComercial.Resources.ErrorMessageValidation), ErrorMessageResourceName = "Required")]
        [StringLength(250, MinimumLength = 1, ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "MaxMinStringLength")]
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Zona")]
        public string Descripcion { get; set; }
        
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Region")]
        [Required(ErrorMessageResourceType = typeof(Rp3.AgendaComercial.Resources.ErrorMessageValidation), ErrorMessageResourceName = "Required")]
        public int IdRegion { get; set; }        
        public short EstadoTabla { get; set; }        
        public string Estado { get; set; }

        public short TipoTabla { get; set; }
        public string Tipo { get; set; }

        public string UsrIng { get; set; }
        public int TiempoMovilizacion { get; set; }
        public double? LongitudPuntoPartida { get; set; }
        public double? LatitudPuntoPartida { get; set; }
        public DateTime FecIng { get; set; }
        public string UsrMod { get; set; }
        public DateTime? FecMod { get; set; }

        [ForeignKey("EstadoTabla,Estado")]
        public virtual GeneralValues EstadoGeneralValue { get; set; }

        [ForeignKey("TipoTabla,Tipo")]
        public virtual GeneralValues TipoGeneralValue { get; set; }

        [ForeignKey("IdRegion")]
        public virtual Region Region { get; set; }
        public virtual List<ZonaDetalle> ZonaDetalles { get; set; }
        public virtual List<ZonaGeocerca> ZonaGeocercas { get; set; }

        public virtual List<ZonaClienteGeocerca> ZonaClienteGeocercas { get; set; }

        [NotMapped]    
        public bool ReadOnly { get; set; }

        public void AsignarId()
        {
            ContextService service = new ContextService();
            this.IdZona = service.Zonas.GetMaxValue<int>(p => p.IdZona, 0) + 1;
        }
    }
}
