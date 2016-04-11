using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Rp3.AgendaComercial.Web.Areas.Consulta.Models
{
    public class DashboardAgenteDetalleParametro
    {
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "FechaInicio")]
        public DateTime? FechaInicio { get; set; }
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "FechaFin")]
        public DateTime? FechaFin { get; set; }
    }
}