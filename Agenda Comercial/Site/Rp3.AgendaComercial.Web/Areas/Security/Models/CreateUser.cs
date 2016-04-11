using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Rp3.Web.Mvc.Application.Security.Models
{
    public class CreateUser
    {
        [Required(ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(Rp3.Resources.LabelFor), Name = "Names")]
        public string Names { get; set; }

        [Required(ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(Rp3.Resources.LabelFor), Name = "LastNames")]
        public string LastNames { get; set; }

        [Required(ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "Required")]
        [Email(ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "InvalidEmailFormat")]
        [Display(ResourceType = typeof(Rp3.Resources.LabelFor), Name = "Email")]
        [Remote("AccountAvalaible", "User", ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "EmailAccountAlreadyExists")]
        public string Email { get; set; }

        [Required(ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "Required")]        
        [Display(ResourceType = typeof(Rp3.Resources.LabelFor), Name = "LogonName")]
        [Remote("LogonNameAvalaible", "User", ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "LogonNameAccountAlreadyExists")]
        public string LogonName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(Rp3.Resources.LabelFor), Name = "Password")]
        [DataType(DataType.Password)]
        [StringLength(15, MinimumLength = 6, ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "MaxMinStringLength")]
        public string Password { get; set; }

        [Required(ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(Rp3.Resources.LabelFor), Name = "ConfirmPassword")]
        [DataType(DataType.Password)]
        [System.Web.Mvc.Compare("Password", ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "ComparePassword")]
        [StringLength(15, MinimumLength = 6, ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "MaxMinStringLength")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(Rp3.Resources.LabelFor), Name = "UserActiveDirectoryEnabled")]
        public bool ActiveDirectoryEnabled { get; set; }

        [Required(ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(Rp3.Resources.LabelFor), Name = "Role")]
        public int RoleId { get; set; }        
    }
}
