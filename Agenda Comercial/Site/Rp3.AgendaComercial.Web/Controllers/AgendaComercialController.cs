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
using Rp3.Web.Mvc;

namespace Rp3.AgendaComercial.Web.Controllers
{
    public class AgendaComercialController : Rp3.Web.Mvc.Controllers.BaseController<Models.ContextService>
    {       
        public AgenteView Agente
        { 
            get
            {
                AgenteView agente = (AgenteView)Rp3.Web.Mvc.Session.GetValue("AgenteView");

                if(agente == null)
                {
                    return LoadAgente();
                }
                
                return agente;
            }
        }

        public AgenteView LoadAgente() 
        {
            var agente = DataBase.Agentes.GetAgenteView(this.UserId);
            Rp3.Web.Mvc.Session.SetValue("AgenteView", agente);
            return agente;
        }

        public int? UserWorkId
        {
            get
            {
                return Convert.ToInt32(Rp3.Web.Mvc.Session.GetValue("UserWorkId"));
            }

            set
            {
                Rp3.Web.Mvc.Session.SetValue("AgenteWork", null);
                Rp3.Web.Mvc.Session.SetValue("UserWorkId", value);
            }
        }

        public AgenteView AgenteWork
        {
            get
            {
                AgenteView agente = (AgenteView)Rp3.Web.Mvc.Session.GetValue("AgenteWork");

                if (agente == null)
                {
                    agente = DataBase.Agentes.GetAgenteView(UserWorkId ?? 0);
                    Rp3.Web.Mvc.Session.SetValue("AgenteWork", agente);
                }

                return agente;
            }
        }

        public bool TodosMisAgentes
        {
            get
            {
                return Convert.ToBoolean(Rp3.Web.Mvc.Session.GetValue("AllMyAgents"));
            }

            set
            {
                Rp3.Web.Mvc.Session.SetValue("AllMyAgents", value);
            }
        }

        public List<int> UserWorkIds
        {
            get
            {
                var list = Rp3.Web.Mvc.Session.GetValue("UserWorkIds") as List<int>;

                if (list == null)
                    list = new List<int>();

                return list;
            }
            set
            {
                Rp3.Web.Mvc.Session.SetValue("AgentesWork", null);

                Rp3.Web.Mvc.Session.SetValue("UserWorkIds", value);
            }
        }

        public List<AgenteView> AgentesWork
        {
            get
            {
                List<AgenteView> agentes = Rp3.Web.Mvc.Session.GetValue("AgentesWork") as List<AgenteView>;

                if (agentes == null)
                {
                    agentes = new List<AgenteView>();

                    foreach (var id in UserWorkIds)
                    {
                        var agente = DataBase.Agentes.GetAgenteView(id);

                        if (agente != null && agente.IdRuta != null)
                            agentes.Add(agente);
                    }

                    Rp3.Web.Mvc.Session.SetValue("AgentesWork", agentes);
                }

                return agentes;
            }
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            ViewBag.AgenteCargoRol = Agente.CargoRol;

            base.OnActionExecuted(filterContext);
        }
    }
}
