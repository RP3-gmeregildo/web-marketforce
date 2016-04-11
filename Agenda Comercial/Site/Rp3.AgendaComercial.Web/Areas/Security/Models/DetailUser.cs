using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Rp3.Web.Mvc.Application.Security.Models
{
    public class DetailUser
    {
        public int UserId { get; set; }

        public int RoleId { get; set; }

        [Display(ResourceType = typeof(Rp3.Resources.LabelFor), Name = "LogonName")]
        public string LogonName { get; set; }

        [Display(ResourceType = typeof(Rp3.Resources.LabelFor), Name = "Email")]
        public string Email { get; set; }
        
        [Display(ResourceType = typeof(Rp3.Resources.LabelFor), Name = "Names")]
        public string Names { get; set; }
        
        [Display(ResourceType = typeof(Rp3.Resources.LabelFor), Name = "LastNames")]
        public string LastNames { get; set; }
        
        [Display(ResourceType = typeof(Rp3.Resources.LabelFor), Name = "Role")]
        public string RoleName { get; set; }
        
        [Display(ResourceType = typeof(Rp3.Resources.LabelFor), Name = "UserActiveDirectoryEnabled")]
        public bool ActiveDirectoryEnabled { get; set; }

        [Display(ResourceType = typeof(Rp3.Resources.LabelFor), Name = "Active")]
        public bool Active { get; set; }
    }
}
