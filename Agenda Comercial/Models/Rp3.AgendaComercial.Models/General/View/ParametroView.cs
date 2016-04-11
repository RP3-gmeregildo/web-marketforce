using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rp3.AgendaComercial.Models.General.View
{
    public class ParametroView
    {
        public string Leyenda { get; set; }
        public string Etiqueta { get; set; }
        public string Name { get; set; }

        public string ParametroName { get { return "parametro" + Name; } }
        public string Value { get; set; }
        public int Tipo { get; set; }

        public int? Minimo { get; set; }

        public int? Maximo { get; set; }
    }
}
