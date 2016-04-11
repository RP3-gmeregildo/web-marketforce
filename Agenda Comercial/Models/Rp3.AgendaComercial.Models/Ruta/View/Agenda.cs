using Rp3.Data.Models.General;
using Rp3.Web.Mvc.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rp3.AgendaComercial.Models.Ruta.View
{
    public class AgendaEdit
    {
        public long IdAgenda { get; set; }
        public int IdRuta { get; set; }
        public int IdClienteDireccion { get; set; }
        public long FechaInicioTicks { get; set; }        
        public DateTime FechaInicio
        {
            get
            {
                return new DateTime(FechaInicioTicks);
            }
            set
            {
                this.FechaInicioTicks = value.Ticks;
            }
        }        
        public long FechaFinTicks { get; set; }
        public DateTime FechaFin
        {
            get
            {
                return new DateTime(FechaFinTicks);
            }
            set
            {
                this.FechaFinTicks = value.Ticks;
            }
        }

        public string DuracionVisita { get; set; }

        public int IdCliente { get; set; }
        public int? IdContacto { get; set; }
        public string TareasSeleccion { get; set; }
        public List<Models.General.Tarea> Tareas { get; set; }
        public string Motivo { get; set; }
    }
    public class AgendaUpdates
    {
        public int IdRuta { get; set; }
        public long IdAgenda { get; set; }
        public string Cliente_Nombre1 { get; set; }
        public string Cliente_Nombre2 { get; set; }
        public string Cliente_Apellido1 { get; set; }
        public string Cliente_Apellido2 { get; set; }
        public string Contacto_Nombre { get; set; }
        public string Contacto_Apellido { get; set; }
        public string ClienteContacto
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Contacto_Nombre))
                    return Models.General.Cliente.GetNombresCompletos(Cliente_Apellido1, Cliente_Apellido2, Cliente_Nombre1, Cliente_Nombre2);//(Cliente_Nombre1 + " " + Cliente_Nombre2 + " " + Cliente_Apellido1 + " " + Cliente_Apellido2);
                else
                    return Models.General.Cliente.GetNombresCompletos(Contacto_Apellido, String.Empty, Contacto_Nombre, String.Empty); //(Contacto_Nombre + " " + Contacto_Apellido);
            }
        }
        public string Direccion { get; set; }
        public long FechaInicioTicks { get; set; }
        public DateTime FechaInicio
        {
            get
            {
                return new DateTime(FechaInicioTicks);
            }
            set
            {
                this.FechaInicioTicks = value.Ticks;
            }
        }
        public long FechaFinTicks { get; set; }
        public DateTime FechaFin
        {
            get
            {
                return new DateTime(FechaFinTicks);
            }
            set
            {
                this.FechaFinTicks = value.Ticks;
            }
        }

        public string DuracionVisita { get; set; }

        public int IdCliente { get; set; }
        public int? IdContacto { get; set; } 
        public string TareasSeleccion { get; set; }
        public List<ListaTarea> Tarea { get; set; }
        public List<Models.General.Tarea> Tareas { get; set; }
        public string Motivo { get; set; }
        public bool EsReprogramada { get; set; }
    }  
}
