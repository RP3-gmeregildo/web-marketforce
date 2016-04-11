using Rp3.AgendaComercial.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Rp3.AgendaComercial.DataService.Services
{
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de clase "Cliente" en el código, en svc y en el archivo de configuración a la vez.
    // NOTA: para iniciar el Cliente de prueba WCF para probar este servicio, seleccione Cliente.svc o Cliente.svc.cs en el Explorador de soluciones e inicie la depuración.
    public class Cliente : ICliente
    {
        public List<Data.Cliente> GetClientes(string token, string logonName, int? idCliente = null)
        {
            try
            {
                List<Data.Cliente> returnList = new List<Data.Cliente>();

                if (System.Web.HttpContext.Current != null)
                    Rp3.Security.Cryptography.KeyFileName = System.Web.HttpContext.Current.Server.MapPath("~/key");
                else
                    Rp3.Security.Cryptography.KeyFileName = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetAssembly(typeof(General)).Location), "Key");

                if (!String.IsNullOrEmpty(token))
                {
                    ContextService db = new ContextService();

                    var agente = db.Agentes.Get(p => p.Usuario.LogonName == logonName).FirstOrDefault();

                    if (agente != null && agente.IdRuta != null)
                    {
                        var list = (from r in db.Rutas.Get(p => p.IdRuta == agente.IdRuta)
                                    join d in db.RutaDetalles.Get() on r.IdRuta equals d.IdRuta
                                    join c in db.Clientes.Get(p=> (idCliente == null || p.IdCliente == idCliente)) on d.IdCliente equals c.IdCliente
                                    select c).ToList();

                        foreach (var item in list)
                        {
                            Data.Cliente det = new Data.Cliente();

                            Rp3.Data.Service.CopyTo(item, det, includeProperties: new string[] {
                            "IdCliente",
                            "Nombre1",
                            "Nombre2",
                            "Apellido1",
                            "Apellido2",
                            "NombresCompletos",
                            "CorreoElectronico",
                            "Calificacion",
                            "IdTipoCliente",
                            "IdCanal"                            
                            });

                            if (item.TipoCliente != null)
                                det.TipoCliente = item.TipoCliente.Descripcion;

                            if (item.Canal != null)
                                det.Canal = item.Canal.Descripcion;

                            var dir = item.ClienteDirecciones.Where(p => p.EsPrincipal).FirstOrDefault();

                            if (dir != null)
                            {
                                det.IdCiudad = dir.IdCiudad;
                                det.Telefono1 = dir.Telefono1;
                                det.Telefono2 = dir.Telefono2;
                                det.Referencia = dir.Referencia;
                                det.Direccion = dir.Direccion;
                                det.Latitud = dir.Latitud;
                                det.Longitud = dir.Longitud;

                                if (dir.Ciudad != null)
                                    det.Ciudad = dir.Ciudad.Name;
                            }

                            returnList.Add(det);
                        }
                    }
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
