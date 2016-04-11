using Rp3.AgendaComercial.Models.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rp3.AgendaComercial.Models.Ruta.View
{
    /*public class LoteZona
    {
        public int IdZona { get; set; }
        public int Descripcion { get; set; }
    }
    public class LoteTipoCliente
    {
        public int IdTipoCliente { get; set; }
        public int Descripcion { get; set; }
    }
    public class LoteCanal
    {
        public int IdCanal { get; set; }
        public int Descripcion { get; set; }
    }*/
    public class LoteParam
    {
        public int? IdLote { get; set; }
        public string Estado { get; set; }
        public string Zona { get; set; }
        public string TipoCliente { get; set; }
        public string Canal { get; set; }
        public int Calificacion { get; set; } 
        public int Pagina { get; set; }
        public int NumReg { get; set; }
        public int isbegin { get; set; }

        public List<LoteView> ListLote { get; set; }
    }
    public class LoteView : Rp3.Data.Entity.EntityBase, IUbicacion
    {
        public int IdCliente { get; set; }
        public string Nombre1 { get; set; }
        public string Nombre2 { get; set; }
        public string Apellido1 { get; set; }
        public string Apellido2 { get; set; }
        public string Cliente
        {
            get
            {
                return (Nombre1 + " " + Nombre2 + " " + Apellido1 + " " + Apellido2);                
            }
        }
        public string Direccion { get; set; }
        public string Identificacion { get; set; }
        public string Correo { get; set; }
        public string TipoCliente { get; set; }
        public string Canal { get; set; }
        public int Calificacion { get; set; }

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
