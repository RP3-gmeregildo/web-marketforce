using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using Rp3.AgendaComercial.Models.Ruta;
using System.Collections.Generic;
using System.Linq;
using DevExpress.XtraCharts;

namespace Rp3.AgendaComercial.Web.Areas.Ruta.Reports
{
    public partial class AgenteGestion : DevExpress.XtraReports.UI.XtraReport
    {
        public AgenteGestion(dynamic viewBag, Rp3.AgendaComercial.Models.Consulta.View.AgenteReporteGestion model, Rp3.AgendaComercial.Models.Consulta.View.AgenteReporteGestionModo modo, bool mostrarFotos)
        {
            InitializeComponent();
            this.bindingSource.DataSource = model;
            this.xrPictureBoxLogo.BackColor = Color.FromArgb(52, 56, 60);
            this.cellTitulo.BackColor = Color.FromArgb(52, 56, 60);
            this.cellRangoFechas.BackColor = Color.FromArgb(52, 56, 60);
            this.cellAgente.BackColor = Color.FromArgb(52, 56, 60);
            this.cellAgente.Text = string.Format("Agente: {0}", viewBag.Agente);
            this.cellRangoFechas.Text = viewBag.RangoFechas;
            this.cellEmitidoEl.Text = DateTime.Now.ToString("g");
            this.cellEmitidoPor.Text = Rp3.Web.Mvc.Session.LogonName;
            this.rowObservacion.Visible = modo == AgendaComercial.Models.Consulta.View.AgenteReporteGestionModo.Detallado;
            this.rowFotos.Visible = mostrarFotos;
            this.foto1.Visible = mostrarFotos;
            this.foto2.Visible = mostrarFotos;
            this.foto3.Visible = mostrarFotos;
            DevExpress.XtraCharts.Series serie = this.xrChartMarcaciones.SeriesSerializable[0];
            DevExpress.XtraCharts.SeriesPoint pointAsistencias = new DevExpress.XtraCharts.SeriesPoint("Asistencias", new object[] { ((object)(model.Asistencias)) }, 0);
            DevExpress.XtraCharts.SeriesPoint pointAusencias = new DevExpress.XtraCharts.SeriesPoint("Ausencias", new object[] { ((object)(model.Ausencias)) }, 1);
            DevExpress.XtraCharts.SeriesPoint pointAtrasos = new DevExpress.XtraCharts.SeriesPoint("Atrasos", new object[] { ((object)(model.Atrasos)) }, 2);
            serie.Points.AddRange(new DevExpress.XtraCharts.SeriesPoint[] { pointAsistencias, pointAusencias, pointAtrasos});

            this.GroupFooterSinEfectividad.Visible = model.Gestionados.Count == 0 & model.ClientesNoVisitados.Count == 0 & model.Proximos.Count == 0;
            this.DetailEfectividad.Visible = !this.GroupFooterSinEfectividad.Visible;

            this.GroupHeaderGruposResumen.Visible = model.GruposResumen.Count != 0;
            this.DetailGruposResumen.Visible = model.GruposResumen.Count != 0;
            this.GroupFooterGruposResumen.Visible = model.GruposResumen.Count != 0;
            this.GroupFooterSinResumen.Visible = model.GruposResumen.Count == 0;

            this.GroupHeaderGruposClientesCreados.Visible = model.ClientesCreados.Count != 0;
            this.DetailGruposClientesCreados.Visible = model.ClientesCreados.Count != 0;
            this.DetailReportClientesCreados.Visible = model.ClientesCreados.Count != 0;

            this.GroupHeaderGruposNoVisitados.Visible = model.GruposNoVisitados.Count != 0;
            this.DetailGruposNoVisitados.Visible = model.GruposNoVisitados.Count != 0;
            this.DetailReportNoVisitadosClientes.Visible = model.GruposNoVisitados.Count != 0;
            this.GroupFooterSinNoVisitados.Visible = model.GruposNoVisitados.Count == 0;

            this.GroupHeaderGruposVisitados.Visible = model.GruposVisitados.Count != 0;
            this.DetailGruposVisitados.Visible = model.GruposVisitados.Count != 0;
            this.DetailReportVisitadosClientes.Visible = model.GruposVisitados.Count != 0;
            this.GroupFooterSinVisitados.Visible = model.GruposVisitados.Count == 0;

            this.GroupHeaderGruposPendientes.Visible = model.GruposPendientes.Count != 0;
            this.DetailGruposPendientes.Visible = model.GruposPendientes.Count != 0;
            this.DetailReportPendientesClientes.Visible = model.GruposPendientes.Count != 0;
            this.GroupFooterSinPendientes.Visible = model.GruposPendientes.Count == 0;

            this.GroupHeaderMarcaciones.Visible = model.Marcaciones.Count != 0;
            this.DetailMarcaciones.Visible = model.Marcaciones.Count != 0;
            this.GroupFooterMarcaciones.Visible = model.Marcaciones.Count != 0;
            this.DetailReportGraficoMarcaciones.Visible = model.Marcaciones.Count != 0;
            this.GroupFooterSinMarcaciones.Visible = model.Marcaciones.Count == 0;

            this.GroupFooterSinClientesCreados.Visible = model.ClientesCreados.Count == 0;

            this.GroupHeaderPermisos.Visible = model.PermisosJustificaciones.Count != 0;
            this.DetailPermisos.Visible = model.PermisosJustificaciones.Count != 0;
            this.GroupFooterSinPermisos.Visible = model.PermisosJustificaciones.Count == 0;

            this.GroupHeaderOportunidades.Visible = model.Oportunidades.Count != 0;
            this.DetailOportunidades.Visible = model.Oportunidades.Count != 0;
            this.GroupFooterOportunidades.Visible = model.Oportunidades.Count != 0;
            this.GroupFooterSinOportunidades.Visible = model.Oportunidades.Count == 0;

        }
    }
}
