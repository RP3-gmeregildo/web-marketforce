using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Rp3.Data.Models.Security;
using Rp3.Web.Mvc.Application.Security.Models;
using Rp3.Web.Mvc;
using Rp3.AgendaComercial.Models.General;
using DevExpress.Web.Mvc;
using Rp3.Data;
using Rp3.Web.Mvc.Html;
using Rp3.AgendaComercial.Models.General.View;
using Rp3.AgendaComercial.Web.Areas.General.Models;
using System.Web.UI.WebControls;

namespace Rp3.AgendaComercial.Web.General.Controllers
{
    public class TareaController : Rp3.Web.Mvc.Controllers.BaseController<Models.ContextService>
    {

        #region MAIN
        // GET: /General/Tarea/
        [Rp3.Web.Mvc.Authorize("TAREA", "QUERY", "AGENDACOMERCIAL")]
        public ActionResult Index()
        {
            return View(GetListIndex());
        }

        public ActionResult GridViewIndex()
        {
            return PartialView("_GridViewIndex", GetListIndex());
        }

        public List<Tarea> GetListIndex()
        {
            return DataBase.Tareas.Get(p=>p.Estado != Rp3.AgendaComercial.Models.Constantes.Estado.Eliminado, includeProperties: "EstadoGeneralValue, TipoTareaGeneralValue").ToList();
        }

        private TareaView GetModel(long id)
        {
            Tarea tarea = DataBase.Tareas.Get(p => p.IdTarea == id, includeProperties: "TareaActividades, TareaRutaAplicas, TareaClienteActualizacion, TareaClienteActualizacionCampos").SingleOrDefault();
            TareaView result = new TareaView();

            result.Actividades = new List<TareaActividadView>();
            List<TareaActividadView> actividadesView = new List<TareaActividadView>();
            int index = 0;
            foreach (var ac in tarea.TareaActividades)
            {
                actividadesView.Add(new TareaActividadView()
                 {
                     IdTareaActividad = ac.IdTareaActividad,
                     IdTareaActividadPadre = ac.IdTareaActividadPadre,
                     Orden = ac.Orden,
                     Descripcion = ac.Descripcion,
                     IdTipoActividad = ac.IdTipoActividad,
                     TipoActividad = ac.TipoActividad.Descripcion,
                     Opciones = ac.Opciones,
                     Valor = ac.Valor,
                     Limite = ac.Limite,
                     Index = index++
                 });
            }

            foreach (var actividad in actividadesView.Where(p => !p.IdTareaActividadPadre.HasValue))
            {
                actividad.Childs = actividadesView.Where(p => p.IdTareaActividadPadre == actividad.IdTareaActividad).ToList();
                result.Actividades.Add(actividad);
            }

            var rutas = DataBase.Rutas.Get(p => p.Estado == Models.Constantes.Estado.Activo, includeProperties: "Agentes").ToList();
            result.TareaRutasAplica = new List<TareaRutaAplicaView>();
            foreach (var rut in rutas)
            {
                result.TareaRutasAplica.Add(new TareaRutaAplicaView()
                {
                    IdRuta = rut.IdRuta,
                    Nombre = rut.Descripcion,
                    Agente = rut.Agente,
                    Aplica = tarea.TareaRutaAplicas.Any(p => p.IdRuta == rut.IdRuta)
                });
            }

            result.Descripcion = tarea.Descripcion;
            result.Estado = tarea.Estado;
            result.EstadoDescripcion = tarea.EstadoGeneralValue.Content;
            result.FechaVigenciaDesde = tarea.FechaVigenciaDesde;
            result.FechaVigenciaHasta = tarea.FechaVigenciaHasta;
            result.IdTarea = tarea.IdTarea;
            result.TipoTarea = tarea.TipoTarea;
            result.TipoTareaDescripcion = tarea.TipoTareaGeneralValue.Content;
            result.AplicaTodasLasRutas = !tarea.AplicaRutasEspecificas;
            result.EsVigenciaIndefinida = tarea.EsVigenciaIndefinida;

            result.TareaClienteActualizacionCampos = tarea.TareaClienteActualizacionCampos;

            if (tarea.TareaClienteActualizacion != null)
            {
                result.PermitirCreacion = tarea.TareaClienteActualizacion.PermitirCreacion;
                result.PermitirModificacion = tarea.TareaClienteActualizacion.PermitirModificacion;
                result.SiempreEditarEnGestion = tarea.TareaClienteActualizacion.SiempreEditarEnGestion;
                result.SoloFaltantesEditarEnGestion = tarea.TareaClienteActualizacion.SoloFaltantesEditarEnGestion;
            }

            return result;
        }

        private void InicializarTab(string tipoTarea, bool isNew = false)
        {
            TabCollection tabCollection = new TabCollection();

            tabCollection.Add("tabdato", TabTarget.HtmlElement, "#tabdato", Rp3.AgendaComercial.Resources.TitleFor.DatosGenerales, true);

            if (!isNew)
            {
                if (tipoTarea == Rp3.AgendaComercial.Models.Constantes.TipoTarea.ActualizacionClientes)
                {
                    tabCollection.Add("tabnuevos", TabTarget.HtmlElement, "#tabnuevos", Rp3.AgendaComercial.Resources.TitleFor.Nuevos, false);
                    tabCollection.Add("tabexistentes", TabTarget.HtmlElement, "#tabexistentes", Rp3.AgendaComercial.Resources.TitleFor.Existentes, false);
                    tabCollection.Add("tabgestion", TabTarget.HtmlElement, "#tabgestion", Rp3.AgendaComercial.Resources.TitleFor.GestionVisita, false);
                }

                if (tipoTarea != Rp3.AgendaComercial.Models.Constantes.TipoTarea.CheckListOportunidad)
                {
                    tabCollection.Add("tabruta", TabTarget.HtmlElement, "#tabruta", Rp3.AgendaComercial.Resources.TitleFor.Ruta, false);
                }

                tabCollection.Add("tabactividad", TabTarget.HtmlElement, "#tabactividad", Rp3.AgendaComercial.Resources.TitleFor.Actividades, false);
            }

            ViewBag.TabCollection = tabCollection;

            ViewBag.ClienteCampoGrupos = new List<ClienteCampoGrupo>()
            {
                new ClienteCampoGrupo() { IdGrupo = 1, Grupo = Rp3.AgendaComercial.Resources.LabelFor.PersonaNatural },
                new ClienteCampoGrupo() { IdGrupo = 2, Grupo = Rp3.AgendaComercial.Resources.LabelFor.PersonaJuridica },
                new ClienteCampoGrupo() { IdGrupo = 3, Grupo = Rp3.AgendaComercial.Resources.LabelFor.Persona },
                new ClienteCampoGrupo() { IdGrupo = 4, Grupo = Rp3.AgendaComercial.Resources.LabelFor.Direccion},
                new ClienteCampoGrupo() { IdGrupo = 5, Grupo = Rp3.AgendaComercial.Resources.LabelFor.Contacto }
            };

            ViewBag.ClienteCampos = DataBase.ParametroClienteCampos.Get();
        }

        #endregion

        #region EDIT

        [Rp3.Web.Mvc.Authorize("TAREA", "EDIT", "AGENDACOMERCIAL")]
        public ActionResult Edit(long id)
        {
            var model = GetModel(id);

            InicializarTab(model.TipoTarea, false);

            ViewBag.TipoActividades = DataBase.TipoActividades
                                        .Get(p => p.Estado == Models.Constantes.Estado.Activo && p.IdTipoActividad != Models.Constantes.TipoActividad.DefaultSeleccion &&
                                            p.IdTipoActividad != Models.Constantes.TipoActividad.DefaultMultipleSeleccion).ToList();

            ViewBag.EditActividad = DataBase.AgendaTareas.Get(p => p.Agenda.EstadoAgenda != Models.Constantes.EstadoAgenda.Eliminado && p.IdTarea == id).Count() == 0;

            var date = this.GetCurrentDateTime();

            ViewBag.ADCTareaRuta = DataBase.TareaRutaAplicas.Get(p=>p.IdTarea != id && p.Tarea.Estado == Models.Constantes.Estado.Activo && p.Tarea.TipoTarea == Models.Constantes.TipoTarea.ActualizacionClientes
                && p.Tarea.AplicaRutasEspecificas
                && (p.Tarea.EsVigenciaIndefinida || p.Tarea.FechaVigenciaHasta >= date));

            ViewBag.ADCExistTodasRutas = DataBase.Tareas.Get(p => p.IdTarea != id && p.Estado == Models.Constantes.Estado.Activo && p.TipoTarea == Models.Constantes.TipoTarea.ActualizacionClientes
                && !p.AplicaRutasEspecificas
                && (p.EsVigenciaIndefinida || p.FechaVigenciaHasta >= date)).Count() > 0;

            return View(model);
        }

        //ModificarListaTarea
        [ChildAction(Order = 1)]
        [Rp3.Web.Mvc.Authorize("TAREA", "EDIT", "AGENDACOMERCIAL", Order = 1)]
        [HttpPost]
        public ActionResult UpdateTarea(TareaView tareaView)
        {
            if (tareaView.Actividades != null)
            {
                foreach (var act in tareaView.Actividades)
                {
                    if (act.IdTipoActividad == null || act.IdTipoActividad == 0)
                    {
                        //this.AddErrorMessage(Rp3.AgendaComercial.Resources.ErrorMessageValidation.HoraInicioMayorHoraFin);
                        this.AddErrorMessage("Todas las actividades deben tener un tipo de actividad");
                    }
                }
            }

            if (!tareaView.EsVigenciaIndefinida && tareaView.FechaVigenciaHasta == null)
            {
                this.AddErrorMessage(Rp3.AgendaComercial.Resources.ErrorMessageValidation.IngresarFechaHasta);
            }

            if (!this.MessageCollection.HasError())
            {
                try
                {
                    Tarea tarea = new Tarea();
                    List<TareaRutaAplica> listaRutaAplica = new List<TareaRutaAplica>();
                    List<TareaActividad> listaActividad = new List<TareaActividad>();

                    tarea.IdTarea = tareaView.IdTarea;
                    tarea.Descripcion = tareaView.Descripcion;
                    tarea.Estado = tareaView.Estado;
                    tarea.UsrIng = this.UserLogonName;
                    tarea.FecMod = this.GetCurrentDateTime();

                    if (tareaView.TipoTarea != Rp3.AgendaComercial.Models.Constantes.TipoTarea.CheckListOportunidad)
                    {
                        tarea.FechaVigenciaDesde = tareaView.FechaVigenciaDesde;
                        tarea.FechaVigenciaHasta = tareaView.FechaVigenciaHasta;
                        tarea.ReadOnly = tareaView.ReadOnly;
                        tarea.EsVigenciaIndefinida = tareaView.EsVigenciaIndefinida;
                        tarea.AplicaRutasEspecificas = !tareaView.AplicaTodasLasRutas;
                    }
                    else
                    {
                        tarea.FechaVigenciaDesde = this.GetCurrentDateTime();
                        tarea.EsVigenciaIndefinida = true;
                    }


                    if (tareaView.TipoTarea == Rp3.AgendaComercial.Models.Constantes.TipoTarea.ActualizacionClientes)
                    {
                        tarea.TareaClienteActualizacion = new TareaClienteActualizacion();

                        tarea.TareaClienteActualizacion.IdTarea = tarea.IdTarea;
                        tarea.TareaClienteActualizacion.PermitirCreacion = tareaView.PermitirCreacion;
                        tarea.TareaClienteActualizacion.PermitirModificacion = tareaView.PermitirModificacion;
                        tarea.TareaClienteActualizacion.SiempreEditarEnGestion = tareaView.SiempreEditarEnGestion;
                        tarea.TareaClienteActualizacion.SoloFaltantesEditarEnGestion = tareaView.SoloFaltantesEditarEnGestion;

                        var campos = DataBase.ParametroClienteCampos.Get();
                        List<TareaClienteActualizacionCampo> listCampo = new List<TareaClienteActualizacionCampo>();

                        foreach (var campo in campos)
                        {
                            listCampo.Add(new TareaClienteActualizacionCampo()
                            {
                                IdTarea = tarea.IdTarea,
                                IdCampo = campo.IdCampo,
                                Creacion = tareaView.TareaClienteActualizacionCampos.Where(p => p.IdCampo == campo.IdCampo && p.Creacion).Count() > 0,
                                Modificacion = tareaView.TareaClienteActualizacionCampos.Where(p => p.IdCampo == campo.IdCampo && p.Modificacion).Count() > 0,
                                Gestion = tareaView.TareaClienteActualizacionCampos.Where(p => p.IdCampo == campo.IdCampo && p.Gestion).Count() > 0
                            });
                        }

                        tarea.TareaClienteActualizacionCampos = listCampo;
                    }

                    if (!tareaView.AplicaTodasLasRutas && tareaView.TipoTarea != Rp3.AgendaComercial.Models.Constantes.TipoTarea.CheckListOportunidad)
                    {
                        if (tareaView.TareaRutasAplica != null)
                        {
                            foreach (var rut in tareaView.TareaRutasAplica)
                            {
                                TareaRutaAplica rutaAplica = new TareaRutaAplica();
                                rutaAplica.IdTarea = tarea.IdTarea;//tareaView.IdTarea;
                                rutaAplica.IdRuta = rut.IdRuta;
                                listaRutaAplica.Add(rutaAplica);
                            }
                        }
                    }

                    if (tareaView.Actividades != null)
                    {
                        int maxId = DataBase.TareaActividades.GetMaxValue<int>(p => p.IdTareaActividad, 0, p => p.IdTarea == tarea.IdTarea) + 1;

                        foreach (var act in tareaView.Actividades)
                        {
                            TareaActividad actividad = new TareaActividad();
                            actividad.IdTarea = tarea.IdTarea;

                            if (act.IdTareaActividad != 0)
                            {
                                actividad.IdTareaActividad = act.IdTareaActividad;
                            }
                            else
                            {
                                actividad.IdTareaActividad = maxId;
                                maxId++;
                            }

                            actividad.Descripcion = act.Descripcion;
                            actividad.IdTipoActividad = act.IdTipoActividad;
                            actividad.Opciones = act.Opciones;
                            actividad.Valor = act.Valor;
                            actividad.Limite = act.Limite;

                            actividad.IdTareaActividadPadre = act.IdTareaActividadPadre;
                            actividad.Orden = act.Orden;
                            listaActividad.Add(actividad);
                        }
                    }

                    tarea.TareaRutaAplicas = listaRutaAplica;
                    tarea.TareaActividades = listaActividad;

                    DataBase.TareasDetalle.UpdateXml(tarea);
                    DataBase.Save();

                    VerificarDependencia(tarea);
                    ProcesarTarea(tarea);

                    this.AddDefaultSuccessMessage();
                }
                catch
                {
                    this.AddDefaultErrorMessage();
                }
            }
            return Json();
        }


        #endregion

        #region CREATE

        [Rp3.Web.Mvc.Authorize("TAREA", "NEW", "AGENDACOMERCIAL")]
        public ActionResult Create()
        {
            var model = new TareaNew();

            ViewBag.TiposSelectList = DataBase.GeneralValues.Get(p => p.Id == Rp3.AgendaComercial.Models.Constantes.TipoTarea.Tabla).ToSelectList();

            return PartialView("_Create", model);
        }

        [HttpPost]
        [Rp3.Web.Mvc.Authorize("TAREA", "NEW", "AGENDACOMERCIAL")]
        public ActionResult Create(TareaNew model)
        {
            try
            {
                Tarea tarea = new Tarea();

                tarea.AsignarId();

                tarea.Descripcion = model.Descripcion;
                tarea.TipoTareaTabla = Rp3.AgendaComercial.Models.Constantes.TipoTarea.Tabla;
                tarea.TipoTarea = model.Tipo;
                tarea.FechaVigenciaDesde = this.GetCurrentDateTime().Date;
                tarea.EsVigenciaIndefinida = true;

                tarea.EstadoTabla = Rp3.AgendaComercial.Models.Constantes.Estado.Tabla;
                tarea.Estado = Rp3.AgendaComercial.Models.Constantes.Estado.Activo;
                tarea.UsrIng = this.UserLogonName;
                tarea.FecIng = this.GetCurrentDateTime();
                tarea.FecMod = this.GetCurrentDateTime();

                if (model.Tipo == Rp3.AgendaComercial.Models.Constantes.TipoTarea.ActualizacionClientes)
                    tarea.AplicaRutasEspecificas = true;

                DataBase.TareasDetalle.InsertXml(tarea);

                if(tarea.TipoTarea == Rp3.AgendaComercial.Models.Constantes.TipoTarea.Actividad)
                {
                    TareaActividad actividad = new TareaActividad();
                    actividad.IdTarea = tarea.IdTarea;

                    actividad.IdTareaActividad = 1;

                    actividad.Descripcion = model.Descripcion;
                    actividad.IdTipoActividad = 2;

                    actividad.Orden = 1;

                    DataBase.TareaActividades.Insert(actividad);
                    DataBase.Save();
                }

                this.AddDefaultSuccessMessage();
                //return Json();

                return new JsonResult() { Data = new { IdTarea = tarea.IdTarea }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            catch
            {
                this.AddDefaultErrorMessage();
                return Json();
            }
        }

        //[Rp3.Web.Mvc.Authorize("TAREA", "NEW", "AGENDACOMERCIAL")]
        //public ActionResult Create()
        //{
        //    InicializarTab(false);

        //    TareaView model = new TareaView();
        //    model.Estado = Models.Constantes.Estado.Activo;
        //    model.TipoTarea = Models.Constantes.TipoTarea.Revision;
        //    model.AplicaTodasLasRutas = true;
        //    model.TareaRutasAplica = new List<TareaRutaAplicaView>();
        //    var rutas = DataBase.Rutas.Get(p => p.Estado == Models.Constantes.Estado.Activo, includeProperties: "Agentes").ToList();
        //    foreach (var rut in rutas)
        //    {
        //        model.TareaRutasAplica.Add(new TareaRutaAplicaView()
        //        {
        //            IdRuta = rut.IdRuta,
        //            Nombre = rut.Descripcion,
        //            Agente = rut.Agente
        //        });
        //    }

        //    ViewBag.IsNew = true;
        //    ViewBag.EditActividad = true;
        //    ViewBag.TipoActividades = DataBase.TipoActividades
        //                                .Get(p => p.Estado == Models.Constantes.Estado.Activo && p.IdTipoActividad != Models.Constantes.TipoActividad.DefaultSeleccion &&
        //                                    p.IdTipoActividad != Models.Constantes.TipoActividad.DefaultMultipleSeleccion).ToList();

        //    return View(model);
        //}

        ////GrabarListaTarea
        //[ChildAction(Order = 1)]
        //[Rp3.Web.Mvc.Authorize("TAREA", "NEW", "AGENDACOMERCIAL", Order = 1)]
        //[HttpPost]
        //public ActionResult InsertTarea(TareaView tareaView)
        //{
        //    try
        //    {
        //        if (!tareaView.EsVigenciaIndefinida && tareaView.FechaVigenciaHasta == null)
        //        {
        //            this.AddErrorMessage(Rp3.AgendaComercial.Resources.ErrorMessageValidation.IngresarFechaHasta);
        //        }

        //        if (!this.MessageCollection.HasError())
        //        {
        //            Tarea tarea = new Tarea();
        //            List<TareaRutaAplica> listaRutaAplica = new List<TareaRutaAplica>();
        //            List<TareaActividad> listaActividad = new List<TareaActividad>();

        //            tarea.AsignarId();
        //            tarea.Descripcion = tareaView.Descripcion;
        //            tarea.TipoTareaTabla = Rp3.AgendaComercial.Models.Constantes.TipoTarea.Tabla;
        //            tarea.TipoTarea = tareaView.TipoTarea;
        //            tarea.EstadoTabla = Rp3.AgendaComercial.Models.Constantes.Estado.Tabla;
        //            tarea.Estado = Rp3.AgendaComercial.Models.Constantes.Estado.Activo;
        //            //tarea.Estado = tareaView.Estado;
        //            tarea.UsrIng = this.UserLogonName;
        //            tarea.FecIng = this.GetCurrentDateTime();
        //            tarea.FecMod = this.GetCurrentDateTime();

        //            tarea.FechaVigenciaDesde = tareaView.FechaVigenciaDesde;
        //            tarea.FechaVigenciaHasta = tareaView.FechaVigenciaHasta;
        //            tarea.ReadOnly = tareaView.ReadOnly;
        //            tarea.EsVigenciaIndefinida = tareaView.EsVigenciaIndefinida;
        //            tarea.AplicaRutasEspecificas = !tareaView.AplicaTodasLasRutas;

        //            if (!tareaView.AplicaTodasLasRutas)
        //            {
        //                if (tareaView.TareaRutasAplica != null)
        //                {
        //                    foreach (var rut in tareaView.TareaRutasAplica)
        //                    {
        //                        TareaRutaAplica rutaAplica = new TareaRutaAplica();
        //                        rutaAplica.IdTarea = tarea.IdTarea;//tareaView.IdTarea;
        //                        rutaAplica.IdRuta = rut.IdRuta;
        //                        listaRutaAplica.Add(rutaAplica);
        //                    }
        //                }
        //            }

        //            int idTareaActividad = 1;

        //            if (tareaView.Actividades != null)
        //            {
        //                foreach (var act in tareaView.Actividades)
        //                {
        //                    TareaActividad actividad = new TareaActividad();

        //                    actividad.IdTarea = tarea.IdTarea;
        //                    actividad.IdTareaActividad = idTareaActividad++;
        //                    actividad.Descripcion = act.Descripcion;
        //                    actividad.IdTipoActividad = act.IdTipoActividad;
        //                    actividad.Opciones = act.Opciones;
        //                    actividad.Orden = act.Orden;
        //                    actividad.IdTareaActividadPadre = act.IdTareaActividadPadre;

        //                    listaActividad.Add(actividad);
        //                }
        //            }

        //            tarea.TareaRutaAplicas = listaRutaAplica;
        //            tarea.TareaActividades = listaActividad;

        //            DataBase.TareasDetalle.InsertXml(tarea);
        //            DataBase.Save();

        //            ProcesarTarea(tarea);

        //            this.AddDefaultSuccessMessage();
        //        }
        //    }
        //    catch
        //    {
        //        this.AddDefaultErrorMessage();
        //    }
        //    return Json();
        //}

        #endregion

        #region DELETE

        [Rp3.Web.Mvc.Authorize("TAREA", "DELETE", "AGENDACOMERCIAL")]
        public ActionResult Delete(long id)
        {
            var model = GetModel(id);

            InicializarTab(model.TipoTarea, true);

            model.ReadOnly = true;

            ViewBag.TipoActividades = DataBase.TipoActividades
                                        .Get(p => p.Estado == Models.Constantes.Estado.Activo && p.IdTipoActividad != Models.Constantes.TipoActividad.DefaultSeleccion &&
                                            p.IdTipoActividad != Models.Constantes.TipoActividad.DefaultMultipleSeleccion).ToList();
            return View(model);
        }

        //EliminarListaTarea
        [ChildAction(Order = 1)]
        [Rp3.Web.Mvc.Authorize("TAREA", "DELETE", "AGENDACOMERCIAL", Order = 1)]
        [HttpPost]
        public ActionResult DeleteTarea(TareaView model)
        {
            try
            {

                Tarea modelDelete = DataBase.Tareas.GetByID(model.IdTarea);
                modelDelete.Estado = Rp3.AgendaComercial.Models.Constantes.Estado.Eliminado;
                modelDelete.UsrMod = this.UserLogonName;
                modelDelete.FecMod = this.GetCurrentDateTime();

                DataBase.Tareas.Update(modelDelete);
                DataBase.Save();

                VerificarDependencia(modelDelete);

                this.AddDefaultSuccessMessage();
            }
            catch
            {
                this.AddDefaultErrorMessage();
            }
            return RedirectToAction("Index", model);
        }

        private void VerificarDependencia(Tarea tarea)
        {
            if (tarea.Estado == Rp3.AgendaComercial.Models.Constantes.Estado.Inactivo ||
                tarea.Estado == Rp3.AgendaComercial.Models.Constantes.Estado.Eliminado)
            {
                DataBase.Tareas.EliminarDependenciaTarea(tarea.IdTarea, this.UserLogonName);
            }
        }

        private void ProcesarTarea(Tarea tarea)
        {
            if (tarea.Estado == Rp3.AgendaComercial.Models.Constantes.Estado.Activo)
            {
                DataBase.Tareas.ProcesarTarea(tarea.IdTarea, this.UserLogonName);
            }
        }

        #endregion

        #region DETAIL

        [Rp3.Web.Mvc.Authorize("TAREA", "DETAIL", "AGENDACOMERCIAL")]
        public ActionResult Detail(long id)
        {
            var model = GetModel(id);

            InicializarTab(model.TipoTarea, false);

            model.ReadOnly = true;

            ViewBag.TipoActividades = DataBase.TipoActividades
                                        .Get(p => p.Estado == Models.Constantes.Estado.Activo && p.IdTipoActividad != Models.Constantes.TipoActividad.DefaultSeleccion &&
                                            p.IdTipoActividad != Models.Constantes.TipoActividad.DefaultMultipleSeleccion).ToList();
            return View(model);
        }

        #endregion

        #region MODAL

        [ChildAction(Order = 1)]
        [Rp3.Web.Mvc.Authorize("TAREA", "QUERY", "AGENDACOMERCIAL", Order = 2)]
        public ActionResult SetOpciones()
        {
            var model = new TipoActividad();

            model.Tipo = Models.Constantes.TipoActividad.Seleccion;
            model.Estado = Rp3.AgendaComercial.Models.Constantes.Estado.Activo;

            ViewBag.TipoActividades = DataBase.TipoActividades
                                        .Get(p => p.IdTipoActividad > 5).ToList();

            ViewBag.Tipos = DataBase.GeneralValues.Get(p => p.Id == Models.Constantes.TipoActividad.Tabla && (p.Code == Models.Constantes.TipoActividad.Seleccion || p.Code == Models.Constantes.TipoActividad.MultipleSeleccion)).ToSelectList();

            return PartialView("_SetOpciones", model);
        }

        [HttpPost]
        [ChildAction(Order = 1)]
        [Rp3.Web.Mvc.Authorize("TAREA", "QUERY", "AGENDACOMERCIAL", Order = 2)]
        public JsonResult SetOpciones(int idTipoActividad, string descripcion, string tipo, string estado, List<String> opciones)
        {
            try
            {
                if (idTipoActividad == 0)
                {
                    var model = new TipoActividad();

                    model.Descripcion = descripcion;
                    model.Tipo = tipo;

                    model.Estado = estado;
                    model.EstadoTabla = Rp3.AgendaComercial.Models.Constantes.Estado.Tabla;
                    model.TipoTabla = Rp3.AgendaComercial.Models.Constantes.TipoActividad.Tabla;

                    model.UsrIng = this.UserLogonName;
                    model.FecIng = this.GetCurrentDateTime();

                    model.TipoActividadOpciones = new List<TipoActividadOpcion>();

                    int index = 1;

                    model.AsignarId();

                    if (opciones != null)
                    {
                        foreach (string opcion in opciones)
                        {
                            model.TipoActividadOpciones.Add(new TipoActividadOpcion()
                            {
                                IdTipoActividad = model.IdTipoActividad,
                                IdTipoActividadOpcion = index,
                                Descripcion = opcion,
                                Orden = index
                            });

                            index++;
                        }
                    }

                    DataBase.TipoActividades.Insert(model);
                }
                else
                {
                    var model = DataBase.TipoActividades.Get(p => p.IdTipoActividad == idTipoActividad).FirstOrDefault();

                    model.Descripcion = descripcion;
                    model.Tipo = tipo;

                    model.Estado = estado;

                    model.UsrMod = this.UserLogonName;
                    model.FecMod = this.GetCurrentDateTime();

                    var list = new List<TipoActividadOpcion>();

                    if (opciones != null)
                    {
                        int index = 1;

                        foreach (string opcion in opciones)
                        {
                            list.Add(new TipoActividadOpcion()
                            {
                                IdTipoActividad = model.IdTipoActividad,
                                IdTipoActividadOpcion = index,
                                Descripcion = opcion,
                                Orden = index
                            });

                            index++;
                        }
                    }

                    DataBase.TipoActividadOpciones.Update(list, model.TipoActividadOpciones);

                    DataBase.TipoActividades.Update(model);
                }

                DataBase.Save();

                this.AddDefaultSuccessMessage();
                return Json();
            }
            catch (Exception e)
            {
                this.AddDefaultErrorMessage();
                return Json();
            }
        }

        [ChildAction]
        [Rp3.Web.Mvc.Authorize("TAREA", "QUERY", "AGENDACOMERCIAL", Order = 2)]
        public ActionResult GetTipoActividad()
        {
            var data = DataBase.TipoActividades
                                        .Get(p => p.Estado == Models.Constantes.Estado.Activo && p.IdTipoActividad != Models.Constantes.TipoActividad.DefaultSeleccion &&
                                            p.IdTipoActividad != Models.Constantes.TipoActividad.DefaultMultipleSeleccion).ToList();

            return new JsonResult() { Data = data.Select(p => new { id = p.IdTipoActividad, text = p.Descripcion }), JsonRequestBehavior = System.Web.Mvc.JsonRequestBehavior.AllowGet };
        }

        [ChildAction(Order = 1)]
        [Rp3.Web.Mvc.Authorize("TAREA", "QUERY", "AGENDACOMERCIAL", Order = 2)]
        [HttpGet]
        public ActionResult openPanel(int idRow, int idTarea, int idTareaActividad)
        {
            ViewBag.TipoActividades = DataBase.TipoActividades
                                        .Get(p => p.Estado == Models.Constantes.Estado.Activo)
                                        .Where(p => p.Tipo != Models.Constantes.TipoActividad.Grupo &&
                                            p.IdTipoActividad != Models.Constantes.TipoActividad.DefaultSeleccion &&
                                            p.IdTipoActividad != Models.Constantes.TipoActividad.DefaultMultipleSeleccion
                                            ).ToList();

            TareaActividad tareaConsulta = new TareaActividad();
            if (idTarea != 0 && idTareaActividad != 0)
            {
                tareaConsulta = DataBase.TareaActividades.Get(p => p.IdTarea == idTarea && p.IdTareaActividad == idTareaActividad).SingleOrDefault();
                ViewBag.isNewItem = 0;
                ViewBag.RowId = idRow;

                if (tareaConsulta.IdTipoActividad == 1)
                {
                    ViewBag.TipoActividades = DataBase.TipoActividades
                                        .Get(p => p.Estado == Models.Constantes.Estado.Activo)
                                        .Where(p => p.Tipo == Models.Constantes.TipoActividad.Grupo).ToList();
                }
            }
            else
            {
                ViewBag.isNewItem = 1;
                ViewBag.RowId = 0;
            }


            return PartialView("_VerDatos", tareaConsulta);
        }

        [ChildAction(Order = 1)]
        [Rp3.Web.Mvc.Authorize("TAREA", "QUERY", "AGENDACOMERCIAL", Order = 2)]
        [HttpGet]
        public string GetTipoActividadOpciones(int id)
        {
            string result = "";
            if (id != 0)
            {
                var tipoActividad = DataBase.TipoActividades.GetSingleOrDefault(p => p.IdTipoActividad == id);
                if (tipoActividad.Tipo == "N")
                {
                    result = "TIPON";
                }
                else if (tipoActividad.Tipo == "V")
                {
                    result = "TIPOV";
                }
                else
                {
                    List<TipoActividadOpcion> data = DataBase.TipoActividadOpciones.Get(p => p.IdTipoActividad == id).ToList();

                    int count = 0;

                    data.ForEach(delegate(TipoActividadOpcion dat)
                    {
                        if (count == 0)
                        {
                            result += dat.Descripcion;
                        }
                        else
                        {
                            result += "," + dat.Descripcion;
                        }
                        count++;
                    });
                }
            }

            return result;
        }

        #endregion

        [ChildAction]
        public JsonResult TareaAutocomplete(string term)
        {
            var tareas = DataBase.Tareas.Get(p => p.Descripcion.Contains(term) && p.TipoTarea == Rp3.AgendaComercial.Models.Constantes.TipoTarea.CheckListOportunidad &&
                p.Estado == Rp3.AgendaComercial.Models.Constantes.Estado.Activo);

            var result = tareas.Select(p => new { label = p.Descripcion, id = p.IdTarea }).Take(15);

            return new JsonResult() { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
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

            settings.Name = "Tareas";
            settings.Width = Unit.Percentage(100);

            settings.Columns.Add("Descripcion");

            var colDesde = settings.Columns.Add("FechaVigenciaDesde", Rp3.AgendaComercial.Resources.LabelFor.Desde);
            colDesde.PropertiesEdit.DisplayFormatString = Rp3.AgendaComercial.Models.Constantes.DateFormat;

            var colHasta = settings.Columns.Add("FechaVigenciaHasta", Rp3.AgendaComercial.Resources.LabelFor.Hasta);
            colHasta.PropertiesEdit.DisplayFormatString = Rp3.AgendaComercial.Models.Constantes.DateFormat;

            settings.Columns.Add("Vigente", Rp3.AgendaComercial.Resources.LabelFor.Vigente, MVCxGridViewColumnType.CheckBox);

            settings.Columns.Add("TipoTareaGeneralValue.Content", Rp3.AgendaComercial.Resources.LabelFor.Tipo);
            settings.Columns.Add("EstadoGeneralValue.Content", Rp3.AgendaComercial.Resources.LabelFor.Estado);

            settings.SettingsExport.Landscape = true;
            settings.SettingsExport.ReportHeader = settings.Name;     

            return settings;
        }

        #endregion Export
    }
}
