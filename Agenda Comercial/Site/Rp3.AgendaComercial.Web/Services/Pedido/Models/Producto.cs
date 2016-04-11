using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rp3.AgendaComercial.Web.Services.Pedido.Models
{
    public class Producto
    {
        public int IdProducto { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public string URLFoto { get; set; }
        public int? IdSubCategoria { get; set; }
    }

    public class Categoria
    {
        public int IdCategoria { get; set; }
        public string Descripcion { get; set; }
    }

    public class SubCategoria
    {
        public int IdSubCategoria { get; set; }
        public int IdCategoria { get; set; }
        public string Descripcion { get; set; }
    }
}