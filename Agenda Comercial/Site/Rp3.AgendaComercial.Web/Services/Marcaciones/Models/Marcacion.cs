using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rp3.AgendaComercial.Web.Services.Marcaciones.Models
{
    public class Marcacion
    {
        public short TipoTabla { get; set; }
        public string Tipo { get; set; }
        public DateTime Fecha { get; set; }
        public long FechaTicks
        {
            get
            {
                return Fecha.Ticks;
            }
            set
            {
                Fecha = new DateTime(value);
            }
        }
        public DateTime? HoraInicioPlan { get; set; }
        public long HoraInicioPlanTicks
        {
            get
            {
                if (HoraInicioPlan.HasValue)
                    return HoraInicioPlan.Value.Ticks;
                return 0;
            }
            set
            {
                if (value == 0)
                    HoraInicioPlan = null;
                else
                    HoraInicioPlan = new DateTime(value);
            }
        }
        public DateTime? HoraFinPlan { get; set; }
        public long HoraFinPlanTicks
        {
            get
            {
                if (HoraFinPlan.HasValue)
                    return HoraFinPlan.Value.Ticks;
                return 0;
            }
            set
            {
                if (value == 0)
                    HoraFinPlan = null;
                else
                    HoraFinPlan = new DateTime(value);
            }
        }
        public DateTime? HoraInicio { get; set; }
        public long HoraInicioTicks
        {
            get
            {
                if (HoraInicio.HasValue)
                    return HoraInicio.Value.Ticks;
                return 0;
            }
            set
            {
                if (value == 0)
                    HoraInicio = null;
                else
                    HoraInicio = new DateTime(value);
            }
        }
        public DateTime? HoraFin { get; set; }
        public long HoraFinTicks
        {
            get
            {
                if (HoraFin.HasValue)
                    return HoraFin.Value.Ticks;
                return 0;
            }
            set
            {
                if (value == 0)
                    HoraFin = null;
                else
                    HoraFin = new DateTime(value);
            }
        }
        public int MinutosAtraso { get; set; }
        public bool EnUbicacion { get; set; }
        public double? LongitudPlan { get; set; }
        public double? LatitudPlan { get; set; }
        public double? Longitud { get; set; }
        public double? Latitud { get; set; }
        public long? IdPermiso { get; set; }
        public Permiso Permiso { get; set; }
    }

    public class Permiso
    {
        public long IdPermiso { get; set; }
        public int? IdAgente { get; set; }
        public short TipoTabla { get; set; }
        public string Tipo { get; set; }
        public string TipoJornada { get; set; }
        public short MotivoTabla { get; set; }
        public string Motivo { get; set; }
        public DateTime FechaInicio { get; set; }
        public long FechaInicioTicks
        {
            get
            {
                return FechaInicio.Ticks;
            }
            set
            {
                FechaInicio = new DateTime(value);
            }
        }
        public DateTime FechaFin { get; set; }
        public long FechaFinTicks
        {
            get
            {
                return FechaFin.Ticks;
            }
            set
            {
                FechaFin = new DateTime(value);
            }
        }
        public DateTime HoraInicio { get; set; }
        public long HoraInicioTicks
        {
            get
            {
                return HoraInicio.Ticks;
            }
            set
            {
                HoraInicio = new DateTime(value);
            }
        }
        public DateTime HoraFin { get; set; }
        public long HoraFinTicks
        {
            get
            {
                return HoraFin.Ticks;
            }
            set
            {
                HoraFin = new DateTime(value);
            }
        }
        public string Observacion { get; set; }
        public bool EsPrevio { get; set; }
        public short EstadoTabla { get; set; }
        public string Estado { get; set; }
    }

    public class PermisoPorAprobar
    {
        public long IdPermiso { get; set; }
        public string ObservacionSupervisor { get; set; }
        public string Estado { get; set; }
    }

    public class Grupo
    {
        public int IdGrupo { get; set; }
        public bool AplicaMarcacion { get; set; }
        public bool AplicaBreak { get; set; }
        public double? LongitudPuntoPartida { get; set; }
        public double? LatitudPuntoPartida { get; set; }
    }
}