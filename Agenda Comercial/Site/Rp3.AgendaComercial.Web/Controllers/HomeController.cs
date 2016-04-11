using Rp3.AgendaComercial.Models;
using Rp3.AgendaComercial.Models.General;
using Rp3.AgendaComercial.Models.General.View;
using Rp3.AgendaComercial.Web.Services.General.Controllers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace Rp3.AgendaComercial.Web.Controllers
{
    public class HomeController : Rp3.AgendaComercial.Web.Controllers.AgendaComercialController
    {
        public ActionResult Index()
        {
            if (this.IsLogged)
            {
                if (Agente != null && Agente.CargoRol == null)
                {
                    if (!this.Agente.EsAgente)
                        this.LoadAgente();

                    if (this.Agente.EsAdministrador)
                    {
                        Agente.CargoRol = AgenteCargoRol.Gerente;
                    }
                    else if (this.Agente.EsSupervisor)
                    {
                        Agente.CargoRol = AgenteCargoRol.Supervisor;
                    }
                    else if (this.Agente.EsAgente)
                    {
                        Agente.CargoRol = AgenteCargoRol.Agente;
                    }
                }

                if (Agente != null && Agente.CargoRol != null)
                {
                    return RedirectToAction("Index", "HomeDashboard", new { Area = "Consulta" }); 
                }
                else
                    return View("Inicio");
            }
            else
                return View("Empezar");
        }

        public ActionResult SeleccionRol()
        {
            List<string> list = new List<string>();

            if (Agente != null) //&& Agente.CargoRol == null
            {
                if (this.Agente.EsSupervisor || this.Agente.EsAdministrador)
                {
                    list.Add(Convert.ToString((int)AgenteCargoRol.Agente));
                    list.Add(Convert.ToString((int)AgenteCargoRol.Supervisor));

                    if (Agente.EsAdministrador)
                    {
                        list.Add(Convert.ToString((int)AgenteCargoRol.Gerente));
                        Agente.CargoRol = AgenteCargoRol.Gerente;
                    }
                    else
                        Agente.CargoRol = AgenteCargoRol.Supervisor;

                    string[] opt = list.ToArray();
                    ViewBag.opt = opt;

                    return View();
                }
                else if (this.Agente.EsAgente)
                {
                    this.Agente.CargoRol = AgenteCargoRol.Agente;
                }
            }

            return RedirectToAction("Index");
            //return View();
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult SeleccionRol(int rol)
        {
            this.Agente.CargoRol = (AgenteCargoRol)rol;

            return RedirectToAction("Index");
        }

        public ActionResult NoAgente()
        {
            if (this.Agente.EsAgente)
            {
                return RedirectToAction("Index");                
            }

            return View();
        }
    }
}
