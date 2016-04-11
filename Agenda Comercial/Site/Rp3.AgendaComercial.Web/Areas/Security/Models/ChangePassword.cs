using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Rp3.Web.Mvc.Application.Security.Models
{
    public class ChangePassword
    {
        [Display(ResourceType = typeof(Rp3.Resources.LabelFor), Name = "Names")]
        [Required(ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "Required")]
        public string Names { get; set; }

        [Display(ResourceType = typeof(Rp3.Resources.LabelFor), Name = "User")]
        [Required(ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "Required")]
        public string LogonName { get; set; }

        [Display(ResourceType = typeof(Rp3.Resources.LabelFor), Name = "CurrentPassword")]
        [Required(ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "RequiredCurrentPassword")]
        [DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        public string CurrentPassword { get; set; }

        [Display(ResourceType = typeof(Rp3.Resources.LabelFor), Name = "NewPassword")]
        [Required(ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "RequiredNewPassword")]
        [StringLength(15, MinimumLength = 6, ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "MaxMinStringLength")]
        [DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        public string NewPassword { get; set; }

        [Display(ResourceType = typeof(Rp3.Resources.LabelFor), Name = "ConfirmNewPassword")]
        [Required(ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "RequiredConfirmNewPassword")]
        [System.Web.Mvc.Compare("NewPassword", ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "ComparePasswordWithNewPassword")]
        [StringLength(15, MinimumLength = 6, ErrorMessageResourceType = typeof(Rp3.Resources.ErrorMessageValidation), ErrorMessageResourceName = "MaxMinStringLength")]
        [DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        public string ConfirmNewPassword { get; set; }
    }
}
