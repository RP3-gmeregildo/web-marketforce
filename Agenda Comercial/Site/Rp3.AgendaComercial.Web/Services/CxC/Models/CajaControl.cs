using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rp3.AgendaComercial.Web.Services.CxC.Models
{
    public class CajaControl
    {
        public int IdCaja { get; set; }
        public int IdControlCaja { get; set; }
        public DateTime FechaApertura { get; set; }
        public DateTime FechaCierre { get; set; }
        public decimal MontoApertura { get; set; }
        public decimal MontoCierre { get; set; }
        public bool Activo { get; set; }
    }
}