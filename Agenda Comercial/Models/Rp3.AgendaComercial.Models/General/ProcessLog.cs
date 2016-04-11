using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rp3.AgendaComercial.Models.General
{
    [Table("tbProcessLog", Schema = "gen")]
    public class ProcessLog : Rp3.Data.Entity.EntityBase
    {
        public override string Key
        {
            get
            {
                return string.Format("{0}-{1}-{2}", this.IdProcess, this.IdProcessStep, this.IdProcessLog);
            }
        }

        public string KeyCombo
        {
            get
            {
                if (this.IdProcessLog == 0) return " ";
                return this.IdProcessLog.ToString();
            }
        }

        [Key]
        [Column(Order = 0)]
        public int IdProcess { get; set; }
        [Key]
        [Column(Order = 1)]
        public int IdProcessStep { get; set; }
        [Key]
        [Column(Order = 2)]
        public int IdProcessLog { get; set; }
        public string Result { get; set; }
        public string ErrorLog { get; set; }
        public string UsrIng { get; set; }
        public DateTime FecIng { get; set; }

        public void AsignarId()
        {
            ContextService service = new ContextService();
            this.IdProcessLog = service.ProcessLogs.GetMaxValue<int>(p => p.IdProcessLog, 0, p => p.IdProcess == this.IdProcess && p.IdProcessStep == this.IdProcessStep) + 1;
        }
    }
}