using Rp3.Data.Models.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Rp3.Web.Mvc.Application.Security.Models
{
    public class EditRole
    {
        public int RoleId { get; set; }

        public bool ReadOnly { get; set; }

        [Required(ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(Rp3.Resources.LabelFor), Name = "Role")]
        public string Name { get; set; }

        [Required(ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(Rp3.Resources.LabelFor), Name = "Active")]
        public bool Active { get; set; }

        public List<RoleApplicationOptionActivity> RoleApplicationOptionActivities { get; set; }
    }
}