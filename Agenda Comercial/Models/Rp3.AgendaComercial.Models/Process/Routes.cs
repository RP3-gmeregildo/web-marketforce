using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Rp3.AgendaComercial.Process.Clases
{
    [DataContract]
    public class GoogleAddress
    {
        [DataMember]
        public List<Address> results { get; set; }
    }

    [DataContract(Name = "results")]
    public class Address
    {
        [DataMember]
        public string formatted_address { get; set; }
    }

    [DataContract]
    public class GoogleData
    {
        [DataMember]
        public List<Routes> routes { get; set; }
    }

    [DataContract(Name = "routes")]
    public class Routes
    {
        [DataMember]
        public List<Legs> legs { get; set; }
    }

    [DataContract]
    public class Legs
    {
        [DataMember]
        public Distance distance { get; set; }
        [DataMember]
        public Duration duration { get; set; }
    }

    [DataContract]
    public class Distance
    {
        [DataMember]
        public string text { get; set; }
        [DataMember]
        public int value { get; set; }
    }

    [DataContract]
    public class Duration
    {
        [DataMember]
        public string text { get; set; }
        [DataMember]
        public int value { get; set; }
    }
}
