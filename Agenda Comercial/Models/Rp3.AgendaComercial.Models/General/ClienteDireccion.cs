using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rp3.Data.Models.General;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rp3.AgendaComercial.Models.General
{
    [Table("tbClienteDireccion", Schema = "gen")]
    public class ClienteDireccion : Rp3.Data.Entity.EntityBase, IUbicacion
    {
        public override string Key
        {
            get
            {
                return string.Format("{0}-{1}", this.IdCliente, this.IdClienteDireccion);
            }
        }

        public string KeyCombo
        {
            get
            {
                if (this.IdClienteDireccion == 0) return " ";
                return this.IdClienteDireccion.ToString();
            }
        }

        public override string DescriptionName
        {
            get
            {
                return this.Etiqueta;
            }
        }

        [Key]
        [Column(Order = 0)]
        public int IdCliente { get; set; }

        [Key]
        [Column(Order = 1)]
        public int IdClienteDireccion { get; set; }

        [Required(ErrorMessageResourceType = typeof(Rp3.AgendaComercial.Resources.ErrorMessageValidation), ErrorMessageResourceName = "Required")]
        [StringLength(250, MinimumLength = 1, ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "MaxMinStringLength")]
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Direccion")]
        public string Direccion { get; set; }      
        public short TipoDireccionTabla { get; set; }       
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "TipoDireccion")]
        public string TipoDireccion { get; set; }

        //[Required(ErrorMessageResourceType = typeof(Rp3.AgendaComercial.Resources.ErrorMessageValidation), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Ciudad")]
        public int? IdCiudad { get; set; }

        [StringLength(20, MinimumLength = 0, ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "MaxMinStringLength")]
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Telefono")]
        public string Telefono1 { get; set; }

        [StringLength(20, MinimumLength = 0, ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "MaxMinStringLength")]
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Telefono")]
        public string Telefono2 { get; set; }

        [StringLength(250, MinimumLength = 0, ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "MaxMinStringLength")]
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Referencia")]
        public string Referencia { get; set; }

        [StringLength(100, MinimumLength = 0, ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "MaxMinStringLength")]
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Descripcion")]
        public string Descripcion { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Latitud")]
        public double? Latitud { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Longitud")]
        public double? Longitud { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "EsPrincipal")]
        public bool EsPrincipal { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "AplicaRuta")]
        public bool AplicaRuta { get; set; }        
        public short EstadoTabla { get; set; }        
        public string Estado { get; set; }
        public string CiudadDescripcion { get; set; }

        [NotMapped]
        public string EtiquetaCliente
        {
            get
            {
                return String.Format("{0} - {1}", this.Cliente.NombresCompletos.Trim(), this.Etiqueta);
            }
        }

        [NotMapped]
        public string Etiqueta 
        { 
            get 
            {
                if (!String.IsNullOrEmpty(Descripcion))
                {
                    string etiqueta = Descripcion;

                    if (Ciudad != null)
                        etiqueta = String.Format("{0}, {1}, {2}", Ciudad.Name, etiqueta, Direccion);

                    return etiqueta;
                }
                else
                {
                    string etiqueta = Direccion;

                    if (!String.IsNullOrEmpty(Referencia))
                        etiqueta = String.Format("{0}, {1}", etiqueta, Referencia);

                    if (Ciudad != null)
                        etiqueta = String.Format("{0}, {1}", Ciudad.Name, etiqueta);

                    return etiqueta;
                }
            } 
        }
        [ForeignKey("EstadoTabla,Estado")]
        public virtual GeneralValues EstadoGeneralValue { get; set; }
        [ForeignKey("TipoDireccionTabla,TipoDireccion")]
        public virtual GeneralValues TipoDireccionGeneralValue { get; set; }

        [ForeignKey("IdCliente")]
        public virtual Cliente Cliente { get; set; }

        [ForeignKey("IdCiudad")]
        public virtual GeopoliticalStructure Ciudad { get; set; }

        public void AsignarId()
        {
            ContextService service = new ContextService();
            this.IdClienteDireccion = service.ClienteDirecciones.GetMaxValue<int>(p => p.IdClienteDireccion, 0, p => p.IdCliente == this.IdCliente) + 1;
        }

        #region IUbicacion

        [NotMapped]
        public int MarkerIndex { get; set; }
        [NotMapped]
        public int MarkerZIndex { get { return 1000 - MarkerIndex; } }
        [NotMapped]
        public int MarkerStart { get { if (MarkerIndex > 0) return 30 * (MarkerIndex - 1); else return 0; } }

        [NotMapped]
        public int IdUbicacion
        {
            get
            {
                return IdClienteDireccion;
            }
            set
            {
                IdClienteDireccion = value;
            }
        }

        [NotMapped]
        public string Titulo
        {
            get
            {
                return this.Direccion;
            }
            set
            {
                this.Direccion = value;
            }
        }

        #endregion IUbicacion
    }
}
