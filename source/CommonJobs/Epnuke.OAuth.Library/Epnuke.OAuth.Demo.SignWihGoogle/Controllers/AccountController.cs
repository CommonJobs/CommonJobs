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
    }
}
