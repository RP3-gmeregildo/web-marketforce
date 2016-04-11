using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Rp3.Web.Mvc.Application.Security.Models
{
    public class EditContact
    {
        public int ContactId { get; set; }

        public string LogonName { get; set; }

        [Display(ResourceType = typeof(Rp3.Resources.LabelFor), Name = "Role")]
        public string RoleName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(Rp3.Resources.LabelFor), Name = "Names")]
        public string Names { get; set; }

        [Required(ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(Rp3.Resources.LabelFor), Name = "LastNames")]
        public string LastNames { get; set; }

        [Required(ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "Required")]
        [Email(ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "InvalidEmailFormat")]
        [Display(ResourceType = typeof(Rp3.Resources.LabelFor), Name = "Email")]
        public string Email { get; set; }

        [Display(ResourceType = typeof(Rp3.Resources.LabelFor), Name = "PhoneNumber")]
        public string PhoneNumber { get; set; }
    }
}
