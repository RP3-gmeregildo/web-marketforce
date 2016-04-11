using Rp3.AgendaComercial.Models.General;
using Rp3.AgendaComercial.Models.Repositories;
using Rp3.AgendaComercial.Models.Ruta;
using Rp3.AgendaComercial.Models.Ruta.View;
using Rp3.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Rp3.AgendaComercial.Web.Ruta.Controllers
{
    public partial class RutaController
    {
        IList<SelectListItem> itemsDias;
        IList<SelectListItem> itemsSemanal;
        IList<SelectListItem> itemsDiasSemana;
        IList<SelectListItem> itemsDiasEl;
        IList<SelectListItem> itemsDiasMensual;
        IList<SelectListItem> itemsNumeroMeses;
        IList<SelectListItem> itemsTipoMensual;
        IList<SelectListItem> itemsActFechaFinal;

        [Rp3.Web.Mvc.Authorize("RUTA", "SCHEDULE", "AGENDACOMERCIAL")]
        public ActionResult Schedule(int id)
        {
            ViewBag.IdRuta = id;
            var ruta = DataBase.Rutas.GetSingleOrDefault(p => p.IdRuta == id);
            ViewBag.Ruta = ruta;
            InicializarTab(ruta);
            ViewBag.TiposClientes = DataBase.TipoClientes.Get().ToSelectList(includeNullItem: true);
            ViewBag.Canales = DataBase.Canales.Get().ToSelectList(includeNullItem: true);
            var lotesIds = DataBase.RutaLotes.Get(p => p.IdRuta == id).Select(p => p.IdLote);
            var selectLotes = DataBase.Lotes.Get(p => lotesIds.Contains(p.IdLote)).ToSelectList(includeNullItem: true);
            var itemsLotes = new List<SelectListItem>();
            foreach (var option in selectLotes)
                itemsLotes.Add(new SelectListItem { Text = option.Text, Value = option.Value });
            itemsLotes.Add(new SelectListItem { Text = "Sin Lote", Value = "0" });

            ViewBag.Lotes = itemsLotes;

            //var model = DataBase.Rutas.GetClienteProgramacion(id, 1, 10, null);        

            return View();
        }

        [HttpPost]
        [Rp3.Web.Mvc.Authorize("RUTA", "SCHEDULE", "AGENDACOMERCIAL")]
        public ActionResult RutaProcess(int IdRuta)
        {
            try
            {
                Rp3.AgendaComercial.Process.Executor.Agenda(DataBase, IdRuta);
                   
                this.AddDefaultSuccessMessage();
            }
            catch
            {
                this.AddDefaultErrorMessage();
            }

            return Json();
        }

        [ChildAction(Order = 1)]
        [Rp3.Web.Mvc.Authorize("RUTA", "SCHEDULE", "AGENDACOMERCIAL", Order = 2)]
        [HttpGet]
        public ActionResult ConsultaRutaScheduleDetalle(int ruta, int pagina, int numreg, string buscar, string idcanal, string idtipocliente, string idlote)
        {
            var data = DataBase.Rutas.GetClienteProgramacion(ruta, pagina, numreg, buscar, idcanal, idtipocliente, idlote);
            
            return PartialView("_ProgramacionesClientes", data);
        }

        public ActionResult GetProgramacion(int? id, int idCliente, int idClienteDireccion, int idRuta)
        {
            InicializarEditProgramacionRuta(idRuta);
            var model = GetProgramacionRuta(id, idCliente, idClienteDireccion, idRuta);
            if (model.Lunes) model.diaString = "1";
            if (model.Martes) model.diaString = "2";
            if (model.Miercoles) model.diaString = "3";
            if (model.Jueves) model.diaString = "4";
            if (model.Viernes) model.diaString = "5";
            if (model.Sabado) model.diaString = "6";
            if (model.Domingo) model.diaString = "7";
            if (model.Dia) model.diaString = "8";
            if (model.DiaLaboral) model.diaString = "9";
            if (model.DiaFinDeSemana) model.diaString = "10";
            if (model.diaString != null)
                model.TipoMensual = "2";
            else
                model.TipoMensual = "1";
            return PartialView("_SetProgramacion", model);
        }

        [HttpPost]
        [Rp3.Web.Mvc.Authorize("RUTA", "SCHEDULE", "AGENDACOMERCIAL")]
        public ActionResult Schedule(ProgramacionRuta model)
        {
            ViewBag.IdRuta = model.IdRuta;
            var ruta = DataBase.Rutas.GetSingleOrDefault(p => p.IdRuta == model.IdRuta);
            ViewBag.Ruta = ruta;
            InicializarTab(ruta);
            
            return View();
        }

        [HttpPost]
        [Rp3.Web.Mvc.Authorize("RUTA", "SCHEDULE", "AGENDACOMERCIAL")]
        public ActionResult DeleteProgramacion(int id)
        {
            var model = DataBase.ProgramacionRutas.Get(p => p.IdProgramacionRuta == id).FirstOrDefault();
            try
            {
                model.Estado = Rp3.AgendaComercial.Models.Constantes.Estado.Eliminado;
                DataBase.ProgramacionRutas.Update(model);
                DataBase.Save();
            }
            catch
            {
                this.AddDefaultErrorMessage();
            }
            var list = DataBase.ProgramacionRutas.Get(p=> p.IdRuta == model.IdRuta && p.IdCliente == model.IdCliente && p.IdClienteDireccion == model.IdClienteDireccion
                && p.Estado == Rp3.AgendaComercial.Models.Constantes.Estado.Activo).ToList();
            return PartialView("_ProgramacionRuta", list);
        }

        [HttpPost]
        [Rp3.Web.Mvc.Authorize("RUTA", "SCHEDULE", "AGENDACOMERCIAL")]
        public ActionResult DeleteProgramacionAgendas(int id)
        {
            DateTime setter = DateTime.Now;
            DateTime hoy = new DateTime(setter.Year, setter.Month, setter.Day);
            hoy = hoy.AddDays(1);
            var list_agenda = DataBase.Agendas.Get(p => p.IdProgramacionRuta == id && p.FechaInicio > hoy && p.EstadoAgenda == Rp3.AgendaComercial.Models.Constantes.EstadoAgenda.Pendiente).ToList();
            try
            {
                foreach(Agenda agd in list_agenda)
                {
                    agd.EstadoAgenda = Rp3.AgendaComercial.Models.Constantes.EstadoAgenda.Eliminado;
                    agd.EstadoAgendaGeneralValue = null;
                    agd.FecMod = GetCurrentDateTime();
                    DataBase.Agendas.UpdateXml(agd);
                }
                DataBase.Save();
            }
            catch
            {
                this.AddDefaultErrorMessage();
            }
            return PartialView();
        }

        [HttpPost]
        [Rp3.Web.Mvc.Authorize("RUTA", "SCHEDULE", "AGENDACOMERCIAL")]
        public ActionResult SaveProgramacion(ProgramacionRuta model)
        {
            try
            {
                ProgramacionRuta modelUpdate = null;

                if(model.IdProgramacionRuta == 0)
                {
                    modelUpdate = new ProgramacionRuta();
                    DataBase.ProgramacionRutas.Update(model, modelUpdate);
                    modelUpdate.AsignarId();
                    foreach (ProgramacionRutaTarea tarea in model.ProgramacionRutaTareas)
                    {
                        tarea.IdProgramacionRuta = modelUpdate.IdProgramacionRuta;
                        DataBase.ProgramacionRutaTareas.Insert(tarea);
                    }
                }
                else
                {
                    modelUpdate = DataBase.ProgramacionRutas.Get(p => p.IdRuta == model.IdRuta && p.IdCliente == model.IdCliente &&
                                                                                p.IdClienteDireccion == model.IdClienteDireccion && p.IdProgramacionRuta == model.IdProgramacionRuta).FirstOrDefault();
                    modelUpdate.ProgramacionRutaTareas = DataBase.ProgramacionRutaTareas.Get(p => p.IdProgramacionRuta == modelUpdate.IdProgramacionRuta).ToList();
                    DataBase.ProgramacionRutaTareas.Update(model.ProgramacionRutaTareas, modelUpdate.ProgramacionRutaTareas);
                }
                


                InitializeCatalogs();
                if (model.Patron == "D")
                {
                    modelUpdate.Descripcion = string.Format(Rp3.AgendaComercial.Resources.MessageFor.DescripcionDiaria,
                        itemsDias.Where(p => p.Value == model.Frecuencia + "").FirstOrDefault().Text.ToLower(), model.FechaInicio.Value.ToString("dd/MM/yyyy"));
                }
                if (model.Patron == "S")
                {
                    string dias = "";
                    if (model.Lunes) dias = dias + "Lunes, ";
                    if (model.Martes) dias = dias + "Martes, ";
                    if (model.Miercoles) dias = dias + "Miércoles, ";
                    if (model.Jueves) dias = dias + "Jueves, ";
                    if (model.Viernes) dias = dias + "Viernes, ";
                    if (model.Sabado) dias = dias + "Sábado, ";
                    if (model.Domingo) dias = dias + "Domingo, ";

                    dias = dias.Substring(0, dias.Length - 2);

                    modelUpdate.Descripcion = string.Format(Rp3.AgendaComercial.Resources.MessageFor.DescripcionSemanal, dias,
                        itemsSemanal.Where(p => p.Value == model.Frecuencia + "").FirstOrDefault().Text.ToLower(), model.FechaInicio.Value.ToString("dd/MM/yyyy"));
                }

                if (model.Patron == "M")
                {
                    if(model.TipoMensual == "1")
                        modelUpdate.Descripcion = string.Format(Rp3.AgendaComercial.Resources.MessageFor.DescripcionMensualDia, model.DiaMes,
                            itemsNumeroMeses.Where(p => p.Value == model.Frecuencia + "").FirstOrDefault().Text.ToLower(), model.FechaInicio.Value.ToString("dd/MM/yyyy"));
                    if (model.TipoMensual == "2")
                    {
                        string dias = "";
                        if (model.Lunes) dias = dias + "Lunes";
                        if (model.Martes) dias = dias + "Martes";
                        if (model.Miercoles) dias = dias + "Miércoles";
                        if (model.Jueves) dias = dias + "Jueves";
                        if (model.Viernes) dias = dias + "Viernes";
                        if (model.Sabado) dias = dias + "Sábado";
                        if (model.Domingo) dias = dias + "Domingo";
                        if (model.Dia) dias = dias + "Día";
                        if (model.DiaLaboral) dias = dias + "Día Laboral";
                        if (model.DiaFinDeSemana) dias = dias + "Fin de Semana";
                        modelUpdate.Descripcion = string.Format(Rp3.AgendaComercial.Resources.MessageFor.DescripcionMensualEl,
                            itemsDiasEl.Where(p => p.Value == model.Semana + "").FirstOrDefault().Text.ToLower(), dias,
                            itemsNumeroMeses.Where(p => p.Value == model.Frecuencia + "").FirstOrDefault().Text.ToLower(),
                            model.FechaInicio.Value.ToString("dd/MM/yyyy"));
                        //modelUpdate.Semana = model.DiaMes;
                    }

                }

                if (model.FechaFinTicks.HasValue && model.FechaFinTicks != 0)
                    modelUpdate.Descripcion = model.Descripcion + (string.Format(Rp3.AgendaComercial.Resources.MessageFor.DescripcionSinFin, model.FechaFin.Value.ToString("dd/MM/yyyy")));

                modelUpdate.PatronTabla = Rp3.AgendaComercial.Models.Constantes.PatronProgramacion.Tabla;
                modelUpdate.EstadoTabla = Rp3.AgendaComercial.Models.Constantes.Estado.Tabla;
                modelUpdate.Estado = Rp3.AgendaComercial.Models.Constantes.Estado.Activo;
                modelUpdate.UsrMod = this.UserLogonName;
                modelUpdate.FecMod = this.GetCurrentDateTime();

                modelUpdate.DiaMes = 0;
                modelUpdate.Lunes = model.Lunes;
                modelUpdate.Martes = model.Martes;
                modelUpdate.Miercoles = model.Miercoles;
                modelUpdate.Jueves = model.Jueves;
                modelUpdate.Viernes = model.Viernes;
                modelUpdate.Sabado = model.Sabado;
                modelUpdate.Domingo = model.Domingo;
                modelUpdate.Dia = model.Dia;
                modelUpdate.DiaLaboral = model.DiaLaboral;
                modelUpdate.DiaFinDeSemana = model.DiaFinDeSemana;
                modelUpdate.DiaMes = model.DiaMes;
                modelUpdate.Frecuencia = model.Frecuencia;
                modelUpdate.Patron = model.Patron;
                modelUpdate.PatronTabla = model.PatronTabla;
                modelUpdate.Semana = model.Semana;
                modelUpdate.FechaInicio = model.FechaInicio;
                modelUpdate.FechaFin = model.FechaFin;

                if (model.IdProgramacionRuta == 0)
                {
                    modelUpdate.UsrIng = this.UserLogonName;
                    modelUpdate.FecIng = this.GetCurrentDateTime();
                    modelUpdate.FechaUltimaEjecucion = model.FechaInicio.Value;
                    DataBase.ProgramacionRutas.Insert(modelUpdate);
                }
                else
                {
                    DataBase.ProgramacionRutas.Update(modelUpdate);
                }
                DataBase.Save();
            }
            catch
            {
                this.AddDefaultErrorMessage();
            }
            var list = DataBase.ProgramacionRutas.Get(p=> p.IdRuta == model.IdRuta && p.IdCliente == model.IdCliente && p.IdClienteDireccion == model.IdClienteDireccion
                && p.Estado == Rp3.AgendaComercial.Models.Constantes.Estado.Activo).ToList();
            return PartialView("_ProgramacionRuta", list);
        }
        private void InicializarEditProgramacionRuta(int idRuta)
        {
            InitializeCatalogs();
            List<int> diasMes = new List<int>();
            for (int i = 1; i <= 31; i++)
                diasMes.Add(i);

            var listTipoMensual = new SelectList(itemsTipoMensual, "Value", "Text");
            var listActFechaFinal = new SelectList(itemsActFechaFinal, "Value", "Text");
            var listDiasSemana = new SelectList(itemsDiasSemana, "Value", "Text");
            var listDiasMes = diasMes.Select(d => new SelectListItem() { Text = d.ToString(), Value = d.ToString() });
            
            ViewBag.ItemsDias = itemsDias;
            ViewBag.ItemsSemanal = itemsSemanal;
            ViewBag.TipoMensual = listTipoMensual;
            ViewBag.ActFechaFinal = listActFechaFinal;
            ViewBag.ItemsDiasSemana = listDiasSemana;
            ViewBag.ItemsDiasEl = itemsDiasEl;
            ViewBag.ItemsDiasMensual = itemsDiasMensual;
            ViewBag.ItemsNumeroMeses = itemsNumeroMeses;
            ViewBag.ItemsDiasMes = listDiasMes;
        }

        private void InitializeCatalogs()
        {
            itemsDias = new List<SelectListItem>
            {
                new SelectListItem{Text = "Cada 1 día", Value = "1"},
            };
            for (int i = 2; i < 31; i++)
                itemsDias.Add(new SelectListItem { Text = "Cada " + i + " días", Value = i + "" });


            itemsSemanal = new List<SelectListItem>
            {
                new SelectListItem{Text = "Cada semana", Value = "1"},
                new SelectListItem{Text = "Cada 2 semanas", Value = "2"},
                new SelectListItem{Text = "Cada 3 semanas", Value = "3"},
                new SelectListItem{Text = "Cada 4 semanas", Value = "4"},
                new SelectListItem{Text = "Última semana", Value = "5"},
            };

            itemsDiasSemana = new List<SelectListItem>
            {
                new SelectListItem{Text = "Lunes", Value = "1"},
                new SelectListItem{Text = "Martes", Value = "2"},
                new SelectListItem{Text = "Miércoles", Value = "3"},
                new SelectListItem{Text = "Jueves", Value = "4"},
                new SelectListItem{Text = "Viernes", Value = "5"},
                new SelectListItem{Text = "Sábado", Value = "6"},
                new SelectListItem{Text = "Domingo", Value = "7"},
            };

            itemsDiasEl = new List<SelectListItem>
            {
                new SelectListItem{Text = "Primer", Value = "1"},
                new SelectListItem{Text = "Segundo", Value = "2"},
                new SelectListItem{Text = "Tercer", Value = "3"},
                new SelectListItem{Text = "Cuarto", Value = "4"},
                new SelectListItem{Text = "Último", Value = "5"},
            };

            itemsDiasMensual = new List<SelectListItem>
            {
                new SelectListItem{Text = "Lunes", Value = "1"},
                new SelectListItem{Text = "Martes", Value = "2"},
                new SelectListItem{Text = "Miércoles", Value = "3"},
                new SelectListItem{Text = "Jueves", Value = "4"},
                new SelectListItem{Text = "Viernes", Value = "5"},
                new SelectListItem{Text = "Sábado", Value = "6"},
                new SelectListItem{Text = "Domingo", Value = "7"},
                new SelectListItem{Text = "Día", Value = "8"},
                new SelectListItem{Text = "Día Laboral", Value = "9"},
                new SelectListItem{Text = "Fin de Semana", Value = "10"},
            };

            itemsNumeroMeses = new List<SelectListItem>
            {
                new SelectListItem{Text = "Cada mes", Value = "1"},
                new SelectListItem{Text = "Cada 2 meses", Value = "2"},
                new SelectListItem{Text = "Cada 3 meses", Value = "3"},
                new SelectListItem{Text = "Cada 4 meses", Value = "4"},
                new SelectListItem{Text = "Cada 5 meses", Value = "5"},
                new SelectListItem{Text = "Cada 6 meses", Value = "6"},
                new SelectListItem{Text = "Cada 7 meses", Value = "7"},
                new SelectListItem{Text = "Cada 8 meses", Value = "8"},
                new SelectListItem{Text = "Cada 9 meses", Value = "9"},
                new SelectListItem{Text = "Cada 10 meses", Value = "10"},
                new SelectListItem{Text = "Cada 11 meses", Value = "11"},
                new SelectListItem{Text = "Cada 12 meses", Value = "12"},
            };

            itemsTipoMensual = new List<SelectListItem>
            {
                new SelectListItem{Text = "Día   ", Value = "1"},
                new SelectListItem{Text = "El   ", Value = "2"},
            };

            itemsActFechaFinal = new List<SelectListItem>
            {
                new SelectListItem{Text = "Con Fecha Fin   ", Value = "1"},
                new SelectListItem{Text = "Sin Fecha Fin   ", Value = "2"},
            };
        }

        private ProgramacionRuta GetProgramacionRuta(int? id, int idCliente, int idClienteDireccion, int idRuta)
        {
            var itemsTareasCatalog = new List<SelectListItem>();
            if (id == null)
            {
                var model = new ProgramacionRuta();
                ViewBag.ButtonLabel = Rp3.AgendaComercial.Resources.LabelFor.Agregar;
                ViewBag.DiarioDisplay = "block";
                ViewBag.SemanalDisplay = "none";
                ViewBag.MensualDisplay = "none";
                ViewBag.MensualDiaDisplay = "block";
                ViewBag.MensualElDisplay = "none";
                model.TipoMensual = "1";
                model.ConFechaFin = true;
                
                var tareas = DataBase.Tareas.GetSync(idRuta, null, null).Where(p => p.TipoTarea != Models.Constantes.TipoTarea.CheckListOportunidad && p.Estado == Models.Constantes.Estado.Activo);
                foreach(Tarea tar in tareas)
                {
                    itemsTareasCatalog.Add(new SelectListItem() { Text = tar.Descripcion, Value = tar.IdTarea + "", Selected = true });
                }
                var itemsTareas = new MultiSelectList(itemsTareasCatalog, "Value", "Text", itemsTareasCatalog.Select(p => p.Value).ToList());
                ViewBag.ItemsTareas = itemsTareas;
                return model;
            }
            else
            {
                var tareasProgramadas = DataBase.ProgramacionRutaTareas.Get(p => p.IdProgramacionRuta == id.Value).Select(p => p.IdTarea).ToList();
                var tareas = DataBase.Tareas.GetSync(idRuta, null, null).Where(p => p.TipoTarea != Models.Constantes.TipoTarea.CheckListOportunidad);
                foreach (Tarea tar in tareas)
                {
                    itemsTareasCatalog.Add(new SelectListItem() { Text = tar.Descripcion, Value = tar.IdTarea + "", Selected = tareasProgramadas.Contains(tar.IdTarea) });
                }
                var itemsTareas = new MultiSelectList(itemsTareasCatalog, "Value", "Text", tareasProgramadas);
                ViewBag.ItemsTareas = itemsTareas;

                ViewBag.ButtonLabel = Rp3.AgendaComercial.Resources.LabelFor.Guardar;
                var model = DataBase.ProgramacionRutas.Get(p => p.IdProgramacionRuta == id).FirstOrDefault();
                model.TipoMensual = "1";
                ViewBag.MensualDiaDisplay = "block";
                ViewBag.MensualElDisplay = "none";

                if(model.Patron == "D")
                {
                    ViewBag.DiarioDisplay = "block";
                    ViewBag.SemanalDisplay = "none";
                    ViewBag.MensualDisplay = "none";
                }

                if (model.Patron == "S")
                {
                    ViewBag.DiarioDisplay = "none";
                    ViewBag.SemanalDisplay = "block";
                    ViewBag.MensualDisplay = "none";
                    itemsDiasSemana = new List<SelectListItem>
                    {
                        new SelectListItem{Text = "Lunes", Value = "1", Selected = model.Lunes},
                        new SelectListItem{Text = "Martes", Value = "2", Selected = model.Martes},
                        new SelectListItem{Text = "Miércoles", Value = "3", Selected = model.Miercoles},
                        new SelectListItem{Text = "Jueves", Value = "4", Selected = model.Jueves},
                        new SelectListItem{Text = "Viernes", Value = "5", Selected = model.Viernes},
                        new SelectListItem{Text = "Sábado", Value = "6", Selected = model.Sabado},
                        new SelectListItem{Text = "Domingo", Value = "7", Selected = model.Domingo},
                    };
                    var selectedItems = itemsDiasSemana.Where(p => p.Selected == true).Select(p=> p.Value).ToList();
                    //var listSelectedItems = new MultiSelectList(selectedItems, "Value", "Text");
                    var listDiasSemana = new MultiSelectList(itemsDiasSemana, "Value", "Text", selectedItems);
                    ViewBag.ItemsDiasSemana = listDiasSemana;
                }

                if (model.Patron == "M")
                {
                    ViewBag.DiarioDisplay = "none";
                    ViewBag.SemanalDisplay = "none";
                    ViewBag.MensualDisplay = "block";

                    if (model.Lunes || model.Martes || model.Miercoles || model.Jueves || model.Viernes || model.Sabado
                        || model.Domingo || model.Dia || model.DiaLaboral || model.DiaFinDeSemana)
                    {
                        itemsDiasMensual = new List<SelectListItem>
                        {
                            new SelectListItem{Text = "Lunes", Value = "1", Selected = model.Lunes},
                            new SelectListItem{Text = "Martes", Value = "2", Selected = model.Martes},
                            new SelectListItem{Text = "Miércoles", Value = "3", Selected = model.Miercoles},
                            new SelectListItem{Text = "Jueves", Value = "4", Selected = model.Jueves},
                            new SelectListItem{Text = "Viernes", Value = "5", Selected = model.Viernes},
                            new SelectListItem{Text = "Sábado", Value = "6", Selected = model.Sabado},
                            new SelectListItem{Text = "Domingo", Value = "7", Selected = model.Domingo},
                            new SelectListItem{Text = "Día", Value = "8", Selected = model.Dia},
                            new SelectListItem{Text = "Día Laboral", Value = "9", Selected = model.DiaLaboral},
                            new SelectListItem{Text = "Fin de Semana", Value = "10", Selected = model.DiaFinDeSemana},
                        };
                        model.TipoMensual = "2";
                        ViewBag.ItemsDiasMensual = itemsDiasMensual;
                    }
                }

                if (model.FechaFinTicks.HasValue && model.FechaFinTicks != 0)
                    model.ConFechaFin = true;
                else
                    model.ConFechaFin = false;
                return model;
            }
        }

    }
}