using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Rp3.Web.Mvc.Application.Security.Models
{
    public class Login
    {
        [Display(ResourceType = typeof(Rp3.Resources.LabelFor), Name = "LoginLogonName")]
        [Required(ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "RequiredLogonName")]
        public string LogonName { get; set; }

        [Display(ResourceType = typeof(Rp3.Resources.LabelFor), Name = "Password")]
        [Required(ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "RequiredPassword")]
        [DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        public string Password { get; set; }
    }
}
