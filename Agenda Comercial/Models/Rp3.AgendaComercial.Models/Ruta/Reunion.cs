using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rp3.Data.Models.General;
using Rp3.AgendaComercial.Models.General;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rp3.AgendaComercial.Models.Ruta
{
    [Table("tbReunion", Schema = "rut")]
    public class Reunion : Rp3.Data.Entity.EntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long IdReunion { get; set; }

        [Required(ErrorMessageResourceType = typeof(Rp3.AgendaComercial.Resources.ErrorMessageValidation), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "FechaInicio")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? FechaInicio { get; set; }

        DateTime? fechaInicioHora;
        [NotMapped]
        public DateTime? FechaInicioHora 
        {
            get
            {
                if (fechaInicioHora != null)
                    return new DateTime(1900, 1, 1).AddHours(fechaInicioHora.Value.Hour).AddMinutes(fechaInicioHora.Value.Minute).AddSeconds(fechaInicioHora.Value.Second);
                else
                    return null;
            }
            set
            {
                fechaInicioHora = value;
            }
        }

        [Required(ErrorMessageResourceType = typeof(Rp3.AgendaComercial.Resources.ErrorMessageValidation), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "FechaFin")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? FechaFin { get; set; }

        DateTime? fechaFinHora;
        [NotMapped]
        public DateTime? FechaFinHora
        {
            get
            {
                if (fechaFinHora != null)
                    return new DateTime(1900, 1, 1).AddHours(fechaFinHora.Value.Hour).AddMinutes(fechaFinHora.Value.Minute).AddSeconds(fechaFinHora.Value.Second);
                else
                    return null;
            }
            set
            {
                fechaFinHora = value;
            }
        }

        #region Ticks

        [NotMapped]
        public long FechaInicioTicks
        {
            get
            {
                if (FechaInicio.HasValue)
                    return FechaInicio.Value.Ticks;
                return 0;
            }
            set
            {
                if (value == 0)
                    FechaInicio = null;
                else
                    FechaInicio = new DateTime(value);
            }
        }

        [NotMapped]
        public long FechaFinTicks
        {
            get
            {
                if (FechaFin.HasValue)
                    return FechaFin.Value.Ticks;
                return 0;              
            }
            set
            {
                if (value == 0)
                    FechaFin = null;
                else
                    FechaFin = new DateTime(value);
            }
        }

        #endregion Ticks

        [Required(ErrorMessageResourceType = typeof(Rp3.AgendaComercial.Resources.ErrorMessageValidation), ErrorMessageResourceName = "Required")]
        [StringLength(250, MinimumLength = 1, ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "MaxMinStringLength")]
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Ubicacion")]
        public string Ubicacion { get; set; }

        [Required(ErrorMessageResourceType = typeof(Rp3.AgendaComercial.Resources.ErrorMessageValidation), ErrorMessageResourceName = "Required")]
        [StringLength(100, MinimumLength = 1, ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "MaxMinStringLength")]
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Asunto")]
        public string Asunto { get; set; }
        
        [StringLength(500, MinimumLength = 1, ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "MaxMinStringLength")]
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Detalle")]
        public string Detalle { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Solicitante")]
        public int IdAgenteSolicitante { get; set; }
        
        public short TipoReunionTabla { get; set; }      
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Tipo")]
        public string TipoReunion { get; set; }
        
        public short ImportanciaTabla { get; set; }
        
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Importancia")]
        public string Importancia { get; set; }        
        public short EstadoReunionTabla { get; set; }
      
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Estado")]
        public string EstadoReunion { get; set; }
        public string UsrIng { get; set; }
        public DateTime FecIng { get; set; }
        public string UsrMod { get; set; }
        public DateTime? FecMod { get; set; }

        [ForeignKey("TipoReunionTabla,TipoReunion")]
        public virtual GeneralValues TipoReunionGeneralValue { get; set; }
        [ForeignKey("ImportanciaTabla,Importancia")]
        public virtual GeneralValues ImportanciaGeneralValue { get; set; }
        [ForeignKey("EstadoReunionTabla,EstadoReunion")]
        public virtual GeneralValues EstadoReunionGeneralValue { get; set; }

        [ForeignKey("IdAgenteSolicitante")]
        public virtual Agente Solicitante { get; set; }

        [NotMapped]
        public string Asistentes
        {
            get
            {
                string idText = String.Empty;

                if (this.ReunionAsistentes != null)
                {
                    foreach (var agente in this.ReunionAsistentes)
                        if (String.IsNullOrEmpty(idText))
                            idText = Convert.ToString(agente.IdAgente);
                        else
                            idText = String.Format("{0}-{1}", idText, agente.IdAgente);
                }

                return idText;
            }

            set
            {
                if (this.ReunionAsistentes == null)
                    this.ReunionAsistentes = new List<ReunionAsistente>();

                this.ReunionAsistentes.Clear();

                if (value != null)
                {
                    string[] keyParts = value.Split('-');

                    foreach (var id in keyParts)
                        if (!String.IsNullOrEmpty(id))
                        {
                            var asistente = new ReunionAsistente() { IdReunion = this.IdReunion, IdAgente = Convert.ToInt32(id) };
                            this.ReunionAsistentes.Add(asistente);
                        }
                }
            }
        }

        public virtual List<ReunionAsistente> ReunionAsistentes { get; set; }

        [NotMapped]
        public bool ReadOnly { get; set; }
        public void AsignarId()
        {
            ContextService service = new ContextService();
            this.IdReunion = service.Reuniones.GetMaxValue<long>(p => p.IdReunion, 0) + 1;
        }
    }
}
