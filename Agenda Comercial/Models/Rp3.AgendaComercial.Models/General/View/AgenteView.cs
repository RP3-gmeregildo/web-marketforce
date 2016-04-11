using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Rp3.AgendaComercial.Models.General.View
{
    public class AgenteView
    {
        public int IdAgente { get; set; }
        public int? IdSupervisor { get; set; }
        public int? IdRuta { get; set; }
        public int? IdGrupo { get; set; }
        public string Descripcion { get; set; }
        public bool EsSupervisor { get; set; }
        public bool EsAgente { get; set; }
        public bool EsAdministrador { get; set; }
        public int IdCargo { get; set; }
        public string Cargo { get; set; }
        public AgenteCargoRol? CargoRol { get; set; }

        public string Foto { get; set; }

        public string GCMId { get; set; }
    }

    public class AgenteUbicacionView
    {
        public int IdAgente { get; set; }
        public DateTime UltimaActualizacion { get; set; }
        public long UltimaActualizacionTicks
        {
            get
            {
                return UltimaActualizacion.Ticks;
            }
            set
            {
                UltimaActualizacion = new DateTime(value);
            }
        }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public double Latitud { get; set; }
        public double Longitud { get; set; }
    }

    public enum AgenteCargoRol
    {
        [Description("Gerente")]
        Gerente = 1,
        [Description("Supervisor")]
        Supervisor = 2,
        [Description("Agente")]
        Agente = 3
    }

    /*public class enumclass
    {
        public AgenteCargoRol AgenteCargoRols { get; set; }
    }*/
}