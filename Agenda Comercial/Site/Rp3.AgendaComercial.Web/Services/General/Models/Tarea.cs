using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rp3.AgendaComercial.Web.Services.General.Models
{
    public class Tarea
    {
        public long IdTarea { get; set; }
        public string Descripcion { get; set; }
        public string Estado { get; set; }
        public string TipoTarea { get; set; }
        public DateTime FechaVigenciaDesde { get; set; }
        public DateTime? FechaVigenciaHasta { get; set; }

        public List<TareaActividad> TareaActividades { get; set; }

        public long FechaVigenciaDesdeTicks
        {
            get
            {

                return FechaVigenciaDesde.Ticks;                
            }            
        }

        public long? FechaVigenciaHastaTicks
        {
            get
            {
                if (FechaVigenciaHasta.HasValue)
                    return FechaVigenciaHasta.Value.Ticks;
                return null;
            }            
        }
    }
}