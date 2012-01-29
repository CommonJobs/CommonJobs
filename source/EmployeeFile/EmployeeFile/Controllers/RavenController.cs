using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using Raven.Client;

namespace EmployeeFile.Controllers
{
    public abstract class RavenController : Controller
    {
        public IDocumentSession RavenSession { get; protected set; }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            RavenSession = RavenSessionManager.GetCurrentSession();
        }
    }
}