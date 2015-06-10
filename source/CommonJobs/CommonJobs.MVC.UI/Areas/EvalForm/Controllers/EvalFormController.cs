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

namespace CommonJobs.Mvc.UI.Areas.EvalForm
{
    [CommonJobsAuthorize]
    //TODO: Add documentation
    [Documentation("docs/manual-de-usuario/evaluaciones")]
    public class EvalFormController : CommonJobsController
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        public ActionResult Index()
        {
            //TODO: ...
            return null;
        }
    }
}
