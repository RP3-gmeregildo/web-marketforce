using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rp3.AgendaComercial.Web.Services.General.Models
{
    public class Agente
    {
        public int IdAgente { get; set; }
        public string Nombre { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
    }

    public class GCMId
    {
        public string AuthId { get; set; }
    }

    public class NotificationToUser
    {
        public int IdAgente { get; set; }
        public string Titulo { get; set; }
        public string Mensaje { get; set; }
    }

    public class Log
    {
        public string Logs { get; set; }
    }
}