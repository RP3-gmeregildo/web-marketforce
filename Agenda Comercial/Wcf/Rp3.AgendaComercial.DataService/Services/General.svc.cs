using Rp3.AgendaComercial.DataService.Data;
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
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de clase "General" en el código, en svc y en el archivo de configuración a la vez.
    // NOTA: para iniciar el Cliente de prueba WCF para probar este servicio, seleccione General.svc o General.svc.cs en el Explorador de soluciones e inicie la depuración.
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class General : IGeneral
    {
        public List<GeneralValues> GetGeneralValues(short? id = null, string code = null)
        {
            try
            {
                List<GeneralValues> returnList = new List<GeneralValues>();

                if (System.Web.HttpContext.Current != null)
                    Rp3.Security.Cryptography.KeyFileName = System.Web.HttpContext.Current.Server.MapPath("~/key");
                else
                    Rp3.Security.Cryptography.KeyFileName = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetAssembly(typeof(General)).Location), "Key");

                ContextService db = new ContextService();

                var list = db.GeneralValues.Get(p => (id == null || p.Id == id) && (code == null || p.Code == code));

                foreach (var item in list)
                {
                    GeneralValues det = new GeneralValues();

                    Rp3.Data.Service.CopyTo(item, det, includeProperties: new string[] {
                        "Id",
                        "Code",
                        "Content",
                        "Reference01",
                        "Reference02"
                        });

                    returnList.Add(det);
                }

                return returnList;
            }
            catch (Exception e)
            {
                return null;
            }    
        }

        public List<GeopoliticalStructure> GetEstructuraGeopolitica(int? geopoliticalStructureTypeId = null, int? parentGeopoliticalStructureId = null, int? geopoliticalStructureId = null)
        {
            try
            {
                List<GeopoliticalStructure> returnList = new List<GeopoliticalStructure>();

                if (System.Web.HttpContext.Current != null)
                    Rp3.Security.Cryptography.KeyFileName = System.Web.HttpContext.Current.Server.MapPath("~/key");
                else
                    Rp3.Security.Cryptography.KeyFileName = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetAssembly(typeof(General)).Location), "Key");

                ContextService db = new ContextService();

                var list = db.GeopoliticalStructures.Get(p => 
                    (geopoliticalStructureId == null || p.GeopoliticalStructureId == geopoliticalStructureId) &&
                    (parentGeopoliticalStructureId == null || p.ParentGeopoliticalStructureId == parentGeopoliticalStructureId) && 
                    (geopoliticalStructureTypeId == null || p.GeopoliticalStructureTypeId == geopoliticalStructureTypeId));

                foreach (var item in list)
                {
                    GeopoliticalStructure det = new GeopoliticalStructure();

                    Rp3.Data.Service.CopyTo(item, det, includeProperties: new string[] {
                        "GeopoliticalStructureId",
                        "GeopoliticalStructureTypeId",
                        "IsoCode",
                        "Name",
                        "Latitude",
                        "Longitude",
                        "ParentGeopoliticalStructureId"
                        });

                    returnList.Add(det);
                }

                return returnList;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
