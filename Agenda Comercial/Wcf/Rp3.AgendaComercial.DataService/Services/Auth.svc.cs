using Rp3.AgendaComercial.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;

namespace Rp3.AgendaComercial.DataService
{
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de clase "Auth" en el código, en svc y en el archivo de configuración a la vez.
    // NOTA: para iniciar el Cliente de prueba WCF para probar este servicio, seleccione Auth.svc o Auth.svc.cs en el Explorador de soluciones e inicie la depuración.
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class Auth : IAuth
    {
        public string Login(string logonName, string password)
        {
            string token = String.Empty;

            if (System.Web.HttpContext.Current != null)
                Rp3.Security.Cryptography.KeyFileName = System.Web.HttpContext.Current.Server.MapPath("~/key");
            else
                Rp3.Security.Cryptography.KeyFileName = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetAssembly(typeof(General)).Location), "Key");                 

            if(Rp3.Security.Authentication.Validity(logonName, password))
                token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());

            return token;
        }

        public bool Logout(string logonName, string password)
        {
            if (System.Web.HttpContext.Current != null)
                Rp3.Security.Cryptography.KeyFileName = System.Web.HttpContext.Current.Server.MapPath("~/key");
            else
                Rp3.Security.Cryptography.KeyFileName = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetAssembly(typeof(General)).Location), "Key");         

            return true;
        }
    }
}
