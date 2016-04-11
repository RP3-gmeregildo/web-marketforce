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
    public class LoteConsulta
    {
        public List<LoteParam> LoteParams { get; set; }
        public List<LoteView> LoteViews { get; set; }
    }
}