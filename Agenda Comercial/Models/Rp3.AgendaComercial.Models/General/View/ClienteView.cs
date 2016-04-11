using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rp3.Data.Models.General;
using Rp3.Web.Mvc;
using Rp3.Web.Mvc.Utility;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rp3.AgendaComercial.Models.General.View
{
    [Table("vwCliente", Schema = "dbo")]
    public class ClienteView : Rp3.Data.Entity.EntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdCliente { get; set; }

        public string NombresCompletos { get; set; }

        public string Identificacion { get; set; }

        public int Calificacion { get; set; }
        public string Canal { get; set; }
        public string TipoCliente { get; set; }
        public string CorreoElectronico { get; set; }
        public string TipoPersona { get; set; }
        public string Ciudad { get; set; }
        public string Direccion { get; set; }
        public string Referencia { get; set; }
        public string Telefono1 { get; set; }
        public string Telefono2 { get; set; }

        public string Estado { get; set; }

        public DateTime? FechaUtimaVisita { get; set; }

        public int? IdAgenteUltimaVisita { get; set; }

        public string AgenteUltimaVisita { get; set; }

        public int? IndiceActividad { get; set; }

        public int? IndiceActividad2 { get; set; }

        public int? TiempoInactividad { get; set; }

        public string Agente { get; set; }

        public string TiempoInactividadText { get; set; }
        //{
        //    get
        //    {
        //        if (TiempoInactividad != null)
        //            return String.Format("{0} {1}", TiempoInactividad, Rp3.AgendaComercial.Resources.LabelFor.Dias);
        //        else
        //            return null;
        //    }
        //}
    }
}
