using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Rp3.Web.Mvc.Application.Base.Controllers
{
    public class HomeController : Rp3.Web.Mvc.Controllers.BaseController
    {
        //
        // GET: /Base/Home/

        public ActionResult Index()        
        {
            if (!this.IsLogged)
                return RedirectToAction("Login", "Account", new { area = "Security" });
            
            return View();
        }

    }
}
