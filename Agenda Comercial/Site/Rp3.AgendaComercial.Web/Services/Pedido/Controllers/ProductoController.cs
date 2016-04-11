using Rp3.Web.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Rp3.AgendaComercial.Web.Services.Pedido.Controllers
{
    public class ProductoController : AgendaComercial.Web.Services.Controllers.ApiAgendaComercialController
    {
        [ApiAuthorization]
        public IHttpActionResult GetProductos(long? ultimaFechaActualizacion = null)
        {
            DateTime? fechaUltimaActualizacion = null;
            var data = DataBase.Productos.Get(p => p.Estado == Rp3.AgendaComercial.Models.Constantes.Estado.Activo);
            if(ultimaFechaActualizacion.HasValue)
            {
                fechaUltimaActualizacion = new DateTime(ultimaFechaActualizacion.Value);
                data = data.Where(p => ( p.FecMod >= fechaUltimaActualizacion.Value));
            }

            List<Models.Producto> productos = new List<Models.Producto>();
            
            foreach(Rp3.AgendaComercial.Models.Pedido.Producto prod in data)
            {
                Models.Producto producto = new Models.Producto();
                producto.IdProducto = prod.IdProducto;
                producto.Descripcion = prod.Descripcion;
                producto.Precio = prod.Precio;
                producto.URLFoto = prod.URLFoto;
                producto.IdSubCategoria = prod.IdSubCategoria;
                productos.Add(producto);
            }
            return Ok(productos);
        }

        [ApiAuthorization]
        public IHttpActionResult GetCategorias(long? ultimaFechaActualizacion = null)
        {
            DateTime? fechaUltimaActualizacion = null;
            var data = DataBase.Categorias.Get(p => p.Estado == Rp3.AgendaComercial.Models.Constantes.Estado.Activo);
            if (ultimaFechaActualizacion.HasValue)
            {
                fechaUltimaActualizacion = new DateTime(ultimaFechaActualizacion.Value);
                data = data.Where(p => (p.FecMod >= fechaUltimaActualizacion.Value));
            }

            List<Models.Categoria> categorias = new List<Models.Categoria>();

            foreach (Rp3.AgendaComercial.Models.Pedido.Categoria cat in data)
            {
                Models.Categoria categoria = new Models.Categoria();
                categoria.IdCategoria = cat.IdCategoria;
                categoria.Descripcion = cat.Descripcion;
                categorias.Add(categoria);
            }
            return Ok(categorias);
        }

        [ApiAuthorization]
        public IHttpActionResult GetSubcategorias(long? ultimaFechaActualizacion = null)
        {
            DateTime? fechaUltimaActualizacion = null;
            var data = DataBase.SubCategorias.Get(p => p.Estado == Rp3.AgendaComercial.Models.Constantes.Estado.Activo);
            if (ultimaFechaActualizacion.HasValue)
            {
                fechaUltimaActualizacion = new DateTime(ultimaFechaActualizacion.Value);
                data = data.Where(p => (p.FecMod >= fechaUltimaActualizacion.Value));
            }

            List<Models.SubCategoria> categorias = new List<Models.SubCategoria>();

            foreach (Rp3.AgendaComercial.Models.Pedido.SubCategoria sub in data)
            {
                Models.SubCategoria subCategoria = new Models.SubCategoria();
                subCategoria.IdCategoria = sub.IdCategoria;
                subCategoria.IdSubCategoria = sub.IdSubCategoria;
                subCategoria.Descripcion = sub.Descripcion;
                categorias.Add(subCategoria);
            }
            return Ok(categorias);
        }
    }
}