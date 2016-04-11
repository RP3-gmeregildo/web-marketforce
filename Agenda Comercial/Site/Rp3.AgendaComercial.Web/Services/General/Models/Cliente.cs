using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rp3.AgendaComercial.Web.Services.General.Models
{
    public class Cliente
    {
        public int IdInterno { get; set; }
        public int IdCliente { get; set; }
        public short? IdTipoIdentificacion { get; set; }
        public string Identificacion { get; set; }
        public string NombresCompletos
        {
            get
            {
                return string.Format("{0}{1}{2}{3}", this.Apellido1, " " + this.Apellido2, " " + this.Nombre1, " " + this.Nombre2);
            }
        }
        public string Nombre1 { get; set; }
        public string Nombre2 { get; set; }
        public string Apellido1 { get; set; }
        public string Apellido2 { get; set; }
        public string CorreoElectronico { get; set; }

        public short? TipoPersonaTabla { get; set; }
        public string TipoPersona { get; set; }

        public short? GeneroTabla { get; set; }
        public string Genero { get; set; }
        public short? EstadoCivilTabla { get; set; }
        public string Estado { get; set; }
        public short? EstadoTabla { get; set; }
        public string EstadoCivil { get; set; }
        public DateTime? FechaNacimiento { get; set; }

        public string Foto { get; set; }

        public string RazonSocial { get; set; }

        public string ActividadEconomica { get; set; }

        public string PaginaWeb { get; set; }

        public long FechaNacimientoTicks
        {
            get
            {
                if (FechaNacimiento.HasValue)
                    return FechaNacimiento.Value.Ticks;
                return 0;
            }
            set
            {
                if (value == 0)
                    FechaNacimiento = null;
                else
                    FechaNacimiento = new DateTime(value);
            }
        }
        public string AgenteUltimaVisita { get; set; }
        public int? IdTipoCliente { get; set; }
        public int? IdCanal { get; set; }
        public int Calificacion { get; set; }
        public DateTime? FechaUtimaVisita { get; set; }
        public long FechaUtimaVisitaTicks
        {
            get
            {
                if (FechaUtimaVisita.HasValue)
                    return FechaUtimaVisita.Value.Ticks;
                return 0;
            }
            set
            {
                if (value == 0)
                    FechaUtimaVisita = null;
                else
                    FechaUtimaVisita = new DateTime(value);
            }
        }

        public int? IdAgenteUltimaVisita { get; set; }

        public int? IndiceActividad { get; set; }

        public int? TiempoInactividad { get; set; }
        public List<ClienteDireccion> ClienteDirecciones { get; set; }

        public List<ClienteContacto> ClienteContactos { get; set; }
    }

    public class ClienteCreateResponse
    {
        public List<ClienteCodigoResult> Codigos { get; set; }
    }

    public class ClienteCodigoResult :IntCodigoResult
    {
        public ClienteCodigoResult()
        {
            Contactos = new List<IntCodigoResult>();
            Direcciones = new List<IntCodigoResult>();
        }
        public List<IntCodigoResult> Contactos { get; set; }
        public List<IntCodigoResult> Direcciones { get; set; }
    }
    
    public class IntCodigoResult
    {
        public int IdInterno { get; set; }
        public int IdServer { get; set; }
    }   

}