using System.Web.Mvc;

namespace Epnuke.OAuth.Demo.SignWihTwitter.Controllers
{
    public class PrivateController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }
    }
}