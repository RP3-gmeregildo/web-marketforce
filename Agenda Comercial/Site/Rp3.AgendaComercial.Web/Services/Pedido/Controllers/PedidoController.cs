using Rp3.Web.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Rp3.AgendaComercial.Web.Services.Pedido.Controllers
{
    public class PedidoController : Rp3.Web.Http.BaseApiController<AgendaComercial.Models.ContextService>
    {
        [ApiAuthorization]
        [HttpPost]
        public IHttpActionResult UpdateFull(Models.Pedido pedido)
        {
            var agente = DataBase.Agentes.Get(p => p.Usuario.LogonName == CurrentUser.LogonName).FirstOrDefault();
            int id = 0;
            if (agente != null)
            {
                    Rp3.AgendaComercial.Models.Pedido.Pedido modelInsert = null;

                    try
                    {
                        bool insert = pedido.IdPedido == 0;

                        AgendaComercial.Models.Ruta.Ruta ruta = DataBase.Rutas.GetRutaOAsignar(agente.IdAgente);
                        agente.IdRuta = ruta.IdRuta;

                        if (insert)
                        {
                            modelInsert = new Rp3.AgendaComercial.Models.Pedido.Pedido() { PedidoDetalles = new List<AgendaComercial.Models.Pedido.PedidoDetalle>() };
                            modelInsert.IdRuta = ruta.IdRuta;
                            modelInsert.EstadoTabla = AgendaComercial.Models.Constantes.EstadoPedido.Tabla;
                        }
                        else
                            modelInsert = DataBase.Pedidos.Get(p => p.IdPedido == pedido.IdPedido).SingleOrDefault();

                        modelInsert.Estado = pedido.Estado;
                        modelInsert.Email = pedido.Email;
                        modelInsert.IdCliente = pedido.IdCliente;

                        modelInsert.ValorTotal = pedido.ValorTotal;

                        if (pedido.IdAgenda == 0)
                            modelInsert.IdAgenda = null;
                        else
                            modelInsert.IdAgenda = pedido.IdAgenda;

                        modelInsert.IdCliente = pedido.IdCliente;

                        modelInsert.FecMod = GetCurrentDateTime();
                        modelInsert.UsrMod = CurrentUser.LogonName;

                        modelInsert.IdAgente = agente.IdAgente;

                        //modelInsert.PedidoDetalles.Clear();

                        if (insert)
                        {
                            modelInsert.AsignarId();
                            modelInsert.FecIng = GetCurrentDateTime();
                            modelInsert.UsrIng = CurrentUser.LogonName;
                        }

                        if (pedido.PedidoDetalles != null)
                        {
                            var detallesUpload = new List<AgendaComercial.Models.Pedido.PedidoDetalle>();
                            int lastId = 0;
                            foreach (var detalle in pedido.PedidoDetalles)
                            {
                                AgendaComercial.Models.Pedido.PedidoDetalle detalleUpload = new AgendaComercial.Models.Pedido.PedidoDetalle()
                                {
                                    IdPedido = modelInsert.IdPedido,
                                    Cantidad = detalle.Cantidad,
                                    Descripcion = detalle.Descripcion,
                                    IdProducto = detalle.IdProducto,
                                    ValorTotal = detalle.ValorTotal,
                                    ValorUnitario = detalle.ValorUnitario
                                };
                                if (lastId == 0)
                                {
                                    detalleUpload.AsignarId();
                                    lastId = detalleUpload.IdPedidoDetalle;
                                }
                                else
                                {
                                    detalleUpload.IdPedidoDetalle = lastId;
                                }
                                lastId++;

                                detallesUpload.Add(detalleUpload);
                            }

                            DataBase.PedidoDetalles.Update(detallesUpload, modelInsert.PedidoDetalles);
                        }

                        if (insert)
                            DataBase.Pedidos.Insert(modelInsert);
                        else
                            DataBase.Pedidos.Update(modelInsert);

                        DataBase.Save();
                        id = modelInsert.IdPedido;

                        DataBase.Pedidos.NotificacionPedido(id);
                    }
                    catch (Exception e)
                    {

                    }
                
            }
            return Ok(id);
        }
    }
}