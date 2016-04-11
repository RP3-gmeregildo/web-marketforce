using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Rp3.AgendaComercial.DataService.Data
{
    [DataContract(Namespace = "http://schemas.rp3.com.ec/agendacomercial/")]
    public class GeopoliticalStructure
    {
        [DataMember]
        public int GeopoliticalStructureId { get; set; }

        [DataMember]
        public int GeopoliticalStructureTypeId { get; set; }

        [DataMember]
        public string IsoCode { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public double Latitude { get; set; }

        [DataMember]
        public double Longitude { get; set; }

        [DataMember]
        public int? ParentGeopoliticalStructureId { get; set; }
    }
}