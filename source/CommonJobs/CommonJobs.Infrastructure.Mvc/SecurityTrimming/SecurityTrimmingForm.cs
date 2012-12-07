using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace CommonJobs.Infrastructure.Mvc.SecurityTrimming
{
    public class SecurityTrimmingForm : SecurityTrimmingHelper
    {
        public MvcForm Form { get; private set; }

        internal SecurityTrimmingForm(HtmlHelper htmlHelper, Func<MvcForm> lazyForm, string actionName, string controllerName = null)
            : base(htmlHelper, actionName, controllerName)
        {
            Form = this.HasPermission ? lazyForm() : null;
        }


        public override void Dispose()
        {
            if (Form != null)
                Form.Dispose();
        }
    }
}
