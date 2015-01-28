using CommonJobs.Application.MyMenu;
using CommonJobs.Domain;
using CommonJobs.Domain.MyMenu;
using CommonJobs.Infrastructure.Mvc;
using CommonJobs.Mvc.UI.Infrastructure;
using NLog;
using Raven.Client.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommonJobs.Utilities;

namespace CommonJobs.Mvc.UI.Areas.MyMenu
{
    [CommonJobsAuthorize]
    [Documentation("docs/manual-de-usuario/pedido-de-almuerzos")]
    public class MyMenuController : CommonJobsController
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        private string DetectUser()
        {
            //TODO: remove hardcoded "CS\\"
            //TODO: move to an AuthorizeAttribute or something more elegant
            if (User != null && User.Identity != null && User.Identity.Name != null)
            {
                var user =  User.Identity.Name;
                if (user.StartsWith("CS\\"))
                    user = user.Substring(3);
                return user;
            }
            else
            {
                throw new ApplicationException("User cannot be detected");
            }
        }

        public ActionResult Index()
        {
            return Edit(DetectUser(), true);
        }
        
        [CommonJobsAuthorize(Roles = "Users,MenuManagers")]
        public ActionResult Edit(string id /*username*/, bool ownMenu = false)
        {
            if (id == "") 
                ownMenu = true;

            ViewBag.AnotherMenuUser = ownMenu ? "" : id;

            ScriptManager.RegisterGlobalJavascript(
                "ViewData",
                new
                {
                    now = DateTime.Now,
                    menuUrl = ownMenu ? Url.Action("OwnMenu") : Url.Action("EmployeeMenu", new { id = id })
                },
                500);
            return View("MyMenu");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [ActionName("OwnMenu")]
        public JsonNetResult PostOwnMenu(EmployeeMenu employeeMenu)
        {
            return PostEmployeeMenu(DetectUser(), employeeMenu);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        [ActionName("OwnMenu")]
        public JsonNetResult GetOwnMenu()
        {
            return GetEmployeeMenu(DetectUser());
        }


        [CommonJobsAuthorize(Roles = "Users,MenuManagers")]
        [AcceptVerbs(HttpVerbs.Post)]
        [ActionName("EmployeeMenu")]
        public JsonNetResult PostEmployeeMenu(string id /*username*/, EmployeeMenu employeeMenu)
        {
            if (string.IsNullOrWhiteSpace(id) || employeeMenu.UserName != id)
                throw new ArgumentException(string.Format("No se permite modificar el menú del usuario o los ids no coinciden ({0}, {1})", id, employeeMenu.UserName));

            ExecuteCommand(new UpdateEmployeeMenuCommand(employeeMenu));

            return Json(new { ok = true });
        }

        [CommonJobsAuthorize(Roles = "Users,MenuManagers")]
        [AcceptVerbs(HttpVerbs.Get)]
        [ActionName("EmployeeMenu")]
        public JsonNetResult GetEmployeeMenu(string id /*username*/)
        {
            var employeeMenu = ExecuteCommand(new GetEmployeeMenuCommand(id));
            return Json(employeeMenu);
        }

        [CommonJobsAuthorize(Roles = "Users,MenuManagers")]
        [Documentation("manual-de-usuario/administracion-de-almuerzos")]
        [Documentation("docs/manual-de-usuario/administracion-de-almuerzos")]
        public ActionResult Admin(string id /*menuid*/ = null)
        {
            ScriptManager.RegisterGlobalJavascript(
                "ViewData",
                new
                {
                    menuId = id,
                    now = DateTime.Now
                },
                500);
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [ActionName("MenuDefinition")]
        [CommonJobsAuthorize(Roles = "Users,MenuManagers")]
        public JsonNetResult PostMenuDefinition(Menu menuDefinition)
        {
            ExecuteCommand(new UpdateMenuDefinitionCommand(menuDefinition, DateTime.Now));
            return Json(new { ok = true });
        }
                
        [AcceptVerbs(HttpVerbs.Get)]
        [ActionName("MenuDefinition")]
        public JsonNetResult GetMenuDefinition(string id /*menuid*/)
        {
            var menu = ExecuteCommand(new GetMenuDefinitionCommand(id));
            
            return Json(menu);
        }

        public ActionResult Order(string id /*menuid*/ = null, DateTime? date = null, int? dateOffset = null)
        {
            ViewBag.HidePersistenceButtons = true;
            ViewBag.date = date;
            ViewBag.dateOffset = dateOffset;

            var orderDate = GenerateDate(date, dateOffset);
            
            var order = ExecuteCommand(new GetOrderCommand() { Date = orderDate, MenuDefinitionId = id });

            ScriptManager.RegisterGlobalJavascript(
                "ViewData",
                new
                {
                    now = DateTime.Now.Date,
                    order = order,
                    baseLink = Url.Action("Edit"),
                    date = date,
                    dateOffset = dateOffset
                },
                500);
            return View();
        }

        private static DateTime GenerateDate(DateTime? date, int? dateOffset)
        {
            return date.GetValueOrDefault(DateTime.Now).AddDays(dateOffset.GetValueOrDefault()).Date;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [CommonJobsAuthorize(Roles = "Users,MenuManagers")]
        public ActionResult GenerateOrder(string id = null, DateTime? date = null, int? dateOffset = null)
        {
            var orderDate = GenerateDate(date, dateOffset);
            ExecuteCommand(new ProcessMenuCommand() { MenuDefinitionId = id, Now = () => orderDate });
            return RedirectToAction("Order", new { date, dateOffset });
        }

    }
}
