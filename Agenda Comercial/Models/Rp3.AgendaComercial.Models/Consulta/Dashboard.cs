using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Rp3.AgendaComercial.Models.Consulta.View
{
    public enum AgenteReporteGestionModo
    {
        Resumido,
        Detallado
    }

    public enum AgenteReporteGestionTipo
    {
        NoVisitado,
        Visitado,
        Reprogramado,
        Pendiente
    }
    public class AgenteReporteGestionItem<T>
    {
        public string Description { get; set; }
        public T Value { get; set; }
    }

    public class AgenteReporteGestionResumen
    {
        public DateTime Fecha { get; set; }
        public int Visitados { get; set; }
        public int NoVisitados { get; set; }
        public int Reprogramados { get; set; }
        public int Pendientes { get; set; }
        public double Parada { get; set; }
        public double Recorrido { get; set; }

        public DateTime ParadaDisplay { get { return new DateTime().AddMinutes(this.Parada); } }

        public DateTime RecorridoDisplay { get { return new DateTime().AddMinutes(this.Recorrido); } }
    }

    public class AgenteReporteGestionGrupo
    {
        public AgenteReporteGestionGrupo(AgenteReporteGestion parent, DateTime fecha, AgenteReporteGestionTipo tipo)
        {
            this.Parent = parent;
            this.Fecha = fecha;
            this.Tipo = tipo;
        }
        private AgenteReporteGestionTipo Tipo { get; set; }
        public DateTime Fecha { get; set; }
        public AgenteReporteGestion Parent { get; private set; }
        public List<Ruta.View.AgendaListado> Clientes 
        { 
            get 
            {
                switch (this.Tipo)
                {
                    case AgenteReporteGestionTipo.NoVisitado:
                        return this.Parent.ClientesNoVisitados.Where(p => p.fechaInicio.Date == this.Fecha.Date).OrderBy(p => p.fechaInicio).ToList();
                    case AgenteReporteGestionTipo.Visitado:
                        return this.Parent.ClientesVisitados.Where(p => p.fechaInicio.Date == this.Fecha.Date).OrderBy(p => p.fechaInicio).ToList();
                    case AgenteReporteGestionTipo.Pendiente:
                        return this.Parent.ClientesPendientes.Where(p => p.fechaInicio.Date == this.Fecha.Date).OrderBy(p => p.fechaInicio).ToList();
                    default:
                        return this.Parent.ClientesReprogramados.Where(p => p.fechaInicio.Date == this.Fecha.Date).OrderBy(p => p.fechaInicio).ToList();
                }
            } 
        }
    }

    public class AgenteReporteGestionClienteGreado
    {
        public AgenteReporteGestionClienteGreado(AgenteReporteGestion parent, DateTime fecha)
        {
            this.Fecha = fecha;
            this.Parent = parent;
        }
        public DateTime Fecha { get; set; }

        public AgenteReporteGestion Parent { get; private set; }
        public List<General.Cliente> Clientes { get { return this.Parent.ClientesCreados.Where(p => p.FecIng.Date == this.Fecha.Date).OrderBy(p => p.NombresCompletos).ToList(); } }
    }

    public class AgenteReporteGestion
    {
        public AgenteReporteGestion()
        {
            this.Gestionados = new List<AgenteReporteGestionItem<int?>>();
            this.NoGestionados = new List<AgenteReporteGestionItem<int?>>();
            this.Proximos = new List<AgenteReporteGestionItem<int?>>();
            this.Clientes = new List<Ruta.View.AgendaListado>();
            this.GruposVisitados = new List<AgenteReporteGestionGrupo>();
            this.GruposNoVisitados = new List<AgenteReporteGestionGrupo>();
            this.GruposReprogramados = new List<AgenteReporteGestionGrupo>();
            this.GruposPendientes = new List<AgenteReporteGestionGrupo>();
            this.GruposResumen = new List<AgenteReporteGestionResumen>();
            this.ClientesCreados = new List<General.Cliente>();
            this.GruposClientesCreados = new List<AgenteReporteGestionClienteGreado>();
            this.Marcaciones = new List<Marcacion.View.ReporteMarcacion>();
            this.PermisosJustificaciones = new List<Marcacion.View.PermisoListado>();
            this.Oportunidades = new List<Oportunidad.View.OportunidadListado>();
        }
        public List<AgenteReporteGestionItem<int?>> Gestionados { get; set; }
        public List<AgenteReporteGestionItem<int?>> NoGestionados { get; set; }
        public List<AgenteReporteGestionItem<int?>> Proximos { get; set; }
        public List<Models.Ruta.View.AgendaListado> Clientes { get; set; }
        public List<Models.Ruta.View.AgendaListado> ClientesNoVisitados { get { return this.Clientes.Where(p => p.Estado == Rp3.AgendaComercial.Models.Constantes.EstadoAgenda.NoVisitado).ToList(); } }
        public List<Models.Ruta.View.AgendaListado> ClientesVisitados { get { return this.Clientes.Where(p => p.Estado == Rp3.AgendaComercial.Models.Constantes.EstadoAgenda.Visitada).ToList(); } }
        public List<Models.Ruta.View.AgendaListado> ClientesReprogramados { get { return this.Clientes.Where(p => p.Estado == Rp3.AgendaComercial.Models.Constantes.EstadoAgenda.Reprogramada).ToList(); } }
        public List<Models.Ruta.View.AgendaListado> ClientesPendientes { get { return this.Clientes.Where(p => p.Estado == Rp3.AgendaComercial.Models.Constantes.EstadoAgenda.Pendiente | p.Estado == Rp3.AgendaComercial.Models.Constantes.EstadoAgenda.Gestionada).ToList(); } }
        public List<AgenteReporteGestionGrupo> GruposVisitados { get; set; }
        public List<AgenteReporteGestionGrupo> GruposNoVisitados { get; set; }
        public List<AgenteReporteGestionGrupo> GruposReprogramados { get; set; }
        public List<AgenteReporteGestionGrupo> GruposPendientes { get; set; }
        public List<AgenteReporteGestionResumen> GruposResumen { get; set; }
        public List<AgenteReporteGestionClienteGreado> GruposClientesCreados { get; set; }
        public List<Models.Marcacion.View.ReporteMarcacion> Marcaciones { get; set; }
        public List<General.Cliente> ClientesCreados { get; set; }
        public List<Models.Marcacion.View.PermisoListado> PermisosJustificaciones { get; set; }
        public List<Models.Oportunidad.View.OportunidadListado> Oportunidades { get; set; }
        public double Efectividad { get; set; }
        public string TotalMarcaciones
        {
            get
            {
                int horas = 0;
                int minutos = 0;
                foreach(var marcacion in this.Marcaciones)
                    if (marcacion.DuracionTotal != null)
                    {
                        horas += marcacion.DuracionTotal.Value.Hour;
                        minutos += marcacion.DuracionTotal.Value.Minute;
                    }
                bool canContinue = true;
                while(canContinue)
                {
                    if (minutos > 60)
                    {
                        minutos -= 60;
                        horas++;
                    }
                    canContinue = minutos > 60;
                }
                return string.Format("{0:00}:{1:00}", horas, minutos);
            }
        }

        public string TotalPermisos
        {
            get
            {
                int horas = 0;
                int minutos = 0;
                foreach (var permiso in this.PermisosJustificaciones)
                {
                    horas += permiso.DuracionPermiso.Hour;
                    minutos += permiso.DuracionPermiso.Minute;
                }
                bool canContine = true;
                while(canContine)
                {
                    if (minutos > 60)
                    {
                        minutos -= 60;
                        horas++;
                    }
                    canContine = minutos > 60;
                }
                return string.Format("{0:00}:{1:00}", horas, minutos);
            }
        }

        public string TotalParadas
        {
            get
            {
                int horas = 0;
                int minutos = 0;
                foreach (var resumen in this.GruposResumen)
                {
                    horas += resumen.ParadaDisplay.Hour;
                    minutos += resumen.ParadaDisplay.Minute;
                }
                bool canContine = true;
                while (canContine)
                {
                    if (minutos > 60)
                    {
                        minutos -= 60;
                        horas++;
                    }
                    canContine = minutos > 60;
                }
                return string.Format("{0:00}:{1:00}", horas, minutos);
            }
        }

        public string TotalRecorridos
        {
            get
            {
                int horas = 0;
                int minutos = 0;
                foreach (var resumen in this.GruposResumen)
                {
                    horas += resumen.RecorridoDisplay.Hour;
                    minutos += resumen.RecorridoDisplay.Minute;
                }
                bool canContine = true;
                while (canContine)
                {
                    if (minutos > 60)
                    {
                        minutos -= 60;
                        horas++;
                    }
                    canContine = minutos > 60;
                }
                return string.Format("{0:00}:{1:00}", horas, minutos);
            }
        }

        public double Asistencias
        {
            get
            {
                return this.Marcaciones.Count(p => !string.IsNullOrEmpty(p.HoraInicioPrimeraJornada) && p.MinutosAtraso == 0);
            }
        }
        public double Atrasos
        {
            get
            {
                return this.Marcaciones.Count(p => p.MinutosAtraso != 0 | p.MinutosAtrasoNoJustificado != 0);
            }
        }
        public double Ausencias
        {
            get
            {
                return this.Marcaciones.Count(p => string.IsNullOrEmpty(p.HoraInicioPrimeraJornada));
            }
        }
    }

    public class Dashboard
    {
        public int? Gestionados { get; set; }
        public int? Proximos { get; set; }
        public int? NoGestionados { get; set; }
        public int? Total { get; set; }

        public double Efectividad 
        { 
            get 
            {
                if (Total != null && Gestionados != null && Total > 0)
                    return ((double)this.Gestionados / (double)this.Total) * 100;
                else
                    return 0;
            } 
        }

        public int Kpi
        {
            get
            {
                if (Efectividad >= 80)
                    return 1;
                else if (Efectividad >= 50)
                    return 0;
                else
                    return -1;
            }
        }

        public string KpiColor
        {
            get
            {
                switch (Kpi)
                {
                    case -1: return "#BE2026";
                    case 0: return "yellow";
                    case 1: return "#6FBF56";
                }

                return String.Empty;
            }
        }
    }

    public class DashboardResumenGestion : Dashboard
    {
        
    }

    public class DashboardResumenGestionAgente : Dashboard
    {
        public int MarkerIndex { get; set; }
        public int? IdAgente { get; set; }
        public string Agente { get; set; }
        public int? DuracionPromedioMinutos { get; set; }
        public string DuracionPromedio
        {
            get
            {
                int horas = (DuracionPromedioMinutos??0) / 60;
                int minutos = (DuracionPromedioMinutos??0) % 60;
                return string.Format("{0:##00}H{1:00}", horas,minutos);
            }
        }
    }

    public class DashboardAgenteCategoriaCliente : DashboardResumenGestionAgente
    {      
        public int IdCliente { get; set; }
        public string Cliente { get; set; }
        public int? IdClienteContacto { get; set; }
        public string ClienteContacto { get; set; }
        public string IdCategoriaGestion { get; set; }
        public string EstadoAgenda { get; set; }
        public string EstadoAgendaDescripcion { get; set; }
        public string EstadoColor { get; set; }

        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public DateTime? FechaInicioGestion { get; set; }
        public DateTime? FechaFinGestion { get; set; }        
    }
}