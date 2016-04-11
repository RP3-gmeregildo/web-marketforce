using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rp3.AgendaComercial.Models.General
{
    [Table("tbTipoActividadOpcion", Schema = "gen")]
    public class TipoActividadOpcion : Rp3.Data.Entity.EntityBase
    {
        public override string Key
        {
            get
            {
                return string.Format("{0}-{1}", this.IdTipoActividad, this.IdTipoActividadOpcion);
            }
        }

        [Key]
        [Column(Order = 0)]
        
        public int IdTipoActividad { get; set; }

        [Key]
        [Column(Order = 1)]        
        public int IdTipoActividadOpcion { get; set; }

        [ForeignKey("IdTipoActividad")]
        public virtual TipoActividad TipoActividad { get; set; }

        [Required(ErrorMessageResourceType = typeof(Rp3.AgendaComercial.Resources.ErrorMessageValidation), ErrorMessageResourceName = "Required")]
        [StringLength(250, MinimumLength = 1, ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "MaxMinStringLength")]
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Actividad")]
        public string Descripcion { get; set; }

        public int Orden { get; set; }

        public void AsignarId()
        {
            ContextService service = new ContextService();
            this.IdTipoActividadOpcion = service.TipoActividadOpciones.GetMaxValue<int>(p => p.IdTipoActividadOpcion, 0, p => p.IdTipoActividad == this.IdTipoActividad) + 1;
        }
    }
}
