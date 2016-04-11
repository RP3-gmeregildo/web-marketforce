using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rp3.Data.Models.General;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Rp3.AgendaComercial.Models.General;

namespace Rp3.AgendaComercial.Models.Ruta
{
    [Table("tbProgramacionRuta", Schema = "rut")]
    public class ProgramacionRuta : Rp3.Data.Entity.EntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdProgramacionRuta { get; set; }

        [Required(ErrorMessageResourceType = typeof(Rp3.AgendaComercial.Resources.ErrorMessageValidation), ErrorMessageResourceName = "Required")]
        [StringLength(250, MinimumLength = 1, ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "MaxMinStringLength")]
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Programacion")]
        public string Descripcion { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Ruta")]
        public int IdRuta { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Cliente")]
        public int IdCliente { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Direccion")]
        public int IdClienteDireccion { get; set; }


        [Required(ErrorMessageResourceType = typeof(Rp3.AgendaComercial.Resources.ErrorMessageValidation), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "FechaInicio")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? FechaInicio { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "FechaFin")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? FechaFin { get; set; }
        public DateTime FechaUltimaEjecucion { get; set; }

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
        public long? FechaFinTicks
        {
            get
            {
                if (FechaFin.HasValue)
                    return FechaFin.Value.Ticks;
                return 0;
            }
            set
            {
                if (value == 0 || value == null)
                    FechaFin = null;
                else
                    FechaFin = new DateTime((long)value);
            }
        }

        #endregion Ticks
        
        public short PatronTabla { get; set; }
        
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Tipo")]
        public string Patron { get; set; }
        public short EstadoTabla { get; set; }
        public string Estado { get; set; }
        public byte DiaMes { get; set; }
        public byte Semana { get; set; }
        public bool Lunes { get; set; }
        public bool Martes { get; set; }
        public bool Miercoles { get; set; }
        public bool Jueves { get; set; }
        public bool Viernes { get; set; }
        public bool Sabado { get; set; }
        public bool Domingo { get; set; }
        public bool Dia { get; set; }
        public bool DiaLaboral { get; set; }
        public bool DiaFinDeSemana { get; set; }
        public byte Frecuencia { get; set; }
        public int? DuracionVisita { get; set; }
        public string UsrIng { get; set; }
        public DateTime? FecIng { get; set; }
        public string UsrMod { get; set; }
        public DateTime? FecMod { get; set; }
        [NotMapped]
        public bool ConFechaFin { get; set; }
        [NotMapped]
        public string TipoMensual { get; set; }
        [NotMapped]
        public string diaString { get; set; }

        [ForeignKey("PatronTabla,Patron")]
        public virtual GeneralValues PatronGeneralValue { get; set; }

        [ForeignKey("IdRuta")]
        public virtual Ruta Ruta { get; set; }

        [ForeignKey("IdRuta")]
        public virtual Cliente Cliente { get; set; }

        [ForeignKey("IdClienteDireccion,IdCliente")]
        public virtual ClienteDireccion ClienteDireccion { get; set; }

        public virtual List<ProgramacionRutaTarea> ProgramacionRutaTareas {get; set;}

        [NotMapped]
        public bool ReadOnly { get; set; }


        public void AsignarId()
        {
            ContextService service = new ContextService();
            this.IdProgramacionRuta = service.ProgramacionRutas.GetMaxValue(p => p.IdProgramacionRuta, 0) + 1;
        }
    }
}
