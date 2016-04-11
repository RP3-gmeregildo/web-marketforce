using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rp3.AgendaComercial.Web.Services.General.Models
{
    public class Calendario
    {
        public int IdCalendario { get; set; }
        public List<DiaLaboral> DiasLaborales { get; set; }
        public List<DiaNoLaborable> DiasNoLaborables { get; set; }
    }

    public class DiaLaboral
    {
        public string IdDia { get; set; }
        public short IdDiaTabla { get; set; }
        public int Orden { get; set; }
        public bool EsLaboral { get; set; }
        public string HoraInicio1 { get; set; }
        public string HoraFin1 { get; set; }
        public string HoraInicio2 { get; set; }
        public string HoraFin2 { get; set; }
    }

    public class DiaNoLaborable
    {
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
        public bool DiaParcial { get; set; }
        public bool EsteAño { get; set; }
        public string HoraInicio { get; set; }
        public string HoraFin { get; set; }
    }
}