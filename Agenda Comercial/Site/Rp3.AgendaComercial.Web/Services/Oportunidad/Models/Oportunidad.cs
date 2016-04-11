using Rp3.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rp3.AgendaComercial.Web.Services.Oportunidad.Models
{
    public class Oportunidad
    {
        public int IdOportunidad { get; set; }
        public int IdOportunidadTipo { get; set; }
        public string Descripcion { get; set; }
        public decimal Probabilidad { get; set; }
        public decimal Importe { get; set; }
        public int IdAgente { get; set; }
        public DateTime? FechaUltimaGestion { get; set; }
        public long FechaUltimaGestionTicks
        {
            get
            {
                if (FechaUltimaGestion.HasValue)
                    return FechaUltimaGestion.Value.Ticks;
                return 0;
            }
            set
            {
                if (value == 0)
                    FechaUltimaGestion = null;
                else
                    FechaUltimaGestion = new DateTime(value);
            }
        }
        public int Calificacion { get; set; }
        public string Observacion { get; set; }
        public string Direccion { get; set; }
        public string Referencia { get; set; }
        public double? Latitud { get; set; }
        public double? Longitud { get; set; }
        public string ReferenciaDireccion { get; set; }
        public string Telefono1 { get; set; }
        public string Telefono2 { get; set; }
        public string CorreoElectronico { get; set; }
        public string PaginaWeb { get; set; }
        public string TipoEmpresa { get; set; }
        public int IdEtapa { get; set; }
        public short EstadoTabla { get; set; }
        public string Estado { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public long FechaCreacionTicks
        {
            get
            {
                if (FechaCreacion.HasValue)
                    return FechaCreacion.Value.Ticks;
                return 0;
            }
            set
            {
                if (value == 0)
                    FechaCreacion = null;
                else
                    FechaCreacion = new DateTime(value);
            }
        }

        public List<OportunidadContacto> OportunidadContactos { get; set; }
        public List<OportunidadMedia> OportunidadMedias { get; set; }
        public List<OportunidadTarea> OportunidadTareas { get; set; }
        public List<OportunidadResponsable> OportunidadResponsables { get; set; }
        public List<OportunidadEtapa> OportunidadEtapas { get; set; }
        public List<OportunidadBitacora> OportunidadBitacoras { get; set; }
    }
    public class OportunidadContacto : IIdentifiable
    {
        public int IdOportunidad { get; set; }
        public int IdOportunidadContacto { get; set; }
        public string Nombre { get; set; }
        public bool EsPrincipal { get; set; }
        public string Telefono1 { get; set; }
        public string Telefono2 { get; set; }
        public string Cargo { get; set; }
        public string CorreoElectronico { get; set; }
        public string Path { get; set; }
        public DateTime? FecMod { get; set; }
        public string UsrMod { get; set; }

        public string Key
        {
            get
            {
                return string.Format("{0}-{1}", this.IdOportunidad, this.IdOportunidadContacto);
            }
        }

        public string DescriptionName
        {
            get { return Nombre; }
        }
    }
    public class OportunidadTarea
    {
        public int IdEtapa { get; set; }
        public long IdTarea { get; set; }
        public short EstadoTabla { get; set; }
        public string Estado { get; set; }
        public DateTime? FechaInicio { get; set; }
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
        public DateTime? FechaFin { get; set; }
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
        public int Orden { get; set; }
        public string Observacion { get; set; }
        public List<OportunidadTareaActividad> OportunidadTareaActividads { get; set; }
    }

    public class OportunidadEtapa
    {
        public int IdEtapa { get; set; }
        public int? IdEtapaPadre { get; set; }
        public short EstadoTabla { get; set; }
        public string Estado { get; set; }
        public DateTime? FechaInicio { get; set; }
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
        public DateTime? FechaFin { get; set; }
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
        public DateTime? FechaFinPlan { get; set; }
        public long FechaFinPlanTicks
        {
            get
            {
                if (FechaFinPlan.HasValue)
                    return FechaFinPlan.Value.Ticks;
                return 0;
            }
            set
            {
                if (value == 0)
                    FechaFinPlan = null;
                else
                    FechaFinPlan = new DateTime(value);
            }
        }
        public string Observacion { get; set; }
        public int Orden { get; set; }
    }

    public class OportunidadTipo
    {
        public int IdOportunidadTipo { get; set; }
        public string Descripcion { get; set; }
    }

    public class OportunidadTareaActividad
    {
        public int IdTareaActividad { get; set; }
        public string Resultado { get; set; }
        public string ResultadoCodigo { get; set; }
    }
    public class OportunidadResponsable : IIdentifiable
    {
        public int IdOportunidad { get; set; }
        public int IdAgente { get; set; }
        public short TipoTabla { get; set; }
        public string Tipo { get; set; }
        public string Key
        {
            get
            {
                return string.Format("{0}-{1}", this.IdOportunidad, this.IdAgente);
            }
        }

        public string DescriptionName
        {
            get { return this.IdAgente + ""; }
        }
    }

    public class OportunidadBitacora : IIdentifiable
    {
        public int IdOportunidad { get; set; }
        public int IdOportunidadBitacora { get; set; }
        public string Detalle { get; set; }
        public int IdAgente { get; set; }
        public DateTime? FecIng { get; set; }
        public long FecIngTicks
        {
            get
            {
                if (FecIng.HasValue)
                    return FecIng.Value.Ticks;
                return 0;
            }
            set
            {
                if (value == 0)
                    FecIng = null;
                else
                    FecIng = new DateTime(value);
            }
        }
        public string Key
        {
            get
            {
                return string.Format("{0}-{1}", this.IdOportunidad, this.IdOportunidadBitacora);
            }
        }

        public string DescriptionName
        {
            get { return this.IdOportunidadBitacora + ""; }
        }
    }

}