using Rp3.AgendaComercial.Models.General;
using Rp3.Web.Mvc.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rp3.AgendaComercial.Models.Ruta.View
{
    public class RutaDetalleGV : Rp3.Data.Entity.EntityBase, IUbicacion
    {
        public int IdCliente { get; set; }
        public int IdClienteDireccion { get; set; }
        public string Cliente_Nombre1 { get; set; }
        public string Cliente_Nombre2 { get; set; }
        public string Cliente_Apellido1 { get; set; }
        public string Cliente_Apellido2 { get; set; }
        public string Cliente
        {
            get
            {
                return (Cliente_Nombre1 + " " + Cliente_Nombre2 + " " + Cliente_Apellido1 + " " + Cliente_Apellido2);
            }
        }
        public string Direccion { get; set; }
        public string Referencia { get; set; }
        public string TipoCliente { get; set; }
        public string Canal { get; set; }
        public string DireccionFull
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Referencia))
                    return (Direccion);
                else
                    return (Direccion + ", " + Referencia);
            }
        }

        #region IUbicacion

        public double? Latitud { get; set; }
        public double? Longitud { get; set; }
        public int MarkerIndex { get; set; }
        public int MarkerZIndex { get { return 1000 - MarkerIndex; } }
        public int MarkerStart { get { if (MarkerIndex > 0) return 30 * (MarkerIndex - 1); else return 0; } }
        public int IdUbicacion
        {
            get
            {
                return IdCliente;
            }
            set
            {
                IdCliente = value;
            }
        }
        public string Titulo
        {
            get
            {
                if (Cliente != null)
                    return Cliente;

                return String.Empty;
            }
        }

        #endregion IUbicacion   
        public int TotalPages { get; set; }
        public int TotalRows { get; set; }
        public int CurrentPage { get; set; }
    }
}
