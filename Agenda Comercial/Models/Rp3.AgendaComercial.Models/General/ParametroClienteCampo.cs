using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rp3.AgendaComercial.Models.General
{
    [Table("tbParametroClienteCampo", Schema = "gen")]
    public class ParametroClienteCampo : Rp3.Data.Entity.EntityBase
    {
        public override string Key
        {
            get
            {
                if (this.IdCampo == null) return " ";
                return this.IdCampo.ToString();
            }
        }

        public override string DescriptionName
        {
            get
            {
                return this.Nombre;
            }
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string IdCampo { get; set; }

        public string Nombre { get; set; }

        public short IdGrupo { get; set; }

        public short Prioridad { get; set; }

        public bool EsObligatorio { get; set; }
    }
}
