using Rp3.AgendaComercial.Models.Consulta.View;
using Rp3.Web.Mvc.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rp3.AgendaComercial.Web.Areas.Consulta.Models
{
    public class DashboardAgenteCategoriaClienteModel
    {
        public TabCollection Tabs { get; set; }

        public List<DashboardAgenteCategoriaCliente> Gestionados { get; set; }
        public List<DashboardAgenteCategoriaCliente> NoGestionados { get; set; }
        public List<DashboardAgenteCategoriaCliente> Proximos { get; set; }
    }
}