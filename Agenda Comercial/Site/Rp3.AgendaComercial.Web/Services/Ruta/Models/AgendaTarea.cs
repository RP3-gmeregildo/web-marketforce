using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rp3.AgendaComercial.Web.Services.Ruta.Models
{
    public class AgendaTarea
    {       
        public int IdRuta { get; set; }
        public long IdAgenda { get; set; }
        public long IdTarea { get; set; }
        public string Nombre { get; set; }
        public string EstadoTarea { get; set; }
        public string TipoTarea { get; set; }

        //public List<AgendaTareaActividad> AgendaTareaActividades { get; set; }
    }

    public class AgendaTareaUpdate
    {
        public long IdTarea { get; set; }
        public string EstadoTarea { get; set; }
        public List<AgendaTareaActividadUpdate> AgendaTareaActividades { get; set; }
    }
}