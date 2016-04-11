using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rp3.AgendaComercial.Web.Services.Oportunidad.Models
{
    public class OportunidadMedia
    {
        public int IdOportunidad { get; set; }
        public short IdMedia { get; set; }
        public string Nombre { get; set; }
        public string Contenido { get; set; }
        public string Path { get; set; }
    }

    public class OportunidadContactoMedia
    {
        public int IdOportunidad { get; set; }
        public int IdOportunidadContacto { get; set; }
        public string Nombre { get; set; }
        public string Contenido { get; set; }
        public string Path { get; set; }
    }
}