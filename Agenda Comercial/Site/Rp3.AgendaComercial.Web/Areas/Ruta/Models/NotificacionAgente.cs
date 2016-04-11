using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Rp3.AgendaComercial.Models.Ruta
{
    public class NotificacionAgente
    {
        public List<UbicacionAgenteDetalle> Agentes { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Titulo")]
        public string Titulo { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Mensaje")]
        public string Mensaje { get; set; }
    }
}