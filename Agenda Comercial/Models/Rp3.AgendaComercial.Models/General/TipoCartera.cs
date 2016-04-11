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
    [Table("tbTipoCartera", Schema = "gen")]
    public class TipoCartera : Rp3.Data.Entity.EntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdTipoCartera { get; set; }

        [Required(ErrorMessageResourceType = typeof(Rp3.AgendaComercial.Resources.ErrorMessageValidation), ErrorMessageResourceName = "Required")]
        [StringLength(250, MinimumLength = 1, ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "MaxMinStringLength")]
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "TipoCartera")]
        public string Descripcion { get; set; }        
        public short EstadoTabla { get; set; }       
        public string Estado { get; set; }
        public string UsrIng { get; set; }
        public DateTime FecIng { get; set; }
        public string UsrMod { get; set; }
        public DateTime? FecMod { get; set; }
        [ForeignKey("EstadoTabla,Estado")]
        public virtual GeneralValues EstadoGeneralValue { get; set; }

        public void AsignarId()
        {
            ContextService service = new ContextService();
            this.IdTipoCartera = service.TipoCarteras.GetMaxValue<int>(p => p.IdTipoCartera, 0) + 1;
        }
    }
}
