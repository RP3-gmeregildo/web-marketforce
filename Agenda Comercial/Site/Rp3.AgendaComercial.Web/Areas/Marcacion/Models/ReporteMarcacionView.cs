using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Rp3.AgendaComercial.Web.Areas.Marcacion.Models
{
    public class ReporteMarcacionView
    {
        public long? FechaTicks { get; set; }

        [Display(ResourceType = typeof(Rp3.AgendaComercial.Resources.LabelFor), Name = "Fecha")]
        public DateTime? Fecha
        {
            get
            {
                if (FechaTicks.HasValue)
                    return new DateTime(FechaTicks.Value);
                else
                    return null;
            }
            set
            {
                if (value.HasValue)
                    this.FechaTicks = value.Value.Ticks;
                else
                {
                    this.FechaTicks = null;
                }
            }
        }
    }
}