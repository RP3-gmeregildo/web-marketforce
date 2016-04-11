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
    public class CanalController : BaseApiController<ContextService>
    {
        [ApiAuthorization]
        public IHttpActionResult Get()
        {
            var models = DataBase.Canales.Get().ToList();
            List<Models.Canal> data = new List<Models.Canal>();

            Assign(models, data);

            return Ok(data);
        }
    }
}
