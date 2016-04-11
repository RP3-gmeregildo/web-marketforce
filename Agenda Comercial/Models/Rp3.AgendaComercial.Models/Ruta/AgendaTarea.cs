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
    [Table("tbAgendaTarea", Schema = "rut")]
    public class AgendaTarea : Rp3.Data.Entity.EntityBase
    {
        public override string Key
        {
            get
            {
                return string.Format("{0}-{1}-{2}", this.IdRuta, this.IdAgenda, this.IdTarea);
            }
        }

        [Key]
        [Column(Order = 0)]        
        public int IdRuta { get; set; }

        [Key]        
        [Column(Order = 1)]
        public long IdAgenda { get; set; }

        [Key]
        [Column(Order = 2)]
        public long IdTarea { get; set; }

        public long? IdProgramacionTarea { get; set; }

        public short EstadoTareaTabla { get; set; }
        public string EstadoTarea { get; set; }

         [ForeignKey("EstadoTareaTabla,EstadoTarea"), Rp3.Data.NonSerializableToXml]
        public virtual GeneralValues EstadoTareaGeneralValue { get; set; }

        [ForeignKey("IdRuta, IdAgenda"), NonSerializableToXmlAttribute]
        public virtual Agenda Agenda { get; set; }

        [ForeignKey("IdTarea"), NonSerializableToXmlAttribute]
        public virtual Tarea Tarea { get; set; }

        public virtual List<AgendaTareaActividad> AgendaTareaActividades { get; set; }

        [NonSerializableToXmlAttribute]
        public void LoadActividades(List<TareaActividad> actividades)
        {
            if (AgendaTareaActividades == null)
                AgendaTareaActividades = new List<AgendaTareaActividad>();

            AgendaTareaActividades.Clear();

            foreach (var actividad in actividades)
            {
                var agendaActividad = new AgendaTareaActividad()
                {
                    IdRuta = this.IdRuta,
                    IdAgenda = this.IdAgenda,
                    IdTarea = this.IdTarea,
                    IdTareaActividad = actividad.IdTareaActividad,
                    Descripcion = actividad.Descripcion,
                    IdTipoActividad = actividad.IdTipoActividad,
                    Opciones = actividad.Opciones,
                    Orden = actividad.Orden,
                    IdTareaActividadPadre = actividad.IdTareaActividadPadre
                };

                this.AgendaTareaActividades.Add(agendaActividad);
            }
        }

        public void SetActividadDetalle()
        {
            if (this.AgendaTareaActividades == null || this.AgendaTareaActividades.Count == 0) _ActividadDetalle = "Ninguna";
            string data = string.Empty;
            if (this.EstadoTarea == Rp3.AgendaComercial.Models.Constantes.EstadoTarea.Realizada)
                this.AgendaTareaActividades.OrderBy(p => p.Orden).ToList().ForEach(p => data += string.Format("{2}.{3} {0}: {1}\r\n", p.Descripcion, p.Resultado, this.Orden, p.Orden));
            else
                this.AgendaTareaActividades.OrderBy(p => p.Orden).ToList().ForEach(p => data += string.Format("{1}.{2} {0}\r\n", p.Descripcion, this.Orden, p.Orden));
            if (data.Length > 2) 
                _ActividadDetalle = data.Substring(0, data.Length - 2);
            else
                _ActividadDetalle = data;
        }

        private string _ActividadDetalle;
        [NotMapped]
        [NonSerializableToXmlAttribute]
        public string ActividadesDetalle { get { return _ActividadDetalle; } }

        [NotMapped]
        public int Orden { get; set; }

    }
}
