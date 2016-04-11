using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rp3.AgendaComercial.Web.Services.General.Models
{
    public class ClienteFoto
    {
        public int IdCliente { get; set; }       
        public int? IdContacto { get; set; }
        public string Nombre { get; set; }
        public string Contenido { get; set; }
    }
}