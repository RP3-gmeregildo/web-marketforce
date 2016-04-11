using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Rp3.Data.Models.General;
using Rp3.AgendaComercial.Models.General;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rp3.AgendaComercial.Web.Ruta
{
    public class GestionDetalladoConsulta
    {
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Ruta")]
        [Required(ErrorMessageResourceType = typeof(Rp3.AgendaComercial.Resources.ErrorMessageValidation), ErrorMessageResourceName = "Required")]
        public int? IdRuta { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Fecha")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? FechaInicioCalendario { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "FechaFin")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? FechaFinCalendario { get; set; }

        public List<GestionDetallado> Gestiones { get; set; }

        #region Ticks

        [NotMapped]
        public long FechaInicioCalendarioTicks
        {
            get
            {
                if (FechaInicioCalendario.HasValue)
                    return FechaInicioCalendario.Value.Ticks;
                return 0;
            }
            set
            {
                if (value == 0)
                    FechaInicioCalendario = null;
                else
                    FechaInicioCalendario = new DateTime(value);
            }
        }

        [NotMapped]
        public long FechaFinCalendarioTicks
        {
            get
            {
                if (FechaFinCalendario.HasValue)
                    return FechaFinCalendario.Value.Ticks;
                return 0;
            }
            set
            {
                if (value == 0)
                    FechaFinCalendario = null;
                else
                    FechaFinCalendario = new DateTime(value);
            }
        }

        #endregion Ticks
    }

    public class GestionDetallado : IUbicacion
    {
        public int IdRuta { get; set; }
        public long IdAgenda { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public DateTime? FechaInicioGestion { get; set; }
        public DateTime? FechaFinGestion { get; set; }
        public int IdCliente { get; set; }
        public int IdClienteDireccion { get; set; }
        public string Cliente { get; set; }
        public string Direccion { get; set; }
        public string Observacion { get; set; }   
        public double? Latitud { get; set; }
        public double? Longitud { get; set; }
        public short EstadoAgendaTabla { get; set; }
        public string EstadoAgenda { get; set; }
        public virtual GeneralValues EstadoAgendaGeneralValue { get; set; }
        public string Tareas { get; set; }

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
                return Cliente;
            }
        }

        #endregion IUbicacion
    }
}