using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rp3.AgendaComercial.Models.General
{
    [Table("tbProcessStep", Schema = "gen")]
    public class ProcessStep : Rp3.Data.Entity.EntityBase
    {

        public override string Key
        {
            get
            {
                return string.Format("{0}-{1}", this.IdProcess, this.IdProcessStep);
            }
        }

        public string KeyCombo
        {
            get
            {
                if (this.IdProcessStep == 0) return " ";
                return this.IdProcessStep.ToString();
            }
        }

        public override string DescriptionName
        {
            get
            {
                return this.Descripcion;
            }
        }

        [Key]
        [Column(Order = 0)]
        public int IdProcess { get; set; }
        [Key]
        [Column(Order = 1)]
        public int IdProcessStep { get; set; }
        public string Descripcion { get; set; }
        public string UsrIng { get; set; }
        public DateTime FecIng { get; set; }

        public void AsignarId()
        {
            ContextService service = new ContextService();
            this.IdProcessStep = service.ProcessSteps.GetMaxValue<int>(p => p.IdProcessStep, 0, p => p.IdProcess == this.IdProcess) + 1;
        }
    }
}
