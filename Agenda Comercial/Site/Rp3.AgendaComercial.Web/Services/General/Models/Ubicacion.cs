using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rp3.AgendaComercial.Web.Services.General.Models
{
    public class Ubicacion
    {
        public DateTime Fecha { get; set; }
        public long FechaTicks { 
            get
            {
                return Fecha.Ticks;
            }
            set
            {
                this.Fecha = new DateTime(value);
            }
        }
        public float Longitud { get; set; }       
        public float Latitud { get; set; }        
    }
}