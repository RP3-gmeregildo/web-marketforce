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
    [Table("tbTareaActividad", Schema = "gen")]
    public class TareaActividad : Rp3.Data.Entity.EntityBase
    {
        public override string Key
        {
            get
            {
                return string.Format("{0}-{1}", this.IdTarea, this.IdTareaActividad);
            }
        }

        [Key]
        [Column(Order = 0)]
        public long IdTarea { get; set; }

        [Key]
        [Column(Order = 1)]
        public int IdTareaActividad { get; set; }

        [Required(ErrorMessageResourceType = typeof(Rp3.AgendaComercial.Resources.ErrorMessageValidation), ErrorMessageResourceName = "Required")]
        [StringLength(250, MinimumLength = 1, ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "MaxMinStringLength")]
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Actividad")]
        public string Descripcion { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Tipo")]
        [Required(ErrorMessageResourceType = typeof(Rp3.AgendaComercial.Resources.ErrorMessageValidation), ErrorMessageResourceName = "Required")]
        public int IdTipoActividad { get; set; }

        public int Orden { get; set; }

        [StringLength(1000, MinimumLength = 0, ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "MaxMinStringLength")]
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Opciones")]
        public string Opciones { get; set; }

        public int? IdTareaActividadPadre { get; set; }

        public int? Valor { get; set; }
        public int? Limite { get; set; }

        [ForeignKey("IdTarea")]
        public virtual Tarea Tarea { get; set; }

        [ForeignKey("IdTipoActividad")]
        public virtual TipoActividad TipoActividad { get; set; }

        public void AsignarOrden()
        {
            ContextService service = new ContextService();

            if(this.IdTareaActividadPadre == null)
                this.Orden = service.TareaActividades.GetMaxValue<int>(p => p.Orden, 0, p => p.IdTarea == this.IdTarea && p.IdTareaActividadPadre == null) + 1;
            else
                this.Orden = service.TareaActividades.GetMaxValue<int>(p => p.Orden, 0, p => p.IdTarea == this.IdTarea && p.IdTareaActividadPadre == this.IdTareaActividadPadre) + 1;
        }
    }
}
