using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Rp3.AgendaComercial.DataService
{
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de interfaz "IAuth" en el código y en el archivo de configuración a la vez.
    [ServiceContract(Namespace = "http://schemas.rp3.com.ec/agendacomercial/")]
    public interface IAuth
    {
        [OperationContract]
        [WebInvoke(Method = "POST",
         UriTemplate = "login/json",
         BodyStyle = WebMessageBodyStyle.Bare,
         ResponseFormat = WebMessageFormat.Json)]
        string Login(string logonName, string password);

        [OperationContract]
        [WebInvoke(Method = "POST",
         UriTemplate = "logout/json",
         BodyStyle = WebMessageBodyStyle.Bare,
         ResponseFormat = WebMessageFormat.Json)]
         bool Logout(string logonName, string password);
    }
}
