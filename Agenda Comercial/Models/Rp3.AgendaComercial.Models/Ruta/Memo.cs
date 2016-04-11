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
    [Table("tbMemo", Schema = "rut")]
    public class Memo : Rp3.Data.Entity.EntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long IdMemo { get; set; }

        [Required(ErrorMessageResourceType = typeof(Rp3.AgendaComercial.Resources.ErrorMessageValidation), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Fecha")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? Fecha { get; set; }

        [NotMapped]
        public long FechaTicks
        {
            get
            {
                if (Fecha.HasValue)
                    return Fecha.Value.Ticks;
                return 0;
            }
            set
            {
                if (value == 0)
                    Fecha = null;
                else
                    Fecha = new DateTime(value);
            }
        }

        [Required(ErrorMessageResourceType = typeof(Rp3.AgendaComercial.Resources.ErrorMessageValidation), ErrorMessageResourceName = "Required")]
        [StringLength(100, MinimumLength = 1, ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "MaxMinStringLength")]
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Titulo")]
        public string Titulo { get; set; }

        [StringLength(500, MinimumLength = 1, ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "MaxMinStringLength")]
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Detalle")]
        public string Detalle { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Remitente")]
        public int IdAgenteRemitente { get; set; }
        
        public short TipoMemoTabla { get; set; }
     
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Tipo")]
        public string TipoMemo { get; set; }
       
        public short ImportanciaTabla { get; set; }
      
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Importancia")]
        public string Importancia { get; set; }
     
        public short EstadoMemoTabla { get; set; }
        
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Estado")]
        public string EstadoMemo { get; set; }
        public string UsrIng { get; set; }
        public DateTime FecIng { get; set; }
        public string UsrMod { get; set; }
        public DateTime? FecMod { get; set; }

        [ForeignKey("TipoMemoTabla,TipoMemo")]
        public virtual GeneralValues TipoMemoGeneralValue { get; set; }
        [ForeignKey("ImportanciaTabla,Importancia")]
        public virtual GeneralValues ImportanciaGeneralValue { get; set; }
        [ForeignKey("EstadoMemoTabla,EstadoMemo")]
        public virtual GeneralValues EstadoMemoGeneralValue { get; set; }

        [ForeignKey("IdAgenteRemitente")]
        public virtual Agente Remitente { get; set; }

        [NotMapped]
        public string Destinatarios
        {
            get
            {
                string idText = String.Empty;

                if (this.MemoDestinatarios != null)
                {
                    foreach (var agente in this.MemoDestinatarios)
                        if (String.IsNullOrEmpty(idText))
                            idText = Convert.ToString(agente.IdAgente);
                        else
                            idText = String.Format("{0}-{1}", idText, agente.IdAgente);
                }

                return idText;
            }

            set
            {
                if (this.MemoDestinatarios == null)
                    this.MemoDestinatarios = new List<MemoDestinatario>();

                this.MemoDestinatarios.Clear();

                if (value != null)
                {
                    string[] keyParts = value.Split('-');

                    foreach (var id in keyParts)
                        if (!String.IsNullOrEmpty(id))
                        {
                            var asistente = new MemoDestinatario() { IdMemo = this.IdMemo, IdAgente = Convert.ToInt32(id) };
                            this.MemoDestinatarios.Add(asistente);
                        }
                }
            }
        }

        public virtual List<MemoDestinatario> MemoDestinatarios { get; set; }

        [NotMapped]
        public bool ReadOnly { get; set; }
        public void AsignarId()
        {
            ContextService service = new ContextService();
            this.IdMemo = service.Memos.GetMaxValue<long>(p => p.IdMemo, 0) + 1;
        }
    }
}
