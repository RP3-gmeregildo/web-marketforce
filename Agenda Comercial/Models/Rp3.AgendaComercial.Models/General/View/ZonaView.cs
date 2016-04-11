using Rp3.Data.Models.General;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rp3.AgendaComercial.Models.General.View
{
    public class ZonaView
    {
        public int TypeId { get; set; }
        public bool EsUltimo { get; set; }
        public Object lista { get; set; }
        public string label { get; set; }
        public string ParentName { get; set; }
        public int? Id { get; set; }
    }

    public class ZonaModel
    {
        [Required(ErrorMessageResourceType = typeof(Rp3.AgendaComercial.Resources.ErrorMessageValidation), ErrorMessageResourceName = "Required")]
        [StringLength(250, MinimumLength = 1, ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "MaxMinStringLength")]
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Zona")]
        public string Name { get; set; }
        public int Id { get; set; }
        public List<ZonaGroup> Children { get; set; }

        public List<ZonaGeocerca> ZonaGeocercas { get; set; }

        public List<ZonaClienteGeocerca> ZonaClienteGeocercas { get; set; }
        public List<Zona> ZonaOther { get; set; }

        public string Estado { get; set; }
        public int IdRegion { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "ZonaTiempoMovilizacion")]
        public string Movilizacion { get; set; }
        public short MovilizacionTabla { get; set; }
        public short EstadoTabla { get; set; }

        public string Tipo { get; set; }
        public short TipoTabla { get; set; }

        public GeneralValues TipoGeneralValue { get; set; }

        public Ubicacion ubicacion { get; set; }
        public GeneralValues EstadoGeneralValue { get; set; }
        public GeneralValues MovilizacionGeneralValue { get; set; }
        public Region Region { get; set; }

    }

    public class ZonaGroup
    {
        [DisplayName("IdParent")]
        public int? IdParent { get; set; }
        [DisplayName("ParentsName")]
        public string ParentsName { get; set; }
        [DisplayName("Lista")]
        public List<int> Lista { get; set; }
    }
}
