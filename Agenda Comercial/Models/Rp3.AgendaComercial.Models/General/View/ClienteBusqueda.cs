using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rp3.AgendaComercial.Models.General.View 
{
    public class ClienteBusqueda: IUbicacion
    {
        public int IdCliente { get; set; }

        public int IdClienteDireccion { get; set; }

        public string Nombre1 { get; set; }

        public string Nombre2 { get; set; }

        public string Apellido1 { get; set; }

        public string Apellido2 { get; set; }

        public string Descripcion { get; set; }

        public string Direccion { get; set; }

        public string Referencia { get; set; }

        public string Ciudad { get; set; }

        public string Etiqueta
        {
            get
            {
                if (!String.IsNullOrEmpty(Descripcion))
                {
                    string etiqueta = Descripcion;

                    if (!String.IsNullOrEmpty(Ciudad))
                        etiqueta = String.Format("{0}, {1}", Ciudad, etiqueta);

                    return etiqueta;
                }
                else
                {
                    string etiqueta = Direccion;

                    if (!String.IsNullOrEmpty(Referencia))
                        etiqueta = String.Format("{0}, {1}", etiqueta, Referencia);

                    if (!String.IsNullOrEmpty(Ciudad))
                        etiqueta = String.Format("{0}, {1}", Ciudad, etiqueta);

                    return etiqueta;
                }
            }
        }

        public string NombresCompletos
        {
            get
            {
                //return string.Format("{0}{1}{2}{3}", this.Apellido1, " " + this.Apellido2, " " + this.Nombre1, " " + this.Nombre2);

                return Rp3.AgendaComercial.Models.General.Cliente.GetNombresCompletos(this.Apellido1, this.Apellido2, this.Nombre1, this.Nombre2);
            }
        }

        #region IUbicacion

        public double? Latitud { get; set; }
        
        public double? Longitud { get; set; }

        [NotMapped]
        public int MarkerIndex { get; set; }
        [NotMapped]
        public int MarkerZIndex { get { return 1000 - MarkerIndex; } }
        [NotMapped]
        public int MarkerStart { get { if (MarkerIndex > 0) return 30 * (MarkerIndex - 1); else return 0; } }

        [NotMapped]
        public int IdUbicacion
        {
            get
            {
                return IdCliente;
            }
            set
            {
                IdCliente = value;
            }
        }

        [NotMapped]
        public string Titulo
        {
            get
            {
                return this.NombresCompletos;
            }
            set
            {
                return;
            }
        }

        #endregion IUbicacion
    }
}
