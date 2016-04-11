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
    [Table("tbTipoActividad", Schema = "gen")]
    public class TipoActividad :  Rp3.Data.Entity.EntityBase
    {
        public override string Key
        {
            get
            {
                if (this.IdTipoActividad == 0) return " ";
                return this.IdTipoActividad.ToString();
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
        public int IdTipoActividad { get; set; }

        [Required(ErrorMessageResourceType = typeof(Rp3.AgendaComercial.Resources.ErrorMessageValidation), ErrorMessageResourceName = "Required")]
        [StringLength(250, MinimumLength = 1, ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "MaxMinStringLength")]
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "TipoActividad")]
        public string Descripcion { get; set; }       
        public short TipoTabla { get; set; }

        [Required(ErrorMessageResourceType = typeof(Rp3.AgendaComercial.Resources.ErrorMessageValidation), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Tipo")]
        public string Tipo { get; set; }                
        public short EstadoTabla { get; set; }        
        public string Estado { get; set; }

        public string UsrIng { get; set; }
        public DateTime FecIng { get; set; }
        public string UsrMod { get; set; }
        public DateTime? FecMod { get; set; }
        [ForeignKey("EstadoTabla,Estado")]
        public virtual GeneralValues EstadoGeneralValue { get; set; }
        [ForeignKey("TipoTabla,Tipo")]
        public virtual GeneralValues TipoGeneralValue { get; set; }

        [NotMapped]
        public bool ReadOnly { get; set; }

        public virtual List<TipoActividadOpcion> TipoActividadOpciones { get; set; }
        
        public void AsignarId()
        {
            ContextService service = new ContextService();
            this.IdTipoActividad = service.TipoActividades.GetMaxValue<int>(p => p.IdTipoActividad, 0) + 1;
        }
    }
}
