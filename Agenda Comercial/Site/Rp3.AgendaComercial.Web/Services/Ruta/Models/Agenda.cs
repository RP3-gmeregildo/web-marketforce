using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rp3.AgendaComercial.Web.Services.Ruta.Models
{
    public class Agenda
    {
        public int IdRuta { get; set; }
        public long IdAgenda { get; set; }
        public int IdCliente { get; set; }
        public int IdClienteDireccion { get; set; }
        public int? IdClienteContacto { get; set; }
        public long? IdProgramacionRuta { get; set; }

        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public DateTime? FechaInicioGestion { get; set; }
        public DateTime? FechaFinGestion { get; set; }
        public DateTime? FechaInicioOriginal { get; set; }
        public DateTime? FechaFinOriginal { get; set; }

        public int? DistanciaUbicacion { get; set; }

        public string EstadoAgenda { get; set; }

        public string NombresCompletos { get; set; }

        public string ContactoNombresCompletos { get; set; }

        public string Direccion { get; set; }

        public string Observacion { get; set; }

        public string Ciudad { get; set; }

        public double Longitud { get; set; }

        public double Latitud { get; set; }

        public string MotivoNoGestion { get; set; }

        public int Duracion { get; set; }
        public int TiempoViaje { get; set; }
        public string MotivoReprogramacion { get; set; }
        public short? MotivoReprogramacionTabla { get; set; }

        #region Ticks

        public long FechaInicioTicks
        {
            get
            {
                if (FechaInicio.HasValue)
                    return FechaInicio.Value.Ticks;
                return 0;
            }
            set
            {
                if (value == 0)
                    FechaInicio = null;
                else
                    FechaInicio = new DateTime(value);
            }
        }
        public long FechaFinTicks
        {
            get
            {
                if (FechaFin.HasValue)
                    return FechaFin.Value.Ticks;
                return 0;
            }
            set
            {
                if (value == 0)
                    FechaFin = null;
                else
                    FechaFin = new DateTime(value);
            }
        }

        public long FechaInicioOriginalTicks
        {
            get
            {
                if (FechaInicioOriginal.HasValue)
                    return FechaInicioOriginal.Value.Ticks;
                return 0;
            }
            set
            {
                if (value == 0)
                    FechaInicioOriginal = null;
                else
                    FechaInicioOriginal = new DateTime(value);
            }
        }
        public long FechaFinOriginalTicks
        {
            get
            {
                if (FechaFinOriginal.HasValue)
                    return FechaFinOriginal.Value.Ticks;
                return 0;
            }
            set
            {
                if (value == 0)
                    FechaFinOriginal = null;
                else
                    FechaFinOriginal = new DateTime(value);
            }
        }

        public long FechaInicioGestionTicks
        {
            get
            {
                if (FechaInicioGestion.HasValue)
                    return FechaInicioGestion.Value.Ticks;
                return 0;
            }
            set
            {
                if (value == 0)
                    FechaInicioGestion = null;
                else
                    FechaInicioGestion = new DateTime(value);
            }
        }
        public long FechaFinGestionTicks
        {
            get
            {
                if (FechaFinGestion.HasValue)
                    return FechaFinGestion.Value.Ticks;
                return 0;
            }
            set
            {
                if (value == 0)
                    FechaFinGestion = null;
                else
                    FechaFinGestion = new DateTime(value);
            }
        }

        #endregion Ticks

        public List<AgendaTarea> AgendaTareas { get; set; }
    }

    public class AgendaNoGestion
    {
        public int IdRuta { get; set; }
        public long IdAgenda { get; set; }
        public string MotivoNoGestion { get; set; }
        public string Observacion { get; set; }        
    }

    public class AgendaReprogramar
    {
        public int IdRuta { get; set; }
        public long IdAgenda { get; set; }
        public int? IdClienteContacto { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string EstadoAgenda { get; set; }
        public string Observacion { get; set; }
        public int Duracion { get; set; }
        public int TiempoViaje { get; set; }
        public string MotivoReprogramacion { get; set; }
        public short? MotivoReprogramacionTabla { get; set; }

        #region Ticks

        public long FechaInicioTicks
        {
            get
            {
                if (FechaInicio.HasValue)
                    return FechaInicio.Value.Ticks;
                return 0;
            }
            set
            {
                if (value == 0)
                    FechaInicio = null;
                else
                    FechaInicio = new DateTime(value);
            }
        }
        public long FechaFinTicks
        {
            get
            {
                if (FechaFin.HasValue)
                    return FechaFin.Value.Ticks;
                return 0;
            }
            set
            {
                if (value == 0)
                    FechaFin = null;
                else
                    FechaFin = new DateTime(value);
            }
        }

        #endregion Ticks
    }

    public class AgendaUpdate
    {
        public int IdRuta { get; set; }
        public long IdAgenda { get; set; }
        public DateTime? FechaInicioGestion { get; set; }
        public DateTime? FechaFinGestion { get; set; }
        public string EstadoAgenda { get; set; }
        public string Observacion { get; set; }
        public double? Longitud { get; set; }
        public double? Latitud { get; set; }
        public int IdCliente { get; set; }
        public int IdClienteDireccion { get; set; }
        public int? IdClienteContacto { get; set; }
        public string MotivoNoGestion { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public long IdInterno { get; set; }
        public int Duracion { get; set; }
        public int TiempoViaje { get; set; }
        public int DistanciaUbicacion { get; set; }
        public string MotivoReprogramacion { get; set; }
        public short? MotivoReprogramacionTabla { get; set; }
        public DateTime? FechaInicioOriginal { get; set; }
        public DateTime? FechaFinOriginal { get; set; }

        #region Ticks

        public long FechaInicioTicks
        {
            get
            {
                if (FechaInicio.HasValue)
                    return FechaInicio.Value.Ticks;
                return 0;
            }
            set
            {
                if (value == 0)
                    FechaInicio = null;
                else
                    FechaInicio = new DateTime(value);
            }
        }
        public long FechaFinTicks
        {
            get
            {
                if (FechaFin.HasValue)
                    return FechaFin.Value.Ticks;
                return 0;
            }
            set
            {
                if (value == 0)
                    FechaFin = null;
                else
                    FechaFin = new DateTime(value);
            }
        }

        public long FechaInicioGestionTicks
        {
            get
            {
                if (FechaInicioGestion.HasValue)
                    return FechaInicioGestion.Value.Ticks;
                return 0;
            }
            set
            {
                if (value == 0)
                    FechaInicioGestion = null;
                else
                    FechaInicioGestion = new DateTime(value);
            }
        }
        public long FechaFinGestionTicks
        {
            get
            {
                if (FechaFinGestion.HasValue)
                    return FechaFinGestion.Value.Ticks;
                return 0;
            }
            set
            {
                if (value == 0)
                    FechaFinGestion = null;
                else
                    FechaFinGestion = new DateTime(value);
            }
        }
        public long FechaInicioOriginalTicks
        {
            get
            {
                if (FechaInicioOriginal.HasValue)
                    return FechaInicioOriginal.Value.Ticks;
                return 0;
            }
            set
            {
                if (value == 0)
                    FechaInicioOriginal = null;
                else
                    FechaInicioOriginal = new DateTime(value);
            }
        }
        public long FechaFinOriginalTicks
        {
            get
            {
                if (FechaFinOriginal.HasValue)
                    return FechaFinOriginal.Value.Ticks;
                return 0;
            }
            set
            {
                if (value == 0)
                    FechaFinOriginal = null;
                else
                    FechaFinOriginal = new DateTime(value);
            }
        }

        #endregion Ticks

        public List<AgendaTareaUpdate> AgendaTareas { get; set; }
    }

    public class AgendaUpdateResponse
    {
        public List<AgendaResult> Result { get; set; }
    }

    public class AgendaResult
    {        
        public long IdInterno { get; set; }
        public int IdRutaServer {get; set;}
        public long IdAgendaServer { get; set;}
        public bool Ok { get; set; }
        public string Error { get; set; }
    }
}