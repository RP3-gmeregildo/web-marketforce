using DevExpress.Web.Mvc;
using Rp3.AgendaComercial.Web.Areas.Consulta.Models;
using Rp3.Web.Mvc.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DevExpress.Data;
using DevExpress.Data.Linq.Helpers;
using Rp3.AgendaComercial.Models;
using Rp3.AgendaComercial.Models.General.View;
using Rp3.AgendaComercial.Models.General;
using Rp3.AgendaComercial.Models.Consulta.View;

namespace Rp3.AgendaComercial.Web.Areas.Consulta.Controllers
{
    public static class EstadisticaEncuestaBindingHandlers
    {
        static IQueryable Model { get { return EstadisticaEncuestaController.GetListIndex(); } }

        public static void GetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e)
        {
            e.DataRowCount = Model.ApplyFilter(e.State.FilterExpression).Count();
        }
        public static void GetData(GridViewCustomBindingGetDataArgs e)
        {
            e.Data = Model.ApplyFilter(e.State.FilterExpression).ApplySorting(e.State.SortedColumns).Skip(e.StartDataRowIndex).Take(e.DataRowCount);
        }
    }

    public class EstadisticaEncuestaController : Rp3.AgendaComercial.Web.Controllers.AgendaComercialController
    {
        // GET: Consulta/EstadisticaEncuesta
        [Rp3.Web.Mvc.Authorize("ESTADENCUESTA", "QUERY", "AGENDACOMERCIAL")]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GridViewIndex()
        {
            var viewModel = GridViewExtension.GetViewModel("gridViewIndex");
            if (viewModel == null)
                viewModel = CreateGridViewModel();
            return GridCustomActionCore(viewModel);
        }

        #region Pagination & Sorting

        public ActionResult GridViewFilteringAction(GridViewColumnState column)
        {
            var viewModel = GridViewExtension.GetViewModel("gridViewIndex");
            viewModel.ApplyFilteringState(column);
            return GridCustomActionCore(viewModel);
        }

        public ActionResult GridViewSortingAction(GridViewColumnState column, bool reset)
        {
            var viewModel = GridViewExtension.GetViewModel("gridViewIndex");
            viewModel.SortBy(column, reset);
            return GridCustomActionCore(viewModel);
        }

        public ActionResult GridViewPagingAction(GridViewPagerState pager)
        {
            var viewModel = GridViewExtension.GetViewModel("gridViewIndex");
            viewModel.Pager.Assign(pager);
            return GridCustomActionCore(viewModel);
        }

        public ActionResult GridCustomActionCore(GridViewModel gridViewModel)
        {
            gridViewModel.ProcessCustomBinding(
                EstadisticaEncuestaBindingHandlers.GetDataRowCount,
                EstadisticaEncuestaBindingHandlers.GetData
            );

            return PartialView("_GridViewIndex", gridViewModel);
        }

        static GridViewModel CreateGridViewModel()
        {
            var viewModel = new GridViewModel();
            viewModel.KeyFieldName = "IdTarea";

            var col = viewModel.Columns.Add("Descripcion");
            col.SortOrder = ColumnSortOrder.Ascending;
            col.SortIndex = 0;

            viewModel.Pager.PageSize = 15;
            return viewModel;
        }

        #endregion Pagination & Sorting

        public static IQueryable<TareaResumenView> GetListIndex()
        {
            ContextService db = new ContextService();

            return db.TareaResumenViews.GetQueryable();
        }

        [Rp3.Web.Mvc.Authorize("ESTADENCUESTA", "DETAIL", "AGENDACOMERCIAL", Order = 0)]
        public ActionResult Detail(int id)
        {
            EstadisticaEncuestaDetail model = new EstadisticaEncuestaDetail();

            var resumen = DataBase.TareaResumenViews.Get(p => p.IdTarea == id).FirstOrDefault();

            model.IdTarea = id;
            model.Descripcion = resumen.Descripcion;
            model.FechaVigenciaDesde = resumen.FechaVigenciaDesde;
            model.FechaVigenciaHasta = resumen.FechaVigenciaHasta;
            model.Vigente = resumen.Vigente;
            model.NumeroClientes = resumen.NumeroClientes;
            model.NumeroGestiones = resumen.NumeroGestiones;

            InicializarIndex(model);
            return View(model);
        }

        private void InicializarIndex(EstadisticaEncuestaDetail model)
        {
           var listActividad =  DataBase.TareaActividades.Get(p => p.IdTarea == model.IdTarea).OrderBy(p => p.Orden);

           var listActividadOrder = new List<TareaActividad>();

           bool isCalificada = true;

           foreach (var item in listActividad.Where(p => p.IdTareaActividadPadre == null).OrderBy(p => p.Orden))
           {
               listActividadOrder.Add(item);
               listActividadOrder.AddRange(listActividad.Where(p => p.IdTareaActividadPadre == item.IdTareaActividad).OrderBy(p => p.Orden));
               if (item.IdTipoActividad != 6)
                   isCalificada = false;

           }

            if(isCalificada)
            {
                listActividadOrder.Add(new TareaActividad
                {
                    IdTarea = model.IdTarea,
                    IdTareaActividad = 0,
                    Descripcion = "Total",
                    TipoActividad = DataBase.TipoActividades.GetSingleOrDefault(p => p.Tipo == Rp3.AgendaComercial.Models.Constantes.TipoActividad.Texto)
                });
            }

            ViewBag.Actividades = listActividadOrder;
            ViewBag.TipoClientes = DataBase.TipoClientes.Get(p => p.Estado == Rp3.AgendaComercial.Models.Constantes.Estado.Activo).ToList();
            ViewBag.Canales = DataBase.Canales.Get(p => p.Estado == Rp3.AgendaComercial.Models.Constantes.Estado.Activo).ToList();
            ViewBag.Zonas = DataBase.Zonas.Get(p => p.Estado == Rp3.AgendaComercial.Models.Constantes.Estado.Activo).ToList();
            //ADDED BY JUCARDE 2015.08.18
            ViewBag.Agentes = DataBase.Agentes.Get(p => p.Estado == Rp3.AgendaComercial.Models.Constantes.Estado.Activo).ToList();
            ViewBag.Clientes = DataBase.Clientes.Get(p => p.Estado == Rp3.AgendaComercial.Models.Constantes.Estado.Activo).ToList();
            ViewBag.ClientesContactos = DataBase.ClienteContactos.Get(p => p.Estado == Rp3.AgendaComercial.Models.Constantes.Estado.Activo).ToList();
            ViewBag.RazonesSociales = DataBase.Clientes.Get(p => p.Estado == Rp3.AgendaComercial.Models.Constantes.Estado.Activo &
                                                                (!string.IsNullOrEmpty(p.RazonSocial) &&
                                                                  p.RazonSocial.ToUpper().Trim() != "NULL")).Select(p => new { RazonSocial = p.RazonSocial }).Distinct().ToList();
        }

        [HttpGet]
        [Rp3.Web.Mvc.Authorize("ESTADENCUESTA", "DETAIL", "AGENDACOMERCIAL", Order = 0)]
        public ActionResult GetDataText(int IdTarea, int IdTareaActividad, long FechaInicio, long FechaFin, string IdZona, string IdTipoCliente, string IdCanal, string IdAgente, string IdCliente, string IdClienteContacto, string RazonSocial)
        {
            DateTime fechaInicio = new DateTime(FechaInicio);
            DateTime fechaFin = new DateTime(FechaFin);

            var list = DataBase.Tareas.GetEstadisticaEncuesta(IdTarea, IdTareaActividad, fechaInicio, fechaFin, IdZona, IdTipoCliente, IdCanal, IdAgente, IdCliente, IdClienteContacto, RazonSocial);
            

            if(IdTareaActividad == 0)
            {
                var listActividad = DataBase.TareaActividades.Get(p => p.IdTarea == IdTarea).OrderBy(p => p.Orden);
                int valorTotal = 0, calificacion = 0;
                foreach (var item in listActividad.Where(p => p.IdTareaActividadPadre == null).OrderBy(p => p.Orden))
                {

                    list = DataBase.Tareas.GetEstadisticaEncuesta(IdTarea, item.IdTareaActividad, fechaInicio, fechaFin, IdZona, IdTipoCliente, IdCanal, IdAgente, IdCliente, IdClienteContacto, RazonSocial);
                    foreach(var calif in list)
                    {
                        valorTotal = valorTotal + item.Valor.Value;
                        if (calif.IdResultado == 1)
                            calificacion = calificacion + item.Valor.Value;
                    }
                }
                var nuevaList = new List<EstadisticaEncuesta>();
                nuevaList.Add(new EstadisticaEncuesta()
                    {
                        Cantidad = 1,
                        IdResultado = 1,
                        Resultado = calificacion + "/" + valorTotal
                    });
                list = nuevaList;
                ViewBag.TituloActividad = "Total";
            }     
            else
                ViewBag.TituloActividad = DataBase.TareaActividades.Get(p => p.IdTarea == IdTarea && p.IdTareaActividad == IdTareaActividad).FirstOrDefault().Descripcion;

            return PartialView("_ActividadText", list);
        }

        [HttpGet]
        [Rp3.Web.Mvc.Authorize("ESTADENCUESTA", "DETAIL", "AGENDACOMERCIAL", Order = 0)]
        public ActionResult GetDataCheck(int IdTarea, int IdTareaActividad, long FechaInicio, long FechaFin, string IdZona, string IdTipoCliente, string IdCanal, string IdAgente, string IdCliente, string IdClienteContacto, string RazonSocial)
        {
            DateTime fechaInicio = new DateTime(FechaInicio);
            DateTime fechaFin = new DateTime(FechaFin);

            var list = DataBase.Tareas.GetEstadisticaEncuesta(IdTarea, IdTareaActividad, fechaInicio, fechaFin, IdZona, IdTipoCliente, IdCanal, IdAgente, IdCliente, IdClienteContacto, RazonSocial);

            ViewBag.TituloActividad = DataBase.TareaActividades.Get(p => p.IdTarea == IdTarea && p.IdTareaActividad == IdTareaActividad).FirstOrDefault().Descripcion;

            return PartialView("_ActividadCheck", list);
        }

        [HttpGet]
        [Rp3.Web.Mvc.Authorize("ESTADENCUESTA", "DETAIL", "AGENDACOMERCIAL", Order = 0)]
        public ActionResult GetDataSelect(int IdTarea, int IdTareaActividad, long FechaInicio, long FechaFin, string IdZona, string IdTipoCliente, string IdCanal, string IdAgente, string IdCliente, string IdClienteContacto, string RazonSocial)
        {
            DateTime fechaInicio = new DateTime(FechaInicio);
            DateTime fechaFin = new DateTime(FechaFin);

            var list = DataBase.Tareas.GetEstadisticaEncuesta(IdTarea, IdTareaActividad, fechaInicio, fechaFin, IdZona, IdTipoCliente, IdCanal, IdAgente, IdCliente, IdClienteContacto, RazonSocial);

            ViewBag.TituloActividad = DataBase.TareaActividades.Get(p => p.IdTarea == IdTarea && p.IdTareaActividad == IdTareaActividad).FirstOrDefault().Descripcion;

            return PartialView("_ActividadSelect", list);
        }
    }
}