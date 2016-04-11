using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rp3.Data.Models.General;
using Rp3.AgendaComercial.Models.Ruta;
using Rp3.Web.Mvc;
using System.ComponentModel.DataAnnotations.Schema;
using Rp3.AgendaComercial.Models.Marcacion;

namespace Rp3.AgendaComercial.Models.General
{
    [Table("tbAgente", Schema = "gen")]
    public class Agente : Rp3.Data.Entity.EntityBase
    {
        public override string Key
        {
            get
            {
                if (this.IdAgente == 0) return " ";
                return this.IdAgente.ToString();
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
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdAgente { get; set; }

        [Required(ErrorMessageResourceType = typeof(Rp3.AgendaComercial.Resources.ErrorMessageValidation), ErrorMessageResourceName = "Required")]
        [StringLength(250, MinimumLength = 1, ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "MaxMinStringLength")]
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Agente")]
        public string Descripcion { get; set; }

        [Required(ErrorMessageResourceType = typeof(Rp3.AgendaComercial.Resources.ErrorMessageValidation), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Cargo")]
        public int IdCargo { get; set; }
        
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Supervisor")]
        public int? IdSupervisor { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Ruta")]
        public int? IdRuta { get; set; }
        
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Usuario")]
        public int? IdUsuario { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Grupo")]
        public int? IdGrupo { get; set; }
        public short EstadoTabla { get; set; }
        public string Estado { get; set; }
        public string UsrIng { get; set; }
        public DateTime FecIng { get; set; }
        public string UsrMod { get; set; }
        public DateTime? FecMod { get; set; }

        public string GCMId { get; set; }

        [ForeignKey("EstadoTabla,Estado")]
        public virtual GeneralValues EstadoGeneralValue { get; set; }

        [ForeignKey("IdCargo")]
        public virtual Cargo Cargo { get; set; }

        [ForeignKey("IdGrupo")]
        public virtual Grupo Grupo { get; set; }

        [ForeignKey("IdSupervisor")]
        public virtual Agente Supervisor { get; set; }

        public virtual List<Agente> Agentes { get; set; }

        [ForeignKey("IdRuta")]
        public virtual Ruta.Ruta Ruta { get; set; }

        [ForeignKey("IdUsuario")]
        public virtual Rp3.Data.Models.Security.UserBase Usuario { get; set; }

        public virtual List<AgenteUbicacion> AgenteUbicaciones { get; set; }

        [NotMapped]
        public int MarkerIndex { get; set; }

        public void AsignarId()
        {
            ContextService service = new ContextService();
            this.IdAgente = service.Agentes.GetMaxValue<int>(p => p.IdAgente, 0) + 1;
        }
    }
}
