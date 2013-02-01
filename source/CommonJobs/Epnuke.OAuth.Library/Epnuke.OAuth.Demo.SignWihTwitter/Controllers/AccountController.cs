using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Epnuke.OAuth.Demo.SignWihTwitter.Controllers
{
    public class AccountController : Controller
    {
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
                RedirectToAction("Index", "Private");
            }

            ViewBag.MensajeError = "Credenciales inválidas";
            return View();
        }

        public ActionResult LogOnTwitter()
        {
            var oauthSession = new OAuthClientSession(ConfigurationManager.AppSettings["consumer-key"],
                ConfigurationManager.AppSettings["consumer-secret"],
                new NonceGenerator32Bytes());
            var uri = ConfigurationManager.AppSettings["request_token"];
            oauthSession.RequestTemporaryCredentials(uri, "POST", "http://localhost:64983/Account/Callback");
            Session["oauth"] = oauthSession;
            return Redirect(oauthSession.GetAuthorizationUri(ConfigurationManager.AppSettings["authorize"]));
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
