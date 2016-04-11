using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Rp3.AgendaComercial.DataService.Data
{
    public class Cliente
    {
        [DataMember]
        public int IdCliente { get; set; }
        [DataMember]
        public string Nombre1 { get; set; }
        [DataMember]
        public string Nombre2 { get; set; }
        [DataMember]
        public string Apellido1 { get; set; }
        [DataMember]
        public string Apellido2 { get; set; }
        [DataMember]
        public string NombresCompletos { get; set; }
        [DataMember]
        public string CorreoElectronico { get; set; }
        [DataMember]
        public int Calificacion { get; set; }

        [DataMember]
        public int? IdTipoCliente { get; set; }
        [DataMember]
        public string TipoCliente { get; set; }
        [DataMember]
        public int? IdCanal { get; set; }
        [DataMember]
        public string Canal { get; set; }        

        [DataMember]
        public int? IdCiudad { get; set; }
        [DataMember]
        public string Ciudad { get; set; }
        [DataMember]
        public string Telefono1 { get; set; }
        [DataMember]
        public string Telefono2 { get; set; }
        [DataMember]
        public string Referencia { get; set; }
        [DataMember]
        public string Direccion { get; set; }
        [DataMember]
        public double? Latitud { get; set; }
        [DataMember]
        public double? Longitud { get; set; }
    }
}