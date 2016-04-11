using Rp3.Web.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Rp3.AgendaComercial.Web.Services.Oportunidad.Controllers
{
    public class EtapaController : AgendaComercial.Web.Services.Controllers.ApiAgendaComercialController
    {
        [ApiAuthorization]
        public IHttpActionResult GetEtapas(long? ultimaActualizacion = null)
        {
            DateTime? fechaUltimaActualizacion = null;

            if (ultimaActualizacion != null)
            {
                fechaUltimaActualizacion = new DateTime(ultimaActualizacion.Value);
            }
            var data = DataBase.Etapas.Get(p => p.Estado == Rp3.AgendaComercial.Models.Constantes.Estado.Activo);
            if (fechaUltimaActualizacion.HasValue)
                data = data.Where(p => (p.FecIng >= fechaUltimaActualizacion.Value || p.FecMod >= fechaUltimaActualizacion.Value));
            List<Models.Etapa> etapas = new List<Models.Etapa>();

            Assign(data.ToList(), etapas);

            return Ok(etapas);
        }

        [ApiAuthorization]
        public IHttpActionResult GetTipos(long? ultimaActualizacion = null)
        {
            DateTime? fechaUltimaActualizacion = null;

            if (ultimaActualizacion != null)
            {
                fechaUltimaActualizacion = new DateTime(ultimaActualizacion.Value);
            }
            var data = DataBase.OportunidadTipos.Get(p => p.Estado == Rp3.AgendaComercial.Models.Constantes.Estado.Activo);
            if (fechaUltimaActualizacion.HasValue)
                data = data.Where(p => (p.FecIng >= fechaUltimaActualizacion.Value || p.FecMod >= fechaUltimaActualizacion.Value));
            List<Models.OportunidadTipo> etapas = new List<Models.OportunidadTipo>();

            Assign(data.ToList(), etapas);

            return Ok(etapas);
        }
    }
}