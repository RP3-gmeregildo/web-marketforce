using Rp3.AgendaComercial.Models.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rp3.AgendaComercial.Web.Ruta
{   
    public class RecorridoAgenteResult
    {
        public DateTime Fecha { get; set; }
        public bool IncluirGestion { get; set; }
        public bool IncluirRecorrido { get; set; }
        public bool IncluirClientes { get; set; }
        public List<ClienteAgendaUbicacion> Clientes { get; set;}
        public List<Ubicacion> Recorrido { get; set; }
        public double RadioDistancia { get; set; }

        public int? IdAgente { get; set; }
    }

    public class ClienteAgendaUbicacion: Ubicacion
    {
        public long IdAgenda { get; set; }
        public int IdRuta { get; set; }
        public string NombresCompletos { get; set; }
        public string Direccion { get; set; }        
        public DateTime Fecha { get; set; }
        public DateTime? FechaInicioGestion { get; set; }
        public DateTime? FechaFinGestion { get; set; }
        public string Estado { get; set; }
        public string ColorEstado { get; set; }
        public string FotoPath { get; set; }
        public double? DistanciaGestion { get; set; }
        public double? LatitudGestion { get; set; }
        public double? LongitudGestion { get; set; }
        public bool Advertencia { get; set; }
        public bool Realizada { get; set; }

    }
       
}