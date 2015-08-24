﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using CommonJobs.Infrastructure.Mvc;
using CommonJobs.Domain;

namespace CommonJobs.Mvc.UI.Controllers
{
    public class AccountController : CommonJobsController
    {
        //
        // GET: /Account/LogOn

        public ActionResult LogOn(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/LogOn

        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = RavenSession.Load<User>("Users/" + model.UserName);

                if (user != null && user.ValidatePassword(model.Password))
                {                    
                    FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                    1,
                    model.UserName,
                    DateTime.Now,
                    DateTime.Now.AddDays(2),
                    model.RememberMe,
                    "",
                    FormsAuthentication.FormsCookiePath);

                    string encTicket = FormsAuthentication.Encrypt(ticket);
                    var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket)
                    {
                        // setting the Expires property to the same value in the future
                        // as the forms authentication ticket validity
                        Expires = ticket.Expiration
                    };
                    Response.Cookies.Add(cookie);

                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "El nombre de usuario o contraseña no son correctos.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/LogOff

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ChangePassword

        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Account/ChangePassword

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                bool changePasswordSucceeded = false;
                try
                {
                    var currentUserName = User.Identity.Name;
                    var user = RavenSession.Load<User>("Users/" + currentUserName);

                    if (user != null && user.ValidatePassword(model.OldPassword))
                    {
                        user.SetPassword(model.NewPassword);
                        changePasswordSucceeded = true;
                    }
                }
                catch (Exception)
                {
                    //TODO: do something?
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "la contraseña actual no es correcta o la nueva contraseña no es válida.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePasswordSuccess

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

    }
}
