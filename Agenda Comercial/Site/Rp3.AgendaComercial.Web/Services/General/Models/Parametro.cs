using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rp3.AgendaComercial.Web.Services.General.Models
{
    public class Parametro
    {
        public long HoraFinTrackingPositionTicks { get; set; }

        public long HoraInicioTrackingPositionTicks { get; set; }

        public int MinutosIntervaloTrackingPosition { get; set; }
        public string DefaultInternationalPhoneNumberCode { get; set; }

        public double MarcacionDistance { get; set; }
        public int AgenteUbicacion1 { get; set; }
        public int AgenteUbicacion2 { get; set; }
        public int AgenteUbicacion3 { get; set; }
    }
}