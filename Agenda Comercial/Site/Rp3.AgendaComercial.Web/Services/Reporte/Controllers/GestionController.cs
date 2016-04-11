using Rp3.Web.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Rp3.AgendaComercial.Web.Services.General.Controllers
{
    public class GestionController : Rp3.Web.Http.BaseApiController<AgendaComercial.Models.ContextService>
    {
        [ApiAuthorization]
        public IHttpActionResult GetResumenGestionAgentes(int idAgenteSupervisor, long? inicio = null, long? fin = null) 
        {
            DateTime? fechaInicio = null;
            DateTime fechaActual = GetCurrentDateTime();

            if(inicio!=null)
            {
                fechaInicio = new DateTime(inicio.Value);
            }
            else
            {                
                fechaInicio = fechaActual.AddMonths(-2).AddDays(-fechaActual.Day + 1).Date;            
            }

            DateTime? fechaFin = null;
            if (fin != null)
            {
                fechaFin = new DateTime(fin.Value);
            }
            else
            {
                fechaFin = fechaActual.Date.AddDays(1).AddSeconds(-1);
            }

            var data = DataBase.Agentes.GetResumenGestionAgentes(idAgenteSupervisor, fechaInicio.Value, fechaFin.Value);            
            
            return Ok(data);
        }

        [ApiAuthorization]
        public IHttpActionResult GetResumenUbicacionAgentes(int idAgenteSupervisor, long? ultActualizacion = null)
        {
            DateTime? ultAc = null;

            if (ultActualizacion != null)
            {
                ultAc = new DateTime(ultActualizacion.Value);
            }

            var data = DataBase.Agentes.GetResumenUbicacionAgentes(idAgenteSupervisor, ultAc);

            return Ok(data);
        }
    }
}