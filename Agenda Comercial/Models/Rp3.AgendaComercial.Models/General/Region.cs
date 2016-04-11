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
    [Table("tbRegion", Schema = "gen")]
    public class Region : Rp3.Data.Entity.EntityBase
    {
        public override string Key
        {
            get
            {
                if (this.IdRegion == 0) return " ";
                return this.IdRegion.ToString();
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
        public int IdRegion { get; set; }

        [Required(ErrorMessageResourceType = typeof(Rp3.AgendaComercial.Resources.ErrorMessageValidation), ErrorMessageResourceName = "Required")]
        [StringLength(250, MinimumLength = 1, ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "MaxMinStringLength")]
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Region")]
        public string Descripcion { get; set; }        
        public short EstadoTabla { get; set; }        
        public string Estado { get; set; }
        public string UsrIng { get; set; }
        public DateTime FecIng { get; set; }
        public string UsrMod { get; set; }
        public DateTime? FecMod { get; set; }
        [ForeignKey("EstadoTabla,Estado")]
        public virtual GeneralValues EstadoGeneralValue { get; set; }

        public List<Zona> Zonas { get; set; }
        public void AsignarId()
        {
            ContextService service = new ContextService();
            this.IdRegion = service.Regiones.GetMaxValue<int>(p => p.IdRegion, 0) + 1;
        }
    }
}
