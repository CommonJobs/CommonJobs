using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace CommonJobs.Raven.Mvc.Authorize
{
    public interface IAlternativeAuthorizationAttribute
    {
        bool Authorize(AuthorizationContext filterContext);
    }
}
