using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rp3.AgendaComercial.Models.Marcacion.View
{
    public class AnalisisAgenteChart 
    {
        public string Descripcion { get; set; }
        public int Valor { get; set; }

        public string Color { get; set; }
    }

    public class AnalisisAgente
    {
        public int IdAgente { get; set; }
        public string Agente { get; set; }
        public int? IdGrupo { get; set; }
        public string Grupo { get; set; }
        public int IdCalendario { get; set; }
        public int DiasLaborales { get; set; }
        public bool AplicaMarcacion { get; set; }
        public int Asistencias { get; set; }
        public int AsistenciasATiempo { get; set; }

        public int Ausencias { get; set; }
        public int Atrasos { get; set; }
        public int MinutosAtraso { get; set; }

        public int MinutosAtrasoNoJustificado { get; set; }

        public double Eficiencia { get; set; }
        public int KpiEficiencia { get; set; }

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
    }
}
