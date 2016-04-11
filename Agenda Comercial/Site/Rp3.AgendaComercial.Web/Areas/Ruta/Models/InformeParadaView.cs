using Rp3.AgendaComercial.Models.General;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Rp3.AgendaComercial.Web.Areas.Ruta.Models
{
    public class InformeParadaDetalle
    {
        public DateTime Fecha { get; set; }
        public string HoraEntrada { get; set; }
        public string Tiempo { get; set; }
        public string HoraSalida { get; set; }
        public double Distancia { get; set; }
        public bool EsMovimiento { get; set; }
        public double Minutos { get; set; }
        public string Ubicacion { get; set; }
        public string Direccion { get; set; }
        public double Latitud { get; set; }
        public double Longitud { get; set; }
    }

    public class InformeParadaView
    {
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Agente")]
        public int IdAgente { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "TiempoDetenido")]
        public string TiempoDetenido { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "TiempoRecorrido")]
        public string TiempoRecorrido { get; set; }


        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "TimpoMinimoParada")]
        public int TiempoMinimoParada { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "HoraPrimerReporte")]
        public string HoraPrimerReporte { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "HoraUltimoReporte")]
        public string HoraUltimoReporte { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Kms")]
        public string Kilometros { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Paradas")]
        public string Paradas { get; set; }


        public long? FechaTicks { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Fecha")]
        public DateTime? Fecha
        {
            get
            {
                if (FechaTicks.HasValue)
                    return new DateTime(FechaTicks.Value);
                else
                    return null;
            }
            set
            {
                if (value.HasValue)
                    this.FechaTicks = value.Value.Ticks;
                else
                {
                    this.FechaTicks = null;
                }
            }
        }

        public string FechaByAgente { get; set; }
    }
}