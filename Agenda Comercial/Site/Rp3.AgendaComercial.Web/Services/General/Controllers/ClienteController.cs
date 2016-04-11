using Rp3.Web.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IO;
using System.Drawing;
using Rp3.AgendaComercial.Models.General;
using Rp3.Web.Mvc.Utility;
using Rp3.AgendaComercial.Models;
using System.Drawing.Imaging;

namespace Rp3.AgendaComercial.Web.Services.General.Controllers
{
    public class ClienteController : Rp3.Web.Http.BaseApiController<AgendaComercial.Models.ContextService>
    {
        private const string clienteImagePath = "~/Content/AgendaComercial/img/clientes";
        //private const string contactoImagePath = "~/Content/AgendaComercial/img/contactos";

        [ApiAuthorization]
        public IHttpActionResult GetClientes(int? idCliente = null, long? ultimaActualizacion = null, int? pagina = null, int? tamanoPagina = null)
        {
            List<General.Models.Cliente> data = new List<General.Models.Cliente>();

            var agente = DataBase.Agentes.Get(p => p.Usuario.LogonName == CurrentUser.LogonName, includeProperties: "Cargo").FirstOrDefault();
          
            if (agente != null && agente.IdRuta != null)
            {
                List<int> rutaIds = new List<int>();

                if (agente.IdRuta != null)
                    rutaIds.Add(agente.IdRuta ?? 0);

                if (agente.Cargo != null && agente.Cargo.EsSupervisor)
                {
                    var agentes = DataBase.Agentes.Get(p => p.IdSupervisor == agente.IdAgente && p.IdRuta != null);

                    rutaIds.AddRange(agentes.Select(p => p.IdRuta.Value).ToList<int>());
                }

                List<Cliente> values = (from r in DataBase.Rutas.Get(p => rutaIds.Contains(p.IdRuta))
                                        join d in DataBase.RutaDetalles.Get() on r.IdRuta equals d.IdRuta
                                        join c in DataBase.Clientes.Get(p => (idCliente == null || p.IdCliente == idCliente), includeProperties: "ClienteDirecciones, ClienteContactos, ClienteDato, Agente") on d.IdCliente equals c.IdCliente
                                        select c).Distinct().ToList();

                if (ultimaActualizacion != null)
                {
                    DateTime fechaModDate = new DateTime(ultimaActualizacion ?? 0);

                    values = values.Where(p => p.FecIng >= fechaModDate || p.FecMod >= fechaModDate).ToList();
                }

                if (pagina != null && tamanoPagina != null)
                {
                    values = values.OrderBy(p=>p.NombresCompletos).Skip((pagina ?? 0 - 1) * tamanoPagina ?? 0).Take(tamanoPagina ?? 0).ToList();
                }

                Assign(values, data);

                foreach (var item in data)
                {
                    var source = values.Where(p => p.IdCliente == item.IdCliente).FirstOrDefault();

                    if (source != null)
                    {
                        if(source.Agente != null)
                            item.AgenteUltimaVisita = source.Agente.Descripcion;
                        item.GeneroTabla = source.ClienteDato.GeneroTabla;
                        item.Genero = source.ClienteDato.Genero;
                        item.EstadoCivilTabla = source.ClienteDato.EstadoCivilTabla;
                        item.EstadoCivil = source.ClienteDato.EstadoCivil;
                        item.FechaNacimiento = source.ClienteDato.FechaNacimiento;
                        item.ActividadEconomica = source.ClienteDato.ActividadEconomica;
                        item.PaginaWeb = source.ClienteDato.PaginaWeb;
                        item.Estado = source.Estado;
                        item.EstadoTabla = source.EstadoTabla;
                    }

                    if (!String.IsNullOrEmpty(item.Foto))
                    {
                        string fileName = Path.GetFileName(item.Foto);
                        item.Foto = fileName;
                    }

                    foreach (var contacto in item.ClienteContactos)
                    {
                        if (!String.IsNullOrEmpty(contacto.Foto))
                        {
                            string fileName = Path.GetFileName(contacto.Foto);
                            contacto.Foto = fileName;
                        }
                    }
                }
            }

            return Ok(data);
        }

        [ApiAuthorization]
        public IHttpActionResult GetIdClientes()
        {
            List<General.Models.Cliente> data = new List<General.Models.Cliente>();
            List<int> values = new List<int>();

            var agente = DataBase.Agentes.Get(p => p.Usuario.LogonName == CurrentUser.LogonName, includeProperties: "Cargo").FirstOrDefault();

            if (agente != null && agente.IdRuta != null)
            {
                List<int> rutaIds = new List<int>();

                if (agente.IdRuta != null)
                    rutaIds.Add(agente.IdRuta ?? 0);

                if (agente.Cargo != null && agente.Cargo.EsSupervisor)
                {
                    var agentes = DataBase.Agentes.Get(p => p.IdSupervisor == agente.IdAgente);

                    rutaIds.AddRange(agentes.Select(p => p.IdAgente).ToList<int>());
                }

                values = (from r in DataBase.Rutas.Get(p => rutaIds.Contains(p.IdRuta))
                                        join d in DataBase.RutaDetalles.Get() on r.IdRuta equals d.IdRuta
                                        join c in DataBase.Clientes.Get() on d.IdCliente equals c.IdCliente
                                        select c.IdCliente).Distinct().ToList();

            }

            return Ok(values);
        }

        [ApiAuthorization]
        public IHttpActionResult GetFoto(int idCliente)
        {
            string data = null;

            var cliente = DataBase.Clientes.Get(p => p.IdCliente == idCliente).SingleOrDefault();

            if (cliente != null && !String.IsNullOrEmpty(cliente.Foto))
            {
                string filePath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(clienteImagePath), Path.GetFileName(cliente.FotoMedium));

                try
                {
                    Image image = Image.FromFile(filePath);

                    using (MemoryStream stream = new MemoryStream())
                    {
                        image.Save(stream, image.RawFormat);
                        data = Convert.ToBase64String(stream.ToArray());
                    }
                }
                catch
                {

                }
            }

            return Ok(data);
        }

        [ApiAuthorization]
        public IHttpActionResult GetContactoFoto(int idCliente, int idClienteContacto)
        {
            string data = null;

            var contacto = DataBase.ClienteContactos.Get(p => p.IdCliente == idCliente && p.IdClienteContacto == idClienteContacto).SingleOrDefault();

            if (contacto != null && !String.IsNullOrEmpty(contacto.Foto))
            {
                string filePath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(clienteImagePath), Path.GetFileName(contacto.FotoMedium));

                try
                {
                    Image image = Image.FromFile(filePath);

                    using (MemoryStream stream = new MemoryStream())
                    {
                        image.Save(stream, image.RawFormat);
                        data = Convert.ToBase64String(stream.ToArray());
                    }
                }
                catch
                {

                }
            }

            return Ok(data);
        }

        [ApiAuthorization]
        [HttpPost]
        public IHttpActionResult UpdateFull(List<General.Models.Cliente> clientes)
        {
            int[] ids = clientes.Select(c=>c.IdCliente).ToArray();

            List<Rp3.AgendaComercial.Models.General.Cliente> clientesModel =
                DataBase.Clientes.Get(p => ids.Contains(p.IdCliente), includeProperties: "ClienteDato,ClienteDirecciones,ClienteContactos").ToList();

            General.Models.ClienteCreateResponse clienteResponse = new Models.ClienteCreateResponse();
            clienteResponse.Codigos = new List<Models.ClienteCodigoResult>();     

            foreach (var cliente in clientes)
            {
                Models.ClienteCodigoResult codigoResult = new Models.ClienteCodigoResult();

                Rp3.AgendaComercial.Models.General.Cliente clienteModel = clientesModel.FirstOrDefault(p => p.IdCliente == cliente.IdCliente);
                
                Rp3.Data.Service.CopyTo(cliente, clienteModel, includeProperties: new string[]{
                    "IdTipoIdentificacion",
                    "Identificacion",                    
                    "Nombre1",
                    "Nombre2",
                    "Apellido1",
                    "Apellido2",
                    "RazonSocial",
                    "IdTipoCliente",
                    "IdCanal",
                    "CorreoElectronico",
                    "TipoPersona",
                    "Genero",
                    "EstadoCivil",
                    "FechaNacimiento",                    
                    "ActividadEconomica",
                    "PaginaWeb"                    
                });

                Rp3.Data.Service.CopyTo(cliente, clienteModel.ClienteDato, includeProperties: new string[]{
                    "Genero",
                    "EstadoCivil",
                    "FechaNacimiento",                    
                    "ActividadEconomica",
                    "PaginaWeb"
                });

                clienteModel.FecMod = this.GetCurrentDateTime();
                clienteModel.UsrMod = this.CurrentUser.LogonName;
                
                DataBase.Clientes.Update(clienteModel);
                DataBase.ClienteDatos.Update(clienteModel.ClienteDato);

                int newIdClienteDireccion = 1;
                if(clienteModel.ClienteDirecciones.Any())
                    newIdClienteDireccion = clienteModel.ClienteDirecciones.Max(p => p.IdClienteDireccion) + 1;

                var listInsert = cliente.ClienteDirecciones.Where(p => p.IdClienteDireccion == 0).ToList();

                foreach (var c in listInsert)
                {
                    c.IdClienteDireccion = newIdClienteDireccion;
                    c.IdCliente = cliente.IdCliente;
                    c.Estado = AgendaComercial.Models.Constantes.Estado.Activo;
                    c.EstadoTabla = AgendaComercial.Models.Constantes.Estado.Tabla;
                    c.AplicaRuta = true;

                    codigoResult.Contactos.Add(new Models.IntCodigoResult()
                    {
                        IdInterno = c.IdInterno,
                        IdServer = newIdClienteDireccion
                    });

                    newIdClienteDireccion++;
                }

                foreach(var c in cliente.ClienteDirecciones)                
                    c.IdCliente = clienteModel.IdCliente;  
                

                DataBase.ClienteDirecciones.Update(cliente.ClienteDirecciones, clienteModel.ClienteDirecciones, new string[]
                    {                        
                        "Direccion",
                        "IdClienteDireccion",
                        "IdCliente",
                        "EsPrincipal",
                        "IdCiudad",   
                        "CiudadDescripcion",
                        "Latitud",
                        "Longitud",
                        "Referencia",
                        "Telefono1",
                        "Telefono2",
                        "TipoDireccion"                        
                    });

                int newIdClienteContacto = 1;
                if (clienteModel.ClienteContactos.Any())
                    newIdClienteContacto = clienteModel.ClienteContactos.Max(p => p.IdClienteContacto) + 1;


                var listContactoNew = cliente.ClienteContactos.Where(p => p.IdClienteContacto == 0);

                foreach (var c in listContactoNew)
                {
                    c.IdClienteContacto = newIdClienteContacto;
                    c.IdCliente = cliente.IdCliente;
                    c.Estado = Rp3.AgendaComercial.Models.Constantes.Estado.Activo;
                    c.EstadoTabla = Rp3.AgendaComercial.Models.Constantes.Estado.Tabla;
                    
                    codigoResult.Contactos.Add(new Models.IntCodigoResult()
                    {
                        IdInterno = c.IdInterno,
                        IdServer = newIdClienteContacto
                    });

                    newIdClienteContacto++;
                }

                foreach (var c in cliente.ClienteContactos)
                    c.IdCliente = clienteModel.IdCliente;                  

                DataBase.ClienteContactos.Update(cliente.ClienteContactos, clienteModel.ClienteContactos, new string[]{
                    "Apellido",
                    "IdCliente",
                    "IdClienteContacto",
                    "Cargo",
                    "CorreoElectronico",                        
                    "Nombre",
                    "Telefono1",
                    "Telefono2"
                });
               


                codigoResult.IdInterno = cliente.IdInterno;
                codigoResult.IdServer = clienteModel.IdCliente;

                clienteResponse.Codigos.Add(codigoResult);                
            }

            DataBase.Save();

            return Ok(clienteResponse);
        }

        [ApiAuthorization]
        [HttpPost]
        public IHttpActionResult Create(List<General.Models.Cliente> clientes)
        {            
            int idCliente = DataBase.Clientes.GetMaxValue<int>(p=>p.IdCliente,0);
            General.Models.ClienteCreateResponse clienteResponse = new Models.ClienteCreateResponse();

            clienteResponse.Codigos = new List<Models.ClienteCodigoResult>();            

            Agente agente = DataBase.Agentes.Get(p => p.IdUsuario == CurrentUser.UserId).SingleOrDefault();
            AgendaComercial.Models.Ruta.Ruta ruta = null;
            if (agente != null)
            {
                ruta = DataBase.Rutas.GetRutaOAsignar(agente.IdAgente);
            }

            foreach(var cliente in clientes)
            {
                Models.ClienteCodigoResult codigoResult = new Models.ClienteCodigoResult();                

                int newIdCliente = ++idCliente;
                int idClienteDireccionPrincipal = 0;

                Rp3.AgendaComercial.Models.General.Cliente clienteModel = new Cliente();
                clienteModel.ClienteDato = new ClienteDato();
                clienteModel.ClienteContactos = new List<ClienteContacto>();
                clienteModel.ClienteDirecciones = new List<ClienteDireccion>();                

                Rp3.Data.Service.CopyTo(cliente, clienteModel,includeProperties: new string[]{
                    "IdTipoIdentificacion",
                    "Identificacion",                    
                    "Nombre1",
                    "Nombre2",
                    "Apellido1",
                    "Apellido2",
                    "RazonSocial",
                    "IdTipoCliente",
                    "IdCanal",
                    "CorreoElectronico",
                    "TipoPersona",
                    "Genero",
                    "EstadoCivil",
                    "FechaNacimiento",                    
                    "ActividadEconomica",
                    "PaginaWeb"                    
                });

                Rp3.Data.Service.CopyTo(cliente, clienteModel.ClienteDato,includeProperties: new string[]{
                    "Genero",
                    "EstadoCivil",
                    "FechaNacimiento",                    
                    "ActividadEconomica",
                    "PaginaWeb"
                });


                clienteModel.Estado = Rp3.AgendaComercial.Models.Constantes.Estado.Activo;
                clienteModel.EstadoTabla = Rp3.AgendaComercial.Models.Constantes.Estado.Tabla;
                clienteModel.FecIng = this.GetCurrentDateTime();
                clienteModel.UsrIng = this.CurrentUser.LogonName;

                clienteModel.TipoPersonaTabla = Rp3.AgendaComercial.Models.Constantes.TipoPersona.Tabla;
                clienteModel.ClienteDato.GeneroTabla = Rp3.AgendaComercial.Models.Constantes.Genero.Tabla;
                clienteModel.ClienteDato.EstadoCivilTabla = Rp3.AgendaComercial.Models.Constantes.TipoPersona.Tabla;

                int newIdClienteDireccion = 1;

                foreach (var direccion in cliente.ClienteDirecciones)
                {
                    if (direccion.EsPrincipal) idClienteDireccionPrincipal = newIdClienteDireccion;

                    clienteModel.ClienteDirecciones.Add(new ClienteDireccion()
                    {
                        AplicaRuta = true,
                        Direccion = direccion.Direccion,
                        EsPrincipal = direccion.EsPrincipal,
                        Estado = Rp3.AgendaComercial.Models.Constantes.Estado.Activo,
                        EstadoTabla = Rp3.AgendaComercial.Models.Constantes.Estado.Tabla,
                        IdCiudad = direccion.IdCiudad,
                        IdCliente = newIdCliente,
                        IdClienteDireccion = newIdClienteDireccion,
                        Latitud = direccion.Latitud,
                        CiudadDescripcion = direccion.CiudadDescripcion,
                        Longitud = direccion.Longitud,
                        Referencia = direccion.Referencia,
                        Telefono1 = direccion.Telefono1,
                        Telefono2 = direccion.Telefono2,
                        TipoDireccion = direccion.TipoDireccion,
                        TipoDireccionTabla = Rp3.AgendaComercial.Models.Constantes.TipoDireccion.Tabla,                        
                    });
                    
                    codigoResult.Direcciones.Add(new Models.IntCodigoResult()
                    {
                        IdInterno = direccion.IdInterno,
                        IdServer = newIdClienteDireccion
                    });

                    newIdClienteDireccion++;
                }

                if (idClienteDireccionPrincipal == 0)
                    idClienteDireccionPrincipal = clienteModel.ClienteDirecciones.Min(p => p.IdClienteDireccion);

                int newIdClienteContacto = 1;
                foreach (var contacto in cliente.ClienteContactos)
                {
                    clienteModel.ClienteContactos.Add(new ClienteContacto()
                    {
                        Apellido = contacto.Apellido,
                        Cargo = contacto.Cargo,
                        CorreoElectronico = contacto.CorreoElectronico,
                        IdCliente = newIdCliente,
                        IdClienteContacto = newIdClienteContacto,
                        IdClienteDireccion = contacto.IdClienteDireccion,
                        Nombre = contacto.Nombre,
                        Telefono1 = contacto.Telefono1,
                        Telefono2 = contacto.Telefono2,
                        Estado = Rp3.AgendaComercial.Models.Constantes.Estado.Activo,
                        EstadoTabla = Rp3.AgendaComercial.Models.Constantes.Estado.Tabla
                    });
                    
                    codigoResult.Contactos.Add(new Models.IntCodigoResult()
                    {
                        IdInterno = contacto.IdInterno,
                        IdServer = newIdClienteDireccion
                    });

                    newIdClienteContacto++;
                }


                clienteModel.IdCliente = newIdCliente;
                clienteModel.ClienteDato.IdCliente = newIdCliente;


                DataBase.Clientes.Insert(clienteModel);
                DataBase.ClienteDatos.Insert(clienteModel.ClienteDato);

                foreach (var contacto in clienteModel.ClienteContactos)
                    DataBase.ClienteContactos.Insert(contacto);

                foreach (var direccion in clienteModel.ClienteDirecciones)                
                    DataBase.ClienteDirecciones.Insert(direccion);

                if(ruta!=null)
                {
                    DataBase.Rutas.AgregarCliente(ruta.IdRuta, newIdCliente, idClienteDireccionPrincipal);
                }

                codigoResult.IdInterno = cliente.IdInterno;
                codigoResult.IdServer = clienteModel.IdCliente;

                clienteResponse.Codigos.Add(codigoResult);
            }

            DataBase.Save();

            return Ok(clienteResponse);
        }

        [HttpPost]
        [ApiAuthorization]
        public IHttpActionResult SetFotoCliente(Rp3.AgendaComercial.Web.Services.General.Models.ClienteFoto clienteFoto)
        {
            byte[] content = Convert.FromBase64String(clienteFoto.Contenido);
            var newfileName = String.Format("{0}{1}", Guid.NewGuid(), System.IO.Path.GetExtension(clienteFoto.Nombre));
            string filePath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(clienteImagePath), newfileName);

            Image image;
            using (MemoryStream ms = new MemoryStream(content))
            {
                image = Image.FromStream(ms);
                ms.Flush();
                image.Save(filePath);
            }

            Thumbnail.SaveThumbnail(filePath, Constantes.ProfileFotosSize.ProfilePictuteMinWidth, Constantes.ProfileFotosSize.ProfilePictuteMinHeight, "min");
            Thumbnail.SaveThumbnail(filePath, Constantes.ProfileFotosSize.ProfilePictuteMedWidth, Constantes.ProfileFotosSize.ProfilePictuteMedHeight, "med");
            Thumbnail.SaveThumbnail(filePath, Constantes.ProfileFotosSize.ProfilePictuteSmaWidth, Constantes.ProfileFotosSize.ProfilePictuteSmaHeight, "sma");

            if(!clienteFoto.IdContacto.HasValue)
            {
                var cliente = DataBase.Clientes.Get(p => p.IdCliente == clienteFoto.IdCliente).SingleOrDefault();

                if (cliente != null)
                {                                        
                    if (!string.IsNullOrEmpty(cliente.Foto))
                    {
                        string deleteImagePath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(Constantes.ProfileFotosSize.ClienteImagePath), Path.GetFileName(cliente.Foto));
                        FileInfo FileDetele = new FileInfo(deleteImagePath);
                        if (FileDetele.Exists)
                            FileDetele.Delete();

                        deleteImagePath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(Constantes.ProfileFotosSize.ClienteImagePath), Path.GetFileName(cliente.FotoMedium));
                        FileDetele = new FileInfo(deleteImagePath);
                        if (FileDetele.Exists)
                            FileDetele.Delete();

                        deleteImagePath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(Constantes.ProfileFotosSize.ClienteImagePath), Path.GetFileName(cliente.FotoSmall));
                        FileDetele = new FileInfo(deleteImagePath);
                        if (FileDetele.Exists)
                            FileDetele.Delete();

                        deleteImagePath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(Constantes.ProfileFotosSize.ClienteImagePath), Path.GetFileName(cliente.FotoMin));
                        FileDetele = new FileInfo(deleteImagePath);
                        if (FileDetele.Exists)
                            FileDetele.Delete();
                    }

                    cliente.Foto = Path.Combine(Constantes.ProfileFotosSize.ClienteImagePath, newfileName);
                    DataBase.Clientes.Update(cliente);
                    DataBase.Save();
                }
            }
            else
            {
                var contacto = DataBase.ClienteContactos.Get(p => p.IdCliente == clienteFoto.IdCliente && p.IdClienteContacto == clienteFoto.IdContacto.Value).SingleOrDefault();

                if (contacto != null)
                {                                                                                
                    if (!string.IsNullOrEmpty(contacto.Foto))
                    {
                        string deleteImagePath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(clienteImagePath), Path.GetFileName(contacto.Foto));
                        FileInfo FileDetele = new FileInfo(deleteImagePath);
                        if (FileDetele.Exists)
                            FileDetele.Delete();

                        deleteImagePath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(clienteImagePath), Path.GetFileName(contacto.FotoMedium));
                        FileDetele = new FileInfo(deleteImagePath);
                        if (FileDetele.Exists)
                            FileDetele.Delete();

                        deleteImagePath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(clienteImagePath), Path.GetFileName(contacto.FotoSmall));
                        FileDetele = new FileInfo(deleteImagePath);
                        if (FileDetele.Exists)
                            FileDetele.Delete();

                        deleteImagePath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(clienteImagePath), Path.GetFileName(contacto.FotoMin));
                        FileDetele = new FileInfo(deleteImagePath);
                        if (FileDetele.Exists)
                            FileDetele.Delete();
                    }

                    contacto.Foto = Path.Combine(Constantes.ProfileFotosSize.ClienteImagePath,newfileName);
                    DataBase.ClienteContactos.Update(contacto);
                    DataBase.Save();
                }
            }

            return Ok(newfileName);
        }


        [ApiAuthorization]
        [HttpPost]
        public IHttpActionResult Update(General.Models.Cliente cliente)
        {
            var agente = DataBase.Agentes.Get(p => p.Usuario.LogonName == CurrentUser.LogonName).FirstOrDefault();

            if (agente != null)
            {
                try
                {
                    var modelUpdate = DataBase.Clientes.Get(p => p.IdCliente == cliente.IdCliente, includeProperties: "ClienteDirecciones").SingleOrDefault();

                    if (modelUpdate != null)
                    {
                        Rp3.Data.Service.CopyTo(cliente, modelUpdate, includeProperties: new string[] {
                            "CorreoElectronico",
                            "FechaNacimientoTicks"
                            });

                        if (cliente.ClienteDirecciones != null)
                        {
                            foreach (var item in cliente.ClienteDirecciones)
                            {
                                var direccion = modelUpdate.ClienteDirecciones.Where(p => p.IdCliente == item.IdCliente && p.IdClienteDireccion == item.IdClienteDireccion).FirstOrDefault();

                                if (direccion != null)
                                    Rp3.Data.Service.CopyTo(item, direccion, includeProperties: new string[] {
                                    "Direccion", 
                                    "Telefono1",
                                    "Telefono2", 
                                    "Referencia", 
                                    //"Latitud", 
                                    //"Longitud"
                                    });
                            }
                        }

                        modelUpdate.UsrMod = CurrentUser.LogonName;
                        modelUpdate.FecMod = GetCurrentDateTime();

                        DataBase.Clientes.UpdateXml(modelUpdate);
                        DataBase.Save();
                    }
                }
                catch(Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            return Ok();
        }
    }
}