using DevExpress.Web.Mvc;
using Rp3.AgendaComercial.Models.General;
using Rp3.Web.Mvc;
using Rp3.Web.Mvc.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace Rp3.AgendaComercial.Web.General.Controllers
{
    public class CalendarioController : Rp3.Web.Mvc.Controllers.BaseController<Models.ContextService>
    {
        // GET: General/Calendario
        [Rp3.Web.Mvc.Authorize("CALENDARIO", "QUERY", "AGENDACOMERCIAL")]
        public ActionResult Index()
        {
            return View(GetListIndex());
        }

        public ActionResult GridViewIndex()
        {
            return PartialView("_GridViewIndex", GetListIndex());
        }

        [Rp3.Web.Mvc.Authorize("CALENDARIO", "NEW", "AGENDACOMERCIAL")]
        public ActionResult Create()
        {
            Calendario model = new Calendario();
            model.DiasLaborales = new List<DiaLaboral>();
            model.DiasNoLaborables = new List<DiasNoLaborable>();

            for (int i = 1; i <= 7; i++)
                model.DiasLaborales.Add(new DiaLaboral()
                {
                    IdDia = i + "",
                    DiaString = DataBase.GeneralValues.Get(p => p.Code == i + "" && p.Id == Rp3.AgendaComercial.Models.Constantes.DiasSemana.Tabla).Select(p => p.Content).FirstOrDefault(),
                    IdDiaTabla = Rp3.AgendaComercial.Models.Constantes.DiasSemana.Tabla,
                    EsLaboral = i < 6 ? true : false,
                    SegundaJornada = i < 6 ? true : false
                });
            InicializarTab();
            ViewBag.ReadOnly = false;

            return View(model);
        }

        [Rp3.Web.Mvc.Authorize("CALENDARIO", "DETAIL", "AGENDACOMERCIAL")]
        public ActionResult Detail(int id)
        {
            Calendario model = DataBase.Calendarios.Get(p => p.IdCalendario == id, includeProperties: "DiasNoLaborables, DiasLaborales").FirstOrDefault();
            InicializarTab();
            foreach (DiaLaboral lab in model.DiasLaborales)
            {
                lab.DiaString = DataBase.GeneralValues.Get(p => p.Code == lab.IdDia && p.Id == Rp3.AgendaComercial.Models.Constantes.DiasSemana.Tabla).Select(p => p.Content).FirstOrDefault();
                if (lab.EsLaboral)
                {
                    lab.HoraInicio1 = lab.HoraInicio1.Replace("h", ":");
                    lab.HoraFin1 = lab.HoraFin1.Replace("h", ":");
                    if (lab.HoraInicio2 != null && lab.HoraInicio2 != "")
                    {
                        lab.HoraInicio2 = lab.HoraInicio2.Replace("h", ":");
                        lab.HoraFin2 = lab.HoraFin2.Replace("h", ":");
                        lab.SegundaJornada = true;
                    }
                }
            }
            ViewBag.ReadOnly = true;
            return View(model);
        }

        [Rp3.Web.Mvc.Authorize("CALENDARIO", "DELETE", "AGENDACOMERCIAL")]
        public ActionResult Delete(int id)
        {
            Calendario model = DataBase.Calendarios.Get(p => p.IdCalendario == id, includeProperties: "DiasNoLaborables, DiasLaborales").FirstOrDefault();
            InicializarTab();
            foreach (DiaLaboral lab in model.DiasLaborales)
            {
                lab.DiaString = DataBase.GeneralValues.Get(p => p.Code == lab.IdDia && p.Id == Rp3.AgendaComercial.Models.Constantes.DiasSemana.Tabla).Select(p => p.Content).FirstOrDefault();
                if (lab.EsLaboral)
                {
                    lab.HoraInicio1 = lab.HoraInicio1.Replace("h", ":");
                    lab.HoraFin1 = lab.HoraFin1.Replace("h", ":");
                    if (lab.HoraInicio2 != null && lab.HoraInicio2 != "")
                    {
                        lab.HoraInicio2 = lab.HoraInicio2.Replace("h", ":");
                        lab.HoraFin2 = lab.HoraFin2.Replace("h", ":");
                        lab.SegundaJornada = true;
                    }
                }
            }
            ViewBag.ReadOnly = true;
            return View(model);
        }

        [Rp3.Web.Mvc.Authorize("CALENDARIO", "EDIT", "AGENDACOMERCIAL")]
        public ActionResult Edit(int id)
        {
            Calendario model = DataBase.Calendarios.Get(p => p.IdCalendario == id, includeProperties: "DiasNoLaborables, DiasLaborales").FirstOrDefault();
            InicializarTab();
            foreach (DiaLaboral lab in model.DiasLaborales)
            {
                lab.DiaString = DataBase.GeneralValues.Get(p => p.Code == lab.IdDia && p.Id == Rp3.AgendaComercial.Models.Constantes.DiasSemana.Tabla).Select(p => p.Content).FirstOrDefault();
                if (lab.EsLaboral)
                {
                    lab.HoraInicio1 = lab.HoraInicio1.Replace("h", ":");
                    lab.HoraFin1 = lab.HoraFin1.Replace("h", ":");
                    if (lab.HoraInicio2 != null && lab.HoraInicio2 != "")
                    {
                        lab.HoraInicio2 = lab.HoraInicio2.Replace("h", ":");
                        lab.HoraFin2 = lab.HoraFin2.Replace("h", ":");
                        lab.SegundaJornada = true;
                    }
                }
            }
            ViewBag.ReadOnly = false;
            return View(model);
        }

        [HttpPost]
        [PreventSpam(Order = 0)]
        [Rp3.Web.Mvc.Authorize("CALENDARIO", "NEW", "AGENDACOMERCIAL", Order = 1)]
        public ActionResult Create(Calendario model)
        {
            try
            {
                model.AsignarId();

                model.Estado = Rp3.AgendaComercial.Models.Constantes.Estado.Activo;
                model.EstadoTabla = Rp3.AgendaComercial.Models.Constantes.Estado.Tabla;
                model.UsrIng = this.UserLogonName;
                model.FecIng = this.GetCurrentDateTime();
                model.UsrMod = this.UserLogonName;
                model.FecMod = this.GetCurrentDateTime();

                if (model.EsDefault)
                {
                    var modelDefaultAnterior = DataBase.Calendarios.Get(p => p.EsDefault == true).FirstOrDefault();
                    if (modelDefaultAnterior != null)
                    {
                        modelDefaultAnterior.EsDefault = false;
                        DataBase.Calendarios.Update(modelDefaultAnterior);
                    }
                }

                DataBase.Calendarios.Insert(model);

                for (int i = 1; i <= 7; i++)
                {
                    DiaLaboral laboral = new DiaLaboral();
                    laboral.IdCalendario = model.IdCalendario;
                    laboral.IdDia = i + "";
                    laboral.IdDiaTabla = Rp3.AgendaComercial.Models.Constantes.DiasSemana.Tabla;

                    laboral.EsLaboral = false;

                    if (Request.Form.Get("eslaboral_" + laboral.IdDia) != null)
                    {
                        laboral.HoraInicio1 = Request.Form.Get("desdeJornada1_" + laboral.IdDia).ToString().Replace(':', 'h');
                        laboral.HoraFin1 = Request.Form.Get("hastaJornada1_" + laboral.IdDia).ToString().Replace(':', 'h');

                        if (!String.IsNullOrEmpty(laboral.HoraInicio1) && !String.IsNullOrEmpty(laboral.HoraFin1))
                        {
                            laboral.EsLaboral = true;

                            if (Request.Form.Get("jornada2_" + laboral.IdDia) != null)
                            {
                                laboral.HoraInicio2 = Request.Form.Get("desdeJornada2_" + laboral.IdDia).ToString().Replace(':', 'h');
                                laboral.HoraFin2 = Request.Form.Get("hastaJornada2_" + laboral.IdDia).ToString().Replace(':', 'h');
                            }

                            if (String.IsNullOrEmpty(laboral.HoraInicio2) || String.IsNullOrEmpty(laboral.HoraFin2))
                            {
                                laboral.HoraInicio2 = null;
                                laboral.HoraFin2 = null;
                            }
                        }
                    }

                    if (!laboral.EsLaboral)
                    {
                        laboral.EsLaboral = false;

                        laboral.HoraInicio1 = null;
                        laboral.HoraInicio1 = null;

                        laboral.HoraInicio2 = null;
                        laboral.HoraFin2 = null;
                    }


                    laboral.UsrIng = this.UserLogonName;
                    laboral.FecIng = this.GetCurrentDateTime();
                    laboral.UsrMod = this.UserLogonName;
                    laboral.FecMod = this.GetCurrentDateTime();
                    DataBase.DiasLaborales.Insert(laboral);
                }

                List<string> valoresNoLaboral = new List<string>();
                for (int g = 0; g < Request.Form.Count; g++)
                {
                    if (Request.Form.GetKey(g).Contains("nolaboral"))
                    {
                        valoresNoLaboral.Add(Request.Form.GetKey(g).Replace("nolaboral", ""));
                    }
                }

                DiasNoLaborable noLaborableId = new DiasNoLaborable();
                noLaborableId.IdCalendario = model.IdCalendario;
                noLaborableId.AsignarId();
                int idNoLaborable = noLaborableId.IdDiaNoLaborable;

                foreach (string id in valoresNoLaboral)
                {
                    if (id.Trim() != "")
                    {
                        DateTime setter = DateTime.Parse(Request.Form.Get("nolaboral" + id).ToString());
                        DiasNoLaborable noLaborable = new DiasNoLaborable();
                        noLaborable.IdCalendario = model.IdCalendario;
                        noLaborable.IdDiaNoLaborable = idNoLaborable;

                        noLaborable.Fecha = setter;
                        //noLaborable.Fecha = setter.ToString("dd/MM");

                        if (Request.Form.Get("esteAnio" + id) != null)
                            noLaborable.EsteAño = true;
                        else
                            noLaborable.EsteAño = false;

                        if (!String.IsNullOrEmpty(Request.Form.Get("nolabInicio_" + id)) && !String.IsNullOrEmpty(Request.Form.Get("nolabFin_" + id)))
                        {
                            noLaborable.DiaParcial = true;
                            noLaborable.HoraInicio = Request.Form.Get("nolabInicio_" + id);
                            noLaborable.HoraFin = Request.Form.Get("nolabFin_" + id);
                        }
                        else
                        {
                            noLaborable.DiaParcial = false;
                            noLaborable.HoraInicio = null;
                            noLaborable.HoraFin = null;
                        }

                        noLaborable.UsrIng = this.UserLogonName;
                        noLaborable.FecIng = this.GetCurrentDateTime();
                        noLaborable.UsrMod = this.UserLogonName;
                        noLaborable.FecMod = this.GetCurrentDateTime();
                        DataBase.DiasNoLaborables.Insert(noLaborable);
                        idNoLaborable++;
                    }
                }

                
                DataBase.Save();
            }
            catch
            {
                this.AddDefaultErrorMessage();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [PreventSpam(Order = 0)]
        [Rp3.Web.Mvc.Authorize("CALENDARIO", "EDIT", "AGENDACOMERCIAL", Order = 1)]
        public ActionResult Edit(Calendario model)
        {
            try
            {
                string descripcion = model.Descripcion;
                bool predeterminado = model.EsDefault;
                model = DataBase.Calendarios.Get(p => p.IdCalendario == model.IdCalendario, includeProperties: "DiasNoLaborables, DiasLaborales").FirstOrDefault();
                model.Descripcion = descripcion;
                model.EsDefault = predeterminado;

                foreach (DiaLaboral laboral in model.DiasLaborales)
                {
                    laboral.IdDiaTabla = Rp3.AgendaComercial.Models.Constantes.DiasSemana.Tabla;

                    laboral.EsLaboral = false;

                    if (Request.Form.Get("eslaboral_" + laboral.IdDia) != null)
                    {
                        laboral.HoraInicio1 = Request.Form.Get("desdeJornada1_" + laboral.IdDia).ToString().Replace(':', 'h');
                        laboral.HoraFin1 = Request.Form.Get("hastaJornada1_" + laboral.IdDia).ToString().Replace(':', 'h');

                        if (!String.IsNullOrEmpty(laboral.HoraInicio1) && !String.IsNullOrEmpty(laboral.HoraFin1))
                        {
                            laboral.EsLaboral = true;

                            if (Request.Form.Get("jornada2_" + laboral.IdDia) != null)
                            {
                                laboral.HoraInicio2 = Request.Form.Get("desdeJornada2_" + laboral.IdDia).ToString().Replace(':', 'h');
                                laboral.HoraFin2 = Request.Form.Get("hastaJornada2_" + laboral.IdDia).ToString().Replace(':', 'h');
                            }

                            if (String.IsNullOrEmpty(laboral.HoraInicio2) || String.IsNullOrEmpty(laboral.HoraFin2))
                            {
                                laboral.HoraInicio2 = null;
                                laboral.HoraFin2 = null;
                            }
                        }
                    }

                    if (!laboral.EsLaboral)
                    {
                        laboral.EsLaboral = false;

                        laboral.HoraInicio1 = null;
                        laboral.HoraInicio1 = null;

                        laboral.HoraInicio2 = null;
                        laboral.HoraFin2 = null;
                    }

                    laboral.UsrMod = this.UserLogonName;
                    laboral.FecMod = this.GetCurrentDateTime();
                    DataBase.DiasLaborales.Update(laboral);
                }

                List<string> valoresNoLaboral = new List<string>();

                for (int g = 0; g < Request.Form.Count; g++)
                {
                    if (Request.Form.GetKey(g).Contains("nolaboral"))
                    {
                        valoresNoLaboral.Add(Request.Form.GetKey(g).Replace("nolaboral", ""));
                    }
                }

                DiasNoLaborable noLaborableId = new DiasNoLaborable();
                noLaborableId.IdCalendario = model.IdCalendario;
                noLaborableId.AsignarId();
                int idNoLaborable = noLaborableId.IdDiaNoLaborable;
                List<int> idsNoLaborables = new List<int>();

                foreach (string id in valoresNoLaboral)
                {
                    if (id.Trim() != "")
                    {
                        if (id.Contains("_"))
                        {
                            int searchId = int.Parse(id.Replace("_", ""));
                            DiasNoLaborable noLaborable = model.DiasNoLaborables.Where(p => p.IdDiaNoLaborable == searchId).FirstOrDefault();
                            DateTime setter = DateTime.Parse(Request.Form.Get("nolaboral" + id).ToString());
                            noLaborable.Fecha = setter;
                            //noLaborable.Descripcion = setter.ToString("dd/MM");

                            if (Request.Form.Get("esteAnio" + id) != null)
                                noLaborable.EsteAño = true;
                            else
                                noLaborable.EsteAño = false;

                            if (!String.IsNullOrEmpty(Request.Form.Get("nolabInicio" + id)) && !String.IsNullOrEmpty(Request.Form.Get("nolabFin" + id)))
                            {
                                noLaborable.DiaParcial = true;
                                noLaborable.HoraInicio = Request.Form.Get("nolabInicio" + id);
                                noLaborable.HoraFin = Request.Form.Get("nolabFin" + id);
                            }
                            else
                            {
                                noLaborable.DiaParcial = false;
                                noLaborable.HoraInicio = null;
                                noLaborable.HoraFin = null;
                            }

                            noLaborable.UsrMod = this.UserLogonName;
                            noLaborable.FecMod = this.GetCurrentDateTime();
                            DataBase.DiasNoLaborables.Update(noLaborable);

                            idsNoLaborables.Add(noLaborable.IdDiaNoLaborable);
                        }
                        else
                        {
                            if (!String.IsNullOrEmpty(Request.Form.Get("nolaboral" + id)))
                            {
                                DateTime setter = DateTime.Parse(Request.Form.Get("nolaboral" + id).ToString());
                                DiasNoLaborable noLaborable = new DiasNoLaborable();
                                noLaborable.IdCalendario = model.IdCalendario;
                                noLaborable.IdDiaNoLaborable = idNoLaborable;

                                noLaborable.Fecha = setter;

                                if (Request.Form.Get("esteAnio" + id) != null)
                                    noLaborable.EsteAño = true;
                                else
                                    noLaborable.EsteAño = false;

                                if (!String.IsNullOrEmpty(Request.Form.Get("nolabInicio_" + id)) && !String.IsNullOrEmpty(Request.Form.Get("nolabFin_" + id)))
                                {
                                    noLaborable.DiaParcial = true;
                                    noLaborable.HoraInicio = Request.Form.Get("nolabInicio_" + id);
                                    noLaborable.HoraFin = Request.Form.Get("nolabFin_" + id);
                                }
                                else
                                {
                                    noLaborable.DiaParcial = false;
                                    noLaborable.HoraInicio = null;
                                    noLaborable.HoraFin = null;
                                }

                                noLaborable.UsrIng = this.UserLogonName;
                                noLaborable.FecIng = this.GetCurrentDateTime();
                                noLaborable.UsrMod = this.UserLogonName;
                                noLaborable.FecMod = this.GetCurrentDateTime();
                                DataBase.DiasNoLaborables.Insert(noLaborable);
                                idsNoLaborables.Add(noLaborable.IdDiaNoLaborable);
                                idNoLaborable++;
                            }
                        }
                    }
                }

                var listDelete = DataBase.DiasNoLaborables.Get(p => p.IdCalendario == model.IdCalendario && !idsNoLaborables.Contains(p.IdDiaNoLaborable));

                foreach (var delete in listDelete)
                    DataBase.DiasNoLaborables.Delete(delete);

                model.Estado = Rp3.AgendaComercial.Models.Constantes.Estado.Activo;
                model.UsrMod = this.UserLogonName;
                model.FecMod = this.GetCurrentDateTime();

                if (model.EsDefault)
                {
                    var modelDefaultAnterior = DataBase.Calendarios.Get(p => p.EsDefault == true && p.IdCalendario != model.IdCalendario).FirstOrDefault();
                    if (modelDefaultAnterior != null)
                    {
                        modelDefaultAnterior.EsDefault = false;
                        DataBase.Calendarios.Update(modelDefaultAnterior);
                    }
                }

                DataBase.Calendarios.Update(model);
                DataBase.Save();

            }
            catch
            {
                this.AddDefaultErrorMessage();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [PreventSpam(Order = 0)]
        [Rp3.Web.Mvc.Authorize("CALENDARIO", "DELETE", "AGENDACOMERCIAL")]
        public ActionResult Delete(Calendario model)
        {
            Calendario modelToUpdate = GetIndex(model.IdCalendario);
            modelToUpdate.Estado = Rp3.AgendaComercial.Models.Constantes.Estado.Eliminado;
            DataBase.Calendarios.Update(modelToUpdate);
            DataBase.Save();
            return RedirectToAction("Index", modelToUpdate);
        }

        public List<Calendario> GetListIndex()
        {
            return DataBase.Calendarios.Get(p => p.Estado != Rp3.AgendaComercial.Models.Constantes.Estado.Eliminado, includeProperties: "EstadoGeneralValue").ToList();
        }

        public Calendario GetIndex(int id)
        {
            return DataBase.Calendarios.Get(p => p.IdCalendario == id).Where(p => p.Estado != Rp3.AgendaComercial.Models.Constantes.Estado.Eliminado).FirstOrDefault();
        }

        private void InicializarTab()
        {
            TabCollection tabCollection = new TabCollection();

            tabCollection.Add("tabdiaslaborales", TabTarget.HtmlElement, "#tabdiaslaborales", Rp3.AgendaComercial.Resources.TitleFor.DiasLaborales, true);
            tabCollection.Add("tabdiasnolaborables", TabTarget.HtmlElement, "#tabdiasnolaborables", Rp3.AgendaComercial.Resources.TitleFor.DiasNoLaborables, false);

            ViewBag.TabCollection = tabCollection;
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

            settings.Name = "Calendarios";
            settings.Width = Unit.Percentage(100);

            settings.Columns.Add("Descripcion");
            settings.Columns.Add("EstadoGeneralValue.Content", Rp3.AgendaComercial.Resources.LabelFor.Estado);

            settings.SettingsExport.Landscape = true;
            settings.SettingsExport.ReportHeader = settings.Name;        

            return settings;
        }

        #endregion Export
    }
}