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
     [Table("tbZonaDetalle", Schema = "gen")]
    public class ZonaDetalle : Rp3.Data.Entity.EntityBase
    {
         public override string Key
         {
             get
             {
                 return string.Format("{0}-{1}", this.IdZona, this.IdGeopoliticalStructure);
             }
         }

        [Key]
        [Column(Order = 0)]
        public int IdZona { get; set; }

        [Key]
        [Column(Order = 1)]
        public int IdGeopoliticalStructure { get; set; }

        [ForeignKey("IdZona")]
        public virtual Zona Zona { get; set; }

        [ForeignKey("IdGeopoliticalStructure")]
        public virtual GeopoliticalStructure GeopoliticalStructure { get; set; }
    }
}
