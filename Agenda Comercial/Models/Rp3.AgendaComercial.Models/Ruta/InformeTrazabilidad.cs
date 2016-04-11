using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rp3.AgendaComercial.Models.Ruta
{
    [Table("tbInformeTrazabilidad", Schema = "rep")]
    public class InformeTrazabilidad : Rp3.Data.Entity.EntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long IdInformeTrazabilidad { get; set; }
        public int IdAgente { get; set; }

        public DateTime Fecha { get; set; }
        public string HoraEntrada { get; set; }
        public string Tiempo { get; set; }
        public string HoraSalida { get; set; }
        public double Distancia { get; set; }
        public bool EsMovimiento { get; set; }
        public double Minutos { get; set; }
        public string Ubicacion { get; set; }
        public string Direccion { get; set; }
        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public short? TipoTabla { get; set; }
        public string Tipo { get; set; }
        public int? IdRuta { get; set; }
        public long? IdAgenda { get; set; }
        public long? IdMarcacion { get; set; }

        [NotMapped]
        public string TipoNombre
        {
            get
            {
                switch(this.Tipo)
                {
                    case Constantes.TipoEntradaTrazabilidad.Detenido: return Rp3.AgendaComercial.Resources.LabelFor.Detenido;
                    case Constantes.TipoEntradaTrazabilidad.EnMovimiento: return Rp3.AgendaComercial.Resources.LabelFor.EnMovimiento;
                    case Constantes.TipoEntradaTrazabilidad.Gestion: return Rp3.AgendaComercial.Resources.LabelFor.Gestion;
                    case Constantes.TipoEntradaTrazabilidad.PrimeraJornada: return Rp3.AgendaComercial.Resources.LabelFor.HoraInicioPrimeraJornada;
                    case Constantes.TipoEntradaTrazabilidad.Break: return Rp3.AgendaComercial.Resources.LabelFor.Break;
                    case Constantes.TipoEntradaTrazabilidad.SegundaJornada: return Rp3.AgendaComercial.Resources.LabelFor.HoraInicioSegundaJornada;
                    case Constantes.TipoEntradaTrazabilidad.FinJornada: return Rp3.AgendaComercial.Resources.LabelFor.HoraFinJornada;
                }
                return Constantes.TipoEntradaTrazabilidad.Indeterminado;
            }
        }

        [NotMapped]
        public string HoraEntradaDisplay
        {
            get
            {
                return this.HoraEntrada;
            }
        }

        [NotMapped]
        public string HoraSalidaDisplay
        {
            get
            {
                switch (this.Tipo)
                {
                    default:
                        return this.HoraSalida;
                    case Constantes.TipoEntradaTrazabilidad.PrimeraJornada:
                    case Constantes.TipoEntradaTrazabilidad.SegundaJornada:
                    case Constantes.TipoEntradaTrazabilidad.Break:
                    case Constantes.TipoEntradaTrazabilidad.FinJornada:
                        return string.Empty;
                }
            }
        }

        public static long Max()
        {
            ContextService service = new ContextService();
            return service.InformeTrazabilidads.GetMaxValue<long>(p => p.IdInformeTrazabilidad, 0) + 1;
        }
    }
}
