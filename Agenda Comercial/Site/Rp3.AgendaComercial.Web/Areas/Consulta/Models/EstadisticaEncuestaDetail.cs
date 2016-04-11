using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Rp3.AgendaComercial.Web.Areas.Consulta.Models
{
    public class EstadisticaEncuestaDetail
    {
      
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Tarea")]
        public int IdTarea { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Actividad")]
        public int? IdTareaActividad { get; set; }

        public string Tipo { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Tarea")]
        public string Descripcion { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Desde")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaVigenciaDesde { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Hasta")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? FechaVigenciaHasta { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Vigente")]
        public bool Vigente { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "NumGestiones")]
        public int NumeroGestiones { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "NumClientes")]
        public int NumeroClientes { get; set; }
    }
}