using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Data.SqlClient;
using Rp3.Data.Models.Security;
using Rp3.Data.Models.General;
using Rp3.Web.Mvc.Application.Security.Models;
using Rp3.Data.DbConnection;
using Rp3.Web.Mvc.Controllers;
using Rp3.Data.Models.Definition;

namespace Rp3.Web.Mvc.Application.Security.Controllers
{
    public class SecurityController : Rp3.Web.Mvc.Controllers.BaseController
    {
        #region Permission

        public ActionResult ApplicationOptionActivitiesAssigned()
        {
            IEnumerable<ApplicationOptionActivity> values = null;
            if (this.IsLogged)
            {                
                values = DataBase.RoleApplicationOptionActivities.Get(p => p.RoleId == this.RoleId && p.ApplicationOptionActivity.ApplicationOption.Active, includeProperties: "ApplicationOptionActivity.ApplicationOption.ParentOption").
                Select(p => p.ApplicationOptionActivity);
            }
            else
            {
                values = new List<ApplicationOptionActivity>();
            }

            return PartialView("_SideBar", values);
        }

        #endregion             

    }
}
