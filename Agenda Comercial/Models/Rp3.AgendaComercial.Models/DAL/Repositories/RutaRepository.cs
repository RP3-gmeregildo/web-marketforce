using Rp3.AgendaComercial.Models.Ruta;
using Rp3.AgendaComercial.Models.Ruta.View;
using Rp3.AgendaComercial.Models.General;
using Rp3.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;

namespace Rp3.AgendaComercial.Models.Repositories
{
    public class RutaRepository : Repository<Ruta.Ruta>
    {
        public RutaRepository(DbContext c)
            : base(c)
        {

        }

        public void ProcesarRuta(Ruta.Ruta entity)
        {
            this.context.Database.ExecuteSqlCommand(string.Format("dbo.spProcesarRuta @IdRuta = {0}", entity.IdRuta));
        }

        public void ReasignarClientes(int IdRutaOrigen, int IdRutaDestino, string Clientes, string UsrMod)
        {
            Context db = context as Context;

            db.Database.ExecuteSqlCommand(String.Format("exec spReasignarClientes {0}, {1}, '{2}', '{3}'", IdRutaOrigen, IdRutaDestino, Clientes, UsrMod));
        }

        public void ReasignarRuta(int IdRuta, int IdAgente, string UsrMod)
        {
            Context db = context as Context;

            db.Database.ExecuteSqlCommand(String.Format("exec spReasignarRuta {0}, {1}, '{2}'", IdRuta, IdAgente, UsrMod));
        }

        public void EliminarDependenciaRuta(int IdRuta, string UsrMod)
        {
            Context db = context as Context;

            db.Database.ExecuteSqlCommand(String.Format("exec spEliminarDependenciaRuta {0}, '{1}'", IdRuta, UsrMod));
        }

        public Ruta.Ruta GetRutaOAsignar(int idAgente)
        {
            Context c = (Context)context;
            Agente agente = c.Agentes.Include("Ruta").Where(p => p.IdAgente == idAgente).FirstOrDefault();

            if (agente.Ruta != null)
            {
                return agente.Ruta;
            }
            else
            {
                Ruta.Ruta rutaInsert = new Ruta.Ruta();
                rutaInsert.Descripcion = string.Format("{0}: {1}", Rp3.AgendaComercial.Resources.LabelFor.Ruta, agente.Descripcion);
                rutaInsert.AsignarId();
                rutaInsert.Estado = Constantes.Estado.Activo;
                rutaInsert.EstadoTabla = Constantes.Estado.Tabla;

                rutaInsert.UsrIng = Rp3.Web.Mvc.Session.LogonName;
                rutaInsert.FecIng = Rp3.Runtime.Current.GetCurrentDateTime();

                Insert(rutaInsert);

                agente.IdRuta = rutaInsert.IdRuta;

                context.Entry<Agente>(agente).State = System.Data.Entity.EntityState.Modified;


                return rutaInsert;
            }
        }

        public void AgregarCliente(int idRuta, int idCliente, int idDireccion)
        {
            Context c = (Context)context;

            AgendaComercial.Models.Ruta.RutaDetalle detalle = new AgendaComercial.Models.Ruta.RutaDetalle();
            AgendaComercial.Models.Ruta.RutaIncluir incluir = new AgendaComercial.Models.Ruta.RutaIncluir();

            incluir.IdCliente = idCliente;
            incluir.IdClienteDireccion = idDireccion;
            incluir.IdRuta = idRuta;

            detalle.IdCliente = idCliente;
            detalle.IdClienteDireccion = idDireccion;
            detalle.IdRuta = idRuta;

            c.RutaIncluirs.Add(incluir);
            c.RutaDetalles.Add(detalle);
        }

        public List<ClienteProgramacion> GetClienteProgramacion(int idRuta, int pagina, int numreg, string search, string idcanal = "0", string idtipocliente = "0", string idlote = "0")
        {
            try
            {
                Db.Connection.Open();

                var cmd = Db.Connection.CreateCommand();

                cmd.CommandText = String.Format("dbo.spSearchProgramacionRuta @idRuta = {0}, @pagina = {1}, @numreg = {2}, @search = '{3}', @idcanal = '{4}', @idtipocliente = '{5}', @idlote = '{6}'",
                    idRuta, pagina, numreg, search, idcanal, idtipocliente, idlote);

                var reader = cmd.ExecuteReader();
                var clientes = ((IObjectContextAdapter)context).ObjectContext.Translate<ClienteProgramacion>(reader).ToList();

                reader.NextResult();

                var programaciones = ((IObjectContextAdapter)context).ObjectContext.Translate<ProgramacionRuta>(reader).ToList();

                foreach (var cliente in clientes)
                {
                    cliente.Programaciones = programaciones.Where(p => p.IdCliente == cliente.IdCliente && p.IdClienteDireccion == cliente.IdClienteDireccion).ToList();
                }

                return clientes;
            }
            finally
            {
                Db.Connection.Close();
            }
        }

        public List<RutaDetalleGV> ConsultaRutaDetalleSP(string ruta, string lote, string excluir, string incluir, string pagina, string numreg, string isfilter, string search, bool sinruta = false, bool todos = false)
        {
            if (ruta == null || ruta == "null") { ruta = ""; }
            if (lote == null || lote == "null") { lote = ""; }
            if (excluir == null || excluir == "null") { excluir = ""; }
            if (incluir == null || incluir == "null") { incluir = ""; }

            return context.Database.SqlQuery<RutaDetalleGV>(string.Format("dbo.SPSearchRutaLote @ruta = '{0}', @lotes = '{1}', @excluir = '{2}', @incluir = '{3}', @pagina = '{4}', @numreg = '{5}', @isfilter = '{6}', @search = '{7}', @sinruta = '{8}', @todos = '{9}'", ruta, lote, excluir, incluir, pagina, numreg, isfilter, search, sinruta, todos)).ToList();
        }

        public List<RutaDetalleGV> GrabarRutaDetalleSP(string ruta, string descripcion, int? idCalendario, string estado, string lote, string excluir, string incluir, string usrmod)
        {
            if (ruta == null || ruta == "null") { ruta = ""; }
            if (descripcion == null || descripcion == "null") { descripcion = ""; }
            if (estado == null || estado == "null") { estado = ""; }
            if (lote == null || lote == "null") { lote = ""; }
            if (excluir == null || excluir == "null") { excluir = ""; }
            if (incluir == null || incluir == "null") { incluir = ""; }

            return context.Database.SqlQuery<RutaDetalleGV>(string.Format("dbo.SPGrabarRutaLote @ruta = '{0}',  @descripcion = '{1}', @idCalendario = '{2}', @estado = '{3}', @lotes = '{4}', @excluir = '{5}', @incluir = '{6}', @usrmod = '{7}'", ruta, descripcion, idCalendario, estado, lote, excluir, incluir, usrmod)).ToList();
        }

        public List<ProgramacionPreview> GetProgramacionPreview(int IdRuta, DateTime FechaInicio, DateTime FechaFin)
        {
            return context.Database.SqlQuery<ProgramacionPreview>("dbo.spGetProgramaciones @IdRuta = {0},  @FechaInicio = {1}, @FechaFin = {2}", IdRuta, FechaInicio.Date, FechaFin.Date).ToList();
        }
    }
}
