using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Epnuke.OAuth.Mvc.Extensions
{
    public static class ModelStateDictionaryExtensions
    {
        public static bool HasOAuthErrors(this ModelStateDictionary @this)
        {
            return @this.ContainsKey("_oauth");
        }

        public static bool HasInvalidSOAuthSignature(this ModelStateDictionary @this)
        {
            return @this["_oauth"].Errors.Any(x => x.ErrorMessage == OAuthRequestError.InvalidSignature.ToString());
        }
    }
}
