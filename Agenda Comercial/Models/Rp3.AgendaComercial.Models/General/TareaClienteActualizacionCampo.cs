using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rp3.AgendaComercial.Models.General
{
    [Table("tbTareaClienteActualizacionCampo", Schema = "gen")]
    public class TareaClienteActualizacionCampo : Rp3.Data.Entity.EntityBase
    {
        public override string Key
        {
            get
            {
                return string.Format("{0}-{1}", this.IdTarea, this.IdCampo);
            }
        }

        public string KeyCombo
        {
            get
            {
                if (this.IdTarea == 0) return " ";
                return this.IdTarea.ToString();
            }
        }
        [Key]
        [Column(Order = 0)]
        public long IdTarea { get; set; }

        [Key]
        [Column(Order = 1)]
        public string IdCampo { get; set; }

        public bool Creacion { get; set; }

        public bool Modificacion { get; set; }

        public bool Gestion { get; set; }

        [ForeignKey("IdCampo")]
        public virtual ParametroClienteCampo Parametro { get; set; }

        [ForeignKey("IdTarea")]
        public virtual Tarea Tarea { get; set; }
    }
}
