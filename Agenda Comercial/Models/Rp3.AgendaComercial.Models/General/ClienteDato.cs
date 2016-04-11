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
    [Table("tbClienteDato", Schema = "gen")]
    public class ClienteDato : Rp3.Data.Entity.EntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdCliente { get; set; }        
        public short? GeneroTabla { get; set; }
        
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Genero")]
        public string Genero { get; set; }
       
        public short? EstadoCivilTabla { get; set; }
        
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "EstadoCivil")]
        public string EstadoCivil { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "FechaNacimiento")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? FechaNacimiento { get; set; }

        [NotMapped]
        public long? FechaNacimientoTicks
        {
            get
            {
                if (FechaNacimiento.HasValue)
                    return FechaNacimiento.Value.Ticks;
                return 0;
            }
            set
            {
                if (value == 0 || value == null)
                    FechaNacimiento = null;
                else
                    FechaNacimiento = new DateTime(value.Value);
            }
        }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "ActividadEconomica")]
        [StringLength(100, MinimumLength = 0, ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "MaxMinStringLength")]
        public string ActividadEconomica { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "PaginaWeb")]
        [StringLength(100, MinimumLength = 0, ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "MaxMinStringLength")]
        public string PaginaWeb { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "InicioActividad")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? InicioActividad { get; set; }

        [NotMapped]
        public long? InicioActividadTicks
        {
            get
            {
                if (InicioActividad.HasValue)
                    return InicioActividad.Value.Ticks;
                return 0;
            }
            set
            {
                if (!value.HasValue)
                    InicioActividad = null;
                else
                    InicioActividad = new DateTime(value.Value);
            }
        }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "RepresentanteLegal")]
        [StringLength(100, MinimumLength = 0, ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "MaxMinStringLength")]
        public string RepresentanteLegal { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "RepresentateLegalIdentificacion")]
        [StringLength(100, MinimumLength = 0, ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "MaxMinStringLength")]
        public string RepresentateLegalIdentificacion { get; set; }
        [ForeignKey("GeneroTabla,Genero")]
        public virtual GeneralValues GeneroGeneralValue { get; set; }
        [ForeignKey("EstadoCivilTabla,EstadoCivil")]
        public virtual GeneralValues EstadoCivilGeneralValue { get; set; }

        public virtual Cliente Cliente { get; set; }
    }
}
