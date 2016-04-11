using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rp3.AgendaComercial.Models.Oportunidad.View
{
    public class EtapaSave : Rp3.Data.Entity.EntityBase
    {
        public List<EtapaView> Etapas { get; set; }
        public OportunidadTipo OportunidadTipo { get; set; }
    }

    public class EtapaView
    {
        public int IdEtapa { get; set; }
        public int IdOportunidadTipo { get; set; }
        public string Descripcion { get; set; }
        public int Orden { get; set; }
        public int? IdEtapaPadre { get; set; }
        public int? Dias { get; set; }
        public bool Nuevo { get; set; }
        public bool EsVariable { get; set; }
        public List<EtapaView> SubEtapas { get; set; }
        public List<EtapaTareaView> Tareas { get; set; }
    }

    public class EtapaTareaView
    {
        public int IdEtapa { get; set; }
        public long IdTarea { get; set; }
        public string Descripcion { get; set; }
        public int Orden { get; set; }
    }
}
