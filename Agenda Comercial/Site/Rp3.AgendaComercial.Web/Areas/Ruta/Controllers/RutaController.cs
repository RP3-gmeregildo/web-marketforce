using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Rp3.Data.Models.Security;
using Rp3.Web.Mvc.Application.Security.Models;
using Rp3.Web.Mvc;
using Rp3.AgendaComercial.Models.Ruta;
using Rp3.Web.Mvc.Html;
using Rp3.AgendaComercial.Models.Ruta.View;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Rp3.AgendaComercial.Models.General.View;
using DevExpress.Web.Mvc;
using System.Web.UI.WebControls;

namespace Rp3.AgendaComercial.Web.Ruta.Controllers
{
    public partial class RutaController : Rp3.AgendaComercial.Web.Controllers.AgendaComercialController
    {
        //
        // GET: /Ruta/Ruta/

        [Rp3.Web.Mvc.Authorize("RUTA", "QUERY", "AGENDACOMERCIAL")]
        public ActionResult Index()
        {
            return View(GetListIndex());
        }

        public ActionResult GridViewIndex()
        {
            return PartialView("_GridViewIndex", GetListIndex());
        }

        public List<Models.Ruta.Ruta> GetListIndex()
        {
            var list = DataBase.Rutas.GetQueryable(p => p.Estado != Rp3.AgendaComercial.Models.Constantes.Estado.Eliminado, includeProperties: "EstadoGeneralValue");

            AgenteView agente = (AgenteView)Rp3.Web.Mvc.Session.GetValue("AgenteView");

            if (agente != null && agente.EsAgente)
            {
                switch (agente.CargoRol)
                {
                    case AgenteCargoRol.Agente:
                        list = list.Where(p => p.IdRuta == agente.IdRuta);
                        break;

                    case AgenteCargoRol.Supervisor:
                        var idRutas = DataBase.Agentes.GetAgentesPermitidos(agente.IdAgente).Where(p => p.IdRuta != null).Select(p => p.IdRuta ?? 0).ToList<int>();
                        list = list.Where(p => idRutas.Contains(p.IdRuta));
                        break;
                }
            }

            return list.ToList();
        }

        private void InicializarEdit()
        {
            ViewBag.Lotes = DataBase.Lotes.Get(p => p.Estado == Models.Constantes.Estado.Activo).ToList();

            ViewBag.CalendariosSelectList = DataBase.Calendarios.Get(p => p.Estado == Models.Constantes.Estado.Activo).ToSelectList(includeNullItem: true);
        }

        private void InicializarTab(Rp3.AgendaComercial.Models.Ruta.Ruta ruta)
        {
            TabCollection tabCollection = new TabCollection();

            tabCollection.Add("tabprogramacion", TabTarget.HtmlElement, "#tabprogramacion",
                Rp3.AgendaComercial.Resources.TitleFor.Programacion + " " + Rp3.AgendaComercial.Resources.TitleFor.Ruta + ": " + ruta.Descripcion, true);
            tabCollection.Add("tabvistaprevia", TabTarget.HtmlElement, "#tabvistaprevia", Rp3.AgendaComercial.Resources.TitleFor.VistaPrevia, false);

            ViewBag.TabCollection = tabCollection;
        }

        [Rp3.Web.Mvc.Authorize("RUTA", "NEW", "AGENDACOMERCIAL")]
        public ActionResult Create()
        {
            InicializarEdit();

            Models.Ruta.Ruta model = new Models.Ruta.Ruta();
            model.Estado = Models.Constantes.Estado.Activo;

            model.RutaLotes = new List<RutaLote>();
            model.RutaDetalles = new List<RutaDetalle>();
            model.RutaIncluirs = new List<RutaIncluir>();

            return View(model);
        }

        private Models.Ruta.Ruta GetModel(int id)
        {
            Models.Ruta.Ruta result = DataBase.Rutas.Get(p => p.IdRuta == id, includeProperties: "RutaLotes, RutaDetalles, RutaIncluirs, RutaExcluirs").SingleOrDefault();

            return result;
        }

        [Rp3.Web.Mvc.Authorize("RUTA", "EDIT", "AGENDACOMERCIAL")]
        public ActionResult Edit(int id)
        {
            InicializarEdit();

            var model = GetModel(id);

            SetUbicacion(model.RutaDetalles);
            SetUbicacion(model.RutaIncluirs);

            return View(model);
        }

        [Rp3.Web.Mvc.Authorize("RUTA", "DETAIL", "AGENDACOMERCIAL")]
        public ActionResult Detail(int id)
        {
            var model = GetModel(id);
            model.ReadOnly = true;

            ViewBag.Lotes = model.RutaLotes.Select(p => p.Lote).Distinct().ToList();

            SetUbicacion(model.RutaDetalles);
            SetUbicacion(model.RutaIncluirs);

            return View(model);
        }

        [Rp3.Web.Mvc.Authorize("RUTA", "DELETE", "AGENDACOMERCIAL")]
        public ActionResult Delete(int id)
        {
            var model = GetModel(id);
            model.ReadOnly = true;

            ViewBag.Lotes = model.RutaLotes.Select(p => p.Lote).Distinct().ToList();

            SetUbicacion(model.RutaDetalles);
            SetUbicacion(model.RutaIncluirs);

            return View(model);
        }

        [Rp3.Web.Mvc.Authorize("RUTA", "REASSIGN", "AGENDACOMERCIAL")]
        public ActionResult Reassign(int id)
        {
            var model = GetModel(id);

            ViewBag.AgenteSelectList = DataBase.Agentes.GetAgentesPermitidos(this.Agente.IdAgente).Where(p=>p.IdRuta == null).OrderBy(p => p.Descripcion).ToSelectList(includeNullItem: true);

            return View(new Rp3.AgendaComercial.Web.Ruta.RutaReassign() { IdRuta = model.IdRuta, Ruta = model.Descripcion });
        }

        [Rp3.Web.Mvc.Authorize("RUTA", "PROCESS", "AGENDACOMERCIAL")]
        public ActionResult Process(int id)
        {
            Models.Ruta.Ruta modelUpdate = GetModel(id);

            try
            {
                ProcessClientes(modelUpdate);

                DataBase.Rutas.Update(modelUpdate);
                DataBase.Save();

                SetUbicacion(modelUpdate.RutaDetalles);

                this.AddSuccessMessage(@Rp3.AgendaComercial.Resources.LegendFor.RutaProcesada);
            }
            catch
            {
                this.AddDefaultErrorMessage();
            }

            ViewBag.IsProcess = true;

            return View(modelUpdate);
        }

        private void ProcessClientes(Models.Ruta.Ruta modelUpdate)
        {
            var listUpdate = new List<RutaDetalle>();

            var lotes = modelUpdate.RutaLotes.Select(p => p.IdLote).ToList<int>();

            var clientes = DataBase.LoteDetalles.Get(p => lotes.Contains(p.IdLote), includeProperties: "ClienteDireccion");

            foreach (var cliente in clientes)
                if (listUpdate.Where(p => p.IdCliente == cliente.IdCliente && p.IdClienteDireccion == cliente.IdClienteDireccion).Count() == 0)
                    listUpdate.Add(new RutaDetalle()
                    {
                        IdRuta = modelUpdate.IdRuta,
                        IdCliente = cliente.IdCliente,
                        IdClienteDireccion = cliente.IdClienteDireccion,
                        ClienteDireccion = cliente.ClienteDireccion
                    });

            foreach (var cliente in modelUpdate.RutaIncluirs)
                if (listUpdate.Where(p => p.IdCliente == cliente.IdCliente && p.IdClienteDireccion == cliente.IdClienteDireccion).Count() == 0)
                    listUpdate.Add(new RutaDetalle()
                    {
                        IdRuta = modelUpdate.IdRuta,
                        IdCliente = cliente.IdCliente,
                        IdClienteDireccion = cliente.IdClienteDireccion,
                        ClienteDireccion = cliente.ClienteDireccion
                    });

            DataBase.RutaDetalles.Update(listUpdate, modelUpdate.RutaDetalles);
        }


        [HttpPost]
        [PreventSpam(Order = 0)]
        [Rp3.Web.Mvc.Authorize("RUTA", "REASSIGN", "AGENDACOMERCIAL")]
        public ActionResult Reassign(Rp3.AgendaComercial.Web.Ruta.RutaReassign model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    DataBase.Rutas.ReasignarRuta(model.IdRuta, model.IdAgente, this.UserLogonName);

                    return RedirectToAction("Index");
                }
            }
            catch
            {
                this.AddDefaultErrorMessage();
            }

            ViewBag.AgenteSelectList = DataBase.Agentes.GetAgentesPermitidos(this.Agente.IdAgente).Where(p => p.IdRuta == null).OrderBy(p => p.Descripcion).ToSelectList(includeNullItem: true);

            return View(model);
        }

        [HttpPost]
        [PreventSpam(Order = 0)]
        [Rp3.Web.Mvc.Authorize("RUTA", "NEW", "AGENDACOMERCIAL", Order = 1)]
        public ActionResult Create(Models.Ruta.Ruta model, string[] lotes, string[] clienteIncluir, string[] clienteExcluir)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.AsignarId();

                    model.Estado = Rp3.AgendaComercial.Models.Constantes.Estado.Activo;
                    model.EstadoTabla = Rp3.AgendaComercial.Models.Constantes.Estado.Tabla;
                    model.UsrIng = this.UserLogonName;
                    model.FecIng = this.GetCurrentDateTime();

                    string DLote = string.Join(",", lotes);
                    if (!String.IsNullOrEmpty(DLote) && DLote[0] == ',') { DLote = DLote.Remove(0, 1); }

                    model.Lotes = DLote;
                    model.RutaIncluirs = GetIncluirListDetail(model.IdRuta, clienteIncluir);
                    model.RutaExcluirs = GetExcluirListDetail(model.IdRuta, clienteExcluir);

                    model.RutaDetalles = new List<RutaDetalle>();

                    ProcessClientes(model);

                    SetResumen(model);

                    DataBase.Rutas.Insert(model);
                    DataBase.Save();

                    this.AddDefaultSuccessMessage();
                    return RedirectToAction("Index", model);
                }
            }
            catch
            {
                this.AddDefaultErrorMessage();
            }
            InicializarEdit();
            model.RutaIncluirs = GetIncluirListDetail(model.IdRuta, clienteIncluir);
            SetUbicacion(model.RutaIncluirs);
            return View(model);
        }

        [HttpPost]
        [PreventSpam(Order = 0)]
        [Rp3.Web.Mvc.Authorize("RUTA", "EDIT", "AGENDACOMERCIAL", Order = 1)]
        public ActionResult Edit(Models.Ruta.Ruta model, string[] lotes, string[] clienteIncluir)
        {
            Models.Ruta.Ruta modelUpdate = GetModel(model.IdRuta);

            try
            {
                if (ModelState.IsValid)
                {
                    modelUpdate.Descripcion = model.Descripcion;
                    modelUpdate.IdCalendario = model.IdCalendario;

                    modelUpdate.Estado = model.Estado;
                    modelUpdate.UsrMod = this.UserLogonName;
                    modelUpdate.FecMod = this.GetCurrentDateTime();

                    DataBase.RutaLotes.Update(model.RutaLotes, modelUpdate.RutaLotes);

                    var listIncluirEdit = GetIncluirListDetail(model.IdRuta, clienteIncluir);
                    DataBase.RutaIncluirs.Update(listIncluirEdit, modelUpdate.RutaIncluirs);

                    ProcessClientes(modelUpdate);

                    SetResumen(modelUpdate);

                    DataBase.Rutas.Update(modelUpdate);
                    DataBase.Save();

                    VerificarDependencia(modelUpdate);

                    this.AddDefaultSuccessMessage();
                    return RedirectToAction("Index", model);
                }
            }
            catch
            {
                this.AddDefaultErrorMessage();
            }

            InicializarEdit();

            model.RutaIncluirs = GetIncluirListDetail(model.IdRuta, clienteIncluir);
            model.RutaDetalles = modelUpdate.RutaDetalles;

            SetUbicacion(model.RutaDetalles);
            SetUbicacion(model.RutaIncluirs);

            return View(model);
        }

        #region Arrays

        public List<RutaIncluir> GetIncluirListDetail(int IdRuta, string[] clienteIncluir)
        {
            List<RutaIncluir> listEdit = new List<RutaIncluir>();
            if (clienteIncluir != null)
            {
                foreach (var insert in clienteIncluir)
                {
                    string[] keyParts = insert.Split('-');
                    RutaIncluir detalle = new RutaIncluir()
                    {
                        IdRuta = IdRuta,
                        IdCliente = Convert.ToInt32(keyParts[0]),
                        IdClienteDireccion = Convert.ToInt32(keyParts[1])
                    };
                    listEdit.Add(detalle);
                }
                var idCliente = listEdit.Select(p => p.IdCliente).ToList<int>();
                var clientes = DataBase.Clientes.Get(p => idCliente.Contains(p.IdCliente), includeProperties: "ClienteDirecciones").ToList();
                foreach (var det in listEdit)
                {
                    var item = clientes.Where(p => p.IdCliente == det.IdCliente).FirstOrDefault();
                    if (item != null)
                    {
                        var direccion = item.ClienteDirecciones.Where(p => p.IdClienteDireccion == det.IdClienteDireccion).FirstOrDefault();
                        if (direccion != null)
                            det.ClienteDireccion = direccion;
                    }
                }
            }
            return listEdit;
        }
        public List<RutaExcluir> GetExcluirListDetail(int IdRuta, string[] clienteExcluir)
        {
            List<RutaExcluir> listEdit = new List<RutaExcluir>();
            if (clienteExcluir != null)
            {
                foreach (var insert in clienteExcluir)
                {
                    string[] keyParts = insert.Split('-');
                    RutaExcluir detalle = new RutaExcluir()
                    {
                        IdRuta = IdRuta,
                        IdCliente = Convert.ToInt32(keyParts[0]),
                        IdClienteDireccion = Convert.ToInt32(keyParts[1])
                    };
                    listEdit.Add(detalle);
                }
                var idCliente = listEdit.Select(p => p.IdCliente).ToList<int>();
                var clientes = DataBase.Clientes.Get(p => idCliente.Contains(p.IdCliente), includeProperties: "ClienteDirecciones").ToList();
                foreach (var det in listEdit)
                {
                    var item = clientes.Where(p => p.IdCliente == det.IdCliente).FirstOrDefault();
                    if (item != null)
                    {
                        var direccion = item.ClienteDirecciones.Where(p => p.IdClienteDireccion == det.IdClienteDireccion).FirstOrDefault();
                        if (direccion != null)
                            det.ClienteDireccion = direccion;
                    }
                }
            }
            return listEdit;
        }

        #endregion Arrays

        [HttpPost]
        [PreventSpam(Order = 0)]
        [Rp3.Web.Mvc.Authorize("RUTA", "DELETE", "AGENDACOMERCIAL")]
        public ActionResult Delete(Models.Ruta.Ruta model)
        {
            try
            {
                Models.Ruta.Ruta modelDelete = GetModel(model.IdRuta);
                modelDelete.Estado = Rp3.AgendaComercial.Models.Constantes.Estado.Eliminado;
                modelDelete.UsrMod = this.UserLogonName;
                modelDelete.FecMod = this.GetCurrentDateTime();
                DataBase.Rutas.Update(modelDelete);
                DataBase.Save();

                VerificarDependencia(modelDelete);

                //DataBase.Rutas.Delete(modelDelete);
                //DataBase.Save();

                this.AddDefaultSuccessMessage();
                return RedirectToAction("Index");
            }
            catch
            {
                this.AddDefaultErrorMessage();
            }
            return RedirectToAction("Index", model);
        }

        public ActionResult UbicacionMapMarkerClient()
        {
            ViewBag.InitMapa = true;
            return PartialView("_UbicacionMapMarkerClient");
        }

        #region Ubicacion

        public void SetUbicacion(List<RutaIncluir> ubicaciones)
        {
            var clientes = ubicaciones.Select(p => p.IdCliente).ToList<int>();

            var direcciones = DataBase.ClienteDirecciones.Get(p => clientes.Contains(p.IdCliente)).ToList();

            foreach (var item in direcciones)
            {
                var ubicacion = ubicaciones.Where(p => p.IdCliente == item.IdCliente && p.IdClienteDireccion == item.IdClienteDireccion).FirstOrDefault();

                if (ubicacion != null)
                {
                    ubicacion.Latitud = item.Latitud;
                    ubicacion.Longitud = item.Longitud;
                    ubicacion.MarkerIndex = 0;
                }
            }

            SetMarkers(ubicaciones);
        }

        private void SetMarkers(List<RutaIncluir> ubicaciones)
        {
            int markerIndex = 1;

            foreach (var item in ubicaciones.OrderBy(p => p.ClienteDireccion.Cliente.Apellido1))
            {
                item.MarkerIndex = markerIndex;
                markerIndex++;

                if (markerIndex >= 100)
                    markerIndex = 100;
            }
        }

        public void SetUbicacion(List<RutaDetalle> ubicaciones)
        {
            var clientes = ubicaciones.Select(p => p.IdCliente).ToList<int>();

            var direcciones = DataBase.ClienteDirecciones.Get(p => clientes.Contains(p.IdCliente)).ToList();

            foreach (var item in direcciones)
            {
                var ubicacion = ubicaciones.Where(p => p.IdCliente == item.IdCliente && p.IdClienteDireccion == item.IdClienteDireccion).FirstOrDefault();

                if (ubicacion != null)
                {
                    ubicacion.Latitud = item.Latitud;
                    ubicacion.Longitud = item.Longitud;
                }

            }

            SetMarkers(ubicaciones);
        }

        private void SetMarkers(List<RutaDetalle> ubicaciones)
        {
            int markerIndex = 1;

            foreach (var item in ubicaciones.OrderBy(p => p.ClienteDireccion.Cliente.Apellido1))
            {
                item.MarkerIndex = markerIndex;
                markerIndex++;

                if (markerIndex >= 100)
                    markerIndex = 100;
            }
        }

        #endregion Ubicacion


        [ChildAction(Order = 1)]
        [Rp3.Web.Mvc.Authorize("RUTA", "QUERY", "AGENDACOMERCIAL", Order = 2)]
        [HttpGet]
        public ActionResult ConsultaRutaDetalle(string ruta, string lote, string excluir, string incluir, string pagina, string numreg, string isfilter, string buscar)
        {

            var data = DataBase.Rutas.ConsultaRutaDetalleSP(ruta, lote, excluir, incluir, pagina, numreg, isfilter, buscar);
            RutaConsulta rutaConsulta = new RutaConsulta();
            List<RutaDetalleGV> detalleRuta = new List<RutaDetalleGV>();
            detalleRuta = data.ToList();
            rutaConsulta.RutaDetalleGVs = detalleRuta;

            return PartialView("_RutaDetalle", rutaConsulta);
        }

        [ChildAction(Order = 1)]
        [Rp3.Web.Mvc.Authorize("RUTA", "EDIT", "AGENDACOMERCIAL", Order = 2)]
        [HttpPost]
        public ActionResult GrabarRutaDetalle(string ruta, string descripcion, int? idCalendario, string estado, string lote, string excluir, string incluir)
        {
            try
            {
                DataBase.Rutas.GrabarRutaDetalleSP(ruta, descripcion, idCalendario, estado, lote, excluir, incluir, this.UserLogonName);
                //this.AddSuccessMessage(@Rp3.AgendaComercial.Resources.LegendFor.RutaProcesada);

                int idRuta = Convert.ToInt32(ruta);
                var model = DataBase.Rutas.Get(p => p.IdRuta == idRuta).FirstOrDefault();

                if (model != null)
                {
                    SetResumen(model);
                    DataBase.Rutas.Update(model);
                    DataBase.Save();
                }

                this.AddDefaultSuccessMessage();
            }
            catch
            {
                this.AddDefaultErrorMessage();
            }
            return Json();
        }


        private void VerificarDependencia(Models.Ruta.Ruta ruta)
        {
            if (ruta.Estado == Rp3.AgendaComercial.Models.Constantes.Estado.Inactivo ||
                ruta.Estado == Rp3.AgendaComercial.Models.Constantes.Estado.Eliminado)
            {
                DataBase.Rutas.EliminarDependenciaRuta(ruta.IdRuta, this.UserLogonName);
            }
        }


        public void SetResumen(Models.Ruta.Ruta model)
        {
            string lotes = String.Empty;

            foreach (var lote in model.RutaLotes)
            {
                var loteItem = DataBase.Lotes.Get(p => p.IdLote == lote.IdLote).FirstOrDefault();

                if (loteItem != null)
                {
                    if (!String.IsNullOrEmpty(lotes))
                        lotes += ", ";

                    lotes += loteItem.Descripcion;
                }
            }

            model.LoteResumen = lotes;
        }

        [ChildAction(Order = 1)]
        [Rp3.Web.Mvc.Authorize("RUTA", "QUERY", "AGENDACOMERCIAL", Order = 2)]
        public string GetPreview(int idRuta, int start = 0, int end = 0)
        {
            DateTime time1 = new DateTime((start * Convert.ToInt64(Math.Pow(10, 9))) / 100);
            time1 = time1.AddYears(1970 - 1);

            DateTime time2 = new DateTime((end * Convert.ToInt64(Math.Pow(10, 9))) / 100);
            time2 = time2.AddYears(1970 - 1);

            var data_json = DataBase.Rutas.GetProgramacionPreview(idRuta, time1, time2);

            string result = string.Empty;
            try
            {
                result = JsonConvert.SerializeObject(data_json, Formatting.None, new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });
            }
            catch
            {

            }

            return result;
        }

        [ChildAction(Order = 1)]
        [Rp3.Web.Mvc.Authorize("RUTA", "QUERY", "AGENDACOMERCIAL", Order = 2)]
        public ActionResult GetMapPreview(int idRuta, DateTime fecha)
        {
            var data = DataBase.Rutas.GetProgramacionPreview(idRuta, fecha, fecha);
            var calendar = DataBase.Calendarios.Get(p => p.IdCalendario == 1, includeProperties: "DiasLaborales, DiasNoLaborables").FirstOrDefault();

            var agendas = Rp3.AgendaComercial.Process.RutaOptima.GetRutaOptima(data, calendar);

            if (agendas == null)
                agendas = new List<ProgramacionPreview>();

            SetMarkers(agendas);
            ViewBag.MapRoute = true;
            ViewBag.MapStart = true;

            return PartialView("_UbicacionMapMarker", agendas);
        }

        private void SetMarkers(List<ProgramacionPreview> ubicaciones)
        {
            int markerIndex = 1;

            foreach (var item in ubicaciones)
            {
                if (!item.Partida)
                {
                    item.MarkerIndex = markerIndex;

                    if (!String.IsNullOrEmpty(item.title) && !String.IsNullOrEmpty(item.Direccion))
                        item.Titulo = String.Format("{0} - {1}", item.title, item.Direccion);

                    markerIndex++;

                    if (markerIndex >= 100)
                        markerIndex = 100;
                }
                else
                {
                    item.MarkerIndex = -1;
                }
            }
        }


        #region Export

        public ActionResult ExportToXls()
        {
            var data = GetListIndex();

            return GridViewExtension.ExportToXlsx(CreateExportGridViewSettings(), data);
        }
        public ActionResult ExportToPdf()
        {
            var data = GetListIndex();

            return GridViewExtension.ExportToPdf(CreateExportGridViewSettings(), data);
        }

        static GridViewSettings CreateExportGridViewSettings()
        {
            GridViewSettings settings = new GridViewSettings();

            settings.Name = "Export";
            settings.Width = Unit.Percentage(100);

            settings.Columns.Add("Descripcion", Rp3.AgendaComercial.Resources.LabelFor.Descripcion);
            settings.Columns.Add("Agente", Rp3.AgendaComercial.Resources.LabelFor.Agente);
            settings.Columns.Add("Calendario.Descripcion", Rp3.AgendaComercial.Resources.LabelFor.Calendario);
            settings.Columns.Add("CantidadClientes", Rp3.AgendaComercial.Resources.LabelFor.Clientes);
            settings.Columns.Add("LoteResumen", Rp3.AgendaComercial.Resources.LabelFor.Lotes);
            settings.Columns.Add("EstadoGeneralValue.Content", Rp3.AgendaComercial.Resources.LabelFor.Estado);

            settings.SettingsExport.Landscape = true;
            settings.SettingsExport.ReportHeader = settings.Name;   

            return settings;
        }

        #endregion Export
    }
}
