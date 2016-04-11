using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rp3.AgendaComercial.Web.Ruta
{
    public class GestionResumidoConsulta
    {
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Ruta")]
        public int? IdRuta { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "FechaInicio")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? FechaInicioCalendario { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "FechaFin")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? FechaFinCalendario { get; set; }

        public List<ResumenEstado> ResumenEstado { get; set; }
        public List<ResumenEstado> ResumenTareaEstado { get; set; }

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

    public class ResumenEstado
    {
        public string EstadoAgenda { get; set; }
        public string EstadoAgendaContent { get; set; }
        public int Cantidad { get; set; }
    }
}