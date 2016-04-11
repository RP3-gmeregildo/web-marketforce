using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rp3.AgendaComercial.Web.Services.CxC.Models
{
    public class Caja
    {
        public int IdCaja { get; set; }
        public string Nombre { get; set; }
        public int SecuenciaRecibo { get; set; }
        public int MaximoDiasApertura { get; set; }
        public List<FormasPago> FormasPago { get; set; }
        public CajaControl CajaControl { get; set; }
    }
}