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
    [Table("tbTarea", Schema = "gen")]
    public class Tarea : Rp3.Data.Entity.EntityBase
    {
        public override string Key
        {
            get
            {
                if (this.IdTarea == 0) return " ";
                return this.IdTarea.ToString();
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
        public long IdTarea { get; set; }

        [Required(ErrorMessageResourceType = typeof(Rp3.AgendaComercial.Resources.ErrorMessageValidation), ErrorMessageResourceName = "Required")]
        [StringLength(250, MinimumLength = 1, ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "MaxMinStringLength")]
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Tarea")]
        public string Descripcion { get; set; }       
        public short TipoTareaTabla { get; set; }       
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Tipo")]
        public string TipoTarea { get; set; }     
        public short EstadoTabla { get; set; }       
        public string Estado { get; set; }
        public string UsrIng { get; set; }
        public DateTime FecIng { get; set; }
        public string UsrMod { get; set; }
        public DateTime? FecMod { get; set; }

        [ForeignKey("IdTarea")]
        public virtual TareaClienteActualizacion TareaClienteActualizacion { get; set; }

        [ForeignKey("EstadoTabla,Estado")]
        public virtual GeneralValues EstadoGeneralValue { get; set; }
        [ForeignKey("TipoTareaTabla,TipoTarea")]
        public virtual GeneralValues TipoTareaGeneralValue { get; set; }
        public virtual List<TareaActividad> TareaActividades { get; set; }
        public virtual List<TareaRutaAplica> TareaRutaAplicas { get; set; }
        public virtual List<TareaClienteActualizacionCampo> TareaClienteActualizacionCampos { get; set; }

        [NotMapped]
        public bool ReadOnly { get; set; }
        public void AsignarId()
        {
            ContextService service = new ContextService();
            this.IdTarea = service.Tareas.GetMaxValue<long>(p => p.IdTarea, 100) + 1;
        }
        public DateTime? FechaVigenciaDesde { get; set; }
        public DateTime? FechaVigenciaHasta { get; set; }
        public bool EsVigenciaIndefinida { get; set; }
        public bool AplicaRutasEspecificas { get; set; }

        public bool Vigente {
            get {

                if(this.FechaVigenciaDesde < DateTime.Now && (this.EsVigenciaIndefinida || this.FechaVigenciaHasta > DateTime.Now))
                    return true;

                return false;
            }
        }
    }
}
