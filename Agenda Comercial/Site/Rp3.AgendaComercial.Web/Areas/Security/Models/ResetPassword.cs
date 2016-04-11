using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Rp3.Web.Mvc.Application.Security.Models
{
    public class ResetPassword
    {
        [Display(ResourceType = typeof(Rp3.Resources.LabelFor), Name = "Email")]
        [Required(ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "RequiredLoginEmail")]
        [Email(ErrorMessageResourceType=typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName="InvalidEmailFormat")]
        public string Email { get; set; }
    }
}
