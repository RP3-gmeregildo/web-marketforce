using Rp3.AgendaComercial.Models.Consulta.View;
using Rp3.AgendaComercial.Models.General;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Rp3.AgendaComercial.Web.Areas.Consulta.Models
{
    public class AgenteReporteGestion
    {
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "MostrarFotos")]
        public bool MostrarFotos { get; set; }
        public List<Agente> Agentes { get; set; }
        public AgenteReporteGestionModo Modo { get; set; }
    }
    public class Geocerca
    {
        public bool IncluirClientes { get; set; }

        public List<Zona> GeoZonas { get; set; }
        public List<GeocercaZonaDetalle> Zonas { get; set; }
    }

    public class GeocercaZonaDetalle
    {
        public int IdZona { get; set; }
    }
}