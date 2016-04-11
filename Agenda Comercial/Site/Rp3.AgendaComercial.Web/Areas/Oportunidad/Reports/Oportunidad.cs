using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using Rp3.AgendaComercial.Models.Ruta;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;
using Rp3.AgendaComercial.Models.General;
using Rp3.AgendaComercial.Models.Oportunidad.View;

namespace Rp3.AgendaComercial.Web.Areas.Oportunidad.Reports
{
    public partial class Oportunidad : DevExpress.XtraReports.UI.XtraReport
    {
        //internal List<InformeTrazabilidad> Results { get; set; }
        private List<Ubicacion> Ubicaciones { get; set; }
        private dynamic ViewBag { get; set; }
        public Oportunidad(dynamic viewBag, OportunidadListado model)
        {
            InitializeComponent();
            this.ViewBag = viewBag;
            this.Ubicaciones = model.Ubicaciones;
            this.bindingSource.DataSource = model;
            //this.bindingSourceCliente.DataSource = model.AgendaClientes[0];
            this.DetailImagenes.Visible = model.Imagen.Count != 0;
            this.GroupFooterNoImagenes.Visible = !this.DetailImagenes.Visible;
            this.xrPictureBoxEstado.ImageUrl = string.Format("{0}/Content/AgendaComercial/img/{1}", this.ViewBag.UrlBase, model.EstadoImageUrl);
            //this.DetailTareas.Visible = model.AgendaClientes.Count != 0 && model.AgendaClientes[0].Tarea.Count != 0;
            //this.GroupFooterNoTareas.Visible = !this.DetailTareas.Visible;

            this.xrLineObservacion.Visible = !string.IsNullOrEmpty(model.Observacion);

            this.xrPictureBoxLogo.BackColor = Color.FromArgb(52, 56, 60);
            this.cellTitulo.BackColor = Color.FromArgb(52, 56, 60);
            this.cellRangoFechas.BackColor = Color.FromArgb(52, 56, 60);

            this.cellEmitidoEl.Text = DateTime.Now.ToString("g");
            this.cellEmitidoPor.Text = Rp3.Web.Mvc.Session.LogonName;
        }

        private void Agenda_DataSourceDemanded(object sender, EventArgs e)
        {
            this.xrPictureBoxMap.Image = Ubicacion.ToStaticMapImage(this.Ubicaciones, this.ViewBag);
        }

    }
}
