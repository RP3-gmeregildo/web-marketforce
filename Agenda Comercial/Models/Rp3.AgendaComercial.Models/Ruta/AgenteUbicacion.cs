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
    [Table("tbAgenteUbicacion", Schema = "rut")]
    public class AgenteUbicacion : Rp3.Data.Entity.EntityBase, IUbicacion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long IdAgenteUbicacion { get; set; }
        public int IdAgente { get; set; }
        public double? Latitud { get; set; }
        public double? Longitud { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Fecha")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Fecha { get; set; }

        [ForeignKey("IdAgente")]
        public virtual Agente Agente { get; set; }




        [NotMapped]
        public int Reportes { get; set; }

        [NotMapped]
        public DateTime? FechaHasta { get; set; }

        [NotMapped]
        public double Tiempo { get; set; }

        [NotMapped]
        public double Distancia { get; set; }

        [NotMapped]
        public string UltimaConexion { get; set; }

        [NotMapped]
        public long HorasUltimaConexion { get; set; }

        [NotMapped]
        public bool EsMovimiento { get; set; }

        [NotMapped]
        public string Ubicacion { get; set; }


        
        public void AsignarId()
        {
            ContextService service = new ContextService();
            this.IdAgenteUbicacion = service.AgenteUbicaciones.GetMaxValue<long>(p => p.IdAgenteUbicacion, 0) + 1;
        }

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
                return Convert.ToInt32(IdAgenteUbicacion);
            }
            set
            {
                IdAgenteUbicacion = value;
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
