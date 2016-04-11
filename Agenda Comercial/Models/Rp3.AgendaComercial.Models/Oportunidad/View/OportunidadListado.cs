using Rp3.AgendaComercial.Models.General;
using Rp3.AgendaComercial.Models.Ruta.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rp3.AgendaComercial.Models.Oportunidad.View
{
    public class OportunidadConsulta
    {
        public bool ReadOnly { get; set; }

        //LISTADO
        public List<OportunidadListado> OportunidadListados { get; set; }

        //LISTADO
        public List<OportunidadCategoria> OportunidadCategorias { get; set; }
    }

    public class OportunidadId
    {
        public int IdOportunidad { get; set; }
    }

    public class OportunidadCategoria
    {
        public string Nombre { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public bool isBusqueda { get; set; }
        public List<OportunidadListado> Oportunidad { get; set; }
    }

    public class OportunidadListado : IUbicacion
    {
        public int IdOportunidad { get; set; }
        public int IdOportunidadTipo { get; set; }
        public string Descripcion { get; set; }
        public decimal Probabilidad { get; set; }
        public decimal Importe { get; set; }
        public int IdAgente { get; set; }
        public string Agente { get; set; }
        public DateTime? FechaUltimaGestion { get; set; }
        public int Calificacion { get; set; }
        public string Observacion { get; set; }
        public string Direccion { get; set; }
        public string ReferenciaDireccion { get; set; }
        public string Referencia { get; set; }
        public double? Latitud { get; set; }
        public double? Longitud { get; set; }
        public int IdEtapa { get; set; }
        public string Etapa { get; set; }
        public int EtapaOrden { get; set; }
        public string Estado { get; set; }
        public string EstadoDescripcion { get; set; }
        public DateTime FechaInicio { get; set; }
        public int? IdProspecto { get; set; }
        public string Prospecto { get; set; }
        public string ProspectoCargo { get; set; }

        public string Telefono1 { get; set; }
        public string Telefono2 { get; set; }
        public string CorreoElectronico { get; set; }
        public string PaginaWeb { get; set; }
        public string TipoEmpresa { get; set; }

        public int TotalPages { get; set; }
        public int TotalRows { get; set; }
        public int CurrentPage { get; set; }

        public OportunidadTipo OportunidadTipo { get; set; }
        public List<Ubicacion> Ubicaciones { get; set; }
        public List<ListaIMG> Imagen { get; set; }
        public List<OportunidadEtapa> Etapas { get; set; }

        public List<OportunidadEtapa> EtapasPrincipales { get { return this.Etapas.Where(p => p.IdEtapaPadre == null).ToList(); } }

        public List<OportunidadContacto> Contactos { get; set; }
        public List<OportunidadResponsable> Responsables { get; set; }

        public string Foto01 { get { return this.Imagen.Count > 0 ? this.Imagen[0].URLMin : string.Empty; } }
        public string Foto02 { get { return this.Imagen.Count > 1 ? this.Imagen[1].URLMin : string.Empty; } }
        public string Foto03 { get { return this.Imagen.Count > 2 ? this.Imagen[2].URLMin : string.Empty; } }
        public string Foto04 { get { return this.Imagen.Count > 3 ? this.Imagen[3].URLMin : string.Empty; } }
        public string Foto05 { get { return this.Imagen.Count > 4 ? this.Imagen[4].URLMin : string.Empty; } }
        public string Foto06 { get { return this.Imagen.Count > 5 ? this.Imagen[5].URLMin : string.Empty; } }

        public int DiasTranscurridos
        {
            get
            {
                if (this.Estado == Rp3.AgendaComercial.Models.Constantes.EstadoOportunidad.Abierta || FechaUltimaGestion == null)
                    return Convert.ToInt32((DateTime.Now - FechaInicio).TotalDays);
                else
                    return Convert.ToInt32((FechaUltimaGestion.Value - FechaInicio).TotalDays);
            }
        }

        public int DiasInactividad
        {
            get
            {
                if (FechaUltimaGestion != null)
                    return Convert.ToInt32((DateTime.Now - FechaUltimaGestion.Value).TotalDays);
                else
                    return Convert.ToInt32((DateTime.Now - FechaInicio).TotalDays);
            }
        }

        public string EstadoImageUrl
        {
            get
            {
                string url = String.Empty;

                switch (Estado)
                {
                    case Rp3.AgendaComercial.Models.Constantes.EstadoOportunidad.Abierta:
                        url = "red-flag.png";
                        break;
                    case Rp3.AgendaComercial.Models.Constantes.EstadoOportunidad.Concretado:
                        url = "green-flag.png";
                        break;
                    case Rp3.AgendaComercial.Models.Constantes.EstadoOportunidad.NoConcretada:
                        url = "gray-flag.png";
                        break;
                    case Rp3.AgendaComercial.Models.Constantes.EstadoOportunidad.Suspendida:
                        url = "blue-flag.png";
                        break;
                }

                return url;
            }
        }

        public string ProbabilidadColor
        {
            get
            {
                if (this.Probabilidad >= 80)
                    return "#17B365";
                else if (this.Probabilidad >= 16)
                    return "#F49630";
                else
                    return "#CE4451";
            }
        }

        public string Etapa01 { get { return "1"; } }
        public string Etapa02 { get { return "2"; } }
        public string Etapa03 { get { return "3"; } }
        public string Etapa04 { get { return "4"; } }
        public string Etapa05 { get { return "5"; } }

        public bool Etapa01Show { get { return this.EtapaOrden - 1 >= 1 | this.Estado == Constantes.EstadoOportunidad.Concretado; } }
        public bool Etapa02Show { get { return this.EtapaOrden - 1 >= 2 | this.Estado == Constantes.EstadoOportunidad.Concretado; } }
        public bool Etapa03Show { get { return this.EtapaOrden - 1 >= 3 | this.Estado == Constantes.EstadoOportunidad.Concretado; } }
        public bool Etapa04Show { get { return this.EtapaOrden - 1 >= 4 | this.Estado == Constantes.EstadoOportunidad.Concretado; } }
        public bool Etapa05Show { get { return this.EtapaOrden - 1 >= 5 | this.Estado == Constantes.EstadoOportunidad.Concretado; } }
        public int Secuencia { get; set; }

        public string UbicacionLeyenda 
        {
            get 
            {
                if (this.Ubicaciones.Count == 0)
                    return "La Ubicación del Prospecto no ha sido especificada";
                else
                    return string.Empty;
            }
        }
        #region IUbicacion
        public int MarkerIndex { get; set; }
        public int MarkerZIndex { get { return 1000 - MarkerIndex; } }
        public int MarkerStart { get { if (MarkerIndex > 0) return 30 * (MarkerIndex - 1); else return 0; } }

        public int IdUbicacion
        {
            get
            {
                return IdOportunidad;
            }
            set
            {
                IdOportunidad = value;
            }
        }

        public string Titulo
        {
            get
            {
                return String.Format("{0}", this.Descripcion);
            }
        }
        #endregion IUbicacion
    }
}
