using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rp3.Data.Models.General;
using System.ComponentModel.DataAnnotations;
using Rp3.AgendaComercial.Models.General;
using System.ComponentModel.DataAnnotations.Schema;
using Rp3.Web.Mvc.Utility;

namespace Rp3.AgendaComercial.Models.Ruta
{
    [Table("tbAgendaMedia", Schema = "rut")]
    public class AgendaMedia : Rp3.Data.Entity.EntityBase
    {
        public override string Key
        {
            get
            {
                return string.Format("{0}-{1}-{2}", this.IdAgenda, this.IdRuta, this.IdMedia);
            }
        }

        [Key]
        [Column(Order = 0)]        
        public int IdRuta { get; set; }

        [Key]        
        [Column(Order = 1)]
        public long IdAgenda { get; set; }

        [Key]
        [Column(Order = 2)]
        public short IdMedia { get; set; }

        public string TipoMedia { get; set; }
        public short TipoMediaTabla { get; set; }
        public string Path { get; set; }
        public string Estado { get; set; }
        public short EstadoTabla { get; set; }
        public DateTime FecMod { get; set; }
        public string UsrMod { get; set; }

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
        [ForeignKey("IdRuta, IdAgenda")]
        public virtual Agenda Agenda { get; set; }
    }
}
