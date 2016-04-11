using Rp3.Data;
using Rp3.Web.Mvc.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rp3.AgendaComercial.Models.Oportunidad
{
    [Table("tbOportunidadContacto", Schema = "opt")]
    public class OportunidadContacto : Rp3.Data.Entity.EntityBase
    {
        public override string Key
        {
            get
            {
                return string.Format("{0}-{1}", this.IdOportunidad, this.IdOportunidadContacto);
            }
        }
        [Key]
        [Column(Order = 0)]
        public int IdOportunidad { get; set; }
        [Key]
        [Column(Order = 1)]
        public int IdOportunidadContacto { get; set; }
        public string Nombre { get; set; }
        public bool EsPrincipal { get; set; }
        public string Telefono1 { get; set; }
        public string Telefono2 { get; set; }
        public string Cargo { get; set; }
        public string CorreoElectronico { get; set; }
        public string TipoMedia { get; set; }
        public short? TipoMediaTabla { get; set; }
        public string Path { get; set; }
        public DateTime FecMod { get; set; }
        public string UsrMod { get; set; }

        [ForeignKey("IdOportunidad"), NonSerializableToXmlAttribute]
        public virtual Oportunidad Oportunidad { get; set; }
        public void AsignarId()
        {
            ContextService service = new ContextService();
            this.IdOportunidadContacto = service.OportunidadContactos.GetMaxValue<int>(p => p.IdOportunidadContacto, 0, p => p.IdOportunidad == this.IdOportunidad) + 1;
        }

        [NotMapped]
        public string PathMin
        {
            get
            {
                if (string.IsNullOrEmpty(this.Path))
                    return string.Empty;
                return Thumbnail.GetPictureMinFromOriginal(this.Path);
            }
        }

        [NotMapped]
        public string PathSmall
        {
            get
            {
                if (string.IsNullOrEmpty(this.Path))
                    return string.Empty;
                return Thumbnail.GetPictureSmallFromOriginal(this.Path);
            }
        }

        [NotMapped]
        public string PathMedium
        {
            get
            {
                if (string.IsNullOrEmpty(this.Path))
                    return string.Empty;
                return Thumbnail.GetPictureMediumFromOriginal(this.Path);
            }
        }
    }
}
