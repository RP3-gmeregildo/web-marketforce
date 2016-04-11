using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rp3.Data.Models.General;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rp3.AgendaComercial.Models.Ruta
{
    [Table("tbLote", Schema = "rut")]
    public class Lote : Rp3.Data.Entity.EntityBase
    {
        public override string Key
        {
            get
            {
                if (this.IdLote == 0) return " ";
                return this.IdLote.ToString();
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
        public int IdLote { get; set; }

        [Required(ErrorMessageResourceType = typeof(Rp3.AgendaComercial.Resources.ErrorMessageValidation), ErrorMessageResourceName = "Required")]
        [StringLength(250, MinimumLength = 1, ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "MaxMinStringLength")]
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Lote")]
        public string Descripcion { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Calificacion")]
        public int Calificacion { get; set; }                
        public short EstadoTabla { get; set; }
        public string Estado { get; set; }
        public string UsrIng { get; set; }
        public DateTime FecIng { get; set; }
        public string UsrMod { get; set; }
        public DateTime? FecMod { get; set; }

        public string CanalResumen { get; set; }
        public string TipoClienteResumen { get; set; }
        public string ZonaResumen { get; set; }
        public int? CantidadClientes { get; set; }

        [ForeignKey("EstadoTabla,Estado")]
        public virtual GeneralValues EstadoGeneralValue { get; set; }

        public virtual List<LoteDetalle> LoteDetalles { get; set; }
        public virtual List<LoteTipoCliente> LoteTipoClientes { get; set; }
        public virtual List<LoteCanal> LoteCanales { get; set; }
        public virtual List<LoteZona> LoteZonas { get; set; }

        [NotMapped]
        public string Canales
        {
            get
            {
                string idText = String.Empty;

                if (this.LoteCanales != null)
                {
                    foreach (var canal in this.LoteCanales)
                        if (String.IsNullOrEmpty(idText))
                            idText = Convert.ToString(canal.IdCanal);
                        else
                            idText = String.Format("{0}-{1}", idText, canal.IdCanal);
                }

                return idText;
            }

            set
            {
                if (this.LoteCanales == null)
                    this.LoteCanales = new List<LoteCanal>();

                this.LoteCanales.Clear();

                if (value != null)
                {
                    string[] keyParts = value.Replace(',', '-').Split('-');

                    foreach (var id in keyParts)
                        if (!String.IsNullOrEmpty(id))
                        {
                            var canal = new LoteCanal() { IdLote = this.IdLote, IdCanal = Convert.ToInt32(id) };
                            this.LoteCanales.Add(canal);
                        }
                }
            }
        }

        [NotMapped]
        public string Zonas
        {
            get
            {
                string idText = String.Empty;

                if (this.LoteZonas != null)
                {
                    foreach (var zona in this.LoteZonas)
                        if (String.IsNullOrEmpty(idText))
                            idText = Convert.ToString(zona.IdZona);
                        else
                            idText = String.Format("{0}-{1}", idText, zona.IdZona);
                }

                return idText;
            }

            set
            {
                if (this.LoteZonas == null)
                    this.LoteZonas = new List<LoteZona>();

                this.LoteZonas.Clear();

                if (value != null)
                {
                    string[] keyParts = value.Replace(',', '-').Split('-');

                    foreach (var id in keyParts)
                        if (!String.IsNullOrEmpty(id))
                        {
                            var zona = new LoteZona() { IdLote = this.IdLote, IdZona = Convert.ToInt32(id) };
                            this.LoteZonas.Add(zona);
                        }
                }
            }
        }

        [NotMapped]
        public string TipoClientes
        {
            get
            {
                string idText = String.Empty;

                if (this.LoteTipoClientes != null)
                {
                    foreach (var tipoCliente in this.LoteTipoClientes)
                        if (String.IsNullOrEmpty(idText))
                            idText = Convert.ToString(tipoCliente.IdTipoCliente);
                        else
                            idText = String.Format("{0}-{1}", idText, tipoCliente.IdTipoCliente);
                }

                return idText;
            }

            set
            {
                if (this.LoteTipoClientes == null)
                    this.LoteTipoClientes = new List<LoteTipoCliente>();

                this.LoteTipoClientes.Clear();

                if (value != null)
                {
                    string[] keyParts = value.Replace(',','-').Split('-');

                    foreach (var id in keyParts)
                        if (!String.IsNullOrEmpty(id))
                        {
                            var tipoCliente = new LoteTipoCliente() { IdLote = this.IdLote, IdTipoCliente = Convert.ToInt32(id) };
                            this.LoteTipoClientes.Add(tipoCliente);
                        }
                }
            }
        }

        [NotMapped]
        public bool ReadOnly { get; set; }
        public void AsignarId()
        {
            ContextService service = new ContextService();
            this.IdLote = service.Lotes.GetMaxValue<int>(p => p.IdLote, 0) + 1;
        }
    }
}
