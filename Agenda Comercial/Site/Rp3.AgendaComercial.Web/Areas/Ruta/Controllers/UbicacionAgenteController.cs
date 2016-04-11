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
using Rp3.AgendaComercial.Models.General;
using Rp3.Web.Mvc.Utility;

namespace Rp3.AgendaComercial.Web.Ruta.Controllers
{
    public class UbicacionAgenteController : Web.Controllers.AgendaComercialController
    {
        //
        // GET: /Ruta/AgenteUbicacion/

        [Rp3.Web.Mvc.Authorize("UBICACIONAGENTE", "QUERY", "AGENDACOMERCIAL")]
        public ActionResult Index()
        {
            InicializarEdit();

            
            var model = new UbicacionAgente();
            model.AgenteUbicaciones = new List<AgenteUbicacion>();
            model.Agentes = GetAgenteListDetail(((List<Agente>)ViewBag.Agentes).Select(p => p.IdAgente.ToString()).ToArray<string>());//new List<UbicacionAgenteDetalle>();

            model.Filter1 = true;
            model.Filter24 = true;
            model.Filter48 = true;
            model.FilterMAS = true;

            DataBase.ParametroHelper.Load();

            ViewBag.AgenteUbicacion1 = DataBase.ParametroHelper.AgenteUbicacion1;
            ViewBag.AgenteUbicacion2 = DataBase.ParametroHelper.AgenteUbicacion2;
            ViewBag.AgenteUbicacion3 = DataBase.ParametroHelper.AgenteUbicacion3;

            return View(model);
        }

        [Rp3.Web.Mvc.Authorize("UBICACIONAGENTE", "QUERY", "AGENDACOMERCIAL")]
        [HttpPost]        
        public ActionResult Index(UbicacionAgente model, string[] agentes)
        {
            InicializarEdit();

            model.Agentes = GetAgenteListDetail(agentes);

            DataBase.ParametroHelper.Load();

            ViewBag.AgenteUbicacion1 = DataBase.ParametroHelper.AgenteUbicacion1;
            ViewBag.AgenteUbicacion2 = DataBase.ParametroHelper.AgenteUbicacion2;
            ViewBag.AgenteUbicacion3 = DataBase.ParametroHelper.AgenteUbicacion3;

            try
            {                   
                var idAgente = model.Agentes.Select(p => p.IdAgente).ToArray<int>();

                model.AgenteUbicaciones = (from e in DataBase.AgenteUbicaciones.GetQueryable(p=> idAgente.Contains(p.IdAgente))
                                          group e by e.IdAgente into g
                                          select g.OrderByDescending(e => e.Fecha).FirstOrDefault() into r
                                          select r).ToList();

                DateTime ahora = GetCurrentDateTime();
                
                foreach(var item in model.AgenteUbicaciones)
                {
                    var agente = ((List<Agente>)ViewBag.Agentes).Where(p => p.IdAgente == item.IdAgente).FirstOrDefault();

                    if (agente != null)
                    {
                        item.MarkerIndex = agente.MarkerIndex;
                        item.Agente = agente;
                    }

                    long ticks = ahora.Ticks - item.Fecha.Ticks;
                    long minutos = ticks / (60 * 1000 * 10000);

                    if (minutos < 60)
                    {
                        item.HorasUltimaConexion = 0;
                        item.UltimaConexion = string.Format(Resources.MessageFor.HaceMinutos, minutos);
                    }
                    else
                    {
                        long horas = minutos / 60;
                        item.HorasUltimaConexion = horas;

                        if (horas > 24)
                            item.UltimaConexion = string.Format(Resources.MessageFor.HaceDias, horas / 24);
                        else
                            item.UltimaConexion = string.Format(Resources.MessageFor.HaceHoras, horas);
                    }
                }

                for (int i = model.AgenteUbicaciones.Count - 1; i >= 0; i--)
                {
                    var item = model.AgenteUbicaciones[i];

                    if ((item.HorasUltimaConexion <= DataBase.ParametroHelper.AgenteUbicacion1 && !model.Filter1) ||
                        (item.HorasUltimaConexion > DataBase.ParametroHelper.AgenteUbicacion1 && item.HorasUltimaConexion <= DataBase.ParametroHelper.AgenteUbicacion2 && !model.Filter24) ||
                        (item.HorasUltimaConexion > DataBase.ParametroHelper.AgenteUbicacion2 && item.HorasUltimaConexion <= DataBase.ParametroHelper.AgenteUbicacion3 && !model.Filter48) ||
                        (item.HorasUltimaConexion > DataBase.ParametroHelper.AgenteUbicacion3 && !model.FilterMAS))
                    {
                        model.AgenteUbicaciones.Remove(item);
                    }
                }

                return View(model);
            }
            catch
            {
                this.AddDefaultErrorMessage();
            }

            model.AgenteUbicaciones = new List<AgenteUbicacion>();            
            return View(model);
        }

        public List<UbicacionAgenteDetalle> GetAgenteListDetail(string[] agentes)
        {
            List<UbicacionAgenteDetalle> listEdit = new List<UbicacionAgenteDetalle>();

            if (agentes != null)
            {
                foreach (var insert in agentes.Where(p => p != "false"))
                {
                    string[] keyParts = insert.Split('-');

                    UbicacionAgenteDetalle detalle = new UbicacionAgenteDetalle()
                    {
                        IdAgente = Convert.ToInt32(keyParts[0])
                    };

                    listEdit.Add(detalle);
                }
            }

            return listEdit;
        }

        private void InicializarEdit()
        {
            List<Models.General.Agente> agentes = null;
            if (Agente.EsAdministrador)
                agentes = DataBase.Agentes.Get(p => p.Estado == Models.Constantes.Estado.Activo).ToList();
            else
                agentes = DataBase.Agentes.Get(p =>
                    p.IdSupervisor == Agente.IdAgente &&
                    p.Estado == Models.Constantes.Estado.Activo).ToList();
                             
            SetMarkers(agentes);

            ViewBag.Agentes = agentes;
        }

        private void SetMarkers(List<Agente> agentes)
        {
            int markerIndex = 1;

            foreach (var item in agentes.OrderBy(p => p.Descripcion))
            {
                item.MarkerIndex = markerIndex;
                markerIndex++;

                if (markerIndex >= 100)
                    markerIndex = 100;
            }
        }

        [ChildAction(Order = 1)]
        [Rp3.Web.Mvc.Authorize("UBICACIONAGENTE", "QUERY", "AGENDACOMERCIAL", Order = 2)]
        [HttpGet]
        public ActionResult Notificacion()
        {
            return PartialView("_Notificacion");
        }

        [ChildAction(Order = 1)]
        [Rp3.Web.Mvc.Authorize("UBICACIONAGENTE", "QUERY", "AGENDACOMERCIAL", Order = 2)]
        [HttpPost]
        public ActionResult Notificacion(int idAgente, string titulo, string mensaje)
        {
            try
            {
                if (String.IsNullOrEmpty(titulo) || String.IsNullOrEmpty(mensaje))
                {
                    this.AddErrorMessage(Rp3.AgendaComercial.Resources.ErrorMessageValidation.DatosIncompletos);
                }

                if (!this.MessageCollection.HasError())
                {
                    var usuario = DataBase.Agentes.Get(p => p.IdUsuario == this.UserId).FirstOrDefault();
                    string emails = String.Empty;

                    var agente = DataBase.Agentes.Get(p => p.IdAgente == idAgente).FirstOrDefault();

                    string footer = String.Format("Enviada por: {0} - el {1:g}", usuario.Descripcion, this.GetCurrentDateTime());

                    if (agente != null && agente.IdUsuario != null)
                    {
                        if (!String.IsNullOrEmpty(agente.Usuario.Contact.Email))
                        {
                            if (!String.IsNullOrEmpty(emails))
                                emails = String.Format("{0};{1}", emails, agente.Usuario.Contact.Email);
                            else
                                emails = agente.Usuario.Contact.Email;
                        }

                        var device = DataBase.Devices.Get(p => p.IdUsuario == agente.IdUsuario).FirstOrDefault();

                        if (device != null && !String.IsNullOrEmpty(device.GCMId))
                        {
                            AndroidNotificationPusher.PushNotification(device.GCMId, titulo, mensaje, null, footer);
                        }
                    }

                    if (!String.IsNullOrEmpty(emails))
                    {
                        DataBase.Notificacions.Send(this.ApplicationId, emails, titulo, String.Format("{0}<br><br>{1}", mensaje, footer), 105);
                    }

                    this.AddDefaultSuccessMessage();
                }
            }
            catch
            {
                this.AddDefaultErrorMessage();
            }

            return Json();
        }
    }
}
