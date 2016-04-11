using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Rp3.Web.Mvc;
using Rp3.AgendaComercial.Models.General;
using Rp3.Web.Mvc.Html;
using Rp3.Data;
using System.IO;
using System.Drawing;
using Rp3.Web.Mvc.Utility;
using Rp3.AgendaComercial.Models;
using DevExpress.Web.Mvc;
using Rp3.Data.Models.Definition;
using Rp3.AgendaComercial.Models.General.View;
using DevExpress.Data;
using DevExpress.Data.Linq.Helpers;
using Rp3.AgendaComercial.Models.Ruta.View;
using Rp3.AgendaComercial.Web.Services.Ruta.Models;
using Rp3.AgendaComercial.Web.Ruta.Controllers;
using Rp3.AgendaComercial.Models.Ruta;
using System.Web.UI.WebControls;
using DevExpress.XtraPrinting;

namespace Rp3.AgendaComercial.Web.General.Controllers
{
    public static class ClienteBindingHandlers
    {
        static IQueryable Model { get { return ClienteController.GetListIndex(); } }

        public static void GetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e)
        {
            e.DataRowCount = Model.ApplyFilter(e.State.FilterExpression).Count();
        }
        public static void GetData(GridViewCustomBindingGetDataArgs e)
        {
            e.Data = Model.ApplyFilter(e.State.FilterExpression).ApplySorting(e.State.SortedColumns).Skip(e.StartDataRowIndex).Take(e.DataRowCount);
        }
    }

    public class ClienteController : Rp3.AgendaComercial.Web.Controllers.AgendaComercialController
    {
        //
        // GET: /General/Cliente/

        [Rp3.Web.Mvc.Authorize("CLIENTE", "QUERY", "AGENDACOMERCIAL")]
        public ActionResult Index()
        {
            InicializarColumns();
            return View();
        }

        public ActionResult GridViewIndex()
        {
            InicializarColumns();

            var viewModel = GridViewExtension.GetViewModel("gridViewIndex");
            if (viewModel == null)
                viewModel = CreateGridViewModel();
            return GridCustomActionCore(viewModel);
            //return PartialView("_GridViewIndex", GetListIndex().ToList());
        }

        #region Pagination & Sorting

        public ActionResult GridViewFilteringAction(GridViewColumnState column)
        {
            InicializarColumns();

            var viewModel = GridViewExtension.GetViewModel("gridViewIndex");
            viewModel.ApplyFilteringState(column);
            return GridCustomActionCore(viewModel);
        }

        public ActionResult GridViewSortingAction(GridViewColumnState column, bool reset)
        {
            InicializarColumns();

            var viewModel = GridViewExtension.GetViewModel("gridViewIndex");
            viewModel.SortBy(column, reset);
            return GridCustomActionCore(viewModel);
        }

        public ActionResult GridViewPagingAction(GridViewPagerState pager)
        {
            InicializarColumns();

            var viewModel = GridViewExtension.GetViewModel("gridViewIndex");
            viewModel.Pager.Assign(pager);
            return GridCustomActionCore(viewModel);
        }

        public ActionResult GridCustomActionCore(GridViewModel gridViewModel)
        {
            gridViewModel.ProcessCustomBinding(
                ClienteBindingHandlers.GetDataRowCount,
                ClienteBindingHandlers.GetData
            );

            return PartialView("_GridViewIndex", gridViewModel);
        }

        static GridViewModel CreateGridViewModel()
        {
            var viewModel = new GridViewModel();
            viewModel.KeyFieldName = "IdCliente";

            var col = viewModel.Columns.Add("NombresCompletos");
            col.SortOrder = ColumnSortOrder.Ascending;
            col.SortIndex = 0;

            viewModel.Pager.PageSize = 15;
            return viewModel;
        }

        #endregion Pagination & Sorting

        public static IQueryable<ClienteView> GetListIndex()
        {
            ContextService db = new ContextService();

            var list = db.ClienteViews.GetQueryable(p => p.Estado != Rp3.AgendaComercial.Models.Constantes.Estado.Eliminado);

            AgenteView agente = (AgenteView)Rp3.Web.Mvc.Session.GetValue("AgenteView");

            List<int> ids = new List<int>();

            if (agente != null && agente.EsAgente)
            {
                switch (agente.CargoRol)
                {
                    case AgenteCargoRol.Agente:
                        ids = db.RutaDetalles.Get(p => p.IdRuta == agente.IdRuta).Select(p => p.IdCliente).ToList<int>();
                        list = list.Where(p => ids.Contains(p.IdCliente));
                        break;

                    case AgenteCargoRol.Supervisor:
                        var idRutas = db.Agentes.GetAgentesPermitidos(agente.IdAgente).Where(p => p.IdRuta != null).Select(p => p.IdRuta ?? 0).ToList<int>();
                        ids = db.RutaDetalles.Get(p => idRutas.Contains(p.IdRuta)).Select(p => p.IdCliente).ToList<int>();
                        list = list.Where(p => ids.Contains(p.IdCliente));
                        break;
                }
            }

            return list;
        }

        List<ApplicationOptionColumn> columns;

        private void InicializarColumns()
        {
            if (columns == null)
                columns = DataBase.ApplicationOptionColumns.Get(p => p.ApplicationId == Rp3.Web.Mvc.Session.ApplicationId && p.OptionId == "CLIENTE" && p.Visible).ToList();

            ViewBag.Columns = columns;
        }

        List<ParametroClienteCampo> campoObligatorio;

        private void InicializarCampoObligatorio(int idCliente)
        {
            if (campoObligatorio == null)
                campoObligatorio = DataBase.ParametroClienteCampos.Get(p => p.EsObligatorio).ToList();

            ViewBag.CampoObligatorio = campoObligatorio;

            ViewBag.DireccionesSelectList = DataBase.ClienteDirecciones.Get(p => p.IdCliente == idCliente && p.Estado != Rp3.AgendaComercial.Models.Constantes.Estado.Eliminado).ToSelectList(valueMember: "KeyCombo", includeNullItem: false);
        }

        private void InicializarEdit()
        {
            ViewBag.TipoIdentificacionSelectList = DataBase.IdentificationTypes.Get().ToSelectList(includeNullItem: true);
            ViewBag.TipoClienteSelectList = DataBase.TipoClientes.Get(p => p.Estado == Models.Constantes.Estado.Activo).ToSelectList(includeNullItem: true);
            ViewBag.CanalSelectList = DataBase.Canales.Get(p => p.Estado == Models.Constantes.Estado.Activo).ToSelectList(includeNullItem: true);
        }

        private void LoadLoteRuta(int IdCliente)
        {
            ViewBag.Lotes = DataBase.Lotes.Get(p => p.Estado == Models.Constantes.Estado.Activo && p.LoteDetalles.Where(q => q.IdCliente == IdCliente).Count() > 0);
            ViewBag.Rutas = DataBase.Rutas.Get(p => p.Estado == Models.Constantes.Estado.Activo && p.RutaDetalles.Where(q => q.IdCliente == IdCliente).Count() > 0);
        }

        private void InicializarTab(bool isNew = false)
        {
            TabCollection tabCollection = new TabCollection();

            tabCollection.Add("tabdatosgenerales", TabTarget.HtmlElement, "#tabdatosgenerales", Rp3.AgendaComercial.Resources.TitleFor.DatosGenerales, true);

            if (!isNew)
            {
                tabCollection.Add("tabdirecciones", TabTarget.HtmlElement, "#tabdirecciones", Rp3.AgendaComercial.Resources.TitleFor.Direcciones, false);
                tabCollection.Add("tabcontactos", TabTarget.HtmlElement, "#tabcontactos", Rp3.AgendaComercial.Resources.TitleFor.Contactos, false);
                tabCollection.Add("tabloteruta", TabTarget.HtmlElement, "#tabloteruta", Rp3.AgendaComercial.Resources.TitleFor.LoteRuta, false);
                tabCollection.Add("tabestadistica", TabTarget.HtmlElement, "#tabestadistica", Rp3.AgendaComercial.Resources.TitleFor.Estadisticas, false);
            }

            ViewBag.TabCollection = tabCollection;
        }

        private void InitAgente()
        {
            AgenteView agente = (AgenteView)Rp3.Web.Mvc.Session.GetValue("AgenteView");
            ViewBag.TieneRuta = agente != null && agente.EsAgente && agente.IdRuta != null && agente.CargoRol == AgenteCargoRol.Agente;
        }

        private void SetMarkers(List<ClienteDireccion> direcciones)
        {
            int markerIndex = 1;

            foreach (var item in direcciones.OrderBy(p => p.TipoDireccionGeneralValue.Reference01))
            {
                item.MarkerIndex = markerIndex;
                markerIndex++;

                if (markerIndex >= 100)
                    markerIndex = 100;
            }
        }

        [Rp3.Web.Mvc.Authorize("CLIENTE", "NEW", "AGENDACOMERCIAL")]
        public ActionResult Create()
        {
            InicializarEdit();
            InicializarTab(true);
            InicializarCampoObligatorio(0);

            Cliente model = new Cliente();
            model.ClienteDato = new ClienteDato();

            model.Estado = Models.Constantes.Estado.Activo;
            model.ClienteDato.EstadoCivil = Models.Constantes.EstadoCivil.Soltero;
            model.ClienteDato.Genero = Models.Constantes.Genero.Masculino;
            model.IdTipoIdentificacion = Models.Constantes.IdentificationType.RUC;
            model.TipoPersona = Models.Constantes.TipoPersona.Juridica;

            InitAgente();
            ViewBag.IsNew = true;

            return View(model);
        }

        private Cliente GetModel(int id)
        {
            Cliente result = DataBase.Clientes.Get(p => p.IdCliente == id, includeProperties: "ClienteDato, ClienteDirecciones, ClienteContactos, ClienteDirecciones.EstadoGeneralValue").SingleOrDefault();

            return result;
        }

        private ClienteContacto GetModel(int id, int idContacto)
        {
            ClienteContacto result = DataBase.ClienteContactos.Get(p => p.IdCliente == id && p.IdClienteContacto == idContacto).SingleOrDefault();

            return result;
        }

        [Rp3.Web.Mvc.Authorize("CLIENTE", "EDIT", "AGENDACOMERCIAL")]
        public ActionResult Edit(int id)
        {
            InicializarEdit();
            InicializarTab();

            InicializarCampoObligatorio(id);

            LoadLoteRuta(id);

            var model = GetModel(id);

            SetMarkers(model.ClienteDirecciones);

            return View(model);
        }

        [Rp3.Web.Mvc.Authorize("CLIENTE", "DETAIL", "AGENDACOMERCIAL")]
        public ActionResult Detail(int id)
        {
            InicializarTab();
            LoadLoteRuta(id);

            var model = GetModel(id);
            model.ReadOnly = true;
            ViewBag.ReadOnly = true;

            SetMarkers(model.ClienteDirecciones);

            ViewBag.MapHeight = "300px";

            return View(model);
        }

        [Rp3.Web.Mvc.Authorize("CLIENTE", "DELETE", "AGENDACOMERCIAL")]
        public ActionResult Delete(int id)
        {
            InicializarTab();
            LoadLoteRuta(id);

            var model = GetModel(id);
            model.ReadOnly = true;
            ViewBag.ReadOnly = true;

            SetMarkers(model.ClienteDirecciones);

            ViewBag.MapHeight = "300px";

            return View(model);
        }

        [HttpPost]
        [PreventSpam(Order = 0)]
        [Rp3.Web.Mvc.Authorize("CLIENTE", "NEW", "AGENDACOMERCIAL", Order = 1)]
        public ActionResult Create(Cliente model, bool asignarRuta, bool editar)
        {
            AgenteView agente = (AgenteView)Rp3.Web.Mvc.Session.GetValue("AgenteView");

            try
            {
                if (ModelState.IsValid)
                {
                    model.AsignarId();

                    if (model.TipoPersona == Models.Constantes.TipoPersona.Juridica || model.TipoPersona == Models.Constantes.TipoPersona.JuridicaPublica)
                    {
                        model.ClienteDato.FechaNacimiento = null;
                        model.IdTipoIdentificacion = Constantes.IdentificationType.RUC;
                    }

                    if (model.TipoPersona == Models.Constantes.TipoPersona.Natural)
                        model.ClienteDato.InicioActividad = null;

                    if (!String.IsNullOrEmpty(model.Identificacion))
                    {
                        model.Identificacion = model.Identificacion.Trim();

                        if (!Rp3.Common.Validators.IdentificationType.IdentificationTypeValidator.IsValid(model.IdTipoIdentificacion, model.TipoPersona, model.Identificacion))
                        {
                            int longitud = 0;

                            switch (model.IdTipoIdentificacion)
                            {
                                case Rp3.Common.Validators.IdentificationType.IdentificationTypeValidator.Codes.CEDULA:
                                    longitud = Rp3.Common.Validators.IdentificationType.IdentificationTypeValidator.Lenght.CEDULA;
                                    break;

                                case Rp3.Common.Validators.IdentificationType.IdentificationTypeValidator.Codes.RUC:
                                    longitud = Rp3.Common.Validators.IdentificationType.IdentificationTypeValidator.Lenght.RUC;
                                    break;

                                case Rp3.Common.Validators.IdentificationType.IdentificationTypeValidator.Codes.PASAPORTE:
                                    longitud = Rp3.Common.Validators.IdentificationType.IdentificationTypeValidator.Lenght.PASAPORTE;
                                    break;
                            }

                            var tipoIden = DataBase.IdentificationTypes.Get(p => p.IdentificationTypeId == model.IdTipoIdentificacion).FirstOrDefault();

                            if (longitud != model.Identificacion.Length && tipoIden != null)
                            {
                                this.AddErrorMessage(String.Format(Rp3.AgendaComercial.Resources.MessageFor.IdentificacionLongitudIncorrecta,
                                    model.Identificacion.Length, tipoIden.Name, longitud));
                            }
                            else
                            {
                                this.AddErrorMessage(String.Format(Rp3.AgendaComercial.Resources.MessageFor.IdentificacionValidacionIncorrecta, tipoIden.Name));
                            }
                            //this.AddErrorMessage(Rp3.AgendaComercial.Resources.MessageFor.IdentificacionIncorrecta);
                        }
                        else if (DataBase.Clientes.Exists(p => p.Estado != Models.Constantes.Estado.Eliminado && p.Identificacion == model.Identificacion && p.IdCliente != model.IdCliente))
                        {
                            this.AddErrorMessage(Rp3.AgendaComercial.Resources.MessageFor.IdentificacionRepetida);
                        }
                    }

                    if (model.ClienteDato.FechaNacimiento != null && model.ClienteDato.FechaNacimiento > this.GetCurrentDateTime())
                        this.AddErrorMessage(Rp3.AgendaComercial.Resources.MessageFor.FechaNacimientoMayorHoy);

                    if (model.ClienteDato.InicioActividad != null && model.ClienteDato.InicioActividad > this.GetCurrentDateTime())
                        this.AddErrorMessage(Rp3.AgendaComercial.Resources.MessageFor.FechaInicioActividadMayorHoy);

                    VerificarCampoObligatorio(model, true);

                    if (!this.MessageCollection.HasError())
                    {
                        model.ClienteDato.EstadoCivilTabla = Models.Constantes.EstadoCivil.Tabla;
                        model.ClienteDato.GeneroTabla = Models.Constantes.Genero.Tabla;

                        model.Estado = Rp3.AgendaComercial.Models.Constantes.Estado.Activo;
                        model.EstadoTabla = Models.Constantes.Estado.Tabla;

                        model.TipoPersonaTabla = Models.Constantes.TipoPersona.Tabla;
                        //model.TipoPersona = Models.Constantes.TipoPersona.Natural;
                        model.UsrIng = this.UserLogonName;
                        model.FecIng = this.GetCurrentDateTime();

                        model.ClienteDirecciones = new List<ClienteDireccion>();

                        if (!String.IsNullOrEmpty(model.Direccion))
                        {
                            model.ClienteDirecciones.Add(new ClienteDireccion()
                            {
                                IdCliente = model.IdCliente,
                                IdClienteDireccion = 1,
                                EstadoTabla = Models.Constantes.Estado.Tabla,
                                Estado = Models.Constantes.Estado.Activo,
                                TipoDireccionTabla = Models.Constantes.TipoDireccion.Tabla,
                                TipoDireccion = Models.Constantes.TipoDireccion.Trabajo,
                                Direccion = model.Direccion,
                                Referencia = model.DireccionReferencia,
                                IdCiudad = model.DireccionIdCiudad,
                                Descripcion = model.DireccionDescripcion,
                                Telefono1 = model.DireccionTelefono1,
                                Telefono2 = model.DireccionTelefono2,
                                AplicaRuta = true,
                                EsPrincipal = true,
                                Latitud = model.DireccionLatitud,
                                Longitud = model.DireccionLongitud
                            });
                        }

                        if (asignarRuta)
                        {
                            if (model.ClienteDirecciones.Count() == 0)
                            {
                                model.ClienteDirecciones.Add(new ClienteDireccion()
                                {
                                    IdCliente = model.IdCliente,
                                    IdClienteDireccion = 1,
                                    EstadoTabla = Models.Constantes.Estado.Tabla,
                                    Estado = Models.Constantes.Estado.Activo,
                                    TipoDireccionTabla = Models.Constantes.TipoDireccion.Tabla,
                                    TipoDireccion = Models.Constantes.TipoDireccion.Trabajo,
                                    Direccion = Rp3.AgendaComercial.Resources.LabelFor.SinEspecificar,
                                    AplicaRuta = true,
                                    EsPrincipal = true
                                });
                            }

                            var ruta = DataBase.Rutas.Get(p => p.IdRuta == agente.IdRuta).FirstOrDefault();

                            if (ruta != null)
                            {
                                var rutaIncluir = new RutaIncluir() { IdRuta = agente.IdRuta ?? 0, IdCliente = model.IdCliente, IdClienteDireccion = 1 };
                                var rutadetalle = new RutaDetalle() { IdRuta = agente.IdRuta ?? 0, IdCliente = model.IdCliente, IdClienteDireccion = 1 };

                                DataBase.RutaIncluirs.Insert(rutaIncluir);
                                DataBase.RutaDetalles.Insert(rutadetalle);

                                ruta.FecMod = this.GetCurrentDateTime();
                                DataBase.Rutas.Update(ruta);
                            }
                        }

                        DataBase.Clientes.Insert(model);
                        DataBase.Save();
                        DataBase.Zonas.Procesar();

                        if (editar)
                            return RedirectToAction("Edit", new { id = model.IdCliente });
                        else 
                            return RedirectToAction("Index");
                    }
                }
            }
            catch
            {
                this.AddDefaultErrorMessage();
            }

            InicializarEdit();
            InicializarTab(true);
            InicializarCampoObligatorio(0);

            InitAgente();
            ViewBag.IsNew = true;

            return View(model);
        }

        private void VerificarCampoObligatorio(Cliente model, bool isNew)
        {
            var campos = DataBase.ParametroClienteCampos.Get(p => p.EsObligatorio).OrderBy(p => p.Prioridad).ToList();

            foreach (var campo in campos)
            {
                if (model.TipoPersona == Models.Constantes.TipoPersona.Natural)
                {
                    switch (campo.IdCampo)
                    {
                        case "Ape1Natural": if (String.IsNullOrEmpty(model.Apellido1)) this.AddErrorMessage(String.Format(Rp3.AgendaComercial.Resources.MessageFor.ValidacionCampoObligatorio, campo.Nombre)); break;
                        case "Ape2Natural": if (String.IsNullOrEmpty(model.Apellido2)) this.AddErrorMessage(String.Format(Rp3.AgendaComercial.Resources.MessageFor.ValidacionCampoObligatorio, campo.Nombre)); break;
                        case "EstCiv": if (String.IsNullOrEmpty(model.ClienteDato.EstadoCivil)) this.AddErrorMessage(String.Format(Rp3.AgendaComercial.Resources.MessageFor.ValidacionCampoObligatorio, campo.Nombre)); break;
                        case "FecNac": if (model.ClienteDato.FechaNacimiento == null) this.AddErrorMessage(String.Format(Rp3.AgendaComercial.Resources.MessageFor.ValidacionCampoObligatorio, campo.Nombre)); break;
                        case "Gen": if (String.IsNullOrEmpty(model.ClienteDato.Genero)) this.AddErrorMessage(String.Format(Rp3.AgendaComercial.Resources.MessageFor.ValidacionCampoObligatorio, campo.Nombre)); break;
                        case "Nom1Natural": if (String.IsNullOrEmpty(model.Nombre1)) this.AddErrorMessage(String.Format(Rp3.AgendaComercial.Resources.MessageFor.ValidacionCampoObligatorio, campo.Nombre)); break;
                        case "Nom2Natural": if (String.IsNullOrEmpty(model.Nombre2)) this.AddErrorMessage(String.Format(Rp3.AgendaComercial.Resources.MessageFor.ValidacionCampoObligatorio, campo.Nombre)); break;
                    }
                }
                else
                {
                    switch (campo.IdCampo)
                    {
                        case "NomJuridico": if (String.IsNullOrEmpty(model.Nombre1)) this.AddErrorMessage(String.Format(Rp3.AgendaComercial.Resources.MessageFor.ValidacionCampoObligatorio, campo.Nombre)); break;
                        case "PagWeb": if (String.IsNullOrEmpty(model.ClienteDato.PaginaWeb)) this.AddErrorMessage(String.Format(Rp3.AgendaComercial.Resources.MessageFor.ValidacionCampoObligatorio, campo.Nombre)); break;
                        case "ActEco": if (String.IsNullOrEmpty(model.ClienteDato.ActividadEconomica)) this.AddErrorMessage(String.Format(Rp3.AgendaComercial.Resources.MessageFor.ValidacionCampoObligatorio, campo.Nombre)); break;
                        case "IniAct": if (model.ClienteDato.InicioActividad == null) this.AddErrorMessage(String.Format(Rp3.AgendaComercial.Resources.MessageFor.ValidacionCampoObligatorio, campo.Nombre)); break;
                        case "RazSoc": if (String.IsNullOrEmpty(model.RazonSocial)) this.AddErrorMessage(String.Format(Rp3.AgendaComercial.Resources.MessageFor.ValidacionCampoObligatorio, campo.Nombre)); break;
                        case "RepLeg": if (String.IsNullOrEmpty(model.ClienteDato.RepresentanteLegal)) this.AddErrorMessage(String.Format(Rp3.AgendaComercial.Resources.MessageFor.ValidacionCampoObligatorio, campo.Nombre)); break;
                        case "CedRep": if (String.IsNullOrEmpty(model.ClienteDato.RepresentateLegalIdentificacion)) this.AddErrorMessage(String.Format(Rp3.AgendaComercial.Resources.MessageFor.ValidacionCampoObligatorio, campo.Nombre)); break;
                    }
                }

                switch (campo.IdCampo)
                {
                    case "Correo": if (String.IsNullOrEmpty(model.CorreoElectronico)) this.AddErrorMessage(String.Format(Rp3.AgendaComercial.Resources.MessageFor.ValidacionCampoObligatorio, campo.Nombre)); break;
                    case "Canal": if (model.IdCanal == null) this.AddErrorMessage(String.Format(Rp3.AgendaComercial.Resources.MessageFor.ValidacionCampoObligatorio, campo.Nombre)); break;
                    case "Foto": if (String.IsNullOrEmpty(model.Foto)) this.AddErrorMessage(String.Format(Rp3.AgendaComercial.Resources.MessageFor.ValidacionCampoObligatorio, campo.Nombre)); break;
                    case "TipoCli": if (model.IdTipoCliente == null) this.AddErrorMessage(String.Format(Rp3.AgendaComercial.Resources.MessageFor.ValidacionCampoObligatorio, campo.Nombre)); break;
                    case "TipoId": if (model.IdTipoIdentificacion == null) this.AddErrorMessage(String.Format(Rp3.AgendaComercial.Resources.MessageFor.ValidacionCampoObligatorio, campo.Nombre)); break;
                    case "Id": if (String.IsNullOrEmpty(model.Identificacion)) this.AddErrorMessage(String.Format(Rp3.AgendaComercial.Resources.MessageFor.ValidacionCampoObligatorio, campo.Nombre)); break;
                }

                if (isNew)
                {
                    switch (campo.IdCampo)
                    {
                        case "Ref": if (String.IsNullOrEmpty(model.DireccionReferencia)) this.AddErrorMessage(String.Format(Rp3.AgendaComercial.Resources.MessageFor.ValidacionCampoObligatorio, campo.Nombre)); break;
                        case "Dir": if (String.IsNullOrEmpty(model.Direccion)) this.AddErrorMessage(String.Format(Rp3.AgendaComercial.Resources.MessageFor.ValidacionCampoObligatorio, campo.Nombre)); break;
                        case "DirDesc": if (String.IsNullOrEmpty(model.DireccionDescripcion)) this.AddErrorMessage(String.Format(Rp3.AgendaComercial.Resources.MessageFor.ValidacionCampoObligatorio, campo.Nombre)); break;
                        case "CiuPar": if (model.DireccionIdCiudad == null) this.AddErrorMessage(String.Format(Rp3.AgendaComercial.Resources.MessageFor.ValidacionCampoObligatorio, campo.Nombre)); break;
                    }
                }
            }
        }

        private void VerificarCampoObligatorio(ClienteDireccion model)
        {
            var campos = DataBase.ParametroClienteCampos.Get(p => p.EsObligatorio && p.IdGrupo == Models.Constantes.GrupoClienteCampo.Direccion).OrderBy(p => p.Prioridad).ToList();

            foreach (var campo in campos)
            {
                switch (campo.IdCampo)
                {
                    case "Ref": if (String.IsNullOrEmpty(model.Referencia)) this.AddErrorMessage(String.Format(Rp3.AgendaComercial.Resources.MessageFor.ValidacionCampoObligatorio, campo.Nombre)); break;
                    case "Dir": if (String.IsNullOrEmpty(model.Direccion)) this.AddErrorMessage(String.Format(Rp3.AgendaComercial.Resources.MessageFor.ValidacionCampoObligatorio, campo.Nombre)); break;
                    case "DirDesc": if (String.IsNullOrEmpty(model.Descripcion)) this.AddErrorMessage(String.Format(Rp3.AgendaComercial.Resources.MessageFor.ValidacionCampoObligatorio, campo.Nombre)); break;
                    case "DirTelf1": if (String.IsNullOrEmpty(model.Telefono1)) this.AddErrorMessage(String.Format(Rp3.AgendaComercial.Resources.MessageFor.ValidacionCampoObligatorio, campo.Nombre)); break;
                    case "DirTelf2": if (String.IsNullOrEmpty(model.Telefono2)) this.AddErrorMessage(String.Format(Rp3.AgendaComercial.Resources.MessageFor.ValidacionCampoObligatorio, campo.Nombre)); break;
                    case "CiuPar": if (model.IdCiudad == null) this.AddErrorMessage(String.Format(Rp3.AgendaComercial.Resources.MessageFor.ValidacionCampoObligatorio, campo.Nombre)); break;
                }
            }
        }

        private void VerificarCampoObligatorio(ClienteContacto model)
        {
            var campos = DataBase.ParametroClienteCampos.Get(p => p.EsObligatorio && p.IdGrupo == Models.Constantes.GrupoClienteCampo.Contacto).OrderBy(p => p.Prioridad).ToList();

            foreach (var campo in campos)
            {
                switch (campo.IdCampo)
                {
                    case "CorreoCont": if (String.IsNullOrEmpty(model.CorreoElectronico)) this.AddErrorMessage(String.Format(Rp3.AgendaComercial.Resources.MessageFor.ValidacionCampoObligatorio, campo.Nombre)); break;
                    case "CargoCont": if (String.IsNullOrEmpty(model.Cargo)) this.AddErrorMessage(String.Format(Rp3.AgendaComercial.Resources.MessageFor.ValidacionCampoObligatorio, campo.Nombre)); break;
                    case "ApeCont": if (String.IsNullOrEmpty(model.Apellido)) this.AddErrorMessage(String.Format(Rp3.AgendaComercial.Resources.MessageFor.ValidacionCampoObligatorio, campo.Nombre)); break;
                    case "DirCont": if (model.IdClienteDireccion == null) this.AddErrorMessage(String.Format(Rp3.AgendaComercial.Resources.MessageFor.ValidacionCampoObligatorio, campo.Nombre)); break;
                    case "FotoCont": if (String.IsNullOrEmpty(model.Foto)) this.AddErrorMessage(String.Format(Rp3.AgendaComercial.Resources.MessageFor.ValidacionCampoObligatorio, campo.Nombre)); break;
                    case "Telf1Cont": if (String.IsNullOrEmpty(model.Telefono1)) this.AddErrorMessage(String.Format(Rp3.AgendaComercial.Resources.MessageFor.ValidacionCampoObligatorio, campo.Nombre)); break;
                    case "Telf2Cont": if (String.IsNullOrEmpty(model.Telefono2)) this.AddErrorMessage(String.Format(Rp3.AgendaComercial.Resources.MessageFor.ValidacionCampoObligatorio, campo.Nombre)); break;
                    case "NomCont": if (String.IsNullOrEmpty(model.Nombre)) this.AddErrorMessage(String.Format(Rp3.AgendaComercial.Resources.MessageFor.ValidacionCampoObligatorio, campo.Nombre)); break;
                }
            }
        }

        [HttpPost]
        [Rp3.Web.Mvc.Authorize("CLIENTE", "EDIT", "AGENDACOMERCIAL", Order = 1)]
        public ActionResult Edit(Cliente model)
        {
            //Cliente modelUpdate = GetModel(model.IdCliente);

            Cliente modelUpdate = DataBase.Clientes.Get(p => p.IdCliente == model.IdCliente, includeProperties: "ClienteDato").SingleOrDefault();

            if (model.ClienteDato == null)
                model.ClienteDato = new ClienteDato();

            if (model.ClienteDirecciones == null)
                model.ClienteDirecciones = new List<ClienteDireccion>();

            if (model.ClienteContactos == null)
                model.ClienteContactos = new List<ClienteContacto>();

            try
            {
                if (ModelState.IsValid)
                {
                    CopyTo(model, modelUpdate, includeProperties: new string[] {
                        "IdTipoIdentificacion",
                        "Identificacion",
                        "Nombre1",
                        "Nombre2",
                        "Apellido1",
                        "Apellido2",
                        "CorreoElectronico",
                        "TipoPersona",
                        "IdTipoCliente",
                        "IdCanal",                        
                        "Calificacion",
                        "RazonSocial",
                        "Estado"
                        //"Foto"
                        });


                    //modelUpdate.EstadoGeneralValue = DataBase.GeneralValues.Get(p => p.Id == modelUpdate.EstadoTabla && p.Code == model.Estado).FirstOrDefault();
                    //modelUpdate.Estado = model.Estado;

                    if (model.TipoPersona == Models.Constantes.TipoPersona.Juridica || model.TipoPersona == Models.Constantes.TipoPersona.JuridicaPublica)
                    {
                        modelUpdate.ClienteDato.FechaNacimiento = null;
                        modelUpdate.IdTipoIdentificacion = Constantes.IdentificationType.RUC;
                    }

                    if (modelUpdate.TipoPersona == Models.Constantes.TipoPersona.Natural)
                        modelUpdate.ClienteDato.InicioActividad = null;

                    if (!String.IsNullOrEmpty(modelUpdate.Identificacion))
                    {
                        modelUpdate.Identificacion = modelUpdate.Identificacion.Trim();

                        if (!Rp3.Common.Validators.IdentificationType.IdentificationTypeValidator.IsValid(modelUpdate.IdTipoIdentificacion, modelUpdate.TipoPersona, modelUpdate.Identificacion))
                        {
                            int longitud = 0;

                            switch (model.IdTipoIdentificacion)
                            {
                                case Rp3.Common.Validators.IdentificationType.IdentificationTypeValidator.Codes.CEDULA:
                                    longitud = Rp3.Common.Validators.IdentificationType.IdentificationTypeValidator.Lenght.CEDULA;
                                    break;

                                case Rp3.Common.Validators.IdentificationType.IdentificationTypeValidator.Codes.RUC:
                                    longitud = Rp3.Common.Validators.IdentificationType.IdentificationTypeValidator.Lenght.RUC;
                                    break;

                                case Rp3.Common.Validators.IdentificationType.IdentificationTypeValidator.Codes.PASAPORTE:
                                    longitud = Rp3.Common.Validators.IdentificationType.IdentificationTypeValidator.Lenght.PASAPORTE;
                                    break;
                            }

                            var tipoIden = DataBase.IdentificationTypes.Get(p => p.IdentificationTypeId == model.IdTipoIdentificacion).FirstOrDefault();

                            if (longitud != model.Identificacion.Length && tipoIden != null)
                            {
                                this.AddErrorMessage(String.Format(Rp3.AgendaComercial.Resources.MessageFor.IdentificacionLongitudIncorrecta,
                                    model.Identificacion.Length, tipoIden.Name, longitud));
                            }
                            else
                            {
                                this.AddErrorMessage(String.Format(Rp3.AgendaComercial.Resources.MessageFor.IdentificacionValidacionIncorrecta, tipoIden.Name));
                            }

                            //this.AddErrorMessage(Rp3.AgendaComercial.Resources.MessageFor.IdentificacionIncorrecta);
                        }
                        else if (DataBase.Clientes.Exists(p => p.Estado != Models.Constantes.Estado.Eliminado && p.Identificacion == modelUpdate.Identificacion && p.IdCliente != modelUpdate.IdCliente))
                        {
                            this.AddErrorMessage(Rp3.AgendaComercial.Resources.MessageFor.IdentificacionRepetida);
                        }
                    }

                    if (model.ClienteDato.FechaNacimiento != null && model.ClienteDato.FechaNacimiento > this.GetCurrentDateTime())
                        this.AddErrorMessage(Rp3.AgendaComercial.Resources.MessageFor.FechaNacimientoMayorHoy);

                    if (model.ClienteDato.InicioActividad != null && model.ClienteDato.InicioActividad > this.GetCurrentDateTime())
                        this.AddErrorMessage(Rp3.AgendaComercial.Resources.MessageFor.FechaInicioActividadMayorHoy);

                    modelUpdate.ClienteDato.Genero = model.ClienteDato.Genero;
                    modelUpdate.ClienteDato.EstadoCivil = model.ClienteDato.EstadoCivil;
                    modelUpdate.ClienteDato.FechaNacimiento = model.ClienteDato.FechaNacimiento;
                    modelUpdate.ClienteDato.ActividadEconomica = model.ClienteDato.ActividadEconomica;
                    modelUpdate.ClienteDato.InicioActividad = model.ClienteDato.InicioActividad;
                    modelUpdate.ClienteDato.RepresentanteLegal = model.ClienteDato.RepresentanteLegal;
                    modelUpdate.ClienteDato.RepresentateLegalIdentificacion = model.ClienteDato.RepresentateLegalIdentificacion;
                    modelUpdate.ClienteDato.PaginaWeb = model.ClienteDato.PaginaWeb;

                    foreach (var item in model.ClienteDirecciones)
                    {
                        var update = modelUpdate.ClienteDirecciones.Where(p => p.IdClienteDireccion == item.IdClienteDireccion).FirstOrDefault();

                        if (update != null)
                        {
                            CopyTo(item, update, includeProperties: new string[] {
                                "TipoDireccion",
                                "Latitud",
                                "Longitud",
                                "Direccion",
                                "Descripcion",
                                "Referencia",
                                "AplicaRuta",
                                "IdCiudad",
                                "EsPrincipal",
                                "Telefono1",                        
                                "Telefono2"
                                });
                        }
                    }

                    foreach (var item in model.ClienteContactos)
                    {
                        var update = modelUpdate.ClienteContactos.Where(p => p.IdClienteContacto == item.IdClienteContacto).FirstOrDefault();

                        if (update != null)
                        {
                            CopyTo(item, update, includeProperties: new string[] {
                                "Nombre",
                                "Apellido",
                                "IdClienteDireccion",
                                "Telefono1",
                                "Telefono2",
                                "Cargo",
                                "CorreoElectronico"
                                });
                        }
                    }

                    VerificarCampoObligatorio(modelUpdate, false);

                    foreach (var direccion in modelUpdate.ClienteDirecciones)
                    {
                        if (direccion.Estado != Rp3.AgendaComercial.Models.Constantes.Estado.Eliminado)
                            VerificarCampoObligatorio(direccion);
                    }

                    foreach (var contacto in modelUpdate.ClienteContactos)
                    {
                        if (contacto.Estado != Rp3.AgendaComercial.Models.Constantes.Estado.Eliminado)
                            VerificarCampoObligatorio(contacto);
                    }

                    if (!this.MessageCollection.HasError())
                    {
                        modelUpdate.UsrMod = this.UserLogonName;
                        modelUpdate.FecMod = this.GetCurrentDateTime();

                        //ProcessDireccion(model);

                        DataBase.ClienteDirecciones.Update(modelUpdate.ClienteDirecciones, modelUpdate.ClienteDirecciones);
                        DataBase.ClienteContactos.Update(modelUpdate.ClienteContactos, modelUpdate.ClienteContactos);

                        DataBase.ClienteDatos.Update(modelUpdate.ClienteDato);
                        DataBase.Clientes.Update(modelUpdate);

                        DataBase.Save();
                        DataBase.Zonas.Procesar();

                        VerificarDependencia(modelUpdate);

                        this.AddDefaultSuccessMessage();

                        //ViewBag.FindRecord = modelUpdate.NombresCompletos;

                        //return RedirectToAction("Index");
                    }
                }
            }
            catch
            {
                this.AddDefaultErrorMessage();

                return Json();
            }

            //model.ClienteDirecciones = modelUpdate.ClienteDirecciones;
            //model.ClienteContactos = modelUpdate.ClienteContactos;

            //InicializarEdit();
            //InicializarTab();
            //SetMarkers(model.ClienteDirecciones);
            //LoadLoteRuta(model.IdCliente);

            //InicializarCampoObligatorio(model.IdCliente);

            //return View(model);

            return Json();
        }

        [HttpPost]
        [PreventSpam(Order = 0)]
        [Rp3.Web.Mvc.Authorize("CLIENTE", "DELETE", "AGENDACOMERCIAL")]
        public ActionResult Delete(Cliente model)
        {
            try
            {
                //Cliente deleteModel = GetModel(model.IdCliente);

                //DataBase.Clientes.Delete(deleteModel);

                Cliente modelUpdate = DataBase.Clientes.Get(p => p.IdCliente == model.IdCliente).SingleOrDefault();

                modelUpdate.Estado = Rp3.AgendaComercial.Models.Constantes.Estado.Eliminado;
                modelUpdate.UsrMod = this.UserLogonName;
                modelUpdate.FecMod = this.GetCurrentDateTime();

                DataBase.Clientes.Update(modelUpdate);

                DataBase.Save();

                VerificarDependencia(modelUpdate);

                return RedirectToAction("Index");
            }
            catch
            {
                this.AddDefaultErrorMessage();
            }

            return RedirectToAction("Index", null, null, true);
        }

        private void VerificarDependencia(Cliente cliente)
        {
            if (cliente.Estado == Rp3.AgendaComercial.Models.Constantes.Estado.Inactivo ||
                cliente.Estado == Rp3.AgendaComercial.Models.Constantes.Estado.Eliminado)
            {
                DataBase.Clientes.EliminarDependenciaCliente(cliente.IdCliente, this.UserLogonName);
            }
        }

        private void VerificarDependencia(ClienteDireccion direccion)
        {
            if (direccion.Estado == Rp3.AgendaComercial.Models.Constantes.Estado.Inactivo ||
                direccion.Estado == Rp3.AgendaComercial.Models.Constantes.Estado.Eliminado)
            {
                DataBase.Clientes.EliminarDependenciaClienteDireccion(direccion.IdCliente, direccion.IdClienteDireccion, this.UserLogonName);
            }
        }

        private void VerificarDependencia(ClienteContacto contacto)
        {
            if (contacto.Estado == Rp3.AgendaComercial.Models.Constantes.Estado.Inactivo ||
                contacto.Estado == Rp3.AgendaComercial.Models.Constantes.Estado.Eliminado)
            {
                DataBase.Clientes.EliminarDependenciaClienteContacto(contacto.IdCliente, contacto.IdClienteContacto, this.UserLogonName);
            }
        }

        [Rp3.Web.Mvc.Authorize("CLIENTE", "EDIT", "AGENDACOMERCIAL")]
        public ActionResult SetUbicacion(int idCliente, int idClienteDireccion, int markerIndex, double? latitud, double? longitud)
        {
            if (longitud == null || longitud == -1)
                longitud = Ubicacion.DefaultLongitud;

            if (latitud == null || latitud == -1)
                latitud = Ubicacion.DefaultLatitud;

            ClienteDireccion model = new ClienteDireccion() { IdCliente = idCliente, IdClienteDireccion = idClienteDireccion, MarkerIndex = markerIndex, Longitud = longitud, Latitud = latitud };

            return PartialView("_SetUbicacion", model);
        }

        [Rp3.Web.Mvc.Authorize("CLIENTE", "EDIT", "AGENDACOMERCIAL")]
        public ActionResult UbicacionMapMarkerClient()
        {
            ViewBag.InitMapa = true;
            return PartialView("_UbicacionMapMarkerClient");
        }

        [Rp3.Web.Mvc.Authorize("CLIENTE", "EDIT", "AGENDACOMERCIAL")]
        public ActionResult UbicacionMapMarkerSingle(int idCliente, int idClienteDireccion, double? latitud, double? longitud, int markerIndex, string titulo)
        {
            var model = new Ubicacion() {
                IdUbicacion = idClienteDireccion,
                MarkerIndex = markerIndex,
                Latitud = latitud,
                Longitud = longitud,
                Titulo = titulo
            };

            ViewBag.MapSelector = String.Format("viewMap{0}", idClienteDireccion);
            ViewBag.MapHeight = "230px";

            return PartialView("_UbicacionMapMarkerSingle", model);
        }

        [HttpPost]
        [Rp3.Web.Mvc.Authorize("CLIENTE", "EDIT", "AGENDACOMERCIAL")]
        public ActionResult SetUbicacion(int idCliente, int idClienteDireccion, double latitud, double longitud)
        {
            try
            {
                var item = DataBase.ClienteDirecciones.Get(p => p.IdCliente == idCliente & p.IdClienteDireccion == idClienteDireccion).FirstOrDefault();

                if (item != null)
                {
                    item.Latitud = latitud;
                    item.Longitud = longitud;

                    DataBase.ClienteDirecciones.Update(item);

                    DataBase.Save();
                }

                this.AddDefaultSuccessMessage();
                return Json();
                //return new JsonResult() { Data = new { Success = true, Message = Rp3.Resources.MessageFor.DefaultSuccessTransactionMessage, MessageType = MessageType.Success } };
            }
            catch (Exception e)
            {
                this.AddDefaultErrorMessage();
                return Json();
                //return new JsonResult() { Data = new { Success = false, Message = Rp3.Resources.MessageFor.DefaultErrorTransactionMessage, MessageType = MessageType.Error } };
            }
        }

        [ChildAction]
        [Rp3.Web.Mvc.Authorize("CLIENTE", "EDIT", "AGENDACOMERCIAL")]
        public ActionResult LoteRuta(int idCliente)
        {
            LoadLoteRuta(idCliente);

            return PartialView("_LoteRuta");
        }

        #region Direccion


        [HttpPost]
        [ChildAction]
        [Rp3.Web.Mvc.Authorize("CLIENTE", "EDIT", "AGENDACOMERCIAL")]
        public ActionResult EliminarDireccion(int idCliente, int idClienteDireccion)
        {
            try
            {
                var item = DataBase.ClienteDirecciones.Get(p => p.IdCliente == idCliente & p.IdClienteDireccion == idClienteDireccion).FirstOrDefault();

                if (item != null)
                {
                    //DataBase.ClienteDirecciones.Delete(item);
                    item.Estado = Models.Constantes.Estado.Eliminado;
                    DataBase.ClienteDirecciones.Update(item);

                    DataBase.Save();

                    VerificarDependencia(item);
                }

                this.AddDefaultSuccessMessage();
                return Json();
                //return new JsonResult() { Data = new { Success = true, Message = Rp3.Resources.MessageFor.DefaultSuccessTransactionMessage, MessageType = MessageType.Success } };
            }
            catch (Exception e)
            {
                this.AddDefaultErrorMessage();
                return Json();
                //return new JsonResult() { Data = new { Success = false, Message = Rp3.Resources.MessageFor.DefaultErrorTransactionMessage, MessageType = MessageType.Error } };
            }
        }

        [ChildAction]
        [Rp3.Web.Mvc.Authorize("CLIENTE", "EDIT", "AGENDACOMERCIAL")]
        public ActionResult DireccionesDetalle(int idCliente)
        {
            var list = DataBase.ClienteDirecciones.Get(p => p.IdCliente == idCliente && p.Estado != Rp3.AgendaComercial.Models.Constantes.Estado.Eliminado).ToList();

            SetMarkers(list);

            InicializarCampoObligatorio(idCliente);

            return PartialView("_DireccionesDetalle", list);
        }

        [ChildAction]
        [Rp3.Web.Mvc.Authorize("CLIENTE", "EDIT", "AGENDACOMERCIAL")]
        public JsonResult ProcessPlaceString(string place)
        {
            string direccion = String.Empty;
            string referencia = String.Empty;
            string ciudad = String.Empty;
            int? idCiudad = null;

            if (!String.IsNullOrEmpty(place))
            {
                string[] places = place.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);

                if (places.Count() == 2 || places.Count() == 3 || places.Count() == 4)
                {
                    direccion = places[0];
                    ciudad = places[1];
                }
                else if (places.Count() == 5)
                {
                    referencia = places[0];
                    direccion = places[1];
                    ciudad = places[2];
                }
            }

            if (!String.IsNullOrEmpty(ciudad))
            {
                var ciu = DataBase.GeopoliticalStructures.Get(p => p.Name.ToUpper() == ciudad.ToUpper()).FirstOrDefault();

                if (ciu != null)
                    idCiudad = ciu.GeopoliticalStructureId;
            }

            return new JsonResult() { Data = new { direccion = direccion, referencia = referencia, idCiudad = idCiudad, ciudad = ciudad }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [ChildAction]
        [Rp3.Web.Mvc.Authorize("CLIENTE", "EDIT", "AGENDACOMERCIAL")]
        public ActionResult SetDireccion(int idCliente, int idClienteDireccion, float? latitud, float? longitud, string place)
        {
            ViewBag.CiudadesSelectList = DataBase.GeopoliticalStructures.Get(p => p.GeopoliticalStructureTypeId == Models.Constantes.GeopoliticalStructureType.Ciudad).ToSelectList(includeNullItem: true);

            ClienteDireccion model = DataBase.ClienteDirecciones.Get(p => p.IdCliente == idCliente && p.IdClienteDireccion == idClienteDireccion).FirstOrDefault();

            if (model == null)
            {
                string direccion = String.Empty;
                string referencia = String.Empty;
                string ciudad = String.Empty;
                int? idCiudad = null;

                if (!String.IsNullOrEmpty(place))
                {
                    string[] places = place.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);

                    if (places.Count() == 2 || places.Count() == 3 || places.Count() == 4)
                    {
                        direccion = places[0];
                        ciudad = places[1];
                    }
                    else if (places.Count() == 5)
                    {
                        referencia = places[0];
                        direccion = places[1];
                        ciudad = places[2];
                    }
                }

                if (!String.IsNullOrEmpty(ciudad))
                {
                    var ciu = DataBase.GeopoliticalStructures.Get(p => p.Name.ToUpper() == ciudad.ToUpper()).FirstOrDefault();

                    if (ciu != null)
                        idCiudad = ciu.GeopoliticalStructureId;
                }

                model = new ClienteDireccion()
                {
                    IdCliente = idCliente,
                    Direccion = direccion,
                    Descripcion = referencia,
                    AplicaRuta = true,
                    Latitud = latitud,
                    Longitud = longitud,
                    IdCiudad = idCiudad,
                    TipoDireccion = Rp3.AgendaComercial.Models.Constantes.TipoDireccion.Trabajo,
                    Estado = Rp3.AgendaComercial.Models.Constantes.Estado.Activo
                };

                if (DataBase.ClienteDirecciones.Get(p => p.IdCliente == idCliente && p.EsPrincipal && p.Estado == Rp3.AgendaComercial.Models.Constantes.Estado.Activo).Count() == 0)
                    model.EsPrincipal = true;
            }

            ViewBag.ModificarCiudad = model.IdCiudad == null || idClienteDireccion == 0;

            InicializarCampoObligatorio(idCliente);

            return PartialView("_SetDireccion", model);
        }

        [HttpPost]
        [ChildAction]
        [Rp3.Web.Mvc.Authorize("CLIENTE", "EDIT", "AGENDACOMERCIAL")]
        public JsonResult SetDireccion(ClienteDireccion model)
        {
            try
            {
                if (model.IdClienteDireccion != 0)
                {
                    var list = DataBase.ClienteDirecciones.Get(p => p.IdCliente == model.IdCliente);

                    foreach (var item in list)
                    {
                        if (item.IdClienteDireccion == model.IdClienteDireccion)
                        {
                            CopyTo(model, item, includeProperties: new string[] {
                            "Direccion",
                            "TipoDireccion",
                            "IdCiudad",
                            "Telefono1",
                            "Telefono2",
                            "Referencia",
                            "EsPrincipal",
                            "AplicaRuta",
                            "Descripcion",
                            "Estado"
                            });

                            VerificarCampoObligatorio(item);

                            if (!this.MessageCollection.HasError())
                            {
                                DataBase.ClienteDirecciones.Update(item);
                            }
                            else
                            {
                                return Json();
                            }
                        }
                        else if (model.EsPrincipal)
                        {
                            item.EsPrincipal = false;
                            DataBase.ClienteDirecciones.Update(item);
                        }
                    }
                }
                else
                {
                    var list = DataBase.ClienteDirecciones.Get(p => p.IdCliente == model.IdCliente);

                    if (list.Count() == 0)
                        model.EsPrincipal = true;

                    model.Estado = Rp3.AgendaComercial.Models.Constantes.Estado.Activo;
                    model.EstadoTabla = Rp3.AgendaComercial.Models.Constantes.Estado.Tabla;
                    model.TipoDireccionTabla = Rp3.AgendaComercial.Models.Constantes.TipoDireccion.Tabla;
                    model.AsignarId();

                    VerificarCampoObligatorio(model);

                    if (!this.MessageCollection.HasError())
                    {
                        DataBase.ClienteDirecciones.Insert(model);
                    }
                    else
                    {
                        return Json();
                    }
                }

                DataBase.Save();

                VerificarDependencia(model);

                this.AddDefaultSuccessMessage();
                return Json();
                //return new JsonResult() { Data = new { Success = true, Message = Rp3.Resources.MessageFor.DefaultSuccessTransactionMessage, MessageType = MessageType.Success } };
            }
            catch (Exception e)
            {
                this.AddDefaultErrorMessage();
                return Json();
                //return new JsonResult() { Data = new { Success = false, Message = Rp3.Resources.MessageFor.DefaultErrorTransactionMessage, MessageType = MessageType.Error } };
            }
        }

        #endregion Direccion

        #region Contacto

        [HttpPost]
        [ChildAction]
        [Rp3.Web.Mvc.Authorize("CLIENTE", "EDIT", "AGENDACOMERCIAL")]
        public ActionResult EliminarContacto(int idCliente, int idClienteContacto)
        {
            try
            {
                var item = DataBase.ClienteContactos.Get(p => p.IdCliente == idCliente & p.IdClienteContacto == idClienteContacto).FirstOrDefault();

                if (item != null)
                {
                    //DataBase.ClienteContactos.Delete(item);
                    item.Estado = Models.Constantes.Estado.Eliminado;
                    DataBase.ClienteContactos.Update(item);

                    DataBase.Save();

                    VerificarDependencia(item);
                }

                this.AddDefaultSuccessMessage();
                return Json();
                //return new JsonResult() { Data = new { Success = true, Message = Rp3.Resources.MessageFor.DefaultSuccessTransactionMessage, MessageType = MessageType.Success } };
            }
            catch (Exception e)
            {
                this.AddDefaultErrorMessage();
                return Json();
                //return new JsonResult() { Data = new { Success = false, Message = Rp3.Resources.MessageFor.DefaultErrorTransactionMessage, MessageType = MessageType.Error } };
            }
        }

        [ChildAction]
        [Rp3.Web.Mvc.Authorize("CLIENTE", "EDIT", "AGENDACOMERCIAL")]
        public ActionResult ContactosDetalle(int idCliente)
        {
            var list = DataBase.ClienteContactos.Get(p => p.IdCliente == idCliente && p.Estado != Rp3.AgendaComercial.Models.Constantes.Estado.Eliminado).ToList();

            //ViewBag.DireccionesSelectList = DataBase.ClienteDirecciones.Get(p => p.IdCliente == idCliente && p.Estado != Rp3.AgendaComercial.Models.Constantes.Estado.Eliminado).ToSelectList(valueMember: "KeyCombo", includeNullItem: false);

            InicializarCampoObligatorio(idCliente);

            return PartialView("_ContactosDetalle", list);
        }

        [ChildAction]
        [Rp3.Web.Mvc.Authorize("CLIENTE", "EDIT", "AGENDACOMERCIAL")]
        public ActionResult SetContacto(int idCliente, int idClienteContacto)
        {
            //ViewBag.DireccionesSelectList = DataBase.ClienteDirecciones.Get(p => p.IdCliente == idCliente && p.Estado != Rp3.AgendaComercial.Models.Constantes.Estado.Eliminado).ToSelectList(valueMember: "KeyCombo", includeNullItem: false);

            ClienteContacto model = DataBase.ClienteContactos.Get(p => p.IdCliente == idCliente && p.IdClienteContacto == idClienteContacto).FirstOrDefault();

            if (model == null)
                model = new ClienteContacto() { IdCliente = idCliente };

            InicializarCampoObligatorio(idCliente);

            return PartialView("_SetContacto", model);
        }

        [HttpPost]
        [ChildAction]
        [Rp3.Web.Mvc.Authorize("CLIENTE", "EDIT", "AGENDACOMERCIAL")]
        public JsonResult SetContacto(ClienteContacto model)
        {
            try
            {
                if (model.IdClienteContacto != 0)
                {
                    var list = DataBase.ClienteContactos.Get(p => p.IdCliente == model.IdCliente);

                    foreach (var item in list)
                    {
                        if (item.IdClienteContacto == model.IdClienteContacto)
                        {
                            CopyTo(model, item, includeProperties: new string[] {
                            "Nombre",
                            "Apellido",
                            "CorreoElectronico",
                            "Telefono1",
                            "Telefono2",
                            "Cargo",
                            "IdClienteDireccion"
                            });

                            VerificarCampoObligatorio(item);

                            if (!this.MessageCollection.HasError())
                            {
                                DataBase.ClienteContactos.Update(item);
                            }
                            else
                            {
                                return Json();
                            }
                        }
                    }
                }
                else
                {
                    var list = DataBase.ClienteContactos.Get(p => p.IdCliente == model.IdCliente);

                    model.Estado = Rp3.AgendaComercial.Models.Constantes.Estado.Activo;
                    model.EstadoTabla = Rp3.AgendaComercial.Models.Constantes.Estado.Tabla;
                    model.AsignarId();

                    VerificarCampoObligatorio(model);

                    if (!this.MessageCollection.HasError())
                    {
                        DataBase.ClienteContactos.Insert(model);
                    }
                    else
                    {
                        return Json();
                    }
                }

                DataBase.Save();

                this.AddDefaultSuccessMessage();
                return Json();
                //return new JsonResult() { Data = new { Success = true, Message = Rp3.Resources.MessageFor.DefaultSuccessTransactionMessage, MessageType = MessageType.Success } };
            }
            catch (Exception e)
            {
                this.AddDefaultErrorMessage();
                return Json();
                //return new JsonResult() { Data = new { Success = false, Message = Rp3.Resources.MessageFor.DefaultErrorTransactionMessage, MessageType = MessageType.Error } };
            }
        }

        #endregion Contacto

        [ChildAction]
        public JsonResult ClienteAutocomplete(string term)
        {
            var clientes = DataBase.Clientes.GetCliente(term);

            var result = clientes.Select(p => new { label = String.Format("{0} - {1}", p.NombresCompletos, p.Etiqueta), cliente = p.NombresCompletos, direccion = p.Etiqueta, idCliente = p.IdCliente, idClienteDireccion = p.IdClienteDireccion, latitud = p.Latitud ?? -1, longitud = p.Longitud ?? -1, markerIndex = p.MarkerIndex, markerstart = p.MarkerStart, markerzindex = p.MarkerZIndex }).Take(15);

            return new JsonResult() { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        #region Foto

        [ChildAction]
        [Rp3.Web.Mvc.Authorize("CLIENTE", "EDIT", "AGENDACOMERCIAL")]
        public ActionResult EditFoto(int id)
        {
            return PartialView(GetModel(id));
        }

        [ChildAction]
        [Rp3.Web.Mvc.Authorize("CLIENTE", "EDIT", "AGENDACOMERCIAL")]
        public ActionResult Foto(int id)
        {
            return PartialView(GetModel(id));
        }

        [HttpPost]
        [Rp3.Web.Mvc.Authorize("CLIENTE", "EDIT", "AGENDACOMERCIAL")]
        public WrappedJsonResult DeleteFoto(int id, int? idContacto)
        {
            string message = Rp3.AgendaComercial.Resources.MessageFor.EliminarImageOk;
            bool valid = false;


            try
            {
                if (idContacto == null)
                {
                    var modelUpdate = GetModel(id);

                    if (modelUpdate != null)
                    {
                        string deleteImagePath = Path.Combine(Server.MapPath(Url.Content(Constantes.ProfileFotosSize.ClienteImagePath)), Path.GetFileName(modelUpdate.Foto));
                        FileInfo FileDetele = new FileInfo(deleteImagePath);
                        if (FileDetele.Exists)
                            FileDetele.Delete();

                        deleteImagePath = Path.Combine(Server.MapPath(Url.Content(Constantes.ProfileFotosSize.ClienteImagePath)), Path.GetFileName(modelUpdate.FotoMedium));
                        FileDetele = new FileInfo(deleteImagePath);
                        if (FileDetele.Exists)
                            FileDetele.Delete();

                        deleteImagePath = Path.Combine(Server.MapPath(Url.Content(Constantes.ProfileFotosSize.ClienteImagePath)), Path.GetFileName(modelUpdate.FotoSmall));
                        FileDetele = new FileInfo(deleteImagePath);
                        if (FileDetele.Exists)
                            FileDetele.Delete();

                        deleteImagePath = Path.Combine(Server.MapPath(Url.Content(Constantes.ProfileFotosSize.ClienteImagePath)), Path.GetFileName(modelUpdate.FotoMin));
                        FileDetele = new FileInfo(deleteImagePath);
                        if (FileDetele.Exists)
                            FileDetele.Delete();

                        modelUpdate.Foto = null;

                        DataBase.Clientes.Update(modelUpdate);
                        DataBase.Save();

                        valid = true;
                    }
                    else
                        message = Rp3.AgendaComercial.Resources.MessageFor.NoExisteFotoEliminar;
                }
                else
                {

                    var modelUpdate = GetModel(id, idContacto ?? 0);

                    if (modelUpdate != null)
                    {
                        string deleteImagePath = Path.Combine(Server.MapPath(Url.Content(Constantes.ProfileFotosSize.ClienteImagePath)), Path.GetFileName(modelUpdate.Foto));
                        FileInfo FileDetele = new FileInfo(deleteImagePath);
                        if (FileDetele.Exists)
                            FileDetele.Delete();

                        deleteImagePath = Path.Combine(Server.MapPath(Url.Content(Constantes.ProfileFotosSize.ClienteImagePath)), Path.GetFileName(modelUpdate.FotoMedium));
                        FileDetele = new FileInfo(deleteImagePath);
                        if (FileDetele.Exists)
                            FileDetele.Delete();

                        deleteImagePath = Path.Combine(Server.MapPath(Url.Content(Constantes.ProfileFotosSize.ClienteImagePath)), Path.GetFileName(modelUpdate.FotoSmall));
                        FileDetele = new FileInfo(deleteImagePath);
                        if (FileDetele.Exists)
                            FileDetele.Delete();

                        deleteImagePath = Path.Combine(Server.MapPath(Url.Content(Constantes.ProfileFotosSize.ClienteImagePath)), Path.GetFileName(modelUpdate.FotoMin));
                        FileDetele = new FileInfo(deleteImagePath);
                        if (FileDetele.Exists)
                            FileDetele.Delete();

                        modelUpdate.Foto = null;

                        DataBase.ClienteContactos.Update(modelUpdate);
                        DataBase.Save();

                        valid = true;
                    }
                    else
                        message = Rp3.AgendaComercial.Resources.MessageFor.NoExisteFotoEliminar;
                }
            }
            catch (Exception)
            {
                message = Rp3.AgendaComercial.Resources.MessageFor.ErrorEliminarFoto;
            }

            return new WrappedJsonResult() { Data = new { Message = message, IsValid = valid } };
        }

        [HttpPost]
        [Rp3.Web.Mvc.Authorize("CLIENTE", "EDIT", "AGENDACOMERCIAL")]
        public WrappedJsonResult UploadFoto(HttpPostedFileWrapper file)
        {
            try
            {
                if (file == null || file.ContentLength == 0)
                {
                    return new WrappedJsonResult
                    {
                        Data = new
                        {
                            IsValid = false,
                            Message = Rp3.AgendaComercial.Resources.MessageFor.NoPudoCargarFoto,
                            ImagePath = string.Empty
                        }
                    };
                }

                if (file.ContentLength > Constantes.ProfileFotosSize.MaxSizeUploadFile)
                {
                    return new WrappedJsonResult
                    {
                        Data = new
                        {
                            IsValid = false,
                            Message = String.Format(Rp3.AgendaComercial.Resources.MessageFor.TamanoFotoExcedido, (Constantes.ProfileFotosSize.MaxSizeUploadFile / 1048576)),
                            ImagePath = string.Empty
                        }
                    };
                }

                var fileName = String.Format("{0}{1}", Guid.NewGuid(), System.IO.Path.GetExtension(file.FileName));
                var imagePath = Path.Combine(Server.MapPath(Url.Content(Constantes.ProfileFotosSize.ClienteImagePath)), fileName);

                file.SaveAs(imagePath);
                //SaveThumbnail(imagePath);              

                return new WrappedJsonResult
                {
                    Data = new
                    {
                        IsValid = true,
                        Message = Rp3.AgendaComercial.Resources.MessageFor.ArchivoSubidoCorrectamente,
                        ImagePath = Url.Content(Constantes.ProfileFotosSize.ClienteImageSavePath + fileName),
                        Width = Constantes.ProfileFotosSize.ProfilePictureWidth,
                        Height = Constantes.ProfileFotosSize.ProfilePictureHeight
                    }
                };
            }
            catch
            {
                return new WrappedJsonResult
                {
                    Data = new
                    {
                        IsValid = false,
                        Message = Rp3.AgendaComercial.Resources.MessageFor.NoPudoCargarArchivo
                    }
                };
            }
        }

        [HttpPost]
        [Rp3.Web.Mvc.Authorize("CLIENTE", "EDIT", "AGENDACOMERCIAL")]
        public JsonResult SaveFoto(int id, int? idContacto, string fileName, double x, double y, double width, double height)
        {
            try
            {
                fileName = Path.Combine(Server.MapPath(Url.Content(Constantes.ProfileFotosSize.ClienteImagePath)), Path.GetFileName(fileName));
                Bitmap src = System.Drawing.Image.FromFile(fileName) as Bitmap;

                if (width > src.Width)
                    width = src.Width;

                if (height > src.Height)
                    height = src.Height;

                Rectangle cropRect = new Rectangle(Convert.ToInt32(x), Convert.ToInt32(y), Convert.ToInt32(width), Convert.ToInt32(height));
                Bitmap target = new Bitmap(cropRect.Width, cropRect.Height);

                using (Graphics g = Graphics.FromImage(target))
                {
                    g.DrawImage(src, new Rectangle(0, 0, target.Width, target.Height),
                                     cropRect,
                                     GraphicsUnit.Pixel);
                }

                FileInfo fileDelete = new FileInfo(fileName);

                var newfileName = String.Format("{0}{1}", Guid.NewGuid(), System.IO.Path.GetExtension(fileDelete.Name));
                var imagePath = Path.Combine(Server.MapPath(Url.Content(Constantes.ProfileFotosSize.ClienteImagePath)), newfileName);

                try
                {
                    fileDelete.Delete();
                }
                catch { }

                target.Save(imagePath);
                string minFileName = Thumbnail.SaveThumbnail(imagePath, Constantes.ProfileFotosSize.ProfilePictuteMinWidth, Constantes.ProfileFotosSize.ProfilePictuteMinHeight, "min");
                Thumbnail.SaveThumbnail(imagePath, Constantes.ProfileFotosSize.ProfilePictuteMedWidth, Constantes.ProfileFotosSize.ProfilePictuteMedHeight, "med");
                Thumbnail.SaveThumbnail(imagePath, Constantes.ProfileFotosSize.ProfilePictuteSmaWidth, Constantes.ProfileFotosSize.ProfilePictuteSmaHeight, "sma");

                if (idContacto == null)
                {

                    var modelUpdate = GetModel(id);

                    if (modelUpdate != null)
                    {
                        if (!string.IsNullOrEmpty(modelUpdate.Foto))
                        {
                            //var deleteImagePath = Path.Combine(Server.MapPath(Url.Content(clienteImagePath)), Path.GetFileName(modelUpdate.Foto));
                            //var deleteMiniImagePath = Path.Combine(Server.MapPath(Url.Content(clienteImagePath)),
                            //    Path.GetFileNameWithoutExtension(modelUpdate.Foto) + "_min" + Path.GetExtension(modelUpdate.Foto));
                            //FileInfo FileDetele = new FileInfo(deleteImagePath);
                            //if (FileDetele.Exists)
                            //    FileDetele.Delete();
                            //FileInfo FileDeteleMin = new FileInfo(deleteMiniImagePath);
                            //if (FileDeteleMin.Exists)
                            //    FileDeteleMin.Delete();

                            string deleteImagePath = Path.Combine(Server.MapPath(Url.Content(Constantes.ProfileFotosSize.ClienteImagePath)), Path.GetFileName(modelUpdate.Foto));
                            FileInfo FileDetele = new FileInfo(deleteImagePath);
                            if (FileDetele.Exists)
                                FileDetele.Delete();

                            deleteImagePath = Path.Combine(Server.MapPath(Url.Content(Constantes.ProfileFotosSize.ClienteImagePath)), Path.GetFileName(modelUpdate.FotoMedium));
                            FileDetele = new FileInfo(deleteImagePath);
                            if (FileDetele.Exists)
                                FileDetele.Delete();

                            deleteImagePath = Path.Combine(Server.MapPath(Url.Content(Constantes.ProfileFotosSize.ClienteImagePath)), Path.GetFileName(modelUpdate.FotoSmall));
                            FileDetele = new FileInfo(deleteImagePath);
                            if (FileDetele.Exists)
                                FileDetele.Delete();

                            deleteImagePath = Path.Combine(Server.MapPath(Url.Content(Constantes.ProfileFotosSize.ClienteImagePath)), Path.GetFileName(modelUpdate.FotoMin));
                            FileDetele = new FileInfo(deleteImagePath);
                            if (FileDetele.Exists)
                                FileDetele.Delete();
                        }

                        modelUpdate.Foto = Path.Combine(Constantes.ProfileFotosSize.ClienteImagePath, newfileName);
                        DataBase.Clientes.Update(modelUpdate);
                        DataBase.Save();
                    }
                }
                else
                {
                    var modelUpdate = GetModel(id, idContacto ?? 0);
                    if (modelUpdate != null)
                    {
                        if (!string.IsNullOrEmpty(modelUpdate.Foto))
                        {
                            //var deleteImagePath = Path.Combine(Server.MapPath(Url.Content(clienteImagePath)), Path.GetFileName(modelUpdate.Foto));
                            //var deleteMiniImagePath = Path.Combine(Server.MapPath(Url.Content(clienteImagePath)),
                            //    Path.GetFileNameWithoutExtension(modelUpdate.Foto) + "_min" + Path.GetExtension(modelUpdate.Foto));
                            //FileInfo FileDetele = new FileInfo(deleteImagePath);
                            //if (FileDetele.Exists)
                            //    FileDetele.Delete();
                            //FileInfo FileDeteleMin = new FileInfo(deleteMiniImagePath);
                            //if (FileDeteleMin.Exists)
                            //    FileDeteleMin.Delete();

                            string deleteImagePath = Path.Combine(Server.MapPath(Url.Content(Constantes.ProfileFotosSize.ClienteImagePath)), Path.GetFileName(modelUpdate.Foto));
                            FileInfo FileDetele = new FileInfo(deleteImagePath);
                            if (FileDetele.Exists)
                                FileDetele.Delete();

                            deleteImagePath = Path.Combine(Server.MapPath(Url.Content(Constantes.ProfileFotosSize.ClienteImagePath)), Path.GetFileName(modelUpdate.FotoMedium));
                            FileDetele = new FileInfo(deleteImagePath);
                            if (FileDetele.Exists)
                                FileDetele.Delete();

                            deleteImagePath = Path.Combine(Server.MapPath(Url.Content(Constantes.ProfileFotosSize.ClienteImagePath)), Path.GetFileName(modelUpdate.FotoSmall));
                            FileDetele = new FileInfo(deleteImagePath);
                            if (FileDetele.Exists)
                                FileDetele.Delete();

                            deleteImagePath = Path.Combine(Server.MapPath(Url.Content(Constantes.ProfileFotosSize.ClienteImagePath)), Path.GetFileName(modelUpdate.FotoMin));
                            FileDetele = new FileInfo(deleteImagePath);
                            if (FileDetele.Exists)
                                FileDetele.Delete();
                        }

                        modelUpdate.Foto = Path.Combine(Constantes.ProfileFotosSize.ClienteImagePath, newfileName);
                        DataBase.ClienteContactos.Update(modelUpdate);
                        DataBase.Save();
                    }
                }

                string fotoFileName = Path.Combine(Constantes.ProfileFotosSize.ClienteImageSavePath + newfileName);
                string fotoMin = Thumbnail.GetPictureMinFromOriginal(fotoFileName);

                return new JsonResult
                {
                    Data = new
                    {
                        IsValid = true,
                        Message = Rp3.AgendaComercial.Resources.MessageFor.ArchivoSubidoCorrectamente,
                        MessageType = MessageType.Success,
                        ImageNamePath = "~" + Url.Content(fotoFileName),
                        ImagePath = Url.Content(fotoFileName),
                        ThumbnailImagePath = Url.Content(fotoMin)
                    }
                };
            }
            catch (Exception)
            {
                return new JsonResult
                {
                    Data = new
                    {
                        IsValid = false,
                        Message = Rp3.AgendaComercial.Resources.MessageFor.NoPudoCargarArchivo,
                        MessageType = MessageType.Error
                    }
                };
            }
        }

        #endregion

        [ChildAction]
        public JsonResult ClienteContactoAutocomplete(string term, string idruta)
        {
            var data = DataBase.Clientes.GetClienteContactoSearchText(term, idruta, 50);

            var result = data.Select(p => new { label = p.DescriptionName, idCliente = p.IdCliente, idContacto = p.IdContacto ?? 0 }).Take(50);

            return new JsonResult() { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }


        #region Export

        public ActionResult ExportToXls()
        {
            var data = GetListIndex().ToList();

            return GridViewExtension.ExportToXlsx(CreateExportGridViewSettings(), data);
        }
        public ActionResult ExportToPdf()
        {
            var data = GetListIndex().ToList();                        
            
            return GridViewExtension.ExportToPdf(CreateExportGridViewSettings(), data);
        }

        public static GridViewSettings CreateExportGridViewSettings()
        {
            GridViewSettings settings = new GridViewSettings();

            settings.Name = "Clientes";
            settings.Width = Unit.Percentage(100);

            settings.Columns.Add("NombresCompletos");
            //settings.Columns.Add("TipoPersona", Rp3.AgendaComercial.Resources.LabelFor.TipoPersona);
            settings.Columns.Add("Identificacion", Rp3.AgendaComercial.Resources.LabelFor.Identificacion);
            settings.Columns.Add("CorreoElectronico", Rp3.AgendaComercial.Resources.LabelFor.CorreoElectronico);
            settings.Columns.Add("Ciudad", Rp3.AgendaComercial.Resources.LabelFor.Ciudad);
            settings.Columns.Add("Direccion", Rp3.AgendaComercial.Resources.LabelFor.Direccion);
            //settings.Columns.Add("Referencia", Rp3.AgendaComercial.Resources.LabelFor.Referencia);
            settings.Columns.Add("Telefono1", Rp3.AgendaComercial.Resources.LabelFor.Telefono);
            //settings.Columns.Add("Telefono2", Rp3.AgendaComercial.Resources.LabelFor.Telefono);
            settings.Columns.Add("TipoCliente", Rp3.AgendaComercial.Resources.LabelFor.TipoCliente);
            settings.Columns.Add("Canal", Rp3.AgendaComercial.Resources.LabelFor.Canal);
            //settings.Columns.Add("Agente", Rp3.AgendaComercial.Resources.LabelFor.Agente);

            //settings.Columns.Add(column =>
            //{
            //    column.Caption = Rp3.AgendaComercial.Resources.LabelFor.FechaUltimaVisita;
            //    column.FieldName = "FechaUtimaVisita";
            //    column.PropertiesEdit.DisplayFormatString = "g";
            //    column.Width = 110;
            //});

            //settings.Columns.Add("AgenteUltimaVisita", Rp3.AgendaComercial.Resources.LabelFor.AgenteUltimaVisita);
            //settings.Columns.Add("IndiceActividad", Rp3.AgendaComercial.Resources.LabelFor.IndiceActividad1);
            //settings.Columns.Add("IndiceActividad2", Rp3.AgendaComercial.Resources.LabelFor.IndiceActividad2);
            //settings.Columns.Add("TiempoInactividadText", Rp3.AgendaComercial.Resources.LabelFor.TiempoInactividad);

            settings.SettingsExport.Landscape = true;
            settings.SettingsExport.ReportHeader = String.Format("Listado de {0}", settings.Name);
            settings.SettingsExport.MaxColumnWidth = 110;

            return settings;
        }

        #endregion Export
    }
}



