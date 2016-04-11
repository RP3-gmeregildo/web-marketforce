using Rp3.Data.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rp3.AgendaComercial.Models.Marcacion.View
{
    public class AgenteGrupo : EntityBase
    {
        public override string Key
        {
            get
            {
                if (this.IdAgente == null && this.IdGrupo == null) 
                    return " ";
                else if (this.IdAgente != null)
                    return String.Format("A-{0}", this.IdAgente);
                else 
                    return String.Format("G-{0}", this.IdGrupo);
            }
        }

        public override string DescriptionName
        {
            get
            {
                return this.Descripcion;
            }
        }

        public int? IdAgente { get; set; }
        public int? IdGrupo { get; set; }

        public string Descripcion { get; set; }
    }

    public class PermisoConsulta
    {
        public bool ReadOnly { get; set; }

        //LISTADO
        public List<PermisoListado> PermisoListados { get; set; }

        //LISTADO
        public List<PermisoCategoria> PermisoCategorias { get; set; }

    }

    public class PermisoCategoria
    {
        public string Nombre { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public bool isBusqueda { get; set; }
        public List<PermisoListado> Permiso { get; set; }
    }

    public class PermisoListado
    {
        public long IdPermiso { get; set; }
        public int? IdAgente { get; set; }
        public string Agente { get; set; }
        public int? IdGrupo { get; set; }
        public string Grupo { get; set; }
        public string Tipo { get; set; }
        public string TipoDesripcion { get; set; }
        public string Motivo { get; set; }
        public string MotivoDescripcion { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public DateTime HoraInicio { get; set; }
        public DateTime HoraFin { get; set; }
        public string Observacion { get; set; }

        public string ObservacionSupervisor { get; set; }
        public bool EsPrevio { get; set; }
        public string Estado { get; set; }
        public string EstadoDescripcion { get; set; }

        public string TipoJornada { get; set; }
        public string TipoJornadaDesripcion { get; set; }

        public int TotalPages { get; set; }
        public int TotalRows { get; set; }
        public int CurrentPage { get; set; }

        public int Secuencia { get; set; }

        public DateTime DuracionPermiso { get { return new DateTime(this.FechaFin.Subtract(this.FechaInicio).Ticks); } }
    }

    public class PermisoEdit
    {
        public long IdPermiso { get; set; }
        public int? IdAgente { get; set; }
        public string Agente { get; set; }
        public int? IdGrupo { get; set; }
        public string Grupo { get; set; }

        public bool EsPrevio { get; set; }

        public string AgenteGrupo { get {
            if (this.IdAgente == null)
                return Grupo;
            else
                return Agente;
        } }

        public string Tipo { get; set; }
        public string Motivo { get; set; }

        public string Estado { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Observacion")]
        public string Observacion { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "ObservacionSupervisor")]
        public string ObservacionSupervisor { get; set; }
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
    }
}
