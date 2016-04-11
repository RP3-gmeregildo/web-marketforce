using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Linq;

namespace Rp3.AgendaComercial.Web.Areas.Pedido.Reports
{
    public partial class Productos : DevExpress.XtraReports.UI.XtraReport
    {
        internal List<AgendaComercial.Models.Pedido.Producto> Results { get; set; }
        public Productos(dynamic viewBag, object model)
        {
            InitializeComponent();
            this.Results = model as List<AgendaComercial.Models.Pedido.Producto>;

            this.bindingSource1.DataSource = this.Results.OrderBy(p => p.Descripcion);
            this.cellEmitidoel.Text = ((DateTime)viewBag.Fecha).ToString("g");
            this.cellEmitidopor.Text = viewBag.User;
            this.cellCantidad.Text = Results.Count() + "";
        }

    }
}
