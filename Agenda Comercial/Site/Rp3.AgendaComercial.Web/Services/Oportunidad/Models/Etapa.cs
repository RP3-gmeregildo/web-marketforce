using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rp3.AgendaComercial.Web.Services.Oportunidad.Models
{
    public class Etapa
    {
        public int IdEtapa { get; set; }
        public int IdOportunidadTipo { get; set; }
        public int Orden { get; set; }
        public string Descripcion { get; set; }
        public short EstadoTabla { get; set; }
        public string Estado { get; set; }
        public int? IdEtapaPadre { get; set; }
        public int? Dias { get; set; }
        public bool EsVariable { get; set; }
        public virtual List<EtapaTarea> EtapaTareas { get; set; }
    }
    public class EtapaTarea
    {
        public long IdTarea { get; set; }
        public int Orden { get; set; }
    }
}