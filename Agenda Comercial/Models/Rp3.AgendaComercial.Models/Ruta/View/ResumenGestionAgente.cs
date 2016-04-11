using Rp3.Web.Mvc.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rp3.AgendaComercial.Models.Ruta.View
{
    public class ResumenGestionAgente
    {
        public int IdAgente { get; set; }
        public string Nombres { get; set; }
        public DateTime Fecha { get; set; }
        public string Apellidos {get; set;}
        public int Gestionados { get; set; }
        public int NoGestionados { get; set; }
        public int Proximos { get; set; }
        public long FechaTicks
        {
            get
            {
                return this.Fecha.Ticks;
            }
        }
        public int Total { get; set; }
    }
}
