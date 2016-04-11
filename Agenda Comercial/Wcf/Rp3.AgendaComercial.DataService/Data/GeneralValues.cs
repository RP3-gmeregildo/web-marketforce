using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Rp3.AgendaComercial.DataService.Data
{
    [DataContract(Namespace = "http://schemas.rp3.com.ec/agendacomercial/")]
    public class GeneralValues
    {
        [DataMember]
        public short Id { get; set; }

        [DataMember]
        public string Code { get; set; }

        [DataMember]
        public string Content { get; set; }

        [DataMember]
        public string Reference01 { get; set; }

        [DataMember]
        public string Reference02 { get; set; }
    }
}