using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Rp3.Data.Models.Security;
using Rp3.Web.Mvc.Application.Security.Models;

namespace Rp3.Web.Mvc.Application.Security.Controllers
{
    public class UserController : Rp3.Web.Mvc.Controllers.BaseController
    {
        [Authorize("USER", "QUERY", "SECURITY")]
        public ActionResult Index()
        {            
            return View(GetListIndex());
        }

         
        public ActionResult GridViewIndex()
        {
            return PartialView("_GridViewIndex", GetListIndex());
        }

        private List<Data.Models.Security.UserRoleBase> GetListIndex()
        {
            List<Data.Models.Security.UserRoleBase> users = DataBase.UserRoles.Get(p =>
                p.Role.OrganizationId == this.OrganizationId && !p.User.IsPrivate).ToList();

            return users;
        }


        [ChildAction]
        [Authorize("USER", "NEW", "SECURITY")]
        [HttpPost]
        [PreventSpam(Order = 0)]
        public ActionResult SyncActiveDirectory()
        {
            int count = 0;
            try
            {
                List<UserBase> userList = Rp3.Security.User.GetLDAPUsers();

                List<UserBase> existUsers = DataBase.Users.Get(p => p.UserRole.Any(r => r.Role.OrganizationId == this.OrganizationId), 
                    includeProperties:"Contact").ToList();

                RoleBase roleBase = DataBase.Roles.Get(p => p.OrganizationId == this.OrganizationId
                    && p.IsDefault).FirstOrDefault();

                var insertUsers = userList.Where(p=> !existUsers.Select(e=>e.LogonName).Contains(p.LogonName) ).ToList();
                
                foreach (var insert in insertUsers)
                {                
                    if(!existUsers.Any(p=>p.Contact.Email == insert.Contact.Email))
                    {
                        insert.Active = true;
                        insert.Contact.GenerateId();
                        insert.UserId = insert.Contact.ContactId;
                        insert.ActiveDirectoryEnabled = true;

                        UserRoleBase userRole = new UserRoleBase();
                        userRole.RoleId = roleBase.RoleId;
                        userRole.UserId = insert.UserId;

                        insert.UserRole = new List<UserRoleBase>();
                        insert.UserRole.Add(userRole);

                        DataBase.Users.Insert(insert);
                        DataBase.UserRoles.Insert(userRole);

                        DataBase.Save();
                        count++;
                    }
                }                
         
                this.AddMessage(string.Format(Rp3.Resources.MessageFor.SyncActiveDirectoryUsersSuccess,count), Data.MessageType.Success);
            }
            catch{
                if(count>0)
                    this.AddMessage(string.Format(Rp3.Resources.MessageFor.SyncActiveDirectoryUsersIncomplete, count), Data.MessageType.Error);                
                else
                    this.AddDefaultErrorMessage();
            }
            return Json();
        }

        [Authorize("USER", "NEW", "SECURITY")]
        public ActionResult Create()
        {
            CreateUser user = new CreateUser();

            if (Rp3.Configuration.Rp3ConfigurationSection.Current.Authentication.AuthenticationType == Configuration.AuthenticationElement.AuthenticationTypes.ActiveDirectory)
                user.ActiveDirectoryEnabled = true;

            ViewBag.RoleSelectList = DataBase.Roles.Get(p => p.OrganizationId == this.OrganizationId
                && p.Active && !p.IsPrivate).ToSelectList();

            return View(user);
        }

        [HttpPost]
        [PreventSpam(Order = 0)]
        [Authorize("USER", "NEW", "SECURITY", Order = 1)]        
        public ActionResult Create(CreateUser user)
        {
            if (ModelState.IsValid)
            {
                UserBase userInsert = new UserBase();
                userInsert.LogonName = user.LogonName;                
                userInsert.Password = Rp3.Security.Authentication.GetEncodePassword(user.LogonName, user.Password);
                userInsert.Contact = new Data.Models.General.ContactBase();
                userInsert.Contact.LastNames = user.LastNames;
                userInsert.Contact.Names = user.Names;
                userInsert.Contact.Email = user.Email;
                userInsert.Active = true;

                if (Rp3.Configuration.Rp3ConfigurationSection.Current.Authentication.AuthenticationType == Configuration.AuthenticationElement.AuthenticationTypes.ActiveDirectory)
                    userInsert.ActiveDirectoryEnabled = user.ActiveDirectoryEnabled;
                
                userInsert.Contact.GenerateId();
                userInsert.UserId = userInsert.Contact.ContactId;

                RoleBase role = DataBase.Roles.GetByID(user.RoleId);
                if (role.OrganizationId == this.OrganizationId)
                {
                    UserRoleBase userRole = new UserRoleBase();
                    userRole.RoleId = user.RoleId;
                    userRole.UserId = userInsert.UserId;

                    userInsert.UserRole = new List<UserRoleBase>();
                    userInsert.UserRole.Add(userRole);

                    DataBase.UserRoles.Insert(userRole);
                }

                DataBase.Users.Insert(userInsert);

                if (!Rp3.Security.User.ExistEmail(userInsert.Contact.Email))
                {
                    DataBase.Save();
                }
                return RedirectToAction("Index");
            }

            ViewBag.RoleSelectList = DataBase.Roles.Get(p => p.OrganizationId == this.OrganizationId
                && p.Active && !p.IsPrivate).ToSelectList();

            return View(user);
        }

        [Authorize("USER", "NEW", "SECURITY")]
        public JsonResult EmailExists(string email)
        {
            return Json(!Rp3.Security.User.ExistEmail(email), JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult LogonNameAvalaible(string logonName)
        {
            return new JsonResult()
            {
                Data = !Rp3.Security.User.ExistAccount(logonName),
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        

        public JsonResult AccountAvalaible(string email)
        {
            return new JsonResult() {
                Data = !Rp3.Security.User.ExistEmail(email),
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        [Authorize("USER", "EDIT", "SECURITY")]
        public ActionResult Edit(int id)
        {
            UserBase user = DataBase.Users.Get(p => p.UserId == id && p.UserRole.Any(q => q.Role.OrganizationId == this.OrganizationId),
                includeProperties: "Contact,UserRole").SingleOrDefault();

            if (user.IsPrivate)
            {
                return RedirectToAction("Index");
            }
            else
            {
                EditUser editUser = new Rp3.Web.Mvc.Application.Security.Models.EditUser();
                editUser.UserId = user.UserId;
                editUser.Email = user.Contact.Email;
                editUser.LogonName = user.LogonName;
                editUser.LastNames = user.Contact.LastNames;
                editUser.Names = user.Contact.Names;
                editUser.Active = user.Active;
                editUser.RoleId = user.UserRole.First(p => p.Role.OrganizationId == this.OrganizationId).RoleId;
                editUser.ActiveDirectoryEnabled = user.ActiveDirectoryEnabled;

                ViewBag.RoleSelectList = DataBase.Roles.Get(p => p.OrganizationId == this.OrganizationId
                    && p.Active && !p.IsPrivate).ToSelectList();

                return View(editUser);
            }            
        }

        [Authorize("USER", "EDIT", "SECURITY")]
        [HttpPost]
        public ActionResult Edit(EditUser user)
        {
            if (DataBase.Users.Exists(p => p.Contact.Email == user.Email && p.UserId != user.UserId))
            {
                this.AddMessage(Rp3.Resources.ErrorMessageValidation.EmailAccountAlreadyExists);                
            }
            else if (ModelState.IsValid)
            {
                UserBase userUpdate = DataBase.Users.Get(p => p.UserId == user.UserId && p.UserRole.Any(q => q.Role.OrganizationId == this.OrganizationId),
                    includeProperties: "Contact,UserRole.Role").SingleOrDefault();

                if (userUpdate.IsPrivate)
                    return RedirectToAction("Index");                

                userUpdate.Contact.Email = user.Email;
                userUpdate.Contact.LastNames = user.LastNames;
                userUpdate.Contact.Names = user.Names;
                userUpdate.Active = user.Active;

                if (userUpdate.ActiveDirectoryEnabled || Rp3.Configuration.Rp3ConfigurationSection.Current.Authentication.AuthenticationType == Configuration.AuthenticationElement.AuthenticationTypes.ActiveDirectory)
                    userUpdate.ActiveDirectoryEnabled = user.ActiveDirectoryEnabled;

                UserRoleBase userRoleOrig = userUpdate.UserRole.FirstOrDefault(p => p.Role.OrganizationId == this.OrganizationId);

                if (userRoleOrig.RoleId != user.RoleId)
                {
                    RoleBase role = DataBase.Roles.GetByID(user.RoleId);
                    if (role.OrganizationId == this.OrganizationId)
                    {

                        DataBase.UserRoles.Delete(userRoleOrig);
                        DataBase.UserRoles.Insert(new UserRoleBase()
                        {
                            UserId = userUpdate.UserId,
                            RoleId = user.RoleId
                        });
                    }
                }

                DataBase.Users.Update(userUpdate);
                DataBase.Contacts.Update(userUpdate.Contact);

                DataBase.Save();

                return RedirectToAction("Index");
            }

            ViewBag.RoleSelectList = DataBase.Roles.Get(p => p.OrganizationId == this.OrganizationId
                && p.Active && !p.IsPrivate).ToSelectList();

            return View(user);
        }

        [Authorize("USER", "DETAIL", "SECURITY")]
        public ActionResult Detail(int id)
        {
            return View(GetDetailUser(id));
        }

        [Authorize("USER", "NEW", "SECURITY")]
        public ActionResult Delete(int id)
        {
            return View(GetDetailUser(id));
        }

        private DetailUser GetDetailUser(int id)
        {
            UserBase user = DataBase.Users.Get(p => p.UserId == id && p.UserRole.Any(q => q.Role.OrganizationId == this.OrganizationId),
                includeProperties: "Contact,UserRole").SingleOrDefault();

            RoleBase role = user.UserRole.First(p => p.Role.OrganizationId == this.OrganizationId).Role;

            DetailUser detailUser = new Security.Models.DetailUser();
            detailUser.UserId = user.UserId;
            detailUser.LogonName = user.LogonName;
            detailUser.Email = user.Contact.Email;
            detailUser.LastNames = user.Contact.LastNames;
            detailUser.Names = user.Contact.Names;
            detailUser.RoleId = role.RoleId;
            detailUser.RoleName = role.Name;
            detailUser.Active = user.Active;

            return detailUser;
        }

        [PreventSpam(Order = 0)]
        [Authorize("USER", "DELETE", "SECURITY", Order = 1)]
        [HttpPost]
        public ActionResult Delete(DetailUser user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    UserBase userDelete = DataBase.Users.Get(p => p.UserId == user.UserId, includeProperties: "UserRole").SingleOrDefault();
                    if (userDelete.IsPrivate)
                        return RedirectToAction("Index");

                    DataBase.Users.Delete(userDelete);

                    List<UserRoleBase> useRole = userDelete.UserRole.ToList();
                    foreach (var roleBase in useRole)
                        DataBase.UserRoles.Delete(roleBase);

                    DataBase.Save();

                    return RedirectToAction("Index");
                }
            }catch
            {                
                this.AddDefaultErrorMessage();
            }

            return View(user);
        }

    }
}
