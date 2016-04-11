using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Rp3.Web.Mvc.Application.Security.Models;
using Rp3.Resources;
using Rp3.Data.Models.General;
using Rp3.Data.Models.Security;
using Rp3.Web.Mvc.Controllers;
using Rp3.Data;
using System.Web.Routing;
using Rp3.Data.DbConnection;

namespace Rp3.Web.Mvc.Application.Security.Controllers
{
    public class AccountController : Rp3.Web.Mvc.Controllers.BaseController
    {
        public virtual ActionResult LogIn()
        {
            if (this.IsLogged)
                return LogInSuccess();
            if (Rp3.Configuration.Rp3ConfigurationSection.Current.UseIndexLogin)
                return View("HomeLogin");
            else
                return View("Login");
        }

        [HttpPost]
        public virtual ActionResult LogIn(Login login)
        {
            if (ModelState.IsValid)
            {
                if (Rp3.Security.Authentication.Validity(login.LogonName, login.Password))
                {
                    SessionStart(login.LogonName, this.OrganizationId);
                    return LogInSuccess();
                }
                else
                {
                    this.AddMessage(MessageFor.AuthenticationFail, Data.MessageType.Error, MessageFor.AuthenticationFailTitle);
                }
            }

            if (Rp3.Configuration.Rp3ConfigurationSection.Current.UseIndexLogin)
                return View("HomeLogin");
            else
                return View("Login");
        }

        protected virtual ActionResult LogInSuccess()
        {
            var url = new UrlHelper(this.Request.RequestContext);
            string redirect = url.Content("~" + Rp3.Configuration.Rp3ConfigurationSection.Current.LogInRedirect.Url);
            return Redirect(redirect);
        }


        public virtual ActionResult RequestingPasswordRecovery()
        {
            return View();
        }

        [HttpPost]
        [PreventSpam(60)]
        public virtual ActionResult RequestingPasswordRecovery(ResetPassword resetPassword)
        {
            if (ModelState.IsValid)
            {
                ExecuteRequestPasswordRecovery(resetPassword);
                return HomeRedirect();
            }
            return View(resetPassword);            
        }


        [HttpPost]
        [PreventSpam(60)]
        public virtual ActionResult RequestPasswordRecovery(ResetPassword resetPassword)
        {
            if (ModelState.IsValid)
            {
                ExecuteRequestPasswordRecovery(resetPassword);
            }
            return RedirectToAction("Login", "Account", null,true);
        }

        [PreventSpam(60)]
        public virtual ActionResult PasswordRecovery(string code)
        {
            ExecutePasswordRecovery(code);

            return View();
        }

        [PreventSpam(60)]
        public virtual ActionResult RegisterValidation(string code)
        {
            ExecuteValidationRegister(code);

            return HomeRedirect();
        }
        

        private bool ExecutePasswordRecovery(string code)
        {
            UserBase user;
            Data.Message message = Rp3.Security.User.ResetPassword(code, out user);
            this.AddMessage(message);

            bool success = message.MessageType == Data.MessageType.Success;

            if (success)
                SessionStart(user.LogonName, this.OrganizationId);

            return success;
        }

        private bool ExecuteValidationRegister(string code)
        {
            UserBase user;
            Data.Message message = Rp3.Security.User.ValidationRegister(code, out user);
            this.AddMessage(message);

            bool success = message.MessageType == Data.MessageType.Success;

            if (success)
                SessionStart(user.LogonName, this.OrganizationId);

            return success;
        }

        public static bool ExecuteRequestRegisterValidation<T>(UserBase user, MessageCollection messages, DbContextManagerService<T> service) where T: DbContextManager
        {
            bool success = false;
            try
            {
                UserActivationCode activationCode = new UserActivationCode();
                success = Rp3.Security.User.RequestValidityRegisterEmail(user, activationCode, service);

                if (success)
                {
                    List<MailContact> contacts = new List<MailContact>();
                    contacts.Add(new MailContact()
                    {
                        Mail = user.Contact.Email,
                        Name = user.Contact.Email
                    });

                    new MailController().SendAsEmail(Rp3.Resources.TitleFor.RequestRegisterValidationMailSubject,
                        contacts, "RequestValidationRegister", activationCode).DeliverAsync();

                }
            }
            catch
            {
                messages.Add(new Message(Rp3.Resources.MessageFor.RequestValidationRegisterFail, Data.MessageType.Error));
                success = false;
            }
            return success;
        }

        private bool ExecuteRequestPasswordRecovery(ResetPassword resetPassword)
        {
            ContactBase contact = null;
            UserActivationCode activationCode = new UserActivationCode();
            bool success = false;
            try
            {
                if (!string.IsNullOrEmpty(resetPassword.Email))
                    resetPassword.Email = resetPassword.Email.Trim();

                if (!Rp3.Text.ValidityEmail(resetPassword.Email))
                {
                    this.AddMessage(Rp3.Resources.ErrorMessageValidation.InvalidEmailFormat,
                        Data.MessageType.Error);
                }
                else
                {
                    contact = DataBase.Contacts.Get(p => p.User.Contact.Email == resetPassword.Email , includeProperties: "User").FirstOrDefault();

                    if (contact == null)
                    {
                        this.AddMessage(Rp3.Resources.MessageFor.EmailAccountNotExists,
                            Data.MessageType.Error);
                    }
                    else
                    {
                        success = Rp3.Security.User.RequestPasswordRecovery(contact.User, activationCode);
                        if (success)
                            this.AddMessage(Rp3.Resources.MessageFor.RequestForPasswordRecovery, Data.MessageType.Success);
                        else
                            this.AddMessage(Rp3.Resources.MessageFor.RequestPasswordRecoveryFail, Data.MessageType.Error);
                    }
                }
            }
            catch
            {
                this.AddMessage(Rp3.Resources.MessageFor.RequestPasswordRecoveryFail, Data.MessageType.Error);
            }

            if (success)
            {
                try
                {
                    List<MailContact> contacts = new List<MailContact>();
                    contacts.Add(new MailContact()
                    {
                        Mail = resetPassword.Email,
                        Name = resetPassword.Email
                    });

                    new MailController().SendAsEmail(Rp3.Resources.TitleFor.RequestPasswordRecoveryMailSubject,
                        contacts, "RequestPasswordRecovery", activationCode).Deliver();
                }
                catch
                {
                    this.AddMessage(Rp3.Resources.MessageFor.RequestPasswordRecoveryFail, Data.MessageType.Error);
                    return false;
                }
            }

            return success;
        }

        [LoggedRequired]
        public virtual ActionResult ChangePassword()
        {
            ChangePassword changePassword = new ChangePassword();
            changePassword.LogonName = Rp3.Web.Mvc.Session.LogonName;
            changePassword.Names = Rp3.Web.Mvc.Session.UserFullName;

            return View(changePassword);
        }

        [HttpPost]
        [PreventSpam]
        [LoggedRequired]
        public virtual ActionResult ChangePassword(ChangePassword changePassword)
        {
            if (ModelState.IsValid)
            {
                if (Rp3.Security.Authentication.Validity(this.UserLogonName, changePassword.CurrentPassword))
                {
                    if (Rp3.Security.Authentication.ChangePassword(this.UserLogonName, changePassword.CurrentPassword, changePassword.NewPassword))
                    {
                        return ChangePasswordSuccess();
                    }
                }
                else
                {
                    this.AddMessage(Rp3.Resources.MessageFor.AuthenticationFail, Data.MessageType.Error, MessageFor.ChangePasswordFailTitle);
                }
            }
            return View(changePassword);
        }

        protected virtual ActionResult ChangePasswordSuccess()
        {
            this.AddMessage(MessageFor.PasswordChangeSuccess);
            return HomeRedirect();
        }

        [HttpPost]
        [ChildAction]
        public virtual ActionResult PartialResetPassword(string email)
        {
            string message = Rp3.Resources.Resource.MessageForResetPasswordFail;
            bool success = false;

            success = ExecuteRequestPasswordRecovery(new ResetPassword() { Email = email });

            if (this.MessageCollection.Any())
                message = this.MessageCollection.First().TextMessage;

            return new JsonResult()
            {
                Data = new
                {
                    Message = message,
                    Success = success
                }
            };        
        }

        public ActionResult ExpiredSession()
        {
            if (this.IsLogged)
                return LogInSuccess();
            return View();
        }

        public ActionResult AccessDenied()
        {
            return View();
        }


        [HttpPost]
        [ChildAction]
        public virtual ActionResult PartialRegister(Register contact)
        {
            string message = string.Empty;
            bool success = false;
            ContactBase contactSave = null;

            if (AccountController.ValidateRegister(contact, out message, false, true, true))
            {
                try
                {
                    contactSave = new ContactBase();
                    contactSave.Email = contact.Email;
                    contactSave.LastNames = contact.LastNames;
                    contactSave.Names = contact.Names;
                    contactSave.RegisterOrganizationName = contact.RegisterOrganizationName;
                    contactSave.RegisterDate = Rp3.Web.Mvc.Session.CurrentDateTime;
                    contactSave.PhoneNumber = contact.PhoneNumber;
                    contactSave.User = new UserBase();
                    contactSave.User.Active = true;
                    contactSave.User.LogonName = contact.Email;
                    contactSave.User.DecodePassword = contactSave.User.Password;
                    contactSave.User.Password = Rp3.Security.Authentication.GetEncodePassword(contactSave.User.LogonName, contact.Password);

                    contactSave.GenerateId();
                    contactSave.User.UserId = contactSave.ContactId;

                    RoleBase defaultRole = DataBase.Roles.Get(p => p.IsDefault && p.OrganizationId == this.OrganizationId).FirstOrDefault();
                    if (defaultRole != null)
                    {
                        UserRoleBase userRole = new UserRoleBase()
                        {
                            Role = defaultRole,
                            User = contactSave.User,
                            UserId = contactSave.User.UserId
                        };
                        DataBase.UserRoles.Insert(userRole);
                    }

                    DataBase.Contacts.Insert(contactSave);
                    DataBase.Users.Insert(contactSave.User);

                    DataBase.Save();

                    SessionStart(contactSave.User.LogonName);

                    message = Rp3.Resources.Resource.MessageForRegisterContactSuccess;
                    success = true;
                }
                catch
                {
                    message = Rp3.Resources.Resource.MessageForRegisterContactFail;
                    success = false;
                }
            }

            try
            {
                if (success)
                {
                    List<MailContact> contacts = new List<MailContact>();
                    contacts.Add(new MailContact()
                    {
                        Mail = contact.Email,
                        Name = contact.Email
                    });

                    contacts.Add(new MailContact()
                    {
                        Mail = RegisterMailAccount,
                        Name = RegisterMailAccount,
                        MailtoType = MailtoType.Cco
                    });

                    new MailController().SendAsEmail(Rp3.Resources.Resource.MailSubjectForRegister, contacts, "Register", contactSave).DeliverAsync();                    
                }
            }
            catch
            {
                //Do Nothing
            }

            return new JsonResult()
            {
                Data = new
                {
                    Message = message,
                    Success = success
                }
            };
        }



        public virtual ActionResult PartialChangePassword(string logonNameOrEmail, string password, string newPassword, string confirmPassword)
        {
            string message = Rp3.Resources.Resource.MessageForPasswordChangeFail;
            bool success = false;

            if (newPassword != confirmPassword)
            {
                message = Rp3.Resources.Resource.MessageForPasswordConfirmIncorrect;
                success = false;
            }
            else if (newPassword.Length < 6)
            {
                message = string.Format(Rp3.Resources.Resource.MessageForPasswordLength, 6);
                success = false;
            }
            else if (Validity(logonNameOrEmail, password))
            {
                if (ExecuteChangePassword(logonNameOrEmail, password, newPassword))
                {
                    message = Rp3.Resources.Resource.MessageForPasswordChangeSuccess;
                    success = true;

                    SessionStart(logonNameOrEmail);
                }
            }

            return new JsonResult()
            {
                Data = new
                {
                    Message = message,
                    Success = success
                }
            };
        }

        protected virtual ActionResult LogOutSuccess()
        {
            var url = new UrlHelper(this.Request.RequestContext);
            string redirect = url.Content("~" + Rp3.Configuration.Rp3ConfigurationSection.Current.LogOutRedirect.Url);

            return Redirect(redirect);
        }

        protected virtual ActionResult HomeRedirect()
        {
            SharedActionMessage();
            var url = new UrlHelper(this.Request.RequestContext);
            string redirect = url.Content("~" + Rp3.Configuration.Rp3ConfigurationSection.Current.HomeRedirect.Url);

            return Redirect(redirect);
        }

        public virtual ActionResult LogOut()
        {
            if (Rp3.Web.Mvc.Session.IsLogged)
                Rp3.Web.Mvc.Session.End();
            else
                Rp3.Web.Mvc.Session.RemoveAll();
            
            
            return LogOutSuccess();            
        }

        [HttpPost]
        [ChildAction]
        public virtual JsonResult PartialLogIn(string logonNameOrEmail, string password, int? organizationId = null)
        {
            string message = string.Empty;
            bool success = false;

            if (Rp3.Security.Authentication.Validity(logonNameOrEmail, password))
            {
                SessionStart(logonNameOrEmail, organizationId);

                message = Rp3.Resources.Resource.MessageForAuthenticationSuccess;
                success = true;
            }
            else
            {
                message = Rp3.Resources.Resource.MessageForAuthenticationFail;
                success = false;
            }

            return new JsonResult()
            {
                Data = new
                {
                    Message = message,
                    Success = success
                }
            };
        }

        private bool Validity(string logonNameOrEmail, string password)
        {
            return Rp3.Security.Authentication.Validity(logonNameOrEmail, password);
        }

        private bool ExecuteChangePassword(string logonNameOrEmail, string password, string newPassword)
        {
            return Rp3.Security.Authentication.ChangePassword(logonNameOrEmail, password, newPassword);
        }
                

        #region Contact

        [LoggedRequired]
        public ActionResult EditContact()
        {
            ContactBase contact = DataBase.Contacts.GetByID(this.UserId);
            EditContact editContact = new EditContact();
            editContact.LogonName = this.UserLogonName;
            editContact.RoleName = this.RoleName;
            contact.CopyTo(editContact);
            return View(editContact);
        }

        [LoggedRequired]
        [HttpPost]
        public ActionResult EditContact(EditContact editContact)
        {
            try
            {
                ContactBase contact = DataBase.Contacts.GetByID(this.UserId);
                editContact.CopyTo(contact, includeProperties: new string[] { "Names", "LastNames", "PhoneNumber" });

                DataBase.Contacts.Update(contact);
                DataBase.Save();

                Rp3.Web.Mvc.Session.SetValue(Rp3.Constants.Session.UserFullName, contact.DefaultFullName);

                this.AddMessage(Rp3.Resources.MessageFor.EditContactSelfSuccess, MessageType.Success);

                return HomeRedirect();
            }
            catch
            {
                this.AddDefaultErrorMessage();
            }
            return View(editContact);
        }


        public static bool ValidateRegister(Register contact, out string message, bool requiredComment = false, bool validateUser = false, bool validateContact = true)
        {
            bool success = true;
            message = string.Empty;

            if (validateContact && !string.IsNullOrEmpty(contact.Email))
                contact.Email = contact.Email.Trim();

            if (validateUser && string.IsNullOrEmpty(contact.Password))
                contact.Password = string.Empty;

            if (validateUser && string.IsNullOrEmpty(contact.ConfirmedPassword))
                contact.ConfirmedPassword = string.Empty;

            if (validateContact && string.IsNullOrEmpty(contact.Names))
            {
                message = Rp3.Resources.Resource.MessageForContactNamesRequired;
                success = false;
            }
            else if (validateContact && string.IsNullOrEmpty(contact.LastNames))
            {
                message = Rp3.Resources.Resource.MessageForContactLastNamesRequired;
                success = false;
            }
            else if (validateContact && string.IsNullOrEmpty(contact.Email))
            {
                message = Rp3.Resources.Resource.MessageForEmailRequired;
                success = false;
            }
            else if (validateContact && !Rp3.Text.ValidityEmail(contact.Email))
            {
                message = Rp3.Resources.Resource.MessageForIncorrectEmailFormat;
                success = false;
            }
            else if (validateUser && Rp3.Security.User.ExistEmail(contact.Email))
            {
                message = Rp3.Resources.Resource.MessageForEmailAlreadyExist;
                success = false;
            }
            else if (validateContact && string.IsNullOrEmpty(contact.RegisterOrganizationName))
            {
                message = Rp3.Resources.Resource.MessageForContactOrganizationRequired;
                success = false;
            }
            else if (validateUser && contact.Password.Length < 6)
            {
                message = string.Format(Rp3.Resources.Resource.MessageForPasswordLength, 6);
                success = false;
            }
            else if (validateUser && contact.Password != contact.ConfirmedPassword)
            {
                message = Rp3.Resources.Resource.MessageForPasswordConfirmIncorrect;
                success = false;
            }
            else if (requiredComment && string.IsNullOrWhiteSpace(contact.Comments))
            {
                message = Rp3.Resources.Resource.MessageForCommentContactUsRequired;
                success = false;
            }

            return success;
        }

        [ChildAction]
        [HttpPost]
        public JsonResult PartialContactUs(Register contact)
        {
            string message = string.Empty;
            bool success = false;

            ContactUs contactUs = new ContactUs();

            try
            {
                if (ValidateRegister(contact, out message, true, false, !Rp3.Web.Mvc.Session.IsLogged))
                {
                    Rp3.Data.DbConnection.DbContextManagerService service = new Data.DbConnection.DbContextManagerService();

                    if (Rp3.Web.Mvc.Session.IsLogged)
                    {
                        //contactUs.ContactId = Rp3.Web.Mvc.Session.UserId;
                        contactUs.Contact = service.Contacts.GetByID(Rp3.Web.Mvc.Session.UserId);
                    }
                    else
                    {
                        ContactBase contactSave = new ContactBase();
                        contactSave.Email = contact.Email;
                        contactSave.LastNames = contact.LastNames;
                        contactSave.Names = contact.Names;
                        contactSave.RegisterDate = Rp3.Web.Mvc.Session.CurrentDateTime;
                        contactSave.RegisterOrganizationName = contact.RegisterOrganizationName;
                        contactSave.PhoneNumber = contact.PhoneNumber;

                        service.Contacts.Insert(contactSave);
                        contactUs.Contact = contactSave;
                    }

                    contactUs.Comments = contact.Comments;
                    contactUs.RecordDate = Rp3.Web.Mvc.Session.CurrentDateTime;

                    service.ContactUs.Insert(contactUs);

                    service.Save();

                    success = true;
                    message = Rp3.Resources.Resource.MessageForContactUsRegisterSuccess;
                }
            }
            catch
            {
                success = false;
                message = Rp3.Resources.Resource.MessageForContactUsRegisterFail;
            }


            try
            {
                if (success)
                {
                    List<MailContact> contacts = new List<MailContact>();
                    contacts.Add(new MailContact()
                    {
                        Mail = ContactUsMailAccount,
                        Name = ContactUsMailAccount
                    });

                    new MailController().SendAsEmail(Rp3.Resources.Resource.MailSubjectForContactUsNotification, contacts, "ContactUs", contactUs).DeliverAsync();
                }
            }
            catch
            {
                //Do Nothing
            }

            return new JsonResult()
            {
                Data = new { Success = success, Message = message }
            };
        }             

        protected string ContactUsMailAccount
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["ContactUsMailAccount"];
            }
        }  

        #endregion

        protected string RegisterMailAccount
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["RegisterMailAccount"];
            }
        }
    }
}