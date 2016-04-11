using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rp3.AgendaComercial.Models.Ruta
{
    public class Notificacion
    {
        public string Titulo { get; set; }
        public string Mensaje { get; set; }
    }

    public class UbicacionAgente
    {
        public List<AgenteUbicacion> AgenteUbicaciones { get; set; }
        public List<UbicacionAgenteDetalle> Agentes { get; set; }
        public bool Radar { get; set; }
        public bool Filter1 { get; set; }
        public bool Filter24 { get; set; }
        public bool Filter48 { get; set; }
        public bool FilterMAS { get; set; }
    }

    public class UbicacionAgenteDetalle
    {
        public int IdAgente { get; set; }
    }
}