using Rp3.AgendaComercial.Models.General;
using Rp3.Data.Models.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rp3.AgendaComercial.Models.Ruta.View
{
    public class ClienteProgramacion
    {
        public int IdCliente { get; set; }
        public int? GeopolitcalStructureId { get; set; }
        public int IdClienteDireccion { get; set; }
        public string NombreCliente { get; set; }
        public string Direccion { get; set; }
        public string GeopoliticalStructureName { get; set; }
        public string TipoCliente { get; set; }
        public string Canal { get; set; }
        public string Lote { get; set; }
        public IEnumerable<ProgramacionRuta> Programaciones { get; set; }

        public int TotalPages { get; set; }
        public int TotalRows { get; set; }
        public int CurrentPage { get; set; }
    }

    public class ProgramacionPreview : IAppointment, IUbicacion
    {
        public int? IdProgramacionRuta { get; set; }
        public int? IdRuta { get; set; }
        public int? IdCliente { get; set; }
        public int? IdClienteDireccion { get; set; }
        public string Patron { get; set; }
        public int? DiaMes { get; set; }
        public short? Semana { get; set; }
        public bool CualquierDia { get; set; }
        public bool Hoy { get; set; }
        public short? Frecuencia { get; set; }
        public string Descripcion { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaAgenda { get; set; }
        public int? Duracion { get; set; }

        public bool Partida { get; set; }


        public string Cliente_Nombre1 { get; set; }
        public string Cliente_Nombre2 { get; set; }
        public string Cliente_Apellido1 { get; set; }
        public string Cliente_Apellido2 { get; set; }
        public string Contacto_Nombre { get; set; }
        public string Contacto_Apellido { get; set; }
        public string Cargo { get; set; }

        public string Direccion { get; set; }
        public int? IdCiudad { get; set; }

        #region Calendar

        public string title
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Contacto_Nombre))
                    return (Cliente_Nombre1 + " " + Cliente_Nombre2 + " " + Cliente_Apellido1 + " " + Cliente_Apellido2);
                else
                    return (Contacto_Nombre + " " + Contacto_Apellido + " - " + Cargo);
            }
        }
        public DateTime start { get { return FechaAgenda.Value; } }
        public DateTime end { get {             
            return FechaAgenda.Value.AddMinutes(this.Duracion??60); 
            } 
        }
        public long id { get; set; }
        public bool allDay { get {return false;} }
        public bool editable { get { return false; } }
        public string color { get { return "#335AA8"; } }
        public long ruta { get; set; }

        #endregion Calendar

        #region Ubicacion

        public int IdUbicacion { get; set; }
        public double? Latitud { get; set; }
        public double? Longitud { get; set; }
        public string Titulo { get; set; }
        public int MarkerIndex { get; set; }
        public int MarkerZIndex { get { return 1000 - MarkerIndex; } }
        public int MarkerStart { get { if (MarkerIndex > 0) return 30 * (MarkerIndex - 1); else return 0; } }

        #endregion Ubicacion
    }
}
