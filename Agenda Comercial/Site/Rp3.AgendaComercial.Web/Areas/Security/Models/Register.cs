using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Rp3.Web.Mvc.Application.Security.Models
{
    public class Register
    {
        public string Names { get; set; }

        public string LastNames { get; set; }

        public string Email { get; set; }

        public string RegisterOrganizationName { get; set; }

        public string Password { get; set; }
        
        public string ConfirmedPassword { get; set; }

        public string Comments { get; set; }

        public string PhoneNumber { get; set; }
    }    
}
