using Rp3.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rp3.AgendaComercial.Web.Services.General.Models
{
    public class ClienteDireccion : IIdentifiable
    {
        public string Key
        {
            get
            {
                return string.Format("{0}-{1}", this.IdCliente, this.IdClienteDireccion);
            }
        }
        public string DescriptionName
        {
            get { return this.Direccion; }
        }

        public int IdInterno { get; set; }
        public int IdCliente { get; set; }
        public int IdClienteDireccion { get; set; }
        public string Direccion { get; set; }
        public short TipoDireccionTabla { get; set; }
        public string TipoDireccion { get; set; }
        public int? IdCiudad { get; set; }
        public string CiudadDescripcion { get; set; }
        public string Telefono1 { get; set; }
        public string Telefono2 { get; set; }
        public string Referencia { get; set; }
        public string Descripcion { get; set; }
        public double? Latitud { get; set; }
        public double? Longitud { get; set; }
        public bool EsPrincipal { get; set; }
        public bool AplicaRuta { get; set; }
        public short EstadoTabla { get; set; }
        public string Estado { get; set; }



    }
}