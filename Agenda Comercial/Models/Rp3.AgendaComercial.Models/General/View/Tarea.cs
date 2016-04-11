using Rp3.Web.Mvc.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rp3.AgendaComercial.Models.General.View
{
    public class TareaView
    {
        public long IdTarea { get; set; }
        [Required(ErrorMessageResourceType = typeof(Rp3.AgendaComercial.Resources.ErrorMessageValidation), ErrorMessageResourceName = "Required")]
        [StringLength(250, MinimumLength = 1, ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "MaxMinStringLength")]
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Tarea")]
        public string Descripcion { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Tipo")]
        public string TipoTarea { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Tipo")]
        public string TipoTareaDescripcion { get; set; }


        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Estado")]
        public string Estado { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Desde")]
        public DateTime? FechaVigenciaDesde 
        {
            get
            {
                if (FechaVigenciaDesdeTicks.HasValue)
                    return new DateTime(FechaVigenciaDesdeTicks.Value);
                else
                    return null;
            }
            set 
            {
                if (value.HasValue)
                    this.FechaVigenciaDesdeTicks = value.Value.Ticks;
                else
                {
                    this.FechaVigenciaDesdeTicks = null;
                }
            }
        }
        public long? FechaVigenciaDesdeTicks { get; set; }
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Hasta")]
        public DateTime? FechaVigenciaHasta 
        {
            get
            {
                if (FechaVigenciaHastaTicks.HasValue)
                    return new DateTime(FechaVigenciaHastaTicks.Value);
                else
                    return null;
            }
            set
            {
                if (value.HasValue)
                    this.FechaVigenciaHastaTicks = value.Value.Ticks;
                else
                {
                    this.FechaVigenciaHastaTicks = null;
                }
            }
        }
        public long? FechaVigenciaHastaTicks { get; set; }
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "AplicarTodas")]
        public bool AplicaTodasLasRutas { get; set; }
        public bool ReadOnly { get; set; }
        public string EstadoDescripcion { get; set; }
        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "EsIndefinida")]
        public bool EsVigenciaIndefinida { get; set; }
        public List<TareaRutaAplicaView> TareaRutasAplica { get; set; }
        public List<TareaActividadView> Actividades { get; set; }

        public List<TareaClienteActualizacionCampo> TareaClienteActualizacionCampos { get; set; }

        public bool PermitirCreacion { get; set; }
        public bool PermitirModificacion { get; set; }
        public bool SiempreEditarEnGestion { get; set; }
        public bool SoloFaltantesEditarEnGestion { get; set; }
    }

    public class ClienteCampoGrupo
    {
        public int IdGrupo { get; set; }
        public string Grupo { get; set; }
    }

    public class TareaActividadView
    {
        public int Index { get; set; }
        public int IdTareaActividad { get; set; }
        public string Descripcion { get; set; }
        public string TipoActividad { get; set; }
        public int Orden { get; set; }
        public int? Valor { get; set; }
        public int? Limite { get; set; }
        public int IdTipoActividad { get; set; }
        public string Opciones { get; set; }
        public int? IdTareaActividadPadre { get; set; }

        public List<TareaActividadView> Childs { get; set; }
    }

    public class TareaRutaAplicaView
    {
        public int IdRuta { get; set; }        
        public string Nombre {get; set;}
        public string Agente {get; set;}
        public bool Aplica { get; set; }
    }   
 
}
