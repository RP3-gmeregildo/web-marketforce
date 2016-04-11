using Rp3.AgendaComercial.Models;
using Rp3.Web.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Rp3.AgendaComercial.Web.Services.General.Controllers
{
    public class TipoClienteController : BaseApiController<ContextService>
    {
        [ApiAuthorization]        
        public IHttpActionResult Get()
        {
            var models = DataBase.TipoClientes.Get().ToList();
            List<Models.TipoCliente> data = new List<Models.TipoCliente>();

            Assign(models, data);

            return Ok(data);
        }
    }
}
