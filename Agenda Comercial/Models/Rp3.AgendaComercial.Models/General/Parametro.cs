using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rp3.AgendaComercial.Models.General
{
    [Table("tbParametro", Schema = "gen")]
    public class Parametro: Rp3.Data.Entity.EntityBase
    {
        [Key]
        public string IdParametro { get; set; }
        public string Valor { get; set; }

        public string Etiqueta { get; set; }

        public string Leyenda { get; set; }

        public int Tipo { get; set; }

        public int? Minimo { get; set; }

        public int? Maximo { get; set; }

        public int Orden { get; set; }

        public string UnidadMedida { get; set; } 
    }
}
