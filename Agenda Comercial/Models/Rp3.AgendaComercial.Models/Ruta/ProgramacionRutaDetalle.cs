using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rp3.Data.Models.General;
using System.ComponentModel.DataAnnotations;
using Rp3.AgendaComercial.Models.General;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rp3.AgendaComercial.Models.Ruta
{
    [Table("tbProgramacionRutaDetalle", Schema = "rut")]
    public class ProgramacionRutaDetalle : Rp3.Data.Entity.EntityBase
    {
        [Key]
        [Column(Order = 0)]
        public int IdProgramacionRuta { get; set; }

        [Key]
        [Column(Order = 1)]
        public int IdProgramacionRutaDetalle { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Cliente")]                
        public int IdCliente { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Cliente")]                
        public int IdClienteDireccion { get; set; }

        [Required(ErrorMessageResourceType = typeof(Rp3.AgendaComercial.Resources.ErrorMessageValidation), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "FechaInicio")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? FechaInicio { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "FechaFin")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? FechaFin { get; set; }

        #region Ticks

        [NotMapped]
        public long FechaInicioTicks
        {
            get
            {
                if (FechaInicio.HasValue)
                    return FechaInicio.Value.Ticks;
                return 0;
            }
            set
            {
                if (value == 0)
                    FechaInicio = null;
                else
                    FechaInicio = new DateTime(value);
            }
        }

        [NotMapped]
        public long FechaFinTicks
        {
            get
            {
                if (FechaFin.HasValue)
                    return FechaFin.Value.Ticks;
                return 0;
            }
            set
            {
                if (value == 0)
                    FechaFin = null;
                else
                    FechaFin = new DateTime(value);
            }
        }

        #endregion Ticks

        [NotMapped]
        public string IdRecurso
        {
            get
            {
                return String.Format("{0}-{1}", this.IdCliente, this.IdClienteDireccion);
            }

            set
            {
                string[] keyParts = value.Split('-');

                if (keyParts.Count() == 2)
                {
                    IdCliente = Convert.ToInt32(keyParts[0]);
                    IdClienteDireccion = Convert.ToInt32(keyParts[1]);
                }
            }
        }

        [NotMapped]
        public string Ubicacion { get { if (this.ClienteDireccion != null) return this.ClienteDireccion.Cliente.NombresCompletos.Trim(); else return String.Empty; } }

        [NotMapped]
        public string Asunto { get; set; }

        [NotMapped]
        public int IdTipo { get; set; }

        [NotMapped]
        public int IdEstado { get; set; }

        [NotMapped]
        public int IdEtiqueta { get; set; }

        [NotMapped]
        public int MarkerIndex { get; set; }

        [ForeignKey("IdCliente, IdClienteDireccion")]
        public virtual ClienteDireccion ClienteDireccion { get; set; }

        [ForeignKey("IdProgramacionRuta")]
        public virtual ProgramacionRuta ProgramacionRuta { get; set; }

        public void AsignarId()
        {
            ContextService service = new ContextService();
            this.IdProgramacionRutaDetalle = service.ProgramacionRutaDetalles.GetMaxValue<int>(p => p.IdProgramacionRutaDetalle, 0, p => p.IdProgramacionRuta == this.IdProgramacionRuta) + 1;
        }
    }
}
