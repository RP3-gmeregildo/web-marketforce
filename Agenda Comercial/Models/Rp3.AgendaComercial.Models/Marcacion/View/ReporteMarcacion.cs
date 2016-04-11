using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rp3.AgendaComercial.Models.Marcacion.View
{
    public class ReporteMarcacion
    {
        public DateTime Fecha { get; set; }
        public int IdAgente { get; set; }
        public string Agente { get; set; }
        public int? IdGrupo { get; set; }
        public string Grupo { get; set; }
        public bool AplicaMarcacion { get; set; }
        public string HoraInicioPrimeraJornada { get; set; }
        public string HoraFinPrimeraJornada { get; set; }
        public string HoraInicioSegundaJornada { get; set; }
        public string HoraFinSegundaJornada { get; set; }
        public bool Ausente { get; set; }
        public int MinutosAtraso { get; set; }

        public int MinutosAtrasoNoJustificado { get; set; }
        public bool PermisoPrevio { get; set; }

        public bool TienePermiso { get; set; }
        public bool EnUbicacion { get; set; }

        public double Eficiencia { get; set; }
        public int KpiEficiencia { get; set; }

        public string DireccionMarcacion { get; set; }

        public int IdMarcacion { get; set; }

        public bool DiaLaboral { get; set; }

        public void ConfigEficiencia(double max, double min)
        {
            if (!string.IsNullOrEmpty(this.HoraInicioPrimeraJornada))
            {
                this.Eficiencia = Models.Marcacion.Marcacion.Efectividad(this.MinutosAtrasoNoJustificado);

                if (this.Eficiencia >= max)
                    this.KpiEficiencia = 1;
                else if (this.Eficiencia >= min)
                    this.KpiEficiencia = 0;
                else
                    this.KpiEficiencia = -1;
            }
            else
            {
                this.Eficiencia = 0;
                this.KpiEficiencia = -2;
            }
        }
        public string KpiEficienciaColor
        {
            get
            {
                switch (KpiEficiencia)
                {
                    case -1: return "#BE2026";
                    case 0: return "yellow";
                    case 1: return "#6FBF56";
                }

                return String.Empty;
            }
        }

        public DateTime? DuracionTotal
        {
            get
            {
                DateTime? jornada1Ini = null;
                DateTime? jornada1Fin = null;
                DateTime? jornada2Ini = null;
                DateTime? jornada2Fin = null;
                DateTime? duracion1 = null;
                DateTime? duracion2 = null;
                if (!string.IsNullOrEmpty(this.HoraInicioPrimeraJornada)) jornada1Ini = Convert.ToDateTime(this.HoraInicioPrimeraJornada);
                if (!string.IsNullOrEmpty(this.HoraFinPrimeraJornada)) jornada1Fin = Convert.ToDateTime(this.HoraFinPrimeraJornada);
                if (!string.IsNullOrEmpty(this.HoraInicioSegundaJornada)) jornada2Ini = Convert.ToDateTime(this.HoraInicioSegundaJornada);
                if (!string.IsNullOrEmpty(this.HoraFinSegundaJornada)) jornada2Fin = Convert.ToDateTime(this.HoraFinSegundaJornada);
                if (jornada1Ini != null)
                    if (jornada1Fin != null)
                        duracion1 = new DateTime(jornada1Fin.Value.Subtract(jornada1Ini.Value).Ticks);
                    else if (jornada2Fin != null)
                        duracion1 = new DateTime(jornada2Fin.Value.Subtract(jornada1Ini.Value).Ticks);
                if (jornada2Ini != null && jornada2Fin != null) 
                    duracion2 = new DateTime(jornada2Fin.Value.Subtract(jornada2Ini.Value).Ticks);

                if (duracion1 != null && duracion2 != null)
                    return duracion1.Value.AddMinutes(duracion2.Value.Minute).AddHours(duracion2.Value.Hour);
                else if (duracion1 != null)
                    return duracion1;
                else if (duracion2 != null)
                    return duracion2;
                else
                    return null;
            }
        }

        public string DuracionTotalDisplay
        {
            get
            {
                DateTime? value = this.DuracionTotal;
                if (value != null) return value.Value.ToString("HH:mm");
                return string.Empty;
            }
        }

        public string TienePermisioDisplay 
        { 
            get 
            {
                if (string.IsNullOrEmpty(this.HoraInicioPrimeraJornada)) return string.Empty;
                return this.PermisoPrevio ? "Sí" : "No"; 
            } 
        }

        public int? MinutosAtrasoDisplay
        {
            get
            {
                if (string.IsNullOrEmpty(this.HoraInicioPrimeraJornada)) return null;
                return this.MinutosAtraso;
            }
        }

        public double? EficienciaDisplay
        {
            get
            {
                if (string.IsNullOrEmpty(this.HoraInicioPrimeraJornada)) return null;
                return this.Eficiencia;
            }
        }
    }
}
