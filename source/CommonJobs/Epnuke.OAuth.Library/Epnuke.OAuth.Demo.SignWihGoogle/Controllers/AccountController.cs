using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Epnuke.OAuth.Demo.SignWihGoogle.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/

        public ActionResult LogOn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogOn(string login, string pwd)
        {
            if (!string.IsNullOrEmpty(login) && login.Equals(pwd))
            {
                FormsAuthentication.SetAuthCookie(login, false);
                return RedirectToAction("Index", "Private");
            }

            ViewBag.MensajeError = "Credenciales inválidas";
            return View();
        }


        public ActionResult LogOnGoogle()
        {
            // It is not working, I think that it could be usefull to check with https://github.com/niuchl/oauth-login-demo and https://developers.google.com/accounts/docs/OAuth2
            // for example: https://github.com/niuchl/oauth-login-demo/blob/master/endpoints.py or https://github.com/niuchl/oauth-login-demo/blob/master/auth.py

            var oauthSession = new OAuthClientSession(
                ckey: "293136734985.apps.googleusercontent.com",
                csecret: "replace", //TODO: reset on https://code.google.com/apis/console/b/1/?pli=1#project:293136734985:access
                nonceGenerator: new NonceGenerator32Bytes() //TODO: Check if it is valid for google
            ); 

            // It is not working for many reasons:
            // * The remote server returned an error: (411) Length Required.
            // * The remote server returned an error: (400) Bad Request. (if I manually set ContentLenght to 0)
            oauthSession.RequestTemporaryCredentials(
                requestTokenUri: "https://accounts.google.com/o/oauth2/token", 
                httpMethod: "POST", 
                callback: "http://localhost:64984/Account/Callback" 
            );

            Session["oauth"] = oauthSession;
            return Redirect(oauthSession.GetAuthorizationUri("https://accounts.google.com/o/oauth2/auth"));
        }

        public ActionResult Callback(string oauth_verifier)
        {
            var oauthSession = Session["oauth"] as OAuthClientSession;
            oauthSession.Authorize(oauth_verifier, ConfigurationManager.AppSettings["access_token"]);

            if (oauthSession.IsAuthorized)
            {
                var name = oauthSession.GetAdditionalData("screen_name");
                FormsAuthentication.SetAuthCookie(name, false);
                return RedirectToAction("Index", "Private");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }
    }
}
