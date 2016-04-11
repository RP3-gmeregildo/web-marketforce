using Rp3.AgendaComercial.Models.General;
using Rp3.AgendaComercial.Models.General.View;
using Rp3.Data.Entity;
using Rp3.Xml;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rp3.AgendaComercial.Models.Repositories
{
    public class ClienteRepository : Rp3.Data.Entity.Repository<Cliente>
    {
        public ClienteRepository(DbContext context)
            :base(context)
        {
        }

        #region StorageProperties

        public string[] StorageProperties
        {
            get
            {
                return new String[] { "IdCliente","IdTipoIdentificacion","Identificacion","Nombre1","Nombre2","Apellido1","Apellido2","RazonSocial","NombreCompleto","Foto","CorreoElectronico","TipoPersonaTabla","TipoPersona","IdTipoCliente","IdCanal","Calificacion","UsrIng","FecIng","UsrMod","FecMod",
                "ClienteDirecciones", "ClienteContactos" };
            }
        }

        public string[] StoragePropertiesDireccion
        {
            get
            {
                return new String[] { "IdCliente","IdClienteDireccion","Direccion","Descripcion","TipoDireccionTabla","TipoDireccion","IdCiudad","Telefono1","Telefono2","Referencia","Latitud","Longitud","EsPrincipal","AplicaRuta","EstadoTabla","Estado" };
            }
        }

        public string[] StoragePropertiesContacto
        {
            get
            {
                return new String[] { "IdCliente", "IdClienteContacto", "Nombre", "Apellido", "IdClienteDireccion", "Cargo", "CorreoElectronico", "Telefono1", "Telefono2", "Foto", };
            }
        }

        #endregion StorageProperties

        public void EliminarDependenciaCliente(int IdCliente, string UsrMod)
        {
            Context db = context as Context;

            db.Database.ExecuteSqlCommand(String.Format("exec spEliminarDependenciaCliente {0}, '{1}'", IdCliente, UsrMod));
        }

        public void EliminarDependenciaClienteDireccion(int IdCliente, int IdClienteDireccion, string UsrMod)
        {
            Context db = context as Context;

            db.Database.ExecuteSqlCommand(String.Format("exec spEliminarDependenciaClienteDireccion {0}, {1}, '{2}'", IdCliente, IdClienteDireccion, UsrMod));
        }

        public void EliminarDependenciaClienteContacto(int IdCliente, int IdClienteContacto, string UsrMod)
        {
            Context db = context as Context;

            db.Database.ExecuteSqlCommand(String.Format("exec spEliminarDependenciaClienteContacto {0}, {1}, '{2}'", IdCliente, IdClienteContacto, UsrMod));
        }

        public List<Cliente> GetClientesSync(int idAgente, int idRuta, DateTime? fechaModificacion, int pageSize, int page)
        {
            Context db = (Context)context;

            var query = from c in db.Clientes.Include("ClienteDirecciones, ClienteContactos, ClienteDato")
                        join r in db.RutaDetalles
                        on c.IdCliente equals r.IdCliente
                        where r.IdRuta == idRuta
                        select c;

            if (fechaModificacion.HasValue)
            {
                query.Where(p => p.FecMod >= fechaModificacion.Value);
            }

            return query.ToList();
        }

        public List<ClienteContactoSearchText> GetClienteContactoSearchText(string texto, string idruta, int top = 30)
        {
            if (idruta == null) { idruta = ""; }

            return context.Database.SqlQuery<ClienteContactoSearchText>("dbo.spFullTextSearchClienteContacto @SearchText = @SearchTextParametro, @IdRuta= @IdRutaParametro, @Top = @TopParametro",
               new SqlParameter("@SearchTextParametro", texto),
               new SqlParameter("@IdRutaParametro", idruta),
               new SqlParameter("@TopParametro", top)).ToList();
        }

        public void UpdateXml(Cliente entityToUpdate)
        {
            //base.Update(entityToUpdate);

            Context db = context as Context;

            string infoXml = entityToUpdate.ToXml(new XmlPropertiesInfo(typeof(Cliente), StorageProperties),
                       new XmlPropertiesInfo(typeof(ClienteDireccion), StoragePropertiesDireccion),
                       new XmlPropertiesInfo(typeof(ClienteContacto), StoragePropertiesContacto)
                       );

            db.Database.ExecuteSqlCommand(String.Format("exec spClienteUpdate '{0}'", infoXml));
        }

        public IEnumerable<ClienteBusqueda> GetCliente(string textSearch)
        {
            string[] entries = null;
            if (textSearch.Contains(","))
                entries = textSearch.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            else
                entries = textSearch.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            StringBuilder queryString = new StringBuilder();
            queryString.Append(QueryFindCliente.ToString());

            for (int i = 0; i < entries.Count(); i++)
            {
                if (i == entries.Count() - 1)
                    queryString.AppendLine(QueryNombreContaint.Replace("@SearchText", entries[i]));
                else
                    queryString.AppendLine(QueryNombreEqual.Replace("@SearchText", entries[i]));
            }

            Context db = context as Context;
            IEnumerable<ClienteBusqueda> result = db.Database.SqlQuery<ClienteBusqueda>(queryString.ToString());
            return result;
        }


        #region Query

        const string QueryFindCliente =
            "SELECT TOP 15 cli.IdCliente, dir.IdClienteDireccion, cli.Apellido1, cli.Apellido2, cli.Nombre1, cli.Nombre2," +
            " dir.Latitud, dir.Longitud, dir.Direccion, dir.Referencia, dir.Descripcion, ciu.Name Ciudad" +
            " FROM gen.tbCliente cli inner join gen.tbClienteDireccion dir on cli.IdCliente = dir.IdCliente and dir.AplicaRuta = 1" +
            " left join gen.tbGeopoliticalStructure ciu on dir.IdCiudad = ciu.GeopoliticalStructureId" +
            " WHERE ";
            //" WHERE cli.Estado = '@Estado'";

        const string QueryNombreContaint = " ( cli.Nombre1 LIKE '@SearchText%'" +
            "OR cli.Nombre2 LIKE '@SearchText%'" +
            "OR cli.Apellido1 LIKE '@SearchText%'" +
            "OR cli.Apellido2 LIKE '@SearchText%')";

        const string QueryNombreEqual = " ( cli.Nombre1 = '@SearchText'" +
            "OR cli.Nombre2 = '@SearchText'" +
            "OR cli.Apellido1 = '@SearchText'" +
            "OR cli.Apellido2 = '@SearchText')";

        #endregion
    }
}
