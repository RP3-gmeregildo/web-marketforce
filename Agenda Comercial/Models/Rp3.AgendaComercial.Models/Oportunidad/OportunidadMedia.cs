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
    [Table("tbOportunidadMedia", Schema = "opt")]
    public class OportunidadMedia : Rp3.Data.Entity.EntityBase
    {
        [Key]
        [Column(Order = 0)]
        public int IdOportunidad { get; set; }

        [Key]
        [Column(Order = 1)]
        public short IdMedia { get; set; }
        public string TipoMedia { get; set; }
        public short TipoMediaTabla { get; set; }
        public string Path { get; set; }
        public string Estado { get; set; }
        public short EstadoTabla { get; set; }
        public DateTime FecMod { get; set; }
        public string UsrMod { get; set; }

        [ForeignKey("IdOportunidad"), NonSerializableToXmlAttribute]
        public virtual Oportunidad Oportunidad { get; set; }

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
