using Rp3.AgendaComercial.Models.General;
using Rp3.Web.Mvc.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rp3.AgendaComercial.Models.Ruta.View
{
    /***********************************************/
    //CALENDARIO
    /***********************************************/
    public class Appointment
    {
        public string Cliente_Nombre1 { get; set; }
        public string Cliente_Nombre2 { get; set; }
        public string Cliente_Apellido1 { get; set; }
        public string Cliente_Apellido2 { get; set; }
        public string Contacto_Nombre { get; set; }
        public string Contacto_Apellido { get; set; }
        public string Cargo { get; set; }
        public string title
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Contacto_Nombre))
                    return Models.General.Cliente.GetNombresCompletos(Cliente_Apellido1, Cliente_Apellido2, Cliente_Nombre1, Cliente_Nombre2);//(Cliente_Nombre1 + " " + Cliente_Nombre2 + " " + Cliente_Apellido1 + " " + Cliente_Apellido2);
                else
                    return Models.General.Cliente.GetNombresCompletos(Contacto_Apellido, String.Empty, Contacto_Nombre, String.Empty) + " - " + Cargo; //(Contacto_Nombre + " " + Contacto_Apellido + " - " + Cargo);
            }
        }
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public long id { get; set; }
        public bool allDay { get; set; }
        public bool editable { get; set; }
        public string color { get; set; }
        public long ruta { get; set; }

    }

    /***********************************************/
    //LISTADO
    /***********************************************/
    public class AgendaListado : IUbicacion
    {
        public int idRuta { get; set; }
        public long idAgenda { get; set; }

        public int? IdAgente { get; set; }
        public string Agente { get; set; }

        public int? IdProgramacionRuta { get; set; }
        public string Cliente_Nombre1 { get; set; }
        public string Cliente_Nombre2 { get; set; }
        public string Cliente_Apellido1 { get; set; }
        public string Cliente_Apellido2 { get; set; }
        public string Contacto_Nombre { get; set; }
        public string Contacto_Apellido { get; set; }
        public string Cargo { get; set; }
        public string ClienteContacto
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Contacto_Nombre))
                    return Models.General.Cliente.GetNombresCompletos(Cliente_Apellido1, Cliente_Apellido2, Cliente_Nombre1, Cliente_Nombre2);
                else
                    return (Models.General.Cliente.GetNombresCompletos(Contacto_Apellido, String.Empty, Contacto_Nombre, String.Empty) + " - " + Cargo + " en " + Cliente_Nombre1 + " " + Cliente_Nombre2 + " " + Cliente_Apellido1 + " " + Cliente_Apellido2);
            }
        }

        public string ClienteContactoShort
        {
            get
            {
                string text = Models.General.Cliente.GetNombresCompletos(Cliente_Apellido1, Cliente_Apellido2, Cliente_Nombre1, Cliente_Nombre2);

                if (text.Trim().Length > 30)
                    return String.Format("{0}...", text.Trim().Substring(0, 25));
                else
                    return text.Trim();
            }
        }

        public string DireccionShort
        {
            get
            {
                if (Direccion.Trim().Length > 40)
                    return String.Format("{0}...", Direccion.Trim().Substring(0, 40));
                else
                    return Direccion.Trim();

            }
        }

        public DateTime fechaInicio { get; set; }
        public DateTime fechaFin { get; set; }

        public DateTime? fechaInicioGestion { get; set; }
        public DateTime? fechaFinGestion { get; set; }

        public string Color { get; set; }
        public string EstadoAgenda { get; set; }
        public string Direccion { get; set; }

        public string Telefono { get; set; }
        public string CorreoElectronico { get; set; }

        public int TotalPages { get; set; }
        public int TotalRows { get; set; }
        public int CurrentPage { get; set; }
        public string Path { get; set; }
        public string Origen { get; set; }
        public string Motivo { get; set; }
        public string Observacion { get; set; }
        public int Secuencia { get; set; }
        public List<string> Fotos { get; set; }
        public string UsrIng { get; set; }

        public void Config()
        {
            if (this.Tareas == null) return;
            int index = 1;
            this.Tareas.ForEach(p => p.Orden = index++);
            this.SetTareaDisplay();
        }

        public List<AgendaComercial.Models.Ruta.AgendaTarea> Tareas { get; set; }

        private void SetTareaDisplay()
        {
            if (this.Tareas == null || this.Tareas.Count == 0) _TareaDisplay = string.Empty;
            string data = string.Empty;
            this.Tareas.ForEach(p => p.SetActividadDetalle());
            this.Tareas.ForEach(p => data = data + string.Format("{2} {0}:\r\n{1}\r\n", p.Tarea.Descripcion.Trim(), p.ActividadesDetalle, p.Orden));
            if (data.Length > 0) _TareaDisplay = data.Substring(0, data.Length - 2);
        }

        private string _TareaDisplay;

        public string TareasDisplay { get { return _TareaDisplay; } }

        public string Estado { get; set; }

        public string Foto1
        {
            get
            {
                if (this.Fotos != null && this.Fotos.Count > 0) return Thumbnail.GetPictureMediumFromOriginal(this.Fotos[0]);
                return string.Empty;
            }
        }

        public string Foto2
        {
            get
            {
                if (this.Fotos != null && this.Fotos.Count > 1) return Thumbnail.GetPictureMediumFromOriginal(this.Fotos[1]);
                return string.Empty;
            }
        }

        public string Foto3
        {
            get
            {
                if (this.Fotos != null && this.Fotos.Count > 2) return Thumbnail.GetPictureMediumFromOriginal(this.Fotos[2]);
                return string.Empty;
            }
        }

        public DateTime? DuracionVisita 
        { 
            get 
            {
                if (this.fechaInicioGestion == null | this.fechaInicioGestion == null) return null;
                return new DateTime?(new DateTime(this.fechaFinGestion.Value.Subtract(this.fechaInicioGestion.Value).Ticks)); 
            } 
        }

        public string PlanificadoDisplay { get { return string.Format("{0:dd/MM/yyyy HH:mm} - {1:HH:mm}", this.fechaInicio, this.fechaFin); } }
        public string EjecutadoDisplay 
        { 
            get 
            {
                if (this.fechaInicioGestion == null | this.fechaInicioGestion == null) return string.Empty;
                return string.Format("{0:dd/MM/yyyy HH:mm} - {1:HH:mm}", this.fechaInicioGestion.Value, this.fechaFinGestion.Value); 
            } 
        }

        public string OrigenImageUrl {
            get {
                string url = String.Empty;

                switch (Origen)
                {
                    case Rp3.AgendaComercial.Models.Constantes.OrigenAgenda.Automatica:
                        url = "agenda-auto.png";
                        break;
                    case Rp3.AgendaComercial.Models.Constantes.OrigenAgenda.Web:
                        url = "agenda-web.png";
                        break;
                    case Rp3.AgendaComercial.Models.Constantes.OrigenAgenda.Movil:
                        url = "agenda-movil.png";
                        break;
                }

                return url;
            }
        } 

        public string PathMed
        {
            get
            {
                if (string.IsNullOrEmpty(this.Path))
                    return string.Empty;
                return Thumbnail.GetPictureMediumFromOriginal(this.Path);
            }
        }

        public string PathSmall
        {
            get
            {
                if (string.IsNullOrEmpty(this.Path))
                    return string.Empty;
                return Thumbnail.GetPictureSmallFromOriginal(this.Path);
            }
        }

        public string PathMin
        {
            get
            {
                if (string.IsNullOrEmpty(this.Path))
                    return string.Empty;
                return Thumbnail.GetPictureMinFromOriginal(this.Path);
            }
        }

        public string Leyenda
        {
            get
            {
                return String.Format("{0} - {1} - {2:dd/MM/yyyy HH:mm} - {3:HH:mm} - {4}", this.ClienteContacto, this.Direccion, this.fechaInicio, this.fechaFin, this.EstadoAgenda);
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
                return Convert.ToInt32(idAgenda);
            }
            set
            {
                idAgenda = value;
            }
        }

        public string Titulo
        {
            get
            {
                return String.Format("{0}", this.ClienteContacto);
            }
        }
        #endregion IUbicacion
    }

    public class AgendaCategoria
    {
        public string Nombre { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public bool isBusqueda { get; set; }
        public List<AgendaListado> Agenda { get; set; }
    }

    /***********************************************/
    //VER DATOS (MODAL)
    /***********************************************/
    public class ListaTarea
    {
        public long idTarea { get; set; }
        public string Nombre { get; set; }
        public string Estado { get; set; }

        public string EstadoDescripcion { get; set; }

        public int Secuencia { get; set; }

        public List<Rp3.AgendaComercial.Models.Ruta.AgendaTareaActividad> Actividades { get; set; }

        public string ActividadesDetalle
        {
            get
            {
                if (this.Actividades == null || this.Actividades.Count == 0) return "Ninguna";
                string data = string.Empty;
                if (this.Estado == Rp3.AgendaComercial.Models.Constantes.EstadoTarea.Realizada)
                    this.Actividades.OrderBy(p => p.Orden).ToList().ForEach(p => data += string.Format("{2}.{3} {0}: {1}\r\n", p.Descripcion, p.Resultado, this.Secuencia, p.Orden));
                else
                    this.Actividades.OrderBy(p => p.Orden).ToList().ForEach(p => data += string.Format("{1}.{2} {0}\r\n", p.Descripcion, this.Secuencia, p.Orden));
                if (data.Length > 2) return data.Substring(0, data.Length - 2);
                return data;
            }
        }
    }
    public class ListaIMG
    {
        public string URL { get; set; }
        public string URLMin
        {
            get
            {
                if (string.IsNullOrEmpty(this.URL))
                    return string.Empty;
                return Thumbnail.GetPictureMinFromOriginal(this.URL);
            }
        }
    }
    public class AgendaClientes
    {
        public int idRuta { get; set; }
        public long idAgenda { get; set; }

        public int? IdAgente { get; set; }
        public string Agente { get; set; }

        public long? IdProgramacionRuta { get; set; }
        public string Foto { get; set; }
        public string FotoContacto { get; set; }
        public string Cliente_Nombre1 { get; set; }
        public string Cliente_Nombre2 { get; set; }
        public string Cliente_Apellido1 { get; set; }
        public string Cliente_Apellido2 { get; set; }
        public string Contacto_Nombre { get; set; }
        public string Contacto_Apellido { get; set; }
        public string ClienteContacto  { 
            get {
                if (string.IsNullOrWhiteSpace(Contacto_Nombre))
                    return Models.General.Cliente.GetNombresCompletos(Cliente_Apellido1, Cliente_Apellido2, Cliente_Nombre1, Cliente_Nombre2);
                else
                    return Models.General.Cliente.GetNombresCompletos(Contacto_Apellido, String.Empty, Contacto_Nombre, String.Empty);//(Contacto_Nombre + " " + Contacto_Apellido);
                }
        }
        public string CargoCanal
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Contacto_Nombre))
                    return (Canal);
                else
                    return (Cargo + " en " + Models.General.Cliente.GetNombresCompletos(Cliente_Apellido1, Cliente_Apellido2, Cliente_Nombre1, Cliente_Nombre2));
            }
        }
        public string Cargo { get; set; }
        public string Canal { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public string Direccion { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }

        public string FechaInicioDisplay
        {
            get
            {
                if (this.FechaInicio != null && this.FechaFin != null) return string.Format("{0:dd/MM/yyyy HH:mm} - {1:HH:mm}", this.FechaInicio.Value, this.FechaFin.Value);
                return string.Empty;
            }
        }

        public DateTime? FechaInicioGestion { get; set; }
        public DateTime? FechaFinGestion { get; set; }



        public DateTime? FechaInicioOriginal { get; set; }
        public double? Latitud { get; set; }
        public double? Longitud { get; set; }
        public List<ListaTarea> Tarea { get; set; }
        public List<ListaIMG> Imagen { get; set; }
        public string Observacion { get; set; }

        public string FotoMin
        {
            get
            {
                if (string.IsNullOrEmpty(this.Foto))
                    return string.Empty;
                return Thumbnail.GetPictureMinFromOriginal(this.Foto);
            }
        }
        public string FotoContactoMin
        {
            get
            {
                if (string.IsNullOrEmpty(this.FotoContacto))
                    return string.Empty;
                return Thumbnail.GetPictureMinFromOriginal(this.FotoContacto);
            }
        }
        public string Color { get; set; }
        public string ColorDetalle { get; set; }
        public string EstadoAgenda { get; set; }
        public string MotivoNoGestion { get; set; }
        public string MotivoReprogramacion { get; set; }
        public bool EsReprogramada { get; set; }
        public string Origen { get; set; }
        public string OrigenImageUrl
        {
            get
            {
                string url = String.Empty;

                switch (Origen)
                {
                    case Rp3.AgendaComercial.Models.Constantes.OrigenAgenda.Automatica:
                        url = "agenda-auto.png";
                        break;
                    case Rp3.AgendaComercial.Models.Constantes.OrigenAgenda.Web:
                        url = "agenda-web.png";
                        break;
                    case Rp3.AgendaComercial.Models.Constantes.OrigenAgenda.Movil:
                        url = "agenda-movil.png";
                        break;
                }

                return url;
            }
        }
 
        public void SetSecuencia()
        {
            int index = 1;
            if (this.Tarea != null) this.Tarea.ForEach(p => p.Secuencia = index++);
        }

        public string Foto01 { get {  return  this.Imagen.Count > 0 ? this.Imagen[0].URLMin : string.Empty;  } }
        public string Foto02 { get { return this.Imagen.Count > 1 ? this.Imagen[1].URLMin : string.Empty; } }
        public string Foto03 { get { return this.Imagen.Count > 2 ? this.Imagen[2].URLMin : string.Empty; } }
        public string Foto04 { get { return this.Imagen.Count > 3 ? this.Imagen[3].URLMin : string.Empty; } }
        public string Foto05 { get { return this.Imagen.Count > 4 ? this.Imagen[4].URLMin : string.Empty; } }
        public string Foto06 { get { return this.Imagen.Count > 5 ? this.Imagen[5].URLMin : string.Empty; } }

        public string EstadoDescripcion
        {
            get 
            {
                switch (this.EstadoAgenda)
                {
                    case Rp3.AgendaComercial.Models.Constantes.EstadoAgenda.Cancelada: return string.Format("Cancelada ({0})", this.MotivoNoGestion);
                    case Rp3.AgendaComercial.Models.Constantes.EstadoAgenda.Eliminado: return string.Format("Eliminada ({0})", this.MotivoNoGestion);;
                    case Rp3.AgendaComercial.Models.Constantes.EstadoAgenda.Gestionada:
                    case Rp3.AgendaComercial.Models.Constantes.EstadoAgenda.Visitada: 
                        return string.Format("{0} ({1:HH:mm} - {2:HH:mm})", this.ColorDetalle, this.FechaInicioGestion.Value, this.FechaFinGestion.Value);
                    case Rp3.AgendaComercial.Models.Constantes.EstadoAgenda.NoVisitado: return string.Format("No Visita ({0})", this.MotivoNoGestion); ;
                    case Rp3.AgendaComercial.Models.Constantes.EstadoAgenda.Pendiente: return "Pendiente";
                    case Rp3.AgendaComercial.Models.Constantes.EstadoAgenda.Reprogramada: return string.Format("Reprogramda ({0})", this.MotivoNoGestion);;
                    case Rp3.AgendaComercial.Models.Constantes.EstadoAgenda.SinProgramar: return string.Format("Sin Programar ({0})", this.MotivoNoGestion); ;

                }
                return this.EstadoAgenda == Rp3.AgendaComercial.Models.Constantes.EstadoAgenda.Visitada ? string.Format("{0} ({1:HH:mm} - {2:HH:mm})", this.ColorDetalle, this.FechaInicioGestion.Value, this.FechaFinGestion.Value) : string.Empty; 
            }
        }

        public bool EnRojo
        {
            get
            {
                return this.EstadoAgenda == Rp3.AgendaComercial.Models.Constantes.EstadoAgenda.Cancelada |
                       this.EstadoAgenda == Rp3.AgendaComercial.Models.Constantes.EstadoAgenda.Eliminado |
                       this.EstadoAgenda == Rp3.AgendaComercial.Models.Constantes.EstadoAgenda.NoVisitado |
                       this.EstadoAgenda == Rp3.AgendaComercial.Models.Constantes.EstadoAgenda.SinProgramar;
            }
        }

        public bool EnCeleste
        {
            get
            {
                return this.EstadoAgenda == Rp3.AgendaComercial.Models.Constantes.EstadoAgenda.Reprogramada;
            }
        }

        public bool EnVerde
        {
            get 
            {
                return this.EstadoAgenda == Rp3.AgendaComercial.Models.Constantes.EstadoAgenda.Gestionada |
                       this.EstadoAgenda == Rp3.AgendaComercial.Models.Constantes.EstadoAgenda.Visitada;
            }
        }

        public bool EnAzul
        {
            get 
            {
                return this.EstadoAgenda == Rp3.AgendaComercial.Models.Constantes.EstadoAgenda.Pendiente;
            }
        }
    }

    public class AgendaTarea
    {
        public long IdTarea { get; set; }
        public string Nombre { get; set; }

        public List<AgendaTareaActividad> Actividades { get; set; }
    }

    public class AgendaTareaActividad
    {
        public int IdRuta { get; set; }
        public long IdAgenda { get; set; }
        public long IdTarea { get; set; }
        public string Pregunta { get; set; }
        public string Respuesta { get; set; }
        public int? IdTareaActividadPadre { get; set; }
        public long IdTareaActividad { get; set; }

        public string IdTipoTarea { get; set; }

        public string RespuestaWidth
        {
            get
            {
                switch(this.IdTipoTarea)
                {
                    case Constantes.TipoTarea.CheckListOportunidad:
                    case Constantes.TipoTarea.Revision:
                    case Constantes.TipoTarea.Encuesta:
                        return "25%";
                    default:
                        return "50%";
                }
            }
        }
    }
    public class ModalParam
    {
        public int idRuta { get; set; }
        public long idAgenda { get; set; }
    }

    /***********************************************/
    //CREAR & EDITAR (MODAL)
    /***********************************************/

    public class ComboCliente
    {
        public int idCliente { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Cliente
        {
            get
            {
                return Models.General.Cliente.GetNombresCompletos(Apellido, String.Empty, Nombre, String.Empty);
            }
        }
    }
    public class ComboContacto
    {
        public int idCliente { get; set; }
        public int idClienteContacto { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public int? idClienteDireccion { get; set; }
    }
    public class ComboDireccion
    {
        public int idCliente { get; set; }
        public int idClienteDireccion { get; set; }
        public string Direccion { get; set; }
    }
    public class ComboTarea
    {
        public long idTarea { get; set; }
        public string Descripcion { get; set; }
    }
    public class ComboGroup
    {
        public List<ComboCliente> ComboClientes { get; set; }
        public List<ComboContacto> ComboContactos { get; set; }
        public List<ComboDireccion> ComboDirecciones { get; set; }
        public List<ComboTarea> ComboTareas { get; set; }
    }
}
