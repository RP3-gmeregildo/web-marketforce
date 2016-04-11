using Rp3.AgendaComercial.Models.Oportunidad;
using Rp3.AgendaComercial.Models.Oportunidad.View;
using Rp3.Data.Entity;
using Rp3.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rp3.AgendaComercial.Models.Repositories
{
    public class EtapaRepository : Repository<Etapa>
    {
        public EtapaRepository(DbContext c)
            : base(c)
        {
            
        }

        #region StorageProperties

        public string[] StorageProperties
        {
            get
            {
                return new String[] { "Etapas" };
            }
        }

        public string[] StoragePropertiesEtapa
        {
            get
            {
                return new String[] { "IdEtapa", "Descripcion", "Orden", "IdEtapaPadre", "Tareas", "Dias", "IdOportunidadTipo", "EsVariable" };
            }
        }

        public string[] StoragePropertiesTarea
        {
            get
            {
                return new String[] { "IdEtapa", "IdTarea", "Orden" };
            }
        }

        #endregion StorageProperties

        public void UpdateXml(EtapaSave entityToUpdate, string logonName)
        {
            //base.Update(entityToUpdate);

            Context db = context as Context;

            string infoXml = entityToUpdate.ToXml(new XmlPropertiesInfo("EtapaSave", typeof(EtapaSave), StorageProperties),
                        new XmlPropertiesInfo(typeof(EtapaView), StoragePropertiesEtapa),
                       new XmlPropertiesInfo(typeof(EtapaTareaView), StoragePropertiesTarea)
                       );

            db.Database.ExecuteSqlCommand(String.Format("exec spEtapaUpdate '{0}', '{1}'", infoXml, logonName));
        }
    }
}
