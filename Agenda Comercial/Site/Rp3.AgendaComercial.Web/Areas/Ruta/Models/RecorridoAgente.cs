using Rp3.AgendaComercial.Models.General;
using Rp3.AgendaComercial.Web.Ruta;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Rp3.AgendaComercial.Models.Ruta
{
    public class RecorridoAgente
    {
        public List<Agente> Agentes { get; set; }               

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "IncluirClientes")]
        public bool IncluirClientes { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "IncluirRecorrido")]
        public bool IncluirRecorrido { get; set; }
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "IncluirGestion")]
        public bool IncluirGestion { get; set; }

        #region Ticks

        public DateTime? FechaInicio { get; set; }

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
      

        #endregion Ticks
        
        public List<Ubicacion> Ubicaciones { get; set; }

        public RecorridoAgenteResult RecorridoResult { get; set; }

        public string FechaInicioByAgente { get; set; }
        public string FechaFinByAgente { get; set; }

        public int? IdByAgente { get; set; }
    }
}