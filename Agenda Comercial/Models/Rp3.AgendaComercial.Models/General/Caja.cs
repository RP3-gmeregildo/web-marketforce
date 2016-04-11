using Rp3.AgendaComercial.Models.CxC;
using Rp3.AgendaComercial.Models.Transaccion;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rp3.AgendaComercial.Models.General
{
    [Table("tbCaja", Schema = "gen")]
    public class Caja : Rp3.Data.Entity.EntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdCaja { get; set; }
        public string Nombre { get; set; }
        public int IdRuta { get; set; }
        public int SecuenciaRecibo { get; set; }
        public int MaximoDiasApertura { get; set; }
        public string UsrIng { get; set; }
        public DateTime FecIng { get; set; }
        public string UsrMod { get; set; }
        public DateTime? FecMod { get; set; }

        public virtual List<CajaControl> CajaControles { get; set; }
        public virtual List<CajaFormaPago> CajaFormasPago { get; set; }
        public void AsignarId()
        {
            ContextService service = new ContextService();
            this.IdCaja = service.Cajas.GetMaxValue<int>(p => p.IdCaja, 0) + 1;
        }
    }
}
