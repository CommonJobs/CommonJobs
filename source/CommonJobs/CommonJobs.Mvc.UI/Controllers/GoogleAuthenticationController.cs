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
using System.Configuration;
using System.Collections.Specialized;
using System.ComponentModel;
using CommonJobs.Mvc.UI.Infrastructure;

namespace CommonJobs.Mvc.UI.Controllers
{
    public class GoogleAuthenticationController : CommonJobsController
    {
        private static Logger log = LogManager.GetCurrentClassLogger();
        private const string EmailSuffix = "@makingsense.com";
        private const string OAuthUrl = "https://accounts.google.com/";
        private const string ApiUrl = "https://www.googleapis.com/";

        public ActionResult Index(string returnUrl = null)
        {
            var authUri = BuildAuthUri(returnUrl);
            return Redirect(authUri);
        }

        public ActionResult Error(string returnUrl, string error)
        {
            ViewBag.Error = error;
            return View();
        }

        public ActionResult LogOnCallback(string state, string code)
        {
            var returnUrl = state;

            if (string.IsNullOrEmpty(code))
            {
                return RedirectToAction("Error", new { returnUrl = returnUrl, error = "Response status is not Authenticated" });
            }

            string email = RetrieveUserEmail(code);

            if (string.IsNullOrWhiteSpace(email))
            {
                return RedirectToAction("Error", new { returnUrl = returnUrl, error = "Response Email is empty" });
            }

            if (!email.EndsWith(EmailSuffix))
            {
                return RedirectToAction("Error", new { returnUrl = returnUrl, error = "Only emails ended with " + EmailSuffix + " are allowed" });
            }

            var username = email.Substring(0, email.Length - EmailSuffix.Length);            

            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
            1,
            username,
            DateTime.Now,
            DateTime.Now.AddDays(2),
            true,
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

            var user = RavenSession.Load<User>("Users/" + username);

            log.Debug("User {0} found: {1}", username, user != null);

            if (user != null)
            {
                log.Dump(LogLevel.Debug, user, "RavenDB User");
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

        private static string GetAppId()
        {
            return ConfigurationManager.AppSettings["CommonJobs/GoogleOAuthClientId"];
        }

        private static string GetAppSecret()
        {
            return ConfigurationManager.AppSettings["CommonJobs/GoogleOAuthSecret"];
        }

        private string BuildAuthUri(string returnUrl)
        {
            var appId = GetAppId();
            var returnUriValue = BuildCallbackUriValue();
            var authUri = OAuthHelpers.BuildUri(OAuthUrl, "o/oauth2/auth", new NameValueCollection()
            {
                { "client_id", appId },
                { "redirect_uri", returnUriValue },
                { "response_type", "code" },
                { "scope", "email" },
                { "state", HttpUtility.UrlEncode(returnUrl) }
            });
            return authUri;
        }

        private string BuildCallbackUriValue()
        {
            var callbackUrl = Url.Action("LogOnCallback", "GoogleAuthentication", null, Request.IsSecureConnection ? "https" : "http");
            return callbackUrl;
        }

        private string RetrieveUserEmail(string authorizationCode)
        {
            string accessToken = RetrieveAccessToken(authorizationCode);
            var userinfo = RetrieveUserInfo(accessToken);
            string email = userinfo.email;
            return email;
        }

        private static dynamic RetrieveUserInfo(string accessToken)
        {
            var uri = OAuthHelpers.BuildUri(ApiUrl, "oauth2/v1/userinfo", new NameValueCollection {
                { "access_token", accessToken }
            });

            var response = OAuthHelpers.GetObjectFromAddress(uri);
            return response;
        }

        private string RetrieveAccessToken(string authorizationCode)
        {
            var appId = GetAppId();
            var appSecret = GetAppSecret();

            var returnUriValue = BuildCallbackUriValue();
            var param = new NameValueCollection {
                 { "client_id",     appId },
                 { "client_secret", appSecret },
                 { "code",          authorizationCode },
                 { "grant_type",    "authorization_code" },
                 { "redirect_uri",  returnUriValue },
            };
            var url = OAuthHelpers.BuildUri(OAuthUrl, "o/oauth2/token", new NameValueCollection());

            string accessToken = OAuthHelpers.GetObjectWithPost(url, param).access_token;
            return accessToken;
        }
    }
}
