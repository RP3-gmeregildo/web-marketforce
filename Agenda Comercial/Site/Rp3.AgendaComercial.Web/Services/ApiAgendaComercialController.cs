using Rp3.Web.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Rp3.AgendaComercial.Models.General.View;

namespace Rp3.AgendaComercial.Web.Services.Controllers
{
    public class ApiAgendaComercialController : Rp3.Web.Http.BaseApiController<AgendaComercial.Models.ContextService>
    {
        public AgenteView Agente
        {
            get
            {
                AgenteView agente = (AgenteView)Rp3.Web.Mvc.Session.GetValue("AgenteView");
                if (agente == null)
                {
                    agente = DataBase.Agentes.GetAgenteView(this.CurrentUser.UserId);
                    Rp3.Web.Mvc.Session.SetValue("AgenteView", agente);
                }
                return agente;
            }
        }      
    }
}