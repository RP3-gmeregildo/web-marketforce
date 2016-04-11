using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rp3.AgendaComercial.Web.Services.Ruta.Models
{
    public class AgendaTareaOpcion
    {
        public int IdRuta { get; set; }
        public long IdAgenda { get; set; }
        public long IdTarea { get; set; }
        public int IdTareaActividad { get; set; }
        public int IdTareaOpcion { get; set; }
        public string Descripcion { get; set; }
        public int Orden { get; set; }
    }
}