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
    [Table("tbCliente", Schema = "gen")]
    public class Cliente : Rp3.Data.Entity.EntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdCliente { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "TipoIdentificacion")]
        public short? IdTipoIdentificacion { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Identificacion")]
        [StringLength(50, MinimumLength = 0, ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "MaxMinStringLength")]
        public string Identificacion { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Cliente")]
        public string NombresCompletos
        {
            get
            {
                return GetNombresCompletos(this.Apellido1, this.Apellido2, this.Nombre1, this.Nombre2);
            }
        }

        public static string GetNombresCompletos(string Apellido1, string Apellido2, string Nombre1, string Nombre2)
        {
             return string.Format("{0}{1}{2}{3}", Apellido1, " " + Apellido2, " " + Nombre1, " " + Nombre2).Trim();
        }

        [Required(ErrorMessageResourceType = typeof(Rp3.AgendaComercial.Resources.ErrorMessageValidation), ErrorMessageResourceName = "Required")]
        [StringLength(100, MinimumLength = 1, ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "MaxMinStringLength")]
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Nombre1")]
        public string Nombre1 { get; set; }

        public string Foto { get; set; }

        [StringLength(100, MinimumLength = 0, ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "MaxMinStringLength")]
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Nombre2")]
        public string Nombre2 { get; set; }

        [StringLength(100, MinimumLength = 0, ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "MaxMinStringLength")]
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Apellido1")]
        public string Apellido1 { get; set; }

        [StringLength(100, MinimumLength = 0, ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "MaxMinStringLength")]
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Apellido2")]
        public string Apellido2 { get; set; }

        [StringLength(100, MinimumLength = 0, ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "MaxMinStringLength")]
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "RazonSocial")]
        public string RazonSocial { get; set; }

        [StringLength(50, MinimumLength = 0, ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "MaxMinStringLength")]
        [Email(ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "InvalidEmailFormat")]
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "CorreoElectronico")]
        public string CorreoElectronico { get; set; }        
        public short? TipoPersonaTabla { get; set; }      
        public string TipoPersona { get; set; }

        public short EstadoTabla { get; set; }
        public string Estado { get; set; }   

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "TipoCliente")]
        public int? IdTipoCliente { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Canal")]
        public int? IdCanal { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Calificacion")]
        public int Calificacion { get; set; }

        public string UsrIng { get; set; }
        public DateTime FecIng { get; set; }
        public string UsrMod { get; set; }
        public DateTime? FecMod { get; set; }

        public DateTime? FechaUtimaVisita { get; set; }

        public int? IdAgenteUltimaVisita { get; set; }

        public int? IndiceActividad { get; set; }

        public int? IndiceActividad2 { get; set; }

        public int? TiempoInactividad { get; set; }

        public string TiempoInactividadText
        {
            get
            {
                if (TiempoInactividad != null)
                    return String.Format("{0} {1}", TiempoInactividad, Rp3.AgendaComercial.Resources.LabelFor.Dias);
                else
                    return null;
            }
        }

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

        [ForeignKey("EstadoTabla,Estado")]
        public virtual GeneralValues EstadoGeneralValue { get; set; }

        [ForeignKey("TipoPersonaTabla,TipoPersona")]
        public virtual GeneralValues TipoPersonaGeneralValue { get; set; }     

        [ForeignKey("IdTipoCliente")]
        public virtual TipoCliente TipoCliente { get; set; }

        [ForeignKey("IdCanal")]
        public virtual Canal Canal { get; set; }
        [ForeignKey("IdAgenteUltimaVisita")]
        public virtual Agente Agente { get; set; }

        [ForeignKey("IdTipoIdentificacion")]
        public virtual IdentificationType TipoIdentificacion { get; set; }

        public virtual List<ClienteDireccion> ClienteDirecciones { get; set; }

        public virtual List<ClienteContacto> ClienteContactos { get; set; }

        public ClienteDireccion ClienteDireccion 
        {
            get 
            {
                return ClienteDirecciones.Where(p => p.EsPrincipal).FirstOrDefault();
            }
        }

        [NotMapped]
        public bool ReadOnly { get; set; }

        public virtual ClienteDato ClienteDato { get; set; }


        [NotMapped]
        public string Direccion { get; set; }

        [NotMapped]
        public int? DireccionIdCiudad { get; set; }

        [NotMapped]
        public string DireccionReferencia { get; set; }

        [NotMapped]
        public string DireccionDescripcion { get; set; }

        [NotMapped]
        public string DireccionTelefono1 { get; set; }

        [NotMapped]
        public string DireccionTelefono2 { get; set; }

        [NotMapped]
        public float? DireccionLatitud { get; set; }

        [NotMapped]
        public float? DireccionLongitud { get; set; }

        [NotMapped]
        public int Secuencia { get; set; }

        public void AsignarId()
        {
            ContextService service = new ContextService();
            this.IdCliente = service.Clientes.GetMaxValue<int>(p => p.IdCliente, 0) + 1;
        }
    }
}
