using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Rp3.Web.Mvc.Application.Areas.Base.Controllers
{
    public class ErrorController : Rp3.Web.Mvc.Controllers.BaseController
    {
        //
        // GET: /Base/Error/

        public ActionResult Index()
        {
            return View("Error");
        }

    }
}
