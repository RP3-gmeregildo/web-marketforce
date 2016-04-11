using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rp3.Data.Models.General;
using System.ComponentModel.DataAnnotations;
using Rp3.AgendaComercial.Models.General;
using System.ComponentModel.DataAnnotations.Schema;
using Rp3.Data;

namespace Rp3.AgendaComercial.Models.Ruta
{
    [Table("tbAgenda", Schema = "rut")]
    public class Agenda : Rp3.Data.Entity.EntityBase, IAppointment
    {
        [Key]
        [Column(Order = 0)]
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Ruta")]
        public int IdRuta { get; set; }

        [Key]
        [Column(Order = 1)]
        public long IdAgenda { get; set; }        

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Cliente")]
        public int IdCliente { get; set; }
        public int IdAgente { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Cliente")]
        public int IdClienteDireccion { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Contacto")]
        public int? IdClienteContacto { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Programacion")]
        public int? IdProgramacionRuta { get; set; }

        [Required(ErrorMessageResourceType = typeof(Rp3.AgendaComercial.Resources.ErrorMessageValidation), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "FechaInicio")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? FechaInicio { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "FechaFin")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? FechaFin { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "FechaInicioGestion")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? FechaInicioGestion { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "FechaFinGestion")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? FechaFinGestion { get; set; }

        public bool EsReprogramada { get; set; }
        
        public string MotivoNoGestion { get; set; }
        public short? MotivoNoGestionTabla { get; set; }

        public string MotivoReprogramacion { get; set; }
        public short? MotivoReprogramacionTabla { get; set; }

        public int? Duracion { get; set; }
        public int? TiempoViaje { get; set; }
        public int? DistanciaUbicacion { get; set; }

        public DateTime? FechaInicioOriginal { get; set; }
        public DateTime? FechaFinOriginal { get; set; }

        #region Ticks

        [NotMapped, Rp3.Data.NonSerializableToXml]
        public long FechaInicioTicks
        {
            get
            {
                if (FechaInicio.HasValue)
                    return FechaInicio.Value.Ticks;
                return 0;
            }
            set
            {
                if (value == 0)
                    FechaInicio = null;
                else
                    FechaInicio = new DateTime(value);
            }
        }

        [NotMapped, Rp3.Data.NonSerializableToXml]
        public long FechaFinTicks
        {
            get
            {
                if (FechaFin.HasValue)
                    return FechaFin.Value.Ticks;
                return 0;
            }
            set
            {
                if (value == 0)
                    FechaFin = null;
                else
                    FechaFin = new DateTime(value);
            }
        }

        [NotMapped, Rp3.Data.NonSerializableToXml]
        public long FechaInicioOriginalTicks
        {
            get
            {
                if (FechaInicioOriginal.HasValue)
                    return FechaInicioOriginal.Value.Ticks;
                return 0;
            }
            set
            {
                if (value == 0)
                    FechaInicioOriginal = null;
                else
                    FechaInicioOriginal = new DateTime(value);
            }
        }

        [NotMapped, Rp3.Data.NonSerializableToXml]
        public long FechaFinOriginalTicks
        {
            get
            {
                if (FechaFinOriginal.HasValue)
                    return FechaFinOriginal.Value.Ticks;
                return 0;
            }
            set
            {
                if (value == 0)
                    FechaFinOriginal = null;
                else
                    FechaFinOriginal = new DateTime(value);
            }
        }

        [NotMapped, Rp3.Data.NonSerializableToXml]
        public long FechaInicioGestionTicks
        {
            get
            {
                if (FechaInicioGestion.HasValue)
                    return FechaInicioGestion.Value.Ticks;
                return 0;
            }
            set
            {
                if (value == 0)
                    FechaInicioGestion = null;
                else
                    FechaInicioGestion = new DateTime(value);
            }
        }

        [NotMapped, Rp3.Data.NonSerializableToXml]
        public long FechaFinGestionTicks
        {
            get
            {
                if (FechaFinGestion.HasValue)
                    return FechaFinGestion.Value.Ticks;
                return 0;
            }
            set
            {
                if (value == 0)
                    FechaFinGestion = null;
                else
                    FechaFinGestion = new DateTime(value);
            }
        }

        #endregion Ticks

        [NotMapped, Rp3.Data.NonSerializableToXml]
        public string TareaIds
        {
            get
            {
                string idText = String.Empty;

                foreach (var tarea in this.AgendaTareas)
                    if (String.IsNullOrEmpty(idText))
                        idText = Convert.ToString(tarea.IdTarea);
                    else
                        idText = String.Format("{0}-{1}", idText, tarea.IdTarea);

                return idText;
            }

            set
            {
                if (this.AgendaTareas == null)
                    this.AgendaTareas = new List<AgendaTarea>();

                //this.AgendaTareas.Clear();

                string[] keyParts = value.Split('-');
                List<long> listIds = new List<long>();

                foreach (var id in keyParts)
                {
                    if (!String.IsNullOrEmpty(id))
                    {
                        var idTarea = Convert.ToInt64(id);
                        listIds.Add(idTarea);

                        if (this.AgendaTareas.Where(p => p.IdTarea == idTarea).Count() == 0)
                        {
                            var tarea = new AgendaTarea() 
                            { 
                                IdRuta = this.IdRuta, 
                                IdAgenda = this.IdAgenda, 
                                IdTarea = idTarea, 
                                EstadoTareaTabla = Constantes.EstadoTarea.Tabla,
                                EstadoTarea = Constantes.EstadoTarea.Pendiente//,
                                //AgendaTareaActividades = new List<AgendaTareaActividad>()
                            };
                            this.AgendaTareas.Add(tarea);
                        }
                    }
                }

                for (int i = this.AgendaTareas.Count - 1; i >= 0; i--)
                    if (!listIds.Contains(this.AgendaTareas[i].IdTarea))
                        this.AgendaTareas.Remove(this.AgendaTareas[i]);
            }
        }       

        [NotMapped]
        public string TareaDescripcion
        {
            get
            {
                String text = String.Empty;

                foreach (var tarea in this.AgendaTareas.OrderBy(p=>p.Tarea.Descripcion))
                {
                    if (tarea.Tarea != null)
                    {
                        if (String.IsNullOrEmpty(text))
                            text = tarea.Tarea.Descripcion;
                        else
                            text = String.Format("{0}, {1}", text, tarea.Tarea.Descripcion.Length <= 7 ? tarea.Tarea.Descripcion : tarea.Tarea.Descripcion.Substring(0, 7));
                    }
                }

                return text;
            }
        }

        [NotMapped, Rp3.Data.NonSerializableToXml]
        public string IdRecurso
        {
            get
            {
                return String.Format("{0}-{1}", this.IdCliente, this.IdClienteDireccion);
            }

            set
            {
                string[] keyParts = value.Split('-');

                if (keyParts.Count() == 2)
                {
                    IdCliente = Convert.ToInt32(keyParts[0]);
                    IdClienteDireccion = Convert.ToInt32(keyParts[1]);
                }
            }
        }

        private string _Ubicacion;
        [NotMapped, Rp3.Data.NonSerializableToXml]
        public string Ubicacion 
        { 
            get 
            {
                if (!string.IsNullOrEmpty(_Ubicacion)) return _Ubicacion;
                string text = String.Empty;

                if (this.ClienteDireccion != null) 
                    text =  this.ClienteDireccion.Cliente.NombresCompletos.Trim();

                if (this.ClienteContacto != null)
                    text = String.Format("{0} [{1}]", text, this.ClienteContacto.NombresCompletos);

                return text; 
            }
            set { _Ubicacion = value; }
        }

        [NotMapped, Rp3.Data.NonSerializableToXml]
        public int Reportes { get; set; }

        [NotMapped, Rp3.Data.NonSerializableToXml]
        public string DireccionTrazabilidad
        {
            get
            {
                string returnValue = string.Empty;
                if (this.ClienteDireccion != null)
                    returnValue += string.Format("Cliente: {0}\r\nDirección: {1}", this.ClienteDireccion.Cliente.NombresCompletos, this.ClienteDireccion.Direccion);

                if (this.ClienteContacto != null)
                    returnValue += string.Format("\r\nContacto: {0}", this.ClienteContacto.NombresCompletos);

                return returnValue;
            }
        }


        [NotMapped, Rp3.Data.NonSerializableToXml]
        public string Asunto { get; set; }

        [NotMapped, Rp3.Data.NonSerializableToXml]
        public string Reference01 { get; set; }

        [NotMapped, Rp3.Data.NonSerializableToXml]
        public int IdTipo { get; set; }

        [NotMapped, Rp3.Data.NonSerializableToXml]
        public int IdEtiqueta { 
            get 
            {
                switch (this.EstadoAgenda)
                { /*1: rojo 2: azul 3:verde 4:cafe*/
                    case Rp3.AgendaComercial.Models.Constantes.EstadoAgenda.Pendiente:
                        return 0;
                    case Rp3.AgendaComercial.Models.Constantes.EstadoAgenda.Cancelada:
                        return 1;
                    case Rp3.AgendaComercial.Models.Constantes.EstadoAgenda.Visitada:
                        return 2;
                    case Rp3.AgendaComercial.Models.Constantes.EstadoAgenda.Gestionada:
                        return 3;
                    case Rp3.AgendaComercial.Models.Constantes.EstadoAgenda.Reprogramada:
                        return 4;
                    case Rp3.AgendaComercial.Models.Constantes.EstadoAgenda.NoVisitado:
                        return 5;
                    default:
                        return 0;
                }
            }
            set { return; } 
        }

        [NotMapped, Rp3.Data.NonSerializableToXml]
        public int IdEstado { get; set; }

        [NotMapped, Rp3.Data.NonSerializableToXml]
        public int MarkerIndex { get; set; }

        [StringLength(500, MinimumLength = 0, ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "MaxMinStringLength")]
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Observacion")]
        public string Observacion { get; set; }

        public string UsrIng { get; set; }
        public DateTime FecIng { get; set; }
        public string UsrMod { get; set; }
        public DateTime? FecMod { get; set; }

        public double? Latitud { get; set; }

        public double? Longitud { get; set; }
        
        public short EstadoAgendaTabla { get; set; }
        
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Estado")]
        public string EstadoAgenda { get; set; }

        public short OrigenTabla { get; set; }
        public string Origen { get; set; }

        [ForeignKey("IdRuta"), NonSerializableToXmlAttribute]
        public virtual Ruta Ruta { get; set; }

        [ForeignKey("IdCliente, IdClienteDireccion"), NonSerializableToXmlAttribute]
        public virtual ClienteDireccion ClienteDireccion { get; set; }

        [ForeignKey("IdCliente, IdClienteContacto"), NonSerializableToXmlAttribute]
        public virtual ClienteContacto ClienteContacto { get; set; }

        [ForeignKey("IdProgramacionRuta"), NonSerializableToXmlAttribute]
        public virtual ProgramacionRuta ProgramacionRuta { get; set; }

        [ForeignKey("EstadoAgendaTabla,EstadoAgenda"), NonSerializableToXmlAttribute]
        public virtual GeneralValues EstadoAgendaGeneralValue { get; set; }

        [ForeignKey("MotivoNoGestionTabla,MotivoNoGestion"), NonSerializableToXmlAttribute]
        public virtual GeneralValues MotivoNoGestionGeneralValue { get; set; }

        [ForeignKey("MotivoReprogramacionTabla,MotivoReprogramacion"), NonSerializableToXmlAttribute]
        public virtual GeneralValues MotivoReprogramacionGeneralValue { get; set; }

        //[NonSerializableToXmlAttribute]
        public virtual List<AgendaTarea> AgendaTareas { get; set; }

        [NonSerializableToXmlAttribute]
        public virtual List<AgendaMedia> AgendaMedias { get; set; }

        [ForeignKey("IdAgente"), NonSerializableToXmlAttribute]
        public virtual Agente Agente { get; set; }

        [NotMapped, Rp3.Data.NonSerializableToXml]
        public bool ReadOnly { get; set; }

        public void AsignarId()
        {
            ContextService service = new ContextService();            
            this.IdAgenda = service.Agendas.GetMaxValue<long>(p => p.IdAgenda, 0, p => p.IdRuta == this.IdRuta) + 1;
        }

        public static long Max(int IdRuta)
        {
            ContextService service = new ContextService();

            return service.Agendas.GetMaxValue<long>(p => p.IdAgenda, 0, p => p.IdRuta == IdRuta) + 1;
        }

        [NotMapped, Rp3.Data.NonSerializableToXml]
        public int? IdCiudad
        {
            get
            {
                if (this.ClienteDireccion == null)
                    this.ClienteDireccion = new ClienteDireccion();

                return this.ClienteDireccion.IdCiudad;
            }
            set
            {
                if (this.ClienteDireccion == null)
                    this.ClienteDireccion = new ClienteDireccion();

                this.ClienteDireccion.IdCiudad = value;
            }
        }
    }

    public interface IAppointment
    {
        int? IdCiudad { get; set; }
    }
}
