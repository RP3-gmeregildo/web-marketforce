using Rp3.AgendaComercial.DataService.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Rp3.AgendaComercial.DataService
{
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de interfaz "IGeneral" en el código y en el archivo de configuración a la vez.
    [ServiceContract(Namespace = "http://schemas.rp3.com.ec/agendacomercial/")]
    public interface IGeneral
    {
        [OperationContract]
        [WebGet(UriTemplate = "getgeneralvalues/json",
         BodyStyle = WebMessageBodyStyle.Bare,
         ResponseFormat = WebMessageFormat.Json)]
        List<GeneralValues> GetGeneralValues(short? id = null, string code = null);

        [OperationContract]
        [WebGet(UriTemplate = "getestructurageopolitica/json",
         BodyStyle = WebMessageBodyStyle.Bare,
         ResponseFormat = WebMessageFormat.Json)]
        List<GeopoliticalStructure> GetEstructuraGeopolitica(int? geopoliticalStructureTypeId = null, int? parentGeopoliticalStructureId = null, int? geopoliticalStructureId = null);
    }
}
