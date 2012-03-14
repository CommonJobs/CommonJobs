using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommonJobs.Raven.Mvc
{
    public abstract class CommonJobsViewPage<TModel> : System.Web.Mvc.WebViewPage<TModel>
    {
        public ScriptManager ScriptManager
        {
            get { return ScriptManager.GetFromViewData(ViewData); }
        }

        public CommonJobsViewPage()
            : base()
        {
            
        }
    }
}