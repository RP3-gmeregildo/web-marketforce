using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rp3.AgendaComercial.Web.Services.Ruta.Models
{
    public class AgendaMedia
    {
        public int IdRuta { get; set; }
        public int IdAgenda { get; set; }
        public short IdMedia { get; set; }     
        public string Nombre { get; set; }
        public string Contenido { get; set; }
    }
}