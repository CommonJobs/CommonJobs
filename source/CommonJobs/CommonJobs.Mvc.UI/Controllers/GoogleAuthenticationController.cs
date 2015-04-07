﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using CommonJobs.Infrastructure.Mvc;
using CommonJobs.Domain;
using NLog;
using CommonJobs.Utilities;
using Microsoft.Web.WebPages.OAuth;
using DotNetOpenAuth.Clients;

namespace CommonJobs.Mvc.UI.Controllers
{
    public class GoogleAuthenticationController : CommonJobsController
    {
        private static Logger log = LogManager.GetCurrentClassLogger();
        private const string EmailSuffix = "@makingsense.com";
        public const string SessionRolesKey = "CommonJobs/Roles";

        public ActionResult Index(string returnUrl = null)
        {
            var callbackUrl = Url.Action("LogOnCallback", "GoogleAuthentication", new { returnUrl = returnUrl }, Request.IsSecureConnection ? "https" : "http");

            OAuthWebSecurity.RequestAuthentication("google-plus", callbackUrl);
            return Content(string.Empty);
        }

        public ActionResult Error(string returnUrl, string error)
        {
            ViewBag.Error = error;
            return View();
        }

        public ActionResult LogOnCallback(string returnUrl)
        {
            GooglePlusOAuthClient.RewriteRequest(); // HACK for Google Plus
            var result = OAuthWebSecurity.VerifyAuthentication();
            if (!result.IsSuccessful)
            {
                return RedirectToAction("Error", new { returnUrl = returnUrl, error = "Response status is not Authenticated" });
            }

            string email;
            result.ExtraData.TryGetValue("email", out email);

            if (string.IsNullOrWhiteSpace(email))
            {
                return RedirectToAction("Error", new { returnUrl = returnUrl, error = "Response Email is empty" });
            }
            

            if (!email.EndsWith(EmailSuffix))
            {
                return RedirectToAction("Error", new { returnUrl = returnUrl, error = "Only emails ended with " + EmailSuffix + " are allowed" });
            }

            var username = email.Substring(0, email.Length - EmailSuffix.Length);
            
            FormsAuthentication.SetAuthCookie(username, true);

            Session[SessionRolesKey] = new string[0];
            var user = RavenSession.Query<User>().Where(u => u.UserName == username).FirstOrDefault();

            log.Debug("User {0} found: {1}", username, user != null);

            if (user != null)
            {
                log.Dump(LogLevel.Debug, user, "RavenDB User");
                Session[SessionRolesKey] = user.Roles ?? new string[0];
            }

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
