using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rp3.Data.Models.General;
using Rp3.Web.Mvc;
using Rp3.Web.Mvc.Utility;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rp3.AgendaComercial.Models.General
{
    [Table("tbClienteContacto", Schema = "gen")]
    public class ClienteContacto : Rp3.Data.Entity.EntityBase
    {
        public override string Key
        {
            get
            {
                return string.Format("{0}-{1}", this.IdCliente, this.IdClienteContacto);
            }
        }

        [Key]
        [Column(Order = 0)]

        public int IdCliente { get; set; }

        [Key]
        [Column(Order = 1)]
        public int IdClienteContacto { get; set; }

        //[Required(ErrorMessageResourceType = typeof(Rp3.AgendaComercial.Resources.ErrorMessageValidation), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Direccion")]
        public int? IdClienteDireccion { get; set; }

        [Required(ErrorMessageResourceType = typeof(Rp3.AgendaComercial.Resources.ErrorMessageValidation), ErrorMessageResourceName = "Required")]
        [StringLength(100, MinimumLength = 1, ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "MaxMinStringLength")]
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Nombres")]
        public string Nombre { get; set; }

        [StringLength(100, MinimumLength = 0, ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "MaxMinStringLength")]
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Apellidos")]
        public string Apellido { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Cliente")]
        public string NombresCompletos
        {
            get
            {
                return string.Format("{0}{1}", this.Apellido, " " + this.Nombre).Trim();
            }
        }

        [StringLength(50, MinimumLength = 0, ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "MaxMinStringLength")]
        [Email(ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "InvalidEmailFormat")]
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "CorreoElectronico")]
        public string CorreoElectronico { get; set; }


        [StringLength(20, MinimumLength = 0, ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "MaxMinStringLength")]
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Telefono")]
        public string Telefono1 { get; set; }

        [StringLength(20, MinimumLength = 0, ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "MaxMinStringLength")]
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Telefono")]
        public string Telefono2 { get; set; }

        [StringLength(100, MinimumLength = 0, ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "MaxMinStringLength")]
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Cargo")]
        public string Cargo { get; set; }

        public string Foto { get; set; }

        [NotMapped]
        public string FotoMin
        {
            get
            {
                if (string.IsNullOrEmpty(this.Foto))
                    return string.Empty;
                return Thumbnail.GetPictureMinFromOriginal(this.Foto);
            }
        }

        [NotMapped]
        public string FotoSmall
        {
            get
            {
                if (string.IsNullOrEmpty(this.Foto))
                    return string.Empty;
                return Thumbnail.GetPictureSmallFromOriginal(this.Foto);
            }
        }

        [NotMapped]
        public string FotoMedium
        {
            get
            {
                if (string.IsNullOrEmpty(this.Foto))
                    return string.Empty;
                return Thumbnail.GetPictureMediumFromOriginal(this.Foto);
            }
        }

        public short EstadoTabla { get; set; }
        public string Estado { get; set; }

        [ForeignKey("IdCliente")]
        public virtual Cliente Cliente { get; set; }

        [ForeignKey("IdCliente, IdClienteDireccion")]
        public virtual ClienteDireccion ClienteDireccion { get; set; }

        public void AsignarId()
        {
            ContextService service = new ContextService();
            this.IdClienteContacto = service.ClienteContactos.GetMaxValue<int>(p => p.IdClienteContacto, 0, p => p.IdCliente == this.IdCliente) + 1;
        }
    }
}
