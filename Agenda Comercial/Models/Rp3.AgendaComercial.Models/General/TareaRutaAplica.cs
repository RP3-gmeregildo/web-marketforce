using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rp3.Data.Models.General;
using Rp3.AgendaComercial.Models.Ruta;
using Rp3.Web.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rp3.AgendaComercial.Models.General
{
    [Table("tbTareaRutaAplica", Schema = "gen")]
    public class TareaRutaAplica : Rp3.Data.Entity.EntityBase
    {
        public override string Key
        {
            get
            {
                return string.Format("{0}-{1}", this.IdTarea, this.IdRuta);
            }
        }

        [Key]
        [Column(Order = 0)]
        public long IdTarea { get; set; }

        [Key]
        [Column(Order = 1)]
        public int IdRuta { get; set; }

        [ForeignKey("IdTarea")]
        public virtual Tarea Tarea { get; set; }

        [ForeignKey("IdRuta")]
        public virtual Ruta.Ruta Ruta { get; set; }

    }
}
