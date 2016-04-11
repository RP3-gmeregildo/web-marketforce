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
    [Table("tbRuta", Schema = "rut")]
    public class Ruta : Rp3.Data.Entity.EntityBase
    {
        public override string Key
        {
            get
            {
                if (this.IdRuta == 0) return " ";
                return this.IdRuta.ToString();
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
        public int IdRuta { get; set; }

        [Required(ErrorMessageResourceType = typeof(Rp3.AgendaComercial.Resources.ErrorMessageValidation), ErrorMessageResourceName = "Required")]
        [StringLength(250, MinimumLength = 1, ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "MaxMinStringLength")]
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Ruta")]
        public string Descripcion { get; set; }
       
        public short EstadoTabla { get; set; }
        public string Estado { get; set; }
        public string UsrIng { get; set; }
        public DateTime FecIng { get; set; }
        public string UsrMod { get; set; }
        public DateTime? FecMod { get; set; }

        public string LoteResumen { get; set; }

        public int? CantidadClientes { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Calendario")]
        public int? IdCalendario { get; set; }

        [ForeignKey("IdCalendario")]
        public virtual Calendario Calendario { get; set; }

        [ForeignKey("EstadoTabla,Estado")]
        public virtual GeneralValues EstadoGeneralValue { get; set; }
        public virtual List<ProgramacionRuta> ProgramacionesRuta { get; set; }
       
        public virtual List<Agente> Agentes { get; set; }
        public virtual List<RutaDetalle> RutaDetalles { get; set; }
        public virtual List<RutaIncluir> RutaIncluirs { get; set; }
        public virtual List<RutaExcluir> RutaExcluirs { get; set; }
        public virtual List<RutaLote> RutaLotes { get; set; }

        [NotMapped]
        public string Lotes
        {
            get
            {
                string idText = String.Empty;

                if (this.RutaLotes != null)
                {
                    foreach (var lote in this.RutaLotes)
                        if (String.IsNullOrEmpty(idText))
                            idText = Convert.ToString(lote.IdLote);
                        else
                            idText = String.Format("{0}-{1}", idText, lote.IdLote);
                }

                return idText;
            }

            set
            {
                if (this.RutaLotes == null)
                    this.RutaLotes = new List<RutaLote>();

                this.RutaLotes.Clear();

                if (value != null)
                {
                    string[] keyParts = value.Replace(',', '-').Split('-');

                    foreach (var id in keyParts)
                        if (!String.IsNullOrEmpty(id))
                        {
                            var zona = new RutaLote() { IdRuta = this.IdRuta, IdLote = Convert.ToInt32(id) };
                            this.RutaLotes.Add(zona);
                        }
                }
            }
        }

        [NotMapped]
        public string Etiqueta
        {
            get 
            {
                string text = this.Descripcion;

                if (Agentes != null)
                {
                    foreach (var agente in this.Agentes)
                        text = String.Format("{0} - {1}", text, agente.Descripcion);
                }

                return text;
            }
        }

        [NotMapped]
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Agente")]
        public string Agente
        {
            get
            {
                string text = String.Empty;

                if (Agentes != null)
                {
                    foreach (var agente in this.Agentes)
                    {
                        if (text.Length > 0)
                            text = String.Format("{0} - {1}", text, agente.Descripcion);
                        else
                            text = agente.Descripcion;
                    }
                }

                return text;
            }
        }

        [NotMapped]
        public bool ReadOnly { get; set; }
        public void AsignarId()
        {
            ContextService service = new ContextService();
            this.IdRuta = service.Rutas.GetMaxValue<int>(p => p.IdRuta, 0) + 1;
        }
    }
}
