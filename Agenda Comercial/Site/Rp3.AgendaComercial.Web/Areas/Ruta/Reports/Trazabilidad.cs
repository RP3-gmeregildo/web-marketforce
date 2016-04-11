using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using Rp3.AgendaComercial.Models.Ruta;
using System.Collections.Generic;
using System.Linq;

namespace Rp3.AgendaComercial.Web.Areas.Ruta.Reports
{
    public partial class Trazabilidad : DevExpress.XtraReports.UI.XtraReport
    {
        internal List<InformeTrazabilidad> Results { get; set; }
        public Trazabilidad(dynamic viewBag, object model)
        {
            InitializeComponent();
            this.Results = model as List<InformeTrazabilidad>;
            
            this.bindingSource.DataSource = this.Results.OrderBy(p => p.Fecha).ThenBy(p => p.HoraEntrada);

            this.xrPictureBoxLogo.BackColor = Color.FromArgb(52, 56, 60);
            this.cellTitulo.BackColor = Color.FromArgb(52, 56, 60);
            this.cellRangoFechas.BackColor = Color.FromArgb(52, 56, 60);
            this.cellAgente.Text = viewBag.Agente;
            this.cellRangoFechas.Text = viewBag.RangoFechas;
            this.cellEmitidoEl.Text = DateTime.Now.ToString("g");
            this.cellEmitidoPor.Text = Rp3.Web.Mvc.Session.LogonName;
            this.cellDistancia.Text = string.Format("{0} Kms.", viewBag.Kms);
            this.cellDetenidoTotal.Text = viewBag.TiempoDetenido;
            this.cellMovimientoTotal.Text = viewBag.TiempoRecorrido;

            if (this.Results.Count > 0)
            {
                this.cellGestiones.Text = string.Format("{0:n0}", this.Results.Count(p => p.Tipo == Rp3.AgendaComercial.Models.Constantes.TipoEntradaTrazabilidad.Gestion));
                var detenciones = this.Results.Where(p => !p.EsMovimiento);
                if (detenciones.Count() > 0)
                {
                    this.cellTiempoMinimo.Text = detenciones.Select(p => p.Tiempo).OrderBy(p => p).First();
                    this.cellTiempoMaximo.Text = detenciones.Select(p => p.Tiempo).OrderBy(p => p).Last();
                    this.cellDetenciones.Text = detenciones.Count().ToString("n0");
                }
                var movimientos = this.Results.Where(p => p.EsMovimiento);
                if (movimientos.Count() > 0)
                {
                    this.cellPrimerMovimiento.Text = movimientos.Select(p => p.HoraEntrada).OrderBy(p => p).First();
                    this.cellUltimoMovimiento.Text = movimientos.Select(p => p.HoraSalida).OrderBy(p => p).Last();
                    this.cellMovimientos.Text = movimientos.Count().ToString("n0");
                }

            }
        }

    }
}
