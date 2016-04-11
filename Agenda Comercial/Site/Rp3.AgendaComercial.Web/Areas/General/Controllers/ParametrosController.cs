using Rp3.AgendaComercial.Models.General;
using Rp3.AgendaComercial.Models.General.View;
using Rp3.Web.Mvc.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Rp3.AgendaComercial.Web.General.Controllers
{
    public class ParametrosController : Rp3.Web.Mvc.Controllers.BaseController<Models.ContextService>
    {
        // GET: General/Parametros
        [Rp3.Web.Mvc.Authorize("PARAMETROS", "EDIT", "AGENDACOMERCIAL")]
        public ActionResult Index()
        {
            return View(GetParametros());
        }

        [Rp3.Web.Mvc.Authorize("PARAMETROS", "EDIT", "AGENDACOMERCIAL")]
        public ActionResult Edit()
        {
            InicializarTab();
            return View(GetParametros());
        }

        [HttpPost]
        [Rp3.Web.Mvc.Authorize("PARAMETROS", "EDIT", "AGENDACOMERCIAL")]
        public ActionResult SaveParametro(Parametro model)
        {
            try
            {
                var modelUpdate = DataBase.Parametros.Get(p => p.IdParametro == model.IdParametro).FirstOrDefault();

                string valor = model.Valor;
                long output;

                switch ((Rp3.AgendaComercial.Models.General.View.ParametroHelper.Tipo)modelUpdate.Tipo)
                {
                    case Rp3.AgendaComercial.Models.General.View.ParametroHelper.Tipo.TextoNumero:

                        if (long.TryParse(valor, out output))
                        {
                            valor = Convert.ToString(output);
                        }
                        else
                        {
                            this.AddErrorMessage(Rp3.AgendaComercial.Resources.MessageFor.SoloNumeros);
                            return Json();
                        }

                        break;

                    case Rp3.AgendaComercial.Models.General.View.ParametroHelper.Tipo.NumeroEntero:

                        if (long.TryParse(valor, out output))
                        {
                            if (modelUpdate.Minimo != null && modelUpdate.Maximo != null)
                            {
                                if (output < modelUpdate.Minimo || output > modelUpdate.Maximo)
                                {
                                    this.AddErrorMessage(String.Format(Rp3.AgendaComercial.Resources.MessageFor.ValorComprendidoEntre, modelUpdate.Minimo, modelUpdate.Maximo));
                                    return Json();
                                }
                            }

                            valor = Convert.ToString(output);
                        }
                        else
                        {
                            this.AddErrorMessage(Rp3.AgendaComercial.Resources.MessageFor.SoloNumeros);
                            return Json();
                        }

                        break;

                    case Rp3.AgendaComercial.Models.General.View.ParametroHelper.Tipo.Hora:
                        var date = new DateTime(Convert.ToInt64(valor));
                        valor = date.ToString("HH:mm");
                        break;
                }

                if (model.IdParametro == Rp3.AgendaComercial.Models.General.View.ParametroHelper.Codigo.HoraInicioTrackingPosition)
                {
                    var fin = DataBase.Parametros.Get(p => p.IdParametro == Rp3.AgendaComercial.Models.General.View.ParametroHelper.Codigo.HoraFinTrackingPosition).FirstOrDefault();
                    
                    var inicioArray = valor.Split(':');
                    var finArray = fin.Valor.Split(':');

                    if (Convert.ToInt32(inicioArray[0]) * 100 + Convert.ToInt32(inicioArray[1]) >= Convert.ToInt32(finArray[0]) * 100 + Convert.ToInt32(finArray[1]))
                    {
                        this.AddErrorMessage(Rp3.AgendaComercial.Resources.MessageFor.InicioMenorAFin);
                        return Json();
                    }

                }

                if (model.IdParametro == Rp3.AgendaComercial.Models.General.View.ParametroHelper.Codigo.HoraFinTrackingPosition)
                {
                    var inicio = DataBase.Parametros.Get(p => p.IdParametro == Rp3.AgendaComercial.Models.General.View.ParametroHelper.Codigo.HoraInicioTrackingPosition).FirstOrDefault();

                    var inicioArray = inicio.Valor.Split(':');
                    var finArray = valor.Split(':');

                    if (Convert.ToInt32(inicioArray[0]) * 100 + Convert.ToInt32(inicioArray[1]) >= Convert.ToInt32(finArray[0]) * 100 + Convert.ToInt32(finArray[1]))
                    {
                        this.AddErrorMessage(Rp3.AgendaComercial.Resources.MessageFor.FinMayorAInicio);
                        return Json();
                    }
                }

                modelUpdate.Valor = valor;

                DataBase.Parametros.Update(modelUpdate);
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

        public List<Rp3.AgendaComercial.Models.General.View.ParametroView> GetParametros()
        {
            var lista = new List<Rp3.AgendaComercial.Models.General.View.ParametroView>();
            var listaNormal = DataBase.Parametros.Get().OrderBy(p=>p.Orden).ToList();
            Ubicacion ub = new Ubicacion();

            foreach(var parametro in listaNormal)
            {
                ParametroView setter = new ParametroView();

                setter.Name = parametro.IdParametro;
                setter.Value = parametro.Valor;
                setter.Etiqueta = parametro.Etiqueta;
                setter.Leyenda = parametro.Leyenda;
                setter.Tipo = parametro.Tipo;
                setter.Minimo = parametro.Minimo;
                setter.Maximo = parametro.Maximo;

                if (setter.Name == Rp3.AgendaComercial.Models.General.View.ParametroHelper.Codigo.LatitudeDefault)
                {
                    ub.Latitud = double.Parse(setter.Value);
                    continue;
                }

                if (setter.Name == Rp3.AgendaComercial.Models.General.View.ParametroHelper.Codigo.LongitudeDefault)
                {
                    ub.Longitud = double.Parse(setter.Value);
                    continue;
                }

                lista.Add(setter);
            }

            ViewBag.Ubicacion = ub;
            return lista;
        }

        [Rp3.Web.Mvc.Authorize("PARAMETROS", "EDIT", "AGENDACOMERCIAL")]
        public ActionResult SetUbicacion(int markerIndex, double? latitud, double? longitud)
        {
            if (longitud == null || longitud == -1)
                longitud = Ubicacion.DefaultLongitud;

            if (latitud == null || latitud == -1)
                latitud = Ubicacion.DefaultLatitud;

            Ubicacion model = new Ubicacion() { MarkerIndex = markerIndex, Longitud = longitud, Latitud = latitud };

            return PartialView("_SetUbicacion", model);
        }

        [Rp3.Web.Mvc.Authorize("PARAMETROS", "EDIT", "AGENDACOMERCIAL")]
        public ActionResult SaveUbicacion(double? latitud, double? longitud)
        {
            try
            {
                var modelLong = DataBase.Parametros.Get(p => p.IdParametro == Rp3.AgendaComercial.Models.General.View.ParametroHelper.Codigo.LongitudeDefault).FirstOrDefault();
                var modelLat = DataBase.Parametros.Get(p => p.IdParametro == Rp3.AgendaComercial.Models.General.View.ParametroHelper.Codigo.LatitudeDefault).FirstOrDefault();

                modelLong.Valor = Convert.ToString(longitud);
                modelLat.Valor = Convert.ToString(latitud);

                DataBase.Parametros.Update(modelLong);
                DataBase.Parametros.Update(modelLat);

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

        private void InicializarTab()
        {
            TabCollection tabCollection = new TabCollection();

            tabCollection.Add("tabgenerales", TabTarget.HtmlElement, "#tabgenerales", Rp3.AgendaComercial.Resources.TitleFor.DatosGenerales, true);
            tabCollection.Add("tabubicaciones", TabTarget.HtmlElement, "#tabubicaciones", Rp3.AgendaComercial.Resources.TitleFor.Ubicaciones, false);

            ViewBag.TabCollection = tabCollection;
        }
    }
}