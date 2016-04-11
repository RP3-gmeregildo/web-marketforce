using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rp3.AgendaComercial.Models.General.View
{
    public class ActualizacionCliente
    {
        public long IdTarea { get; set; }
        public bool PermitirCreacion { get; set; }
        public bool PermitirModificacion { get; set; }
        public bool SiempreEditarEnGestion { get; set; }
        public bool SoloFaltantesEditarEnGestion { get; set; }
        public List<Campo> Campos { get; set; }
    }

    public class Campo
    {
        public string IdCampo { get; set; }
        public bool C { get; set; }
        public bool M { get; set; }
        public bool G { get; set; }
        public bool O { get; set; }
    }
}
