using Rp3.Data.Models.Security;
using Rp3.Web.Mvc.Application.Security.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Rp3.Web.Mvc.Application.Security.Controllers
{
    public class RoleController : Rp3.Web.Mvc.Controllers.BaseController
    {
        private void SetApplicationSelectList()
        {
            ViewBag.ApplicationSelectList = DataBase.Applications.Get(p => p.Active).ToSelectList(p => p.ApplicationId == this.ApplicationId);
        }

        [Authorize("ROLE", "QUERY", "SECURITY")]
        public ActionResult Index()
        {            
            return View(GetListIndex());
        }

        [Authorize("ROLE", "QUERY", "SECURITY")]
        public ActionResult GridViewIndex()
        {
            return PartialView("_GridViewIndex",GetListIndex());
        }

        private List<Data.Models.Security.RoleBase> GetListIndex()
        {
            List<Data.Models.Security.RoleBase> roles = DataBase.Roles.Get(p =>
                p.OrganizationId == this.OrganizationId && !p.IsPrivate).ToList();

            return roles;
        }

        [Authorize("ROLE", "NEW", "SECURITY")]
        public ActionResult Create()
        {
            SetApplicationSelectList();

            EditRole role = new EditRole();
            role.Active = true;
            role.RoleApplicationOptionActivities = new List<Data.Models.Security.RoleApplicationOptionActivity>();
            ViewBag.ApplicationOptionActivities = DataBase.ApplicationOptionActivities.Get(p=>p.AllowAssignPermission,includeProperties: "ApplicationOption,Activity").ToList();

            return View(role);
        }

        [Authorize("ROLE", "EDIT", "SECURITY")]
        public ActionResult Edit(int id)
        {           
            RoleBase roleBase = DataBase.Roles.Get(p => p.OrganizationId == this.OrganizationId && p.RoleId == id,
                includeProperties: "RoleApplicationOptionActivities").SingleOrDefault();            

            if (roleBase.IsPrivate)
                return RedirectToAction("Index");
            
            SetApplicationSelectList();

            EditRole role = new EditRole();
            this.CopyTo(roleBase, role);
            role.RoleApplicationOptionActivities = roleBase.RoleApplicationOptionActivities;

            ViewBag.ApplicationOptionActivities = DataBase.ApplicationOptionActivities.Get(p=>p.AllowAssignPermission,includeProperties: "ApplicationOption,Activity").ToList();

            return View(role);
        }

        [Authorize("ROLE", "DETAIL", "SECURITY")]
        public ActionResult Detail(int id)
        {
            RoleBase roleBase = DataBase.Roles.Get(p => p.OrganizationId == this.OrganizationId && p.RoleId == id,
                includeProperties: "RoleApplicationOptionActivities.ApplicationOptionActivity.ApplicationOption").SingleOrDefault();

            if (roleBase.IsPrivate)
                return RedirectToAction("Index");

            SetApplicationSelectList();

            EditRole role = new EditRole();
            role.ReadOnly = true;
            this.CopyTo(roleBase, role);
            role.RoleApplicationOptionActivities = roleBase.RoleApplicationOptionActivities;

            ViewBag.ApplicationOptionActivities = role.RoleApplicationOptionActivities.Select(p => p.ApplicationOptionActivity).Distinct().ToList();

            return View(role);
        }

        [Authorize("ROLE", "DELETE", "SECURITY")]
        public ActionResult Delete(int id)
        {            
            RoleBase roleBase = DataBase.Roles.Get(p => p.OrganizationId == this.OrganizationId && p.RoleId == id,
                includeProperties: "RoleApplicationOptionActivities.ApplicationOptionActivity.ApplicationOption").SingleOrDefault();
            
            if (roleBase.IsPrivate)
                return RedirectToAction("Index");

            SetApplicationSelectList();

            EditRole role = new EditRole();
            role.ReadOnly = true;
            this.CopyTo(roleBase, role);
            role.RoleApplicationOptionActivities = roleBase.RoleApplicationOptionActivities;

            ViewBag.ApplicationOptionActivities = role.RoleApplicationOptionActivities.Select(p => p.ApplicationOptionActivity).Distinct().ToList();

            return View(role);
        }

        [HttpPost]
        [PreventSpam(Order = 0)]
        [Authorize("ROLE", "NEW", "SECURITY", Order = 1)]
        public ActionResult Create(EditRole role, string[] applicationOptionActivities)
        {
            try
            {
                RoleBase roleBase = new RoleBase();                

                if (ModelState.IsValid)
                {
                    roleBase.OrganizationId = this.OrganizationId;
                    roleBase.Name = role.Name;
                    roleBase.Active = role.Active;
                    roleBase.RoleApplicationOptionActivities = new List<RoleApplicationOptionActivity>();

                    DataBase.Roles.Insert(roleBase);

                    if (applicationOptionActivities != null)
                    {
                        foreach (var insert in applicationOptionActivities.Where(p=>p!="false"))
                        {
                            string[] keyParts = insert.Split('-');

                            RoleApplicationOptionActivity roleApplicationOptionActivity = new RoleApplicationOptionActivity()
                            {
                                ApplicationId = keyParts[0],
                                OptionId = keyParts[1],
                                ActivityId = keyParts[2]
                            };
                            roleBase.RoleApplicationOptionActivities.Add(roleApplicationOptionActivity);
                            DataBase.RoleApplicationOptionActivities.Insert(roleApplicationOptionActivity);
                        }
                    }

                    DataBase.Save();

                    return RedirectToAction("Index");
                }
            }catch
            {
                this.AddDefaultErrorMessage();
            }

            return RedirectToAction("Index","Role",null,true);
        }

        [HttpPost]
        [PreventSpam(Order = 0)]
        [Authorize("ROLE", "EDIT", "SECURITY", Order = 1)]
        public ActionResult Edit(EditRole role, string[] applicationOptionActivities)
        {
            try
            {                
                RoleBase roleBase = DataBase.Roles.Get(p => p.OrganizationId == this.OrganizationId && p.RoleId == role.RoleId,
                    includeProperties: "RoleApplicationOptionActivities").SingleOrDefault();

                if (roleBase.IsPrivate)
                    return RedirectToAction("Index");

                if (ModelState.IsValid)
                {
                    roleBase.OrganizationId = this.OrganizationId;
                    roleBase.Name = role.Name;
                    roleBase.Active = role.Active;                    

                    DataBase.Roles.Update(roleBase);

                    if(applicationOptionActivities==null) applicationOptionActivities = new string[0];                    

                    var deleteList = roleBase.RoleApplicationOptionActivities.Where(p => 
                        p.ApplicationOptionActivity.AllowAssignPermission &&
                        !applicationOptionActivities.Where(q => q != "false").Contains(p.ApplicationOptionActivity.Key)).ToList();

                    var insertList = applicationOptionActivities.Where(p => p != "false" &&
                        !roleBase.RoleApplicationOptionActivities.Any(q => q.ApplicationOptionActivity.AllowAssignPermission
                        && q.ApplicationOptionActivity.Key == p)).ToList();

                    foreach (var delete in deleteList)
                    {
                        DataBase.RoleApplicationOptionActivities.Delete(delete);
                    }

                    foreach (var insert in insertList.Where(p => p != "false"))
                    {
                        string[] keyParts = insert.Split('-');

                        RoleApplicationOptionActivity roleApplicationOptionActivity = new RoleApplicationOptionActivity()
                        {
                            ApplicationId = keyParts[0],
                            OptionId = keyParts[1],
                            ActivityId = keyParts[2],
                            RoleId = roleBase.RoleId
                        };
                        roleBase.RoleApplicationOptionActivities.Add(roleApplicationOptionActivity);
                        DataBase.RoleApplicationOptionActivities.Insert(roleApplicationOptionActivity);
                    }                    

                    DataBase.Save();

                    return RedirectToAction("Index");
                }
            }
            catch
            {
                this.AddDefaultErrorMessage();
            }

            return RedirectToAction("Index", "Role", null, true);
        }

        [HttpPost]
        [PreventSpam(Order = 0)]
        [Authorize("ROLE", "DELETE", "SECURITY")]
        public ActionResult Delete(EditRole role)
        {
            try
            {
                RoleBase roleBase = DataBase.Roles.Get(p => p.OrganizationId == this.OrganizationId && p.RoleId == role.RoleId,
                    includeProperties: "RoleApplicationOptionActivities").SingleOrDefault();

                if (roleBase.IsPrivate)
                    return RedirectToAction("Index");

                DataBase.Roles.Delete(roleBase);
                DataBase.Save();
                
                return RedirectToAction("Index");
            }
            catch
            {                
                this.AddDefaultErrorMessage();
            }
            return RedirectToAction("Index",null,null,true);
        }

    }
}
