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
    [Table("tbZonaClienteGeocerca", Schema = "gen")]
    public class ZonaClienteGeocerca : Rp3.Data.Entity.EntityBase
    {
        public override string Key
        {
            get
            {
                return string.Format("{0}-{1}-{2}", this.IdZona, this.IdCliente, this.IdClienteDireccion);
            }
        }

        [Key]
        [Column(Order = 0)]
        public int IdZona { get; set; }

        [Key]
        [Column(Order = 1)]
        public int IdCliente { get; set; }

        [Key]
        [Column(Order = 2)]
        public int IdClienteDireccion { get; set; }

        public int IdZonaGrupoGeocerca { get; set; }

        [ForeignKey("IdZona")]
        public virtual Zona Zona { get; set; }

        [ForeignKey("IdCliente, IdClienteDireccion")]
        public virtual ClienteDireccion ClienteDireccion { get; set; }
    }
}
