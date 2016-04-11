using Rp3.AgendaComercial.Models.General;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rp3.AgendaComercial.Models.Ruta
{
    [Table("tbAgenteUltimaUbicacion", Schema = "rut")]
    public class AgenteUltimaUbicacion : Rp3.Data.Entity.EntityBase, IUbicacion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdAgente { get; set; }
        public double? Latitud { get; set; }
        public double? Longitud { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Fecha")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Fecha { get; set; }

        [ForeignKey("IdAgente")]
        public virtual Agente Agente { get; set; }

        #region IUbicacion

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
                return Convert.ToInt32(IdAgente);
            }
            set
            {
                IdAgente = value;
            }
        }

        [NotMapped]
        public string Titulo
        {
            get
            {
                if (Agente != null)
                    return String.Format("{0} - {1:G}", Agente.Descripcion, Fecha);

                return String.Format("{0:G}", Fecha);
            }
        }



        #endregion IUbicacion
    }
}
