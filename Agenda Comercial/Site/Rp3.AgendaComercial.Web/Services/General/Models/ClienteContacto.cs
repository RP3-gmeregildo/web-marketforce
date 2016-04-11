using Rp3.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rp3.AgendaComercial.Web.Services.General.Models
{
    public class ClienteContacto: IIdentifiable
    {
        public  string Key
        {
            get
            {
                return string.Format("{0}-{1}", this.IdCliente, this.IdClienteContacto);
            }
        }
        public string DescriptionName
        {
            get
            {
                return this.Nombre;
            }
        }
        public int IdCliente { get; set; }
        public int IdInterno { get; set; }
        public int IdClienteContacto { get; set; }
        public int? IdClienteDireccion { get; set; }

        public string Nombre { get; set; }

        public string Apellido { get; set; }

        public string CorreoElectronico { get; set; }

        public string Cargo { get; set; }

        public string Foto { get; set; }

        public string Estado { get; set; }
        public short? EstadoTabla { get; set; }

        public string Telefono1 { get; set; }
        public string Telefono2 { get; set; }
    }
}