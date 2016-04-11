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
using Rp3.Data.Entity;

namespace Rp3.AgendaComercial.Models.General.View
{
    public class ClienteContactoSearchText : IIdentifiable
    {
        

        public int IdCliente {get; set;}
        public int? IdContacto {get; set;}
        public string ClienteNombre1 {get; set;}
        public string ClienteNombre2 {get; set;}
        public string ClienteApellido1 {get; set;}
        public string ClienteApellido2 { get; set; }
        public string ClienteIdentificacion {get; set;}
        public string ContactoNombres { get; set;}
        public string ContactoApellidos { get; set; }
        public string ContactoCargo {get; set; }


        public DateTime? FechaUtimaVisita { get; set; }

        public int? IdAgenteUltimaVisita { get; set; }

        public string AgenteUltimaVisita { get; set; }

        public int? IndiceActividad { get; set; }

        public int? IndiceActividad2 { get; set; }

        public int? TiempoInactividad { get; set; }

        public string TiempoInactividadText
        {
            get
            {
                if (TiempoInactividad != null)
                    return String.Format("{0} {1}", TiempoInactividad, Rp3.AgendaComercial.Resources.LabelFor.Dias);
                else
                    return null;
            }
        }
       
         
        public string Key
        {
            get { return string.Format("{0}-{1}", this.IdCliente, this.IdContacto??0); }
        }

        public string DescriptionName
        {
            get {
                StringBuilder sb = new StringBuilder();
                if(this.IdContacto.HasValue)
                {                    
                    //sb.Append(this.ContactoNombres);
                    //if (!string.IsNullOrWhiteSpace(this.ContactoApellidos))
                    //    sb.Append(" " + this.ContactoApellidos);

                     sb.Append(Models.General.Cliente.GetNombresCompletos(this.ContactoApellidos, String.Empty, this.ContactoNombres, String.Empty));

                    if(!string.IsNullOrWhiteSpace(this.ContactoCargo))
                        sb.Append(" - " + this.ContactoCargo + " en ");                    
                }

                sb.Append(Models.General.Cliente.GetNombresCompletos(ClienteApellido1, ClienteApellido2, ClienteNombre1, ClienteNombre2));
                //sb.Append(this.ClienteNombre1);
                //if (!string.IsNullOrWhiteSpace(this.ClienteNombre2))
                //    sb.Append(" " + this.ClienteNombre2);
                //if (!string.IsNullOrWhiteSpace(this.ClienteApellido1))
                //    sb.Append(" " + this.ClienteApellido1);
                //if (!string.IsNullOrWhiteSpace(this.ClienteApellido2))
                //    sb.Append(" " + this.ClienteApellido2);

                return sb.ToString();
            }
        }
    }
}
