using Rp3.AgendaComercial.Models.General;
using Rp3.AgendaComercial.Models.Ruta;
using Rp3.Web.Mvc.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Rp3.AgendaComercial.Web.Ruta.Controllers
{
    public class NotificacionAgenteController : Web.Controllers.AgendaComercialController
    {
        // GET: Ruta/NotificacionAgente
         [Rp3.Web.Mvc.Authorize("NOTIFIAGENTE", "QUERY", "AGENDACOMERCIAL")]
        public ActionResult Index()
        {
            InicializarEdit();

            var model = new NotificacionAgente();
            model.Agentes = GetAgenteListDetail(((List<Agente>)ViewBag.Agentes).Select(p => p.IdAgente.ToString()).ToArray<string>());

            return View(model);
        }

        [Rp3.Web.Mvc.Authorize("NOTIFIAGENTE", "QUERY", "AGENDACOMERCIAL")]
        [HttpPost]
        public ActionResult Index(NotificacionAgente model, string[] agentes)
        {
            InicializarEdit();

            model.Agentes = GetAgenteListDetail(agentes);

            try
            {
                var usuario = DataBase.Agentes.Get(p => p.IdUsuario == this.UserId).FirstOrDefault();
                string emails = String.Empty;

                string footer = String.Format("Enviada por: {0} - el {1:g}", usuario.Descripcion, this.GetCurrentDateTime());

                if (!String.IsNullOrEmpty(model.Titulo) && !String.IsNullOrEmpty(model.Mensaje) && usuario != null)
                {
                    foreach (var item in model.Agentes)
                    {
                        try
                        {
                            var agente = DataBase.Agentes.Get(p => p.IdAgente == item.IdAgente).FirstOrDefault();

                            if (agente != null && agente.IdUsuario != null)
                            {
                                if(!String.IsNullOrEmpty(agente.Usuario.Contact.Email))
                                {
                                    if (!String.IsNullOrEmpty(emails))
                                        emails = String.Format("{0};{1}", emails, agente.Usuario.Contact.Email);
                                    else
                                        emails = agente.Usuario.Contact.Email;
                                }

                                var device = DataBase.Devices.Get(p => p.IdUsuario == agente.IdUsuario).FirstOrDefault();

                                if (device != null && !String.IsNullOrEmpty(device.GCMId))
                                {
                                    AndroidNotificationPusher.PushNotification(device.GCMId, model.Titulo, model.Mensaje, null, footer);
                                }
                            }
                        }
                        catch
                        {
                        }
                    }

                    if (!String.IsNullOrEmpty(emails))
                    {
                        DataBase.Notificacions.Send(this.ApplicationId, emails, model.Titulo, String.Format("{0}<br><br>{1}", model.Mensaje, footer), 105);
                    }

                    this.AddDefaultSuccessMessage();

                    model.Titulo = String.Empty;
                    model.Mensaje = String.Empty;

                    return View(model);
                }
                else {
                    this.AddErrorMessage(Rp3.AgendaComercial.Resources.ErrorMessageValidation.DatosIncompletos);
                }
            }
            catch
            {
                this.AddDefaultErrorMessage();
            }

            return View(model);
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

            ViewBag.Agentes = agentes;
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
    }
}