using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Rp3.AgendaComercial.Models.Ruta;
using Rp3.AgendaComercial.Models.Ruta.View;

namespace Rp3.AgendaComercial.Web.Ruta
{
    public class RutaConsulta
    {
        public List<RutaDetalleGV> RutaDetalleGVs { get; set; }
    }

    public class RutaReassign
    {
        public int IdRuta { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Ruta")]
        public string Ruta { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Agente")]
        public int IdAgente { get; set; }
    }

}