using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rp3.Data.Models.General;
using Rp3.AgendaComercial.Models.General;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rp3.AgendaComercial.Models.Ruta
{
    [Table("tbRutaDetalle", Schema = "rut")]
    public class RutaDetalle : Rp3.Data.Entity.EntityBase, IUbicacion
    {
        public override string Key
        {
            get
            {
                return string.Format("{0}-{1}-{2}", this.IdRuta, this.IdCliente, this.IdClienteDireccion);
            }
        }

        [Key]
        [Column(Order = 0)]
        public int IdRuta { get; set; }

        [Key]
        [Column(Order = 1)]
        public int IdCliente { get; set; }

        [Key]
        [Column(Order = 2)]
        public int IdClienteDireccion { get; set; }

        [ForeignKey("IdRuta")]
        public virtual Ruta Ruta { get; set; }

        [ForeignKey("IdCliente, IdClienteDireccion")]
        public virtual ClienteDireccion ClienteDireccion { get; set; }

        #region IUbicacion

        [NotMapped]
        public double? Latitud { get; set; }

        [NotMapped]
        public double? Longitud { get; set; }

        [NotMapped]
        public int MarkerIndex { get; set; }
        [NotMapped]
        public int MarkerZIndex { get { return 1000 - MarkerIndex; } }
        [NotMapped]
        public int MarkerStart { get { if (MarkerIndex > 0) return 30 * (MarkerIndex - 1); else return 0; } }

        [NotMapped]
        public int IdUbicacion
        {
            get
            {
                return Convert.ToInt32(IdCliente);
            }
            set
            {
                IdCliente = value;
            }
        }

        [NotMapped]
        public string Titulo
        {
            get
            {
                if (ClienteDireccion != null && ClienteDireccion.Cliente != null)
                    return ClienteDireccion.Cliente.NombresCompletos;

                return String.Empty;
            }
        }

        #endregion IUbicacion       
    }
}
