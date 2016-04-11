using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rp3.AgendaComercial.Models.General
{
    [Table("tbFrecuencia", Schema = "gen")]
    public class Frecuencia : Rp3.Data.Entity.EntityBase
    {
        public override string Key
        {
            get
            {
                if (this.IdFrecuencia == 0) return " ";
                return this.IdFrecuencia.ToString();
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
        public int IdFrecuencia { get; set; }

        [Required(ErrorMessageResourceType = typeof(Rp3.AgendaComercial.Resources.ErrorMessageValidation), ErrorMessageResourceName = "Required")]
        [StringLength(250, MinimumLength = 1, ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "MaxMinStringLength")]
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Frecuencia")]
        public string Descripcion { get; set; }

        public string Modo { get; set; }

        public int Valor { get; set; }

        public void AsignarId()
        {
            ContextService service = new ContextService();
            this.IdFrecuencia = service.Frecuencias.GetMaxValue<int>(p => p.IdFrecuencia, 0) + 1;
        }
    }
}
